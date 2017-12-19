using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using UaLib = Tut.MppOpcUaClientLib;

namespace ASE6030
{
    /// <summary>
    /// Class listening for changes in the system
    /// 
    /// Saves the most recent values and send update calls to mainwindow when values change.
    /// </summary>
    public class Listener// : IDisposable
    {
        private UaLib.MppClient m_mppClient = null;
        private MainWindow window;
        private object lock1 = new object();
        //Let it slide
        private SortedDictionary<string, dynamic> storage;
        private string[] INT_ITEMS = new string[] {
                                            "V102",
                                            "V104",
                                            "P100",
                                            "P200",
                                            "LI100",
                                            "LI200",
                                            "PI300",
                                            "LI400"};
        private string[] DOUBLE_ITEMS = new string[] {
                                            "TI100",
                                            "TI300"};

        private string[] BOOL_ITEMS = new string[] {
                                            "V103",
                                            "V201",
                                            "V204",
                                            "V301",
                                            "V302",
                                            "V303",
                                            "V304",
                                            "V401",
                                            "V404",
                                            "E100",
                                            "LA+100",
                                            "LS-200",
                                            "LS+300",
                                            "LS-300"};

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="window">Mainwindow to send update calls to</param>
        public Listener(MainWindow window)
        {
            storage = new SortedDictionary<string, dynamic>();
            this.window = window;
        }

        /// <summary>
        /// Get the most recent value for a device that has a value type integer
        /// </summary>
        /// <param name="key">Name of the device. Example: "P100"</param>
        /// <returns>Current value</returns>
        public int getInt(string key) {
            // Error handling
            lock (lock1)
            {
                return storage[key];
            }
        }

        /// <summary>
        /// Get the most recent value for a device that has a value type double
        /// </summary>
        /// <param name="key">Name of the device. Example: "TI300"</param>
        /// <returns>Current value</returns>
        public double getDouble(string key) {
            // Error handling
            lock (lock1)
            {
                return storage[key];
            }
        }

        /// <summary>
        /// Get the most recent value for a device that has a value type bool
        /// </summary>
        /// <param name="key">Name of the device. Example: "E100"</param>
        /// <returns>Current value</returns>
        public bool getBool(string key){
            // Error handling
            lock (lock1)
            {
                return storage[key];
            }
        }

        /// <summary>
        /// Connect to the system and subscribe to listen for changes in values. Base code from course material
        /// </summary>
        /// <param name="url">URL for the system to be connected to</param>
        public void connect(string url)
        {
            try
            {
                // Create a new MPP client instance
                var ctorParams = new UaLib.MppClient.MppClientCtorParams(url);
                m_mppClient = new UaLib.MppClient(ctorParams);
                // Signing up for events
                m_mppClient.ProcessItemsChanged +=
                    m_mppClient_ProcessItemsChanged;
                // Adding process items to subscription.
                // A ProcessItemsChanged event will instantly be raised after
                // addToSubscription() has been called!
                m_mppClient.ConnectionStatus += M_mppClient_ConnectionStatus;
                foreach (var key in INT_ITEMS)
                {
                    m_mppClient.addToSubscription(key);
                }
                foreach (var key in BOOL_ITEMS)
                {
                    m_mppClient.addToSubscription(key);
                }
                foreach (var key in DOUBLE_ITEMS)
                {
                    m_mppClient.addToSubscription(key);
                }
            }
            catch (Exception e)
            {
                // Handle the exception...
                Console.WriteLine("Error connecting listener: " + e.Message);
            }
        }

        /// <summary>
        /// Listens for changes in connection. Notifies the user if connection is lost.
        /// </summary>
        private void M_mppClient_ConnectionStatus(object source, UaLib.ConnectionStatusEventArgs args)
        {
            Console.WriteLine(args.SimplifiedStatus);
            if (args.SimplifiedStatus.ToString() == "CONNECTING")
            {
                System.Windows.MessageBox.Show("CONNECTION LOST" + "\n" + "Reconnecting");
            }
        }
        /// <summary>
        /// Listens for changes in the system. Saves the current values and notifies the Mainwindow for changes. 
        /// Throws an exception if system sends wrong data.
        /// </summary>
        private void m_mppClient_ProcessItemsChanged(object source,
            UaLib.ProcessItemChangedEventArgs args)
        {
            try
            {
                // Checking which values have changed
                foreach (var key in args.ChangedItems.Keys)
                {
                    if (Array.Exists(INT_ITEMS, element => element == key))
                    {
                        var valueObject = (UaLib.MppValueInt)args.ChangedItems[key];
                        var actualValue = valueObject.Value;
                        lock (lock1)
                        {
                            storage[key] = actualValue;
                        }
                    }
                    else if (Array.Exists(BOOL_ITEMS, element => element == key))
                    {
                        var valueObject = (UaLib.MppValueBool)args.ChangedItems[key];
                        var actualValue = valueObject.Value;
                        lock (lock1)
                        {
                            storage[key] = actualValue;
                        }
                    }
                    else if (Array.Exists(DOUBLE_ITEMS, element => element == key))
                    {
                        var valueObject = (UaLib.MppValueDouble)args.ChangedItems[key];
                        var actualValue = valueObject.Value;
                        lock (lock1)
                        {
                            storage[key] = actualValue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Wrong values from simulator");
                    }
                }
                // A _copy_ of storage for the updateCall
                window.updateCall(new SortedDictionary<string, dynamic>(storage));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(" ------------------- CANNOT READ --------------------");
            }
        }
    }
    
}