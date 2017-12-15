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

        private void setParameters()
        {
            try
            {
                parameters.impregnationTime = int.Parse(ImpregnationInput.Text);
                parameters.cookingTime = int.Parse(CookingTimeInput.Text);
                parameters.cookingTemperature = int.Parse(CookingTemperatureInput.Text);
                parameters.cookingPressure = int.Parse(CookingPressureInput.Text);

                parameters.gain = double.Parse(GainInput.Text);
                parameters.integrationTime = double.Parse(IntegrationTimeInput.Text);
 
            } catch
            {
                Err("Incorrect input values!");
            }

            if (parameters.impregnationTime <= 0 || parameters.impregnationTime > 60) Err("Check impregnation time");
            else if (parameters.cookingTime < 0 || parameters.cookingTime > 100) Err("Check cooking time");
            else if (parameters.cookingTemperature < 20 || parameters.cookingTemperature > 80) Err("Check cooking temperature");
            else if (parameters.cookingPressure < 0 || parameters.cookingPressure > 350) Err("Check cooking pressure");
            else if (parameters.gain < 0 || parameters.gain > 1) Err("Check gain");
            else if (parameters.integrationTime < 0 || parameters.integrationTime > 1) Err("Check integration time");
            else return;

        }
        private void Err(string e)
        {
            System.Windows.MessageBox.Show(e);
            throw new Exception("Error: " + e);
        }
        
        private void Impregnation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                setParameters();
                controller.startImpregnation();
                AbortButton.IsEnabled = true;
                StartButton.IsEnabled = false;

            } catch (Exception error)
            {
                Console.WriteLine(error);
            }
        }

        private void AbortImpregnation_Click(object sender, RoutedEventArgs e)
        {
            setParameters();
            controller.abortImpregnation();
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

            LI200.Content = state["LI200"].ToString() + " mm";
            T200Fill.Height = state["LI200"]/3.5;
            PI300.Content = state["PI300"].ToString() + " bar";
            TI300.Content = state["TI300"].ToString("N2") + " C";
            LI400.Content = state["LI400"].ToString() + " mm";
            T400Fill.Height = state["LI400"]/3.5;


            // Valves
            V102.Background = state["V102"] == 100 ? GREEN : RED;
            V103.Background = state["V103"] ? GREEN : RED;
            V104.Background = state["V104"] > 0 ? GREEN : RED;

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
