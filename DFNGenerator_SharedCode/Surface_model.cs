using System;
using System.Collections.Generic;
using System.Text;

namespace DFNGenerator_SharedCode
{
    class Surface_model
    { 
        public int NumberOfRows { get; set; }
        public int NumberOfColumns { get; set; }
        public double[,] x_coordinates { get; set; }
        public double[,] y_coordinates { get; set; }

        public int NumberOfTimesteps { get; set; }
        public int NumberOfSurfaces { get; set; }

        public List<Surface> surfaces { get; set; }

        public void SetCoordinates()
        {
            // code to set x and y coordinates
        }


        public void AddSurface ()
        {
            Surface NewSurface = new Surface( NumberOfRows, NumberOfColumns);

            // code to set surface data, check position relative to other sufaces, etc


            surfaces.Add(NewSurface);
        }

        private int subfunction()
        {
            return 1;
        }


        public void Backstrip()
        {
            int d = subfunction();
            // code to do backstripping calculation 


        }


        public Surface_model(int NumRows,int NumCols)
        {
            NumberOfRows = NumRows;
            NumberOfColumns = NumCols;

            NumberOfTimesteps = 1;
            NumberOfSurfaces = 0;

            x_coordinates = new double[NumRows, NumCols];
            y_coordinates = new double[NumRows, NumCols];

            surfaces = new List<Surface>();

        }

    }
}
