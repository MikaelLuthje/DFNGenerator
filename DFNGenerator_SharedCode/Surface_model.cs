using System;
using System.Collections.Generic;
using System.Text;

namespace DFNGenerator_SharedCode
{
    class Surface_model
    {
        public int NumberOfRows { get; set; }
        public int NumberOfColumns { get; set; }
        public float xmin { get; set; }
        public float xmax { get; set; }
        public float ymin { get; set; }
        public float ymax { get; set; }

        public double[] hors1dfine { get; set; }
        public int Nx { get; set; }

        public double phi0_sd { get; set; }
        public double phi0_sh { get; set; }

        public int Ny { get; set; }
        public double[,] x_coordinates { get; set; }
        public double[,] y_coordinates { get; set; }

        public int[] sublayers { get; set; }

        public int NumberOfHorizon { get; set; }

        public double[,,] horsraw { get; set; }
        public double[,,] horsrawi { get; set; }
        public int NumberOfTimesteps { get; set; }
        public int NumberOfSurfaces { get; set; }

        public int no_hors_raw { get; set; }

        public List<Surface> surfaces { get; set; }

        public void SetCoordinates()
        {
            // code to set x and y coordinates
        }


        public void AddSurface()
        {
            Surface NewSurface = new Surface(NumberOfRows, NumberOfColumns);

            // code to set surface data, check position relative to other sufaces, etc


            surfaces.Add(NewSurface);
        }

        private int subfunction()
        {
            return 1;
        }

        private void copy_surface()
        {
            //copies data
            for (int j = 0; j < NumberOfHorizon - 1; j++)
            {
                for (int counter_x = 0; counter_x < Nx; counter_x++)
                {
                    for (int counter_y = 0; counter_y < Ny; counter_y++)
                    {
                        horsrawi[counter_x, counter_y, j + 1] = horsraw[counter_x, counter_y, j];

                    }
                }
            }

            //copies top row
            for (int counter_x = 0; counter_x < Nx; counter_x++)
            {
                for (int counter_y = 0; counter_y < Ny; counter_y++)
                {
                    horsrawi[counter_x, counter_y, 0] = horsrawi[counter_x, counter_y, 1];
                }
            }
        }


        public void Backstrip()
        {
            int d = subfunction();
            // code to do backstripping calculation 

            var bsf = new Backstrip_functions();

            for (int i = 1; i < Nx - 1; i++)
            {
                for (int j = 0; j < Ny; j++)
                {
                    hors1dfine = bsf.Backstrip_refine_hors1dfine(hors1dfine, horsrawi, sublayers, i - 1, j, no_hors_raw); //Calculates depth of sublayers
                    //agesfine = bsf.Backstrip_refine_agesfine(agesfine, agesraw, sublayers, no_hors_raw); //calculates "age" of all layers
                    //bottomhist = bsf.Backstrip_refine(bottomhist, hors1dfine, phi0_sd, phi0_sh, phics_sdfine, phics_shfine, svsfine, modeltypesfine);

                }
            }
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
