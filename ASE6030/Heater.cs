using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASE6030
{
    class Heater
    {
        private String name;
        private bool isOn;
        private Tut.MppOpcUaClientLib.MppClient client;
        //private Thread regulatorThread;
        private Listener listener;

        public Heater(String name, ref Tut.MppOpcUaClientLib.MppClient client, ref Listener listener)
        {
            this.name = name;
            isOn = false;
            this.client = client;
            this.listener = listener;
        }
        public void turnOn()
        {
            client.setOnOffItem(name, true);
            isOn = true;
        }

        public void turnOff()
        {
            client.setOnOffItem(name, false);
            isOn = false;
        }

        public void regulate(double targetTemp, string targetDevice, int Tc)
        {
            double currentTemp;
            var startTime = DateTime.UtcNow;
            while (DateTime.UtcNow - startTime < TimeSpan.FromSeconds(Tc))
            {
                currentTemp = listener.getDouble(targetDevice);
                if (currentTemp < targetTemp && !isOn) turnOn();
                else if (currentTemp >= targetTemp && isOn) turnOff();
 
                Thread.Sleep(100);
            }
        }
    }
}

