using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASE6030
{
    class SequenceParameters
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
