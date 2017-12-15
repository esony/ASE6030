using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;

namespace ASE6030
{
    class Controller
    {
        private Tut.MppOpcUaClientLib.MppClient.MppClientCtorParams simulatorParams;
        public Tut.MppOpcUaClientLib.MppClient client;
        public Listener listener;
        private string SIMULATOR_URL = "opc.tcp://127.0.0.1:8087";
        private string DEVICE_URL = "opc.tcp://192.168.137.101";
        private string URL;
        private int LOOP_TIME = 200;
        
        private Pump p100;
        private Pump p200;

        private OnOffValve v103;
        private OnOffValve v201;
        private OnOffValve v204;
        private OnOffValve v301;
        private OnOffValve v302;
        private OnOffValve v303;
        private OnOffValve v304;
        private OnOffValve v401;
        private OnOffValve v404;

        private FlowControlValve v102;
        private FlowControlValve v104;

        private Heater e100;

        private Thread ListenerThread;
        private Thread ImpregnationThread;
        private Thread BlackLiquorThread;
        private Thread WhiteLiquorThread;
        private Thread CookingThread;

        private MainWindow window;

        public Controller(MainWindow window) {
            client = null;
            listener = new Listener(window);
            URL = SIMULATOR_URL;
            this.window = window;
        }

        private void startListener()
        {
            ListenerThread = new Thread(() =>
            {
                Console.WriteLine("Listener thread started");
                listener.connect(URL);
                
            }
                );

            ListenerThread.Start();
        }

        // Connect to selected client
        public void connectClient(string URL)
        {
            // Physical device if selected
            if (URL == "DEVICE") this.URL = DEVICE_URL;
            // Simulator by default
            else this.URL = SIMULATOR_URL;
    
            // Try connecting to client
            try
            {
                simulatorParams = new Tut.MppOpcUaClientLib.MppClient.MppClientCtorParams(this.URL);
                client = new Tut.MppOpcUaClientLib.MppClient(simulatorParams);
                startListener();
                assignUnits();

            //Handle exception if failed
            } catch (Exception e)
            {
                Console.WriteLine("------- ERROR CONNECTING--------");
                Console.WriteLine(e);
                Console.WriteLine("-------                 --------");
                throw;
            }
        }

        //Assing all the units to client
        private void assignUnits()
        {
            // Pumps
            p100 = new Pump("P100", ref client);
            p200 = new Pump("P200", ref client);

            // On/Off Valves
            v103 = new OnOffValve("V103", ref client);

            v204 = new OnOffValve("V204", ref client);
            v201 = new OnOffValve("V201", ref client);
            
            v301 = new OnOffValve("V301", ref client);
            v302 = new OnOffValve("V302", ref client);
            v303 = new OnOffValve("V303", ref client);
            v304 = new OnOffValve("V304", ref client);
            
            v401 = new OnOffValve("V401", ref client);
            v404 = new OnOffValve("V404", ref client);

            //Flow control valves
            v102 = new FlowControlValve("V102", ref client, ref listener);
            v104 = new FlowControlValve("V104", ref client, ref listener);

            //Heater
            e100 = new Heater("E100", ref client, ref listener);
        }

        // Sequence functionalities
        public void startImpregnation()
        {
            ImpregnationThread = new Thread(() =>
            {
                EM2_OP1();
                EM5_OP1();
                EM3_OP2();
                

                // Wait until LS+300 activates                
                while(true)
                {
                    if (listener.getBool("LS+300")) {
                        break;
                    } else
                    {
                        Thread.Sleep(LOOP_TIME);
                    }
                }
                Console.WriteLine("Done Sleeping");

                EM3_OP1();

                // Wait for Time Ti
                int Ti = 1000;

                Thread.Sleep(Ti);

                System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Start();
                EM2_OP2();
                EM5_OP3();
                EM3_OP6();
                stopWatch.Stop();
                Console.WriteLine(stopWatch.ElapsedMilliseconds + "ms");
                Console.WriteLine("-----------------");
                
                // Delay time Td
                int Td = 1000;
                EM3_OP8(Td);

                //Here for now
                startBlackLiquorFill();
            });

            //Thread set to background for development
            ImpregnationThread.IsBackground = true;
            ImpregnationThread.Start();
            
        }
        // Abort whatever is happening
        public void abortImpregnation()
        {
            //Probably needs a thread container to kill all
            ImpregnationThread.Abort();
            BlackLiquorThread.Abort();
        }

        public void startBlackLiquorFill()
        {
            BlackLiquorThread = new Thread(() =>
            {
                EM3_OP2();
                EM5_OP1();
                EM4_OP1();
                Console.WriteLine("EM4_OP1");
                // Wait for LI400 to drop below 35
                while (true) {
                    if(listener.getInt("LI400") < 35) break;
                    else Thread.Sleep(LOOP_TIME);
                }
                Console.WriteLine("LI400 below 35");
                EM3_OP6();
                EM5_OP3();
                EM4_OP2();

                //Start whiteliquor
                startWhiteLiquorFill();
            });
            BlackLiquorThread.IsBackground = true;
            BlackLiquorThread.Start();
        }

        public void startWhiteLiquorFill()
        {
            WhiteLiquorThread = new Thread(() =>
            {
                EM3_OP3();
                EM1_OP2();
                // Wait for LI400 to rise above 80
                while (true)
                {
                    if (listener.getInt("LI400") > 80) break;
                    else Thread.Sleep(LOOP_TIME);
                }
                Console.WriteLine("LI400 above 80");
                EM3_OP6();
                EM1_OP4();
                startCooking(23.00, 100);

            });
            WhiteLiquorThread.IsBackground = true;
            WhiteLiquorThread.Start();
        }

        public void startCooking(double cookingTemperature, int Tc)
        {
            CookingThread = new Thread(() =>
            {
                EM3_OP4();
                EM1_OP1();

                while (true)
                {
                    if (listener.getDouble("TI300") >= cookingTemperature) break;
                    else Thread.Sleep(LOOP_TIME);
                }

                EM3_OP1();
                EM1_OP2();

                U1_OP1();
                U1_OP2();

                U1_OP3();
                U1_OP4();

                EM3_OP6();
                EM1_OP4();

                EM3_OP8(1000);
                discharge();
            });
            CookingThread.IsBackground = true;
            CookingThread.Start();
        }

        public void discharge()
        {
            EM5_OP2();
            EM3_OP5();

            //Wait for LS-300 to deactivate
            while(true)
            {
                if (!listener.getBool("LS-300")) break;
                else Thread.Sleep(LOOP_TIME);
            }
            Console.WriteLine("LS-300 deactivated");

            EM5_OP4();
            EM3_OP7();
            Console.WriteLine("Process finished");
            
        }

        //Open route to digester/T300, pump and heat
        private void EM1_OP1()
        {
            v102.open();
            v304.open();
            p100.turnOn();

            e100.turnOn();
        }
        //Open route to digester/T300 and pump
        private void EM1_OP2()
        {
            v102.open();
            v304.open();
            p100.turnOn();
        }
        //Close route to digester/T300, pump and heater off
        private void EM1_OP3()
        {
            v102.close();
            v304.close();
            p100.turnOff();

            e100.turnOff();
        }
        //Close route to digester/T300 and pump off
        private void EM1_OP4()
        {
            v102.close();
            v304.close();
            p100.turnOff();
        }
        // Look up the rest in tech details docs
        private void EM2_OP1() 
        {
            v201.open();
        }

        private void EM2_OP2()
        {
            v201.close();
        }

        private void EM3_OP1() 
        {
            v104.close();
            v204.close();
            v401.close();
        }

        private void EM3_OP2()
        {
            v204.open();
            v301.open();
        }

        private void EM3_OP3()
        {
            v301.open();
            v401.open();
        }

        private void EM3_OP4()
        {
            v104.open();
            v301.open();
        }

        private void EM3_OP5()
        {
            v204.open();
            v302.open();
        }

        private void EM3_OP6()
        {
            v104.close();
            v204.close();
            v301.close();
            v401.close();
        }

        private void EM3_OP7()
        {
            v302.close();
            v204.close();
        }

        private void EM3_OP8(int delay)
        {
            v204.open();
            Thread.Sleep(delay);
            v204.close();
        }

        private void EM4_OP1()
        {
            v404.open();
        }

        private void EM4_OP2()
        {
            v404.close();
        }

        private void EM5_OP1()
        {
            v303.open();
            p200.turnOn();
        }

        private void EM5_OP2()
        {
            v103.open();
            v303.open();
            p200.turnOn();
        }

        private void EM5_OP3()
        {
            v303.close();
            p200.turnOff();
        }

        private void EM5_OP4()
        {
            v103.close();
            v303.close();
            p200.turnOff();
        }

        private void U1_OP1()
        {
            //tba
            Console.WriteLine("U1_OP1: Throttling v104 aggresively");
            // Wórks fine with integrationTime=controlTime
            v104.startPI(0.2, 0, 100, 100, 100, 30);
        }
        private void U1_OP2()
        {
            //tba
            Console.WriteLine("U1_OP2: Heating stuff with E100");
            e100.regulate(23, "TI300", 30);
            Console.WriteLine("U1_OP2: Done regulating stuff");



        }
        private void U1_OP3()
        {
            v104.close();
        }
        private void U1_OP4()
        {
            e100.turnOff();
        }

/*
        public void valveOpen()
        {
            Console.WriteLine();
            Console.WriteLine("Valve open");
            Console.WriteLine(client.ToString());

            client.setValveOpening("V102", 100);
            client.setOnOffItem("V304", true);
            client.setOnOffItem("V301", true);
            client.setOnOffItem("V204", true);
        }

        public void valveClose()
        {
    //        client.setValveOpening("V102", 0);
  //          client.setOnOffItem("V304", false);
//            client.setOnOffItem("V301", false);
            client.setOnOffItem("V204", false);

        }
        
        public void pumpOn()
        {
            e100.turnOn();            
        }
        public void pumpOff()
        {
            Console.WriteLine(listener.getInt("PI300"));
            v104.startPI(0.2, 0, 100, 5, 1, 30);

        }
        */

    }
}
