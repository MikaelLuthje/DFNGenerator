using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DFNGenerator_SharedCode;
//using System.Linq;

namespace DFNGenerator_Standalone
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            int NumberOfRow = 5;
            int NumberOfColumns = 5;

            Surface_model Model = new Surface_model(NumberOfRow, NumberOfColumns);


            // code to read first surface to get file data and populate them with initial data
            int header_length = 20; //Hard coded for now (20) 
            Model.count = System.IO.Directory.EnumerateFiles("C:\\Documents\\Drenthe").Count(); //number of files in directory

            var file = Directory.GetFiles(@"C:\\Documents\\Drenthe", "*.*")
            .FirstOrDefault(f => f != @"C:\\Documents\\Drenthe\1"); //loads first file in directory

            double NoFileLines = File.ReadAllLines(file).Length; //Number of file lines in first file

            NoFileLines = NoFileLines - header_length;

            Model.Ny = 0; //Number of y value
            Model.Nx = 0; //Number of x values

            Model.file_length = 0; //to count the number of lines
            
            int total_file_length = 0; //file length incl. header
            int count_row = 0; //number of rows    

            var x = new List<float>(); //x values
            var y = new List<float>(); //y values
            var arrays1 = new List<float[]>(); //input array first file

            //var mx1 = new List<float>(); //input x, y, z values first file
            //var my1 = new List<float>();
            //var mz1 = new List<float>();
            
            foreach (string line in System.IO.File.ReadLines(file))
            {
                if (total_file_length > header_length - 1) //to remove header lines
                {
                    count_row = 0;
                    // Create a new List<float> representing all the comma separated numbers in this line
                    var lineArray = new List<float>();

                    foreach (var s in line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        float flt1 = float.Parse(s, System.Globalization.CultureInfo.InvariantCulture);

                        if (count_row < 3) //reads only the first three rows
                        {
                            lineArray.Add(flt1);
                            count_row++;
                        }
                    }

                    arrays1.Add(lineArray.ToArray());
                    
                    //float test = Convert.ToUInt64(arrays1[Model.file_length][0]);
                    //Model.todelete = test;
                    //Model.mx1[2] = test;
                    //Model.mx1[Model.file_length] = test;
                    //Model.mx1[Model.file_length] = (Convert.ToUInt64(arrays1[Model.file_length][0]));
                    //Model.my1[Model.file_length] = (Convert.ToUInt64(arrays1[Model.file_length][1]));
                    //Model.my1[Model.file_length] = (arrays1[Model.file_length][2]);

                    Model.file_length++;
                }
                total_file_length++;
            }

            //*/

            Model.xmin = Convert.ToUInt64(arrays1[0][0]);
            Model.xmax = Convert.ToUInt64(arrays1[0][0]);
            Model.ymin = Convert.ToUInt64(arrays1[0][1]);
            Model.xmax = Convert.ToUInt64(arrays1[0][1]);
            //Model.xmin = Model.mx1[0];
            //Model.xmax = Model.mx1[0];
            //Model.ymin = Model.my1[0];
            //Model.xmax = Model.my1[0];

            //Model.x_min_max();
            //Model.y_min_max();


            for (int i = 0; i < Model.file_length; i++)
            {
                //finds xmin
                if (Convert.ToUInt64(arrays1[i][0]) < Model.xmin)
                    Model.xmin = Convert.ToUInt64(arrays1[i][0]);

                //finds xmax
                if (Convert.ToUInt64(arrays1[i][0]) > Model.xmax)
                    Model.xmax = Convert.ToUInt64(arrays1[i][0]);

                bool isDuplicate_x = false;

                //finds unique x values
                for (int k = 0; k < i; k++)
                {
                    if (arrays1[i][0] == arrays1[k][0])
                    {
                        isDuplicate_x = true;
                        break;
                    }
                }

                if (!isDuplicate_x)
                {
                    x.Add(arrays1[i][0]);
                }
            }

            for (int i = 0; i < Model.file_length; i++)
            {
                //finds ymin
                if (Convert.ToUInt64(arrays1[i][1]) < Model.ymin)
                    Model.ymin = Convert.ToUInt64(arrays1[i][1]);

                //finds ymax
                if (Convert.ToUInt64(arrays1[i][1]) > Model.ymax)
                    Model.ymax = Convert.ToUInt64(arrays1[i][1]);

                bool isDuplicate_y = false;

                //finds unique y values
                for (int k = 0; k < i; k++)
                {
                    if (arrays1[i][1] == arrays1[k][1])
                    {
                        isDuplicate_y = true;
                        break;
                    }
                }

                if (!isDuplicate_y)
                {
                    y.Add(arrays1[i][1]);
                }
            }

            Model.Nx = x.Count;
            Model.Ny = y.Count;

            //Model.count_y = 0;
            //Model.count_x = 0;
            //Model.isDuplicate = true;

            //Model.x_uniqe();
            //Model.y_uniqe();

            //resolution
            Model.Res_x();
            Model.Res_y();

            Model.no_hors_raw = Model.count + 1; //number of files/horizons + one
            Model.sublayers_hors = 2;   //no. of sublayers per horizont. Can be non-equal but must then be hard coded for now.

            //if var_sublayers not defined
            for (int i = 0; i < Model.no_hors_raw; i++)
            {
                if (i == 0)
                    Model.sublayers[0] = 1;
                else
                    Model.sublayers[i] = Model.sublayers_hors;
            }

            Model.Nv = 0; //total number of horizons incl sublayers
            for (int i = 0; i < Model.no_hors_raw; i++)
                Model.Nv = Model.Nv + Model.sublayers[i];

            //int[] modeltypesraw = new int[Model.Nv];
            for (int i = 0; i < Model.no_hors_raw; i++)
                Model.modeltypesraw[i] = 3;

            Model.raw_count = 0;
            //int[] input_layer = new int[Model.Nv]; //Layers that are input horizon marked with "1"

            for (int i = 0; i < Model.no_hors_raw - 1; i++)
            {
                if (i == 0)
                    Model.raw_count = 0;
                else
                    Model.raw_count = Model.raw_count + Model.sublayers[i];

                Model.input_layer[Model.raw_count + 1] = 1;
            }

            //parametres used for the revil method
            Model.phi0_sd = 0.54;
            Model.phi0_sh = 0.65;

            for (int i = 0; i < Model.no_hors_raw; i++)
            {
                Model.phics_sddraw[i] = 0.45;
            }

            for (int i = 0; i < Model.no_hors_raw; i++)
            {
                Model.agesraw[i] = Model.no_hors_raw - i;
            }

            for (int i_counter = 0; i_counter < Model.Nv; i_counter++)
            {
                Model.agesfine[i_counter] = 0;
                Model.hors1dfine[i_counter] = 0;
                
            }

            Model.filesnumber = Directory.GetFiles("C:\\Documents\\Drenthe");
            Model.file_name_sorted = new string[Model.count];

            Model.no_files = 0;
        }
    }
}
