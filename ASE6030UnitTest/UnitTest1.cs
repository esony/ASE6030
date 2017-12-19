using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ASE6030;

namespace ControllerUnitTest
{
    /// <summary>
    /// Unit test for Controller class
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        private SequenceParameters parameters = new SequenceParameters();
        private Controller controller = new Controller(new MainWindow());
        private void setCorrectParameters()
        {
            parameters.impregnationTime = 1;
            parameters.cookingTime = 2;
            parameters.cookingTemperature = 30;
            parameters.cookingPressure = 100;
            parameters.gain = 0.2;
            parameters.integrationTime = 0.2;
        }

        /// <summary>
        /// Testing for user input of some correct values
        /// </summary>
        [TestMethod]
        public void CorrectValues()
        {
            try
            {
                setCorrectParameters();
                controller.setParams(parameters);
            } catch (Exception e)
            {
                Assert.Fail(string.Format("Unexpected error: ") + e);
            }
        }

        /// <summary>
        /// Testing for the upper limit of user input for cooking pressure
        /// </summary>
        [TestMethod]
        public void MaxPressure()
        {
            try
            {
                setCorrectParameters();
                parameters.cookingPressure = 350;
                controller.setParams(parameters);
            } catch
            {
                Assert.Fail(string.Format("350bar should have passed for pressure"));
            }
        }

        /// <summary>
        /// Testing for too low user input for cooking temperature
        /// </summary>
        [TestMethod]
        public void TooLowTemperature()
        {
            try
            {
                setCorrectParameters();
                parameters.cookingTemperature = 1;
                controller.setParams(parameters);
                Assert.Fail("Should have failed");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Error: Cooking temperature must be between 20 and 80", e.Message);
            }
        }
        /// <summary>
        /// Testing for too high user input for cooking temperature
        /// </summary>
        [TestMethod]
        public void TooHighTemperature()
        {
            try
            {
                setCorrectParameters();
                parameters.cookingTemperature = 351;
                controller.setParams(parameters);
                Assert.Fail("Should have failed");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Error: Cooking temperature must be between 20 and 80", e.Message);
            }
        }

        /// <summary>
        /// Testing for too high user input for cooking pressure
        /// </summary>
        [TestMethod]
        public void TooHighPressure()
        {
            try
            {
                setCorrectParameters();
                parameters.cookingPressure = 351;
                controller.setParams(parameters);
                Assert.Fail("Should have failed");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Error: Cooking pressure must be between 0 and 350", e.Message);
            }
        }

        /// <summary>
        /// Testing for too low user input for cooking pressure
        /// </summary>
        [TestMethod]
        public void TooLowPressure()
        {
            try
            {
                setCorrectParameters();
                parameters.cookingPressure = -1;
                controller.setParams(parameters);
                Assert.Fail("Should have failed");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Error: Cooking pressure must be between 0 and 350", e.Message);
            }
        }

        /// <summary>
        /// Testing for legal user inputs. Type double inputs with 0.01 accuracy
        /// </summary>
        [TestMethod]
        public void LegalInputs()
        {
            try
            {
                setCorrectParameters();
                ///Test for legal int inputs
                int i = 0;

                while (i <= 60)
                {
                    parameters.cookingTime = i;
                    parameters.cookingPressure = i;
                    parameters.impregnationTime = i;

                    controller.setParams(parameters);
                    ++i;
                }
                while (i<=100)
                {
                    parameters.cookingTime = i;
                    parameters.cookingPressure = i;

                    controller.setParams(parameters);
                    ++i;
                }
                while (i<=350)
                {
                    parameters.cookingPressure = i;
                    controller.setParams(parameters);
                    ++i;
                }
                
                ///Test for cooking temperature inputs
                for (double t = 20; t <= 80; t+=0.01)
                {
                    parameters.cookingTemperature = t;
                    controller.setParams(parameters);
                }
                for (double n = 0; n <= 1; n+=0.01)
                {
                    parameters.gain = n;
                    parameters.integrationTime = n;
                    controller.setParams(parameters);
                }

            } catch (Exception E)
            {
                Assert.Fail("Error thrown: " + E.Message);
            }
        }

        /// <summary>
        /// Test for using random string as URL. Should use simulator as default
        /// </summary>
        [TestMethod]
        public void URLTest()
        {
            controller.connectClient("hihihi");
            if (controller.getURL() != "opc.tcp://127.0.0.1:8087")
            {
                Assert.Fail("Should have used simulator URL");
            }
        }
    }
}
