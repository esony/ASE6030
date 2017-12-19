using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASE6030
{
    /// <summary>
    /// Class for controlling a pump
    /// 
    /// Creates an object instance of a pump that can be used to control the pump.
    /// </summary>
    public class Pump
    {
        private String name;
        Tut.MppOpcUaClientLib.MppClient client;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name (code) of the corresponding device. Example: "P100"</param>
        /// <param name="client">Reference to MppClient object</param>
        public Pump(String name, ref Tut.MppOpcUaClientLib.MppClient client){
            this.name = name;
            this.client = client;
        }

        /// <summary>
        /// Set pump preset and turn on the pump
        /// </summary>
        public void turnOn(){
            // Set Pump preset
            setPreset();
            client.setPumpControl(name, 100);
        }

        /// <summary>
        /// Turn on the pump
        /// </summary>
        public void turnOff()
        {
            client.setPumpControl(name, 0);
        }

        /// <summary>
        /// Set pump preset
        /// </summary>
        public void setPreset(){
            // Do stuff
            client.setOnOffItem("P100_P200_PRESET", true);
        }
    }
}
