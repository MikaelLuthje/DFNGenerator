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


        public double[] Sort_Input(double[,,] horsraw, int Nx, int Ny)
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

    }
}