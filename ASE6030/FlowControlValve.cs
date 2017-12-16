using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;

namespace ASE6030
{
    class FlowControlValve
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

        public FlowControlValve(String name, ref Tut.MppOpcUaClientLib.MppClient client, ref Listener listener) {
            this.name = name;
            this.client = client;
            this.listener = listener;
            //Some default values
            this.pValue = 0;
            this.iValue = 0;
            this.targetValue = 0;
        }

        //pValue 0-1, targertValue 0-350, integrationTime ms, controlPeriod ms, executionTime s
        public void startPI(double pValue, double targetValue, double integrationTime, double controlPeriod, int executionTime)
        {
            piThread = new Thread(() =>
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
            });
            piThread.IsBackground = true;
            piThread.Start();

        }

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
            Console.WriteLine("iValue: " + iValue);
            Console.WriteLine("pValue: " + pValue);

            Console.WriteLine("difference: " + difference);
            Console.WriteLine("control:" + controlValue);
            Console.WriteLine("-------------------");

            return controlValue;
        }


        public void open() { 
            //Do stuff
            client.setValveOpening(name, 100);
        }

        public void close()
        {
            //Do stuff
            client.setValveOpening(name, 0);
        }
    }
}
