using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASE6030
{
    class Pump
    {
        private bool isOn;
        private bool preset;
        private String name;
        Tut.MppOpcUaClientLib.MppClient client;

        public Pump(String name, ref Tut.MppOpcUaClientLib.MppClient client){
            this.isOn = false;
            this.preset = false;
            this.name = name;
            this.client = client;
        }

        public void turnOn(){
            //Do stuff
            //The client probably needs to be locked
            
            // ---------------------------------------
            // Set Pump preset
            // Not sure if needs to be checked first
            setPreset();
            // ---------------------------------------

            client.setPumpControl(name, 100);
            this.isOn = true;
        }

        public void turnOff()
        {
            //Do stuff
            //The client probably needs to be locked

            client.setPumpControl(name, 0);
            this.isOn = false;
        }

        public void setPreset(){
            // Do stuff
            client.setOnOffItem("P100_P200_PRESET", true);
            this.preset = true;

        }

    }
}
