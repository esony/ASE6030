using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASE6030
{
    /// <summary>
    /// Class for saving the sequence parameters.
    /// 
    /// For now used only as a struct, but could also be used to handle the input type and range check.
    /// </summary>
    public class SequenceParameters
    {
        // Impregnation
        public int impregnationTime;

        // Cooking
        public int cookingTime;
        public double cookingTemperature;
        public int cookingPressure;

        // PI Controller
        public double gain;
        public double integrationTime;

    }
}
