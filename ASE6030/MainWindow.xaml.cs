using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ASE6030
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Controller controller;
        private SequenceParameters parameters;
        private SortedDictionary<string, dynamic> state;
        private SolidColorBrush GREEN;
        private SolidColorBrush RED;

        public MainWindow()
        {
            InitializeComponent();
            controller = new Controller(this);
            parameters = new SequenceParameters();
            state = new SortedDictionary<string, dynamic>();

            GREEN = new SolidColorBrush();
            GREEN.Color = Colors.GreenYellow;
            RED = new SolidColorBrush();
            RED.Color = Colors.OrangeRed;
        }
        /*
        private void test()
        {
            SortedDictionary<string, dynamic> asd = new SortedDictionary<string, dynamic>();
            asd["A"] = "asd";
            asd["B"] = 1;
            asd["C"] = true;
            test2(new SortedDictionary<string, dynamic>(asd));
            Console.WriteLine(asd["A"]);
            Console.WriteLine(asd["B"]);
            Console.WriteLine(asd["C"]);

        }
        private void test2(SortedDictionary<string, dynamic> dsa)
        {
            dsa["A"] = "Vityns";
            dsa["B"] = 2;
            dsa["C"] = false;
        }
        */
        private void setParameters()
        {
            // Type check
            try
            {
                parameters.impregnationTime = int.Parse(ImpregnationInput.Text);
                parameters.cookingTime = int.Parse(CookingTimeInput.Text);
                parameters.cookingTemperature = double.Parse(CookingTemperatureInput.Text);
                parameters.cookingPressure = int.Parse(CookingPressureInput.Text);

                parameters.gain = double.Parse(GainInput.Text);
                parameters.integrationTime = double.Parse(IntegrationTimeInput.Text);
                
            } catch
            {
                Err("Incorrect input type!");
            }

            // Controller handles value check
            try
            {
                controller.setParams(parameters);
            } catch (Exception e)
            {
                Err(e.Message);
            }
        }

        private void Err(string e)
        {
            System.Windows.MessageBox.Show(e);
            throw new Exception("Error: " + e);
        }
        
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                setParameters();
                controller.startSequence();
                AbortButton.IsEnabled = true;
                StartButton.IsEnabled = false;
                ImpregnationInput.IsEnabled = false;
                CookingTimeInput.IsEnabled = false;
                CookingTemperatureInput.IsEnabled = false;
                CookingPressureInput.IsEnabled = false;
                IntegrationTimeInput.IsEnabled = false;
                GainInput.IsEnabled = false;

            } catch (Exception error)
            {
                Console.WriteLine(error);
            }
        }

        private void AbortButton_Click(object sender, RoutedEventArgs e)
        {
            controller.abortSequence();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string URL = "SIMULATOR";
                if (DeviceRadioButton.IsChecked == true) URL = "DEVICE";
                
                controller.connectClient(URL);
                StartButton.IsEnabled = true;
                ConnectButton.IsEnabled = false;
                SimulatorRadioButton.IsEnabled = false;
                DeviceRadioButton.IsEnabled = false;
                
            } catch {
                System.Windows.MessageBox.Show("Error connecting!");
            }
        }

        private void SimulatorRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        public void updateCall(SortedDictionary<string, dynamic> state)
        {
            // Invoking in the UI thread
            Dispatcher.BeginInvoke((Action)(() => updateState(state)));
        }
        
        // Save state changes
        private void updateState(SortedDictionary<string, dynamic> state)
        {
            try
            {
                this.state = state;
                updateView();
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                System.Windows.MessageBox.Show("Error updating values!");

            }
        }
        // Update the view in mainwindow rather than giving access to everyone
        private void updateView()
        {
            // Tanks
            LI100.Content = state["LI100"].ToString() + " mm";
            T100Fill.Height = state["LI100"]/2;
            TI100.Content = state["TI100"].ToString("N2") + " C";
            LA100.Background = state["LA+100"] ? GREEN : RED;

            LI200.Content = state["LI200"].ToString() + " mm";
            T200Fill.Height = state["LI200"]/3.5;
            LS_200.Background = state["LS-200"] ? GREEN : RED;

            PI300.Content = state["PI300"].ToString() + " bar";
            TI300.Content = state["TI300"].ToString("N2") + " C";
            LS300.Background = state["LS+300"] ? GREEN : RED;
            LS_300.Background = state["LS-300"] ? GREEN : RED;

            LI400.Content = state["LI400"].ToString() + " mm";
            T400Fill.Height = state["LI400"]/3.5;


            // Valves
            V102.Background = state["V102"] == 100 ? GREEN : RED;
            V103.Background = state["V103"] ? GREEN : RED;
            V104.Background = state["V104"] > 0 ? GREEN : RED;
            V104Value.Content = state["V104"].ToString() + "%";

            V201.Background = state["V201"] ? GREEN : RED;
            V204.Background = state["V204"] ? GREEN : RED;

            V301.Background = state["V301"] ? GREEN : RED;
            V302.Background = state["V302"] ? GREEN : RED;
            V303.Background = state["V303"] ? GREEN : RED;
            V304.Background = state["V304"] ? GREEN : RED;

            V401.Background = state["V401"] ? GREEN : RED;
            V404.Background = state["V404"] ? GREEN : RED;

            // Pumps
            P100.Background = state["P100"] > 0 ? GREEN : RED;
            P200.Background = state["P200"] > 0 ? GREEN : RED;

            // Heater
            E100.Background = state["E100"] ? GREEN : RED;


        }

        public void updateProcessFlow(int step)
        {
            Dispatcher.BeginInvoke((Action)(() => updateProcessView(step)));
        }

        private void updateProcessView(int step)
        {
            Step1.Background = RED;
            Step2.Background = RED;
            Step3.Background = RED;
            Step4.Background = RED;
            Step5.Background = RED;
            if (step != 0) {
                Label lbl = (Label)FindName("Step" + step.ToString());
                lbl.Background = GREEN;
            } else
            {
                AbortButton.IsEnabled = false;
                StartButton.IsEnabled = true;
                ImpregnationInput.IsEnabled = true;
                CookingTimeInput.IsEnabled = true;
                CookingTemperatureInput.IsEnabled = true;
                CookingPressureInput.IsEnabled = true;
                IntegrationTimeInput.IsEnabled = true;
                GainInput.IsEnabled = true;
            }
        }

        private void DeviceRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ImpregnationInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void CookingTimeInput_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void CookingPressureInput_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void CookingTemperatureInput_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void GainInput_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void IntegrationTimeInput_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
