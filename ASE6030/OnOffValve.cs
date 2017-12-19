using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASE6030
{
    /// <summary>
    /// Class for controlling an On/Off valve
    /// 
    /// Creates an object instance of an On/Off valve that can be used to control the valve
    /// </summary>
    public class OnOffValve
    {
        private String name; /**Name of the device. Example: "V204"*/
        private Tut.MppOpcUaClientLib.MppClient client;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the corresponding device. Example: "V302"</param>
        /// <param name="client">Reference to MppClient object</param>
        public OnOffValve(String name, ref Tut.MppOpcUaClientLib.MppClient client)
        {
            this.name = name;
            this.client = client;
        }

        /// Open On/Off valve
        public void open() {
            //Do stuff
            client.setOnOffItem(name, true);
            //Async wait for confirmation from server?
        }

        /// Close On/Off valve
        public void close()
        {
            //Do stuff
            client.setOnOffItem(name, false);
        }
    }
}
