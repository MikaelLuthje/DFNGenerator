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
        public float[] y { get; set; }
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
        public int sublayers_hors { get; set;}
        
        //public List<float[]> arrays1 { get; set; }

        //public List<float> y { get; set; }

        public void SetCoordinates()
        {
            // code to set x and y coordinates
        }


        public void AddSurface()
        {
            Surface NewSurface = new Surface(NumberOfRows, NumberOfColumns);
            // code to set surface data, check position relative to other sufaces, etc


            //surfaces.Add(NewSurface);
        }

        //public void SetParam()
        //{
        //    Para NewPara = new  
        //    
        //}

        private int subfunction()
        {
            return 1;
        }

        //public float find_y_max(float ymax, int file_length, List<float[]> arrays1, List<float> y)
        public float find_y_max(float ymax, int file_length, List<float[]> arrays1)
        {
            ymax = Convert.ToUInt64(arrays1[0][1]);


            var mx1 = new List<float>();
            var my1 = new List<float>();
            var mz1 = new List<float>();

            for (int i = 0; i < file_length; i++)
            {
                //Splits the data into x,y,z components
                mx1.Add(Convert.ToUInt64(arrays1[i][0]));
                my1.Add(Convert.ToUInt64(arrays1[i][1]));
                mz1.Add((arrays1[i][2]));
            }

            int 
            for (int i = 0; i < file_length; i++)
            {
                //finds ymax
                if (Convert.ToUInt64(arrays1[i][1]) > ymax)
                    ymax = Convert.ToUInt64(arrays1[i][1]);

                bool isDuplicate_x = false;

                //finds unique x values
                for (int k = 0; k < i; k++)
                {
                    if (mx1[i] == mx1[k])
                    {
                        isDuplicate_x = true;
                        break;
                    }
                }

                if (!isDuplicate_x)
                {
                    y.Add(my1[i]);
                }


            }
            return ymax;
        }



        public void Backstrip()
        {
            int d = subfunction();
            // code to do backstripping calculation 

            var bsf = new Backstrip_functions();

            horsrawi = bsf.copy_surface(NumberOfHorizon, Nx, Ny, horsraw, horsrawi);

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
