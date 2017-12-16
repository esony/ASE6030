using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASE6030
{
    class OnOffValve
    {
        private String name;
        private Tut.MppOpcUaClientLib.MppClient client;

        public OnOffValve(String name, ref Tut.MppOpcUaClientLib.MppClient client)
        {
            this.name = name;
            this.client = client;
        }
        public void open() {
            //Do stuff
            client.setOnOffItem(name, true);
            //Async wait for confirmation from server?
        }

        public void close()
        {
            //Do stuff
            client.setOnOffItem(name, false);
        }
    }
}
