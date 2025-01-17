﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DFNGenerator_SharedCode
{
    class Surface_model
    {
        public int NumberOfRows { get; set; }
        public int NumberOfColumns { get; set; }
        public float todelete { get; set; }
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
        public int sublayers_hors { get; set; }

        public int file_length { get; set; } //to count the number of lines

        public float[] mx1 { get; set; } //input x, y, z values first file
        public float[] my1 { get; set; }
        public float[] mz1 { get; set; }
        //public List<float[]> arrays1 { get; set; }

        //public float[,] arrays1 { get; set;}// new List<float[]>(); //input array first file

        //public List<float> y { get; set; }

        public float[] x { get; set; }

        public int count_y { get; set; }
        public int count_x { get; set; }

        public bool isDuplicate { get; set; }

        public float dx {get; set; } //x resolution
        public float dy { get; set; } //y resolution

        public int Nv { get; set; } //total number of horizons incl sublayer

        public int count { get; set; } //number of files in directory

        public double[,] grid_x { get; set; }
        public double[,] grid_y { get; set; }
        public double[,] grid_z { get; set; }

        //public double[,,] horsraw { get; set; } //z data
        //public double[,,] horsrawi { get; set; } //z data moved down to make space for copy of top row
        public double[,,,] input_xyz { get; set; }//x, y, z data

        public double[,] bottomhist { get; set; }
        public double[] agesfine { get; set; }
    //double[] hors1dfine = new double[Model.Nv];

        public int no_files { get; set; }

        public string[] file_name_sorted { get; set; }

        public string[] filesnumber { get; set; }

        public double[,] raw_data_input { get; set; }

        public int[] modeltypesraw { get; set; }

        public double[] phics_sddraw { get; set; }
        public double[] agesraw { get; set; }

        public int[] input_layer { get; set; } //Layers that are input horizon marked with "1"

        public int raw_count { get; set; }

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

        public void x_min_max()
        {
            for (int i = 0; i < file_length; i++)
            {
                //finds xmin
                if (mx1[i] < xmin)
                    xmin = mx1[i];

                //finds xmax
                if (mx1[i] > xmax)
                    xmax = mx1[i];

            }
        }


        public void y_min_max()
        {
            for (int i = 0; i < file_length; i++)
            {
                //finds ymin
                if (my1[i] < ymin)
                    ymin = my1[i];

                //finds ymax
                if (my1[i] > ymax)
                    ymax = my1[i];

            }
        }

        public void x_uniqe()
        {
            for (int i = 0; i < file_length; i++)
            {
                isDuplicate = false;

                //finds unique x values
                for (int k = 0; k < i; k++)
                {
                    if (mx1[i] == mx1[k])
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                if (!isDuplicate)
                {
                    count_x++;
                    x[count_x] = mx1[i];
                }
            }

        }

        public void y_uniqe()
        {
            for (int i = 0; i < file_length; i++)
            {
                isDuplicate = false;

                //finds unique x values
                for (int k = 0; k < i; k++)
                {
                    if (my1[i] == my1[k])
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                if (!isDuplicate)
                {
                    count_y++;
                    y[count_y] = my1[i];
                }
            }

        }

        public void Res_x()
        {
            dx = (xmax - xmin) / (Nx - 1);
        }

        public void Res_y()
        {
            dy = (ymax - ymin) / (Ny - 1);
        }


        private int subfunction()
        {
            return 1;
        }

        //public float find_y_max(float ymax, int file_length, List<float[]> arrays1, List<float> y)
        /*public float find_y_max(float ymax, int file_length, List<float[]> arrays1)
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

            //int 
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
                    //y.Add(my1[i]);
                }


            }
            return ymax;
        }
        */



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
