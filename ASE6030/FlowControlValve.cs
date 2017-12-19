using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;

namespace ASE6030
{
    /// <summary>
    /// Class for controlling a flow control valve
    /// 
    /// Creates an object instance of a flow control valve that can be used to control the valve. 
    /// Includes a PI controller to regulate pressure.
    /// </summary>
    public class FlowControlValve
    {
        private String name;
        private Thread piThread;
        private double pValue;
        private double iValue;
        private double targetValue;
        private double integrationTime;
        private double controlPeriod;
        private Listener listener;

        private Tut.MppOpcUaClientLib.MppClient client;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name (code) of the corresponding device. Example: V104</param>
        /// <param name="client">Reference to MppClient object</param>
        /// <param name="listener">Reference to Listener object</param>
        public FlowControlValve(String name, ref Tut.MppOpcUaClientLib.MppClient client, ref Listener listener) {
            this.name = name;
            this.client = client;
            this.listener = listener;
            //Some default values
            this.pValue = 0;
            this.iValue = 0;
            this.targetValue = 0;
            this.piThread = new Thread(()=> {});
        }

        /// <summary>
        /// Starts the PI controller for valve.
        /// </summary>
        /// <param name="pValue">Gain</param>
        /// <param name="targetValue">Target value</param>
        /// <param name="integrationTime">Integration time</param>
        /// <param name="controlPeriod">Control period (interval)</param>
        /// <param name="executionTime">Execution time, how long the controller is on.</param>
        //pValue 0-1, targertValue 0-350, integrationTime ms, controlPeriod ms, executionTime s
        public void startPI(double pValue, double targetValue, double integrationTime, double controlPeriod, int executionTime)
        {
            piThread = new Thread(() =>
            {
                try
                {
                    this.pValue = pValue;
                    this.iValue = 0;
                    this.targetValue = targetValue;
                    this.integrationTime = integrationTime;
                    this.controlPeriod = controlPeriod;

                    //Loop for time Tc
                    // Tc = cooking time
                    var startTime = DateTime.UtcNow;
                    while (DateTime.UtcNow - startTime < TimeSpan.FromSeconds(executionTime))
                    {
                        client.setValveOpening(name, control());
                        Thread.Sleep((int)controlPeriod);
                    }
                    Console.WriteLine("Done with PI");
                } catch (ThreadAbortException)
                {
                    Console.WriteLine("PI Controller shutdown");
                }
            });
            piThread.IsBackground = true;
            piThread.Start();

        }

        /// Returns true if PI controller thread is alive = PI controller is on, false if not.
        public bool isAlive()
        {
            return piThread.IsAlive;
        }

        /// Shuts down PI controller
        public void shutDown()
        {
            if (piThread.IsAlive)
            {
                Console.WriteLine("Killing PI");
                piThread.Abort();
            }
        }

        /// <summary>
        /// Calculates the new control value from feedback measurement.
        /// </summary>
        /// <returns>New control value for valve.</returns>
        private int control()
        {
            int controlValue = 0;
            // Hard coded, sorry
            Console.WriteLine(listener.getInt("PI300"));

            int currentValue = listener.getInt("PI300");
            double difference = currentValue - targetValue;
            int unlimitedControl = (int)(pValue * difference + iValue);
            int lowerLimit = 0;
            int upperLimit = 100;

            if (unlimitedControl <= lowerLimit)
            {
                controlValue = lowerLimit;
            }
            else if (unlimitedControl >= upperLimit)
            {
                controlValue = upperLimit;
            }
            else controlValue = unlimitedControl;

            iValue = iValue + pValue * integrationTime / controlPeriod * difference;
            return controlValue;
        }

        /// Fully open the valve
        public void open() { 
            client.setValveOpening(name, 100);
        }

        /// Fully close the valve, shut down PI controller if on
        public void close()
        {
            shutDown();
            client.setValveOpening(name, 0);
        }
    }
}
