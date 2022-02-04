using System;
using System.Collections.Generic;
using System.Text;

namespace DFNGenerator_SharedCode
{
    class surface_point_data
    {
        public double Depth {get; set;}
        public double Porosity { get; set; }

        public double VClay { get; set; }

        public Tensor2S Strain { get; set; }

        public surface_point_data()
        {
            Strain = new Tensor2S();
        }

    }
}
