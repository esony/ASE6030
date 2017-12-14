using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using UaLib = Tut.MppOpcUaClientLib;

//Base code from course material
namespace ASE6030
{
    class Listener// : IDisposable
    {
        private UaLib.MppClient m_mppClient = null;
        private MainWindow window;
        //Let it slide
        private SortedDictionary<string, dynamic> storage;
        private string[] INT_ITEMS = new string[] {
                                            "LI100",
                                            "LI200",
                                            "PI300",
                                            "LI400"};
        private string[] DOUBLE_ITEMS = new string[] {
                                            "TI100",
                                            "TI300"};

        private string[] BOOL_ITEMS = new string[] { 
                                            "E100",
                                            "LA+100",
                                            "LS-200",
                                            "LS+300",
                                            "LS-300"};
        public Listener(MainWindow window)
        {
            storage = new SortedDictionary<string, dynamic>();
            this.window = window;
        }

        public int getInt(string key) {
            // Error handling
            return storage[key];
        }

        public double getDouble(string key) {
            // Error handling
            return storage[key];
        }

        public bool getBool(string key){
            // Error handling
            return storage[key];
        }

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
                
                foreach (var key in INT_ITEMS)
                {
                    m_mppClient.addToSubscription(key);
                    Console.WriteLine(key + " added");
                }
                foreach (var key in BOOL_ITEMS)
                {
                    m_mppClient.addToSubscription(key);
                    Console.WriteLine(key + " added");
                }
                foreach (var key in DOUBLE_ITEMS)
                {
                    m_mppClient.addToSubscription(key);
                    Console.WriteLine(key + " added");
                }
            }
            catch (Exception e)
            {
                // Handle the exception...
                Console.WriteLine("Error connecting: " + e.Message);
            }
        }
        
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
                        storage[key] = actualValue;
                        
                    //    Console.WriteLine(key + ": " + storage[key]);
                    }
                    else if (Array.Exists(BOOL_ITEMS, element => element == key))
                    {
                        var valueObject = (UaLib.MppValueBool)args.ChangedItems[key];
                        var actualValue = valueObject.Value;
                        storage[key] = actualValue;
                    //    Console.WriteLine(key + ": " + storage[key]);
                    }
                    else if (Array.Exists(DOUBLE_ITEMS, element => element == key))
                    {
                        var valueObject = (UaLib.MppValueDouble)args.ChangedItems[key];
                        var actualValue = valueObject.Value;
                        storage[key] = actualValue;
                    //    Console.WriteLine(key + ": " + storage[key]);
                    }
                    else
                    {
                        Console.WriteLine("Wrong values from simulator");
                    }
                }
                window.updateCall(storage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
    
}