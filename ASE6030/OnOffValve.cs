using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASE6030
{
    class OnOffValve
    {
        private String name;
        private bool isOpen;
        private Tut.MppOpcUaClientLib.MppClient client;

        public OnOffValve(String name, ref Tut.MppOpcUaClientLib.MppClient client)
        {
            this.name = name;
            this.isOpen = false;
            this.client = client;
        }
        public void open() {
            //Do stuff
            client.setOnOffItem(name, true);
            this.isOpen = true;
        }

        public void close()
        {
            //Do stuff
            client.setOnOffItem(name, false);
            this.isOpen = false;
        }
    }
}
