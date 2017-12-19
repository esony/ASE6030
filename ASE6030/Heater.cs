using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASE6030
{
    /// <summary>
    /// Class for controlling on/off heater element
    /// 
    /// Creates an object instance of a heater element.
    /// </summary>
    public class Heater
    {
        private String name; /**< Name of the device to be controlled*/
        private bool isOn; /**< Tells whether the heater is on or not. Used internally when regulating temperature.*/
        private Tut.MppOpcUaClientLib.MppClient client;
        private Listener listener;

        /// Constructor
        /// <param name="name">Name (code) of the device</param>
        /// <param name="client">Reference to MppClient object</param>
        /// <param name="listener">Reference to Listener object</param>
        public Heater(String name, ref Tut.MppOpcUaClientLib.MppClient client, ref Listener listener)
        {
            this.name = name;
            isOn = false;
            this.client = client;
            this.listener = listener;
        }

        /// Turn heater on
        /** Sends signal to simulator/device to turn on the heater.
         */
        public void turnOn()
        {
            client.setOnOffItem(name, true);
            isOn = true;
        }

        /// Turn heater off
        /** Sends signal to simulator/device to turn off the heater.
         */
        public void turnOff()
        {
            client.setOnOffItem(name, false);
            isOn = false;
        }

        /// <summary>
        /// Set the heater to regulate the defined target temperature targetTemp at thermometer targetDevice, 
        /// for time controlTime
        /// </summary>
        /// <param name="targetTemp">Target temperature</param>
        /// <param name="targetDevice">Which thermometer to fetch the current temperature from</param>
        /// <param name="Tc">Control interval in seconds. Determines how often the thermostate 
        /// checks the current temperature and, if needed, switches the heater on/off</param>
        /// <param name="controlTime">Control time defines how long the regulator is running</param>
        public void regulate(double targetTemp, string targetDevice, int Tc, int controlTime)
        {
            double currentTemp;
            var startTime = DateTime.UtcNow;
            while (DateTime.UtcNow - startTime < TimeSpan.FromSeconds(Tc))
            {
                currentTemp = listener.getDouble(targetDevice);
                if (currentTemp < targetTemp && !isOn) turnOn();
                else if (currentTemp >= targetTemp && isOn) turnOff();
 
                Thread.Sleep(controlTime);
            }
        }
    }
}

