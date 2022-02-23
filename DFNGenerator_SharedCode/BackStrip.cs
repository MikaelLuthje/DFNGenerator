using System;
//using LumenWorks.Framework.IO.Csv;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace DFNGenerator_SharedCode
{
    class Backstrip_functions
    {
        public double[] Backstrip_refine_hors1dfine(double[] hors1dfine, double[,,] horsrawi, int[] sublayers, int i, int j, int no_hors_raw)
        {

            double raw_top, raw_bot;
            int hfine = 0;
            int bound;

            for (int h = 0; h < no_hors_raw; h++)
            {
                raw_bot = horsrawi[i, j, h];

                if (h < no_hors_raw - 1)
                {
                    raw_top = horsrawi[i, j, h + 1];
                }
                else
                    raw_top = 0;

                bound = sublayers[h];

                for (int hfinelocal = 0; hfinelocal < bound; hfinelocal++)
                {
                    hors1dfine[hfine] = raw_bot - (raw_bot - raw_top) * (hfinelocal) / sublayers[h];
                    hfine++;
                }
            }

            return hors1dfine;
        }

        public double[,,] copy_surface(int NumberOfHorizon, int Nx, int Ny, double[,,] horsraw, double[,,] horsrawi)
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

            return horsrawi;


        }


        /*
         * public double[] Sort_Input(double[,,] horsraw, int Nx, int Ny, int no_hors_raw)
        {
            //finds how the input data should be sorted so the deepest data is in top of the arrays
            Boolean isArrayEqual = false;
            double[] values_to_sort_1 = new double[no_hors_raw];
            double[] values_to_sort_key_1 = new double[no_hors_raw];
            double[] values_to_sort_2 = new double[no_hors_raw];
            double[] values_to_sort_key_2 = new double[no_hors_raw];

            int random_count = 0;
            while (isArrayEqual == false)
            {
                //select 2 random locations if the horizons are cutting
                Random rnd = new Random();
                int x1 = rnd.Next(1, Nx - 1);
                int x2 = rnd.Next(1, Nx - 1);
                int y1 = rnd.Next(1, Ny - 1);
                int y2 = rnd.Next(1, Ny - 1);

                for (int i = 0; i < no_hors_raw; i++)
                {
                    values_to_sort_1[i] = horsraw[x1, y1, i];
                    values_to_sort_key_1[i] = i;
                }

                Array.Sort(values_to_sort_1, values_to_sort_key_1);
                Array.Reverse(values_to_sort_1);
                Array.Reverse(values_to_sort_key_1);

                for (int i = 0; i < no_hors_raw; i++)
                {
                    values_to_sort_2[i] = horsraw[x2, y2, i];
                    values_to_sort_key_2[i] = i;
                }

                Array.Sort(values_to_sort_2, values_to_sort_key_2);
                Array.Reverse(values_to_sort_2);
                Array.Reverse(values_to_sort_key_2);

                isArrayEqual = values_to_sort_key_1.SequenceEqual(values_to_sort_key_2);

                if (random_count > 10)
                    break; random_count++; // increment
            }
        }
        */

        public float find_y_min(float ymin, List<float[]> arrays1, int file_length, List<float> y)
        {
            ymin = Convert.ToUInt64(arrays1[0][1]);

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


            for (int i = 0; i < file_length; i++)
            {
                //finds ymin
                if (Convert.ToUInt64(arrays1[i][1]) < ymin)
                    ymin = Convert.ToUInt64(arrays1[i][1]);

                bool isDuplicate_y = false;

                //finds unique y values
                for (int k = 0; k < i; k++)
                {
                    if (my1[i] == my1[k])
                    {
                        isDuplicate_y = true;
                        break;
                    }
                }

                if (!isDuplicate_y)
                {
                    y.Add(my1[i]);
                }


            }

            return ymin;
        }


        public float find_y_max(float ymax, List<float[]> arrays1, int file_length, List<float> y)
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





        public float find_x_max(float xmax, List<float[]> arrays1, int file_length, List<float> x)
        {
            xmax = Convert.ToUInt64(arrays1[0][0]);

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

            for (int i = 0; i < file_length; i++)
            {
                //finds xmax
                if (Convert.ToUInt64(arrays1[i][0]) > xmax)
                    xmax = Convert.ToUInt64(arrays1[i][0]);

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
                    x.Add(mx1[i]);
                }
            }

            return xmax;

        }

        public float find_x_min(float xmin, List<float[]> arrays1, int file_length, List<float> x)
        {
            xmin = Convert.ToUInt64(arrays1[0][0]);

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

            for (int i = 0; i < file_length; i++)
            {
                //finds xmin
                if (Convert.ToUInt64(arrays1[i][0]) < xmin)
                    xmin = Convert.ToUInt64(arrays1[i][0]);

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
                    x.Add(mx1[i]);
                }


            }
            return xmin;

        }

    }
}