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
        

        public MainWindow()
        {
            InitializeComponent();
            controller = new Controller(this);
            parameters = new SequenceParameters();
            state = new SortedDictionary<string, dynamic>();
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
                AbortButton.IsEnabled = true;
                ConnectButton.Visibility = 0;
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
            foreach (var key in state.Keys)
            {
                Console.WriteLine(key);
            }
            LI100.Content = state["LI100"].ToString();
            T100Fill.Height = state["LI100"]/3.5;
            LI200.Content = state["LI200"].ToString();
            T200Fill.Height = state["LI200"]/3.5;
            LI400.Content = state["LI400"].ToString();
            T400Fill.Height = state["LI400"]/3.5;

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
