using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASE6030
{
    /// <summary>
    /// Class for saving the sequence parameters from user.
    /// 
    /// For now used only as a struct, but could also be used to handle the input type and range check.
    /// </summary>
    public class SequenceParameters
    {
        /// Impregnation time
        public int impregnationTime;

        /// Cooking time
        public int cookingTime;

        /// Cooking temperature
        public double cookingTemperature;

        /// Cooking pressure
        public int cookingPressure;

        /// PI Controller gain
        public double gain;

        /// PI controller integration time
        public double integrationTime;

    }
}
