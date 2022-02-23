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
            //Model.xmin = 7;

            // code to read first surface to get file data.        and populate them with initial data
            int header_length = 20; //Hard coded for now (20) 
            int count = System.IO.Directory.EnumerateFiles("C:\\Documents\\Drenthe").Count(); //number of files in directory
            //Model.NumberOfRows = 2;

            var file = Directory.GetFiles(@"C:\\Documents\\Drenthe", "*.*")
            .FirstOrDefault(f => f != @"C:\\Documents\\Drenthe\1"); //loads first file in directory

            double NoFileLines = File.ReadAllLines(file).Length; //Number of file lines in first file

            NoFileLines = NoFileLines - header_length;

            float dy = 0; //x resolution
            float dx = 0; //y resolution

            //int Ny = 0; //Number of y value
            //int Nx = 0; //Number of x values
            int file_length = 0; //to count the number of lines
            int total_file_length = 0; //file length incl. header
            int count_row = 0; //number of rows    

            //var x = new List<float>(); //x values
            //var y = new List<float>(); //y values
            var arrays1 = new List<float[]>(); //input array first file
            var mx1 = new List<float>(); //input x, y, z values first file
            var my1 = new List<float>();
            var mz1 = new List<float>();

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

                    file_length++;

                    // Add lineArray to main array
                    arrays1.Add(lineArray.ToArray());
                }
                total_file_length++;
            }

            Model.xmin = Convert.ToUInt64(arrays1[0][0]);
            Model.xmax = Convert.ToUInt64(arrays1[0][0]);

            Model.ymin = Convert.ToUInt64(arrays1[0][1]);
            Model.ymax = Convert.ToUInt64(arrays1[0][1]);

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
                if (Convert.ToUInt64(arrays1[i][0]) < Model.xmin)
                    Model.xmin = Convert.ToUInt64(arrays1[i][0]);

                //finds xmax
                if (Convert.ToUInt64(arrays1[i][0]) > Model.xmax)
                    Model.xmax = Convert.ToUInt64(arrays1[i][0]);

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
                    Model.x[i] = mx1[i];
                }
            }

            int count_y = 0;
            for (int i = 0; i < file_length; i++)
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
                    if (my1[i] == my1[k])
                    {
                        isDuplicate_y = true;
                        break;
                    }
                }

                if (!isDuplicate_y)
                {
                    Model.y[count_y] = my1[i];
                    count_y++;
                }

                Model.Nx = Model.x.Length;
            }


        }

        /*

            //Surface_model;
            //Surface_model Param = new SetParam();
            //var bsf = new Surface_model();


            //Model.Backstrip();
            //float ymax = 0;
            //Model.find_y_max(ymax, file_length, arrays1, Model.y);
            //Model.find_y_max(ymax, file_length, arrays1);
            // code to extract data from surface model and write it to file

            //Initialise xmin, xmax, ymin, ymax

            //number of unique x and y values

            //find_y

        //    Model.Nx = x.Count;
        //Model.Ny = Model.y.Count();

        //resolution
        dx = (Model.xmax - Model.xmin) / (Model.Nx - 1);
        dy = (Model.ymax - Model.ymin) / (Model.Ny - 1);

        double[,] raw_data_input = new double[(int)NoFileLines, 3]; //x,y,z input data for first file

        Model.no_hors_raw = count + 1; //number of files/horizons + one
        Model.sublayers_hors = 2;   //no. of sublayers per horizont. Can be non-equal but must then be hard coded for now.
        //int[] sublayers = new int[no_hors_raw]; //sublayers per horizon. Top layer is a dummy layer

        //var var_sublayers = new int[7] { 1, 2, 2, 2, 2, 2, 2 }; //to be changed

        //if var_sublayers not defined
        for (int i = 0; i < Model.no_hors_raw; i++)
        {
            if (i == 0)
                Model.sublayers[0] = 1;
            else
                Model.sublayers[i] = Model.sublayers_hors;
        }

        //if var_sublayers defined
        /*
        for (int i = 0; i < no_hors_raw; i++)
        {
           sublayers[i] = var_sublayers[i];
        }
        */

        /*
        int Nv = 0; //total number of horizons incl sublayers
        for (int i = 0; i < Model.no_hors_raw; i++)
            Nv = Nv + Model.sublayers[i];

        int[] modeltypesraw = new int[Nv];
        for (int i = 0; i < Model.no_hors_raw; i++)
            modeltypesraw[i] = 3;

        int raw_count = 0;
        int[] input_layer = new int[Nv]; //Layers that are input horizon marked with "1"

        for (int i = 0; i < Model.no_hors_raw - 1; i++)
        {
            if (i == 0)
                raw_count = 0;
            else
                raw_count = raw_count + Model.sublayers[i];

            input_layer[raw_count + 1] = 1;
        }

        //parametres used for the revil method
        Model.phi0_sd = 0.54;
        Model.phi0_sh = 0.65;
        double[] phics_sddraw = new double[Model.no_hors_raw];

        for (int i = 0; i < Model.no_hors_raw; i++)
        {
            phics_sddraw[i] = 0.45;
        }

        double[] agesraw = new double[Model.no_hors_raw];

        for (int i = 0; i < Model.no_hors_raw; i++)
        {
            agesraw[i] = Model.no_hors_raw - i;
        }

        double[,] bottomhist = new double[Nv, Nv];
        double[] agesfine = new double[Nv];
        double[] hors1dfine = new double[Nv];

        for (int i_counter = 0; i_counter < Nv; i_counter++)
        {
            agesfine[i_counter] = 0;
            hors1dfine[i_counter] = 0;
        }

        double[,] grid_x = new double[Model.Nx, Model.Ny];
        double[,] grid_y = new double[Model.Nx, Model.Ny];
        double[,] grid_z = new double[Model.Nx, Model.Ny];

        //double[,,] horsraw = new double[Model.Nx, Model.Ny, no_hors_raw]; //z data
        //double[,,] horsrawi = new double[Model.Nx, Model.Ny, no_hors_raw]; //z data moved down to make space for copy of top row
        double[,,,] input_xyz = new double[Model.Nx, Model.Ny, Nv, 3];//x, y, z data

        string[] filesnumber = Directory.GetFiles("C:\\Documents\\Drenthe");
        string[] file_name_sorted = new string[count];

        int no_files = 0;

        double[,] datamapi = new double[Model.Nx, Model.Ny];
        double[,] gooddata = new double[Model.Nx, Model.Ny];

        foreach (string filename in filesnumber) //reads all the files in directory (as for the top file above)
        {
            var data = System.IO.File.ReadAllText(filename);

            var arrays = new List<float[]>();
            var mx = new List<float>();
            var my = new List<float>();
            var mz = new List<float>();

            // Split data file content into lines
            var lines = data.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries); //number of lines
            int linearraycount = 0; //to count the number of lines

            int header_lines = 0;
            int elements = 0;
            foreach (var line in lines)
            {
                if (header_lines > header_length - 1)
                {
                    elements = 0;

                    var lineArray = new List<float>();

                    foreach (var s in line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        float flt1 = float.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
                        if (elements < 3)
                        {
                            lineArray.Add(flt1);
                            elements++;
                        }
                    }

                    linearraycount++;

                    arrays.Add(lineArray.ToArray());
                }
                header_lines++;
            }

            for (int i = 0; i < linearraycount; i++)
            {
                //Splits the data into x,y,z components
                mx1.Add(Convert.ToUInt64(arrays[i][0]));
                my.Add(Convert.ToUInt64(arrays[i][1]));
                mz.Add((arrays[i][2]));
            }

            //Since int values are needed for the counters below
            int Nx_int = Convert.ToInt32(Model.Nx);
            int Ny_int = Convert.ToInt32(Model.Ny);

            if (Nx_int == 0)
                Console.WriteLine("Warning Nx not defined");
            if (Ny_int == 0)
                Console.WriteLine("Warning Ny not defined");

            float i_count;
            float j_count;
            float i_count_rounded;
            float j_count_rounded;

            //creates x * y grids for x, y, z.
            for (int i = 0; i < linearraycount; i++)
            {
                i_count = (mx1[i] - Convert.ToInt32(Model.xmin)) / dx;
                j_count = (my[i] - Convert.ToInt32(Model.ymin)) / dy;
                if ((Math.Abs(i_count - Math.Round(i_count, 0)) < 0.01) && (Math.Abs(j_count - Math.Round(j_count, 0))) < 0.01)
                {
                    i_count_rounded = (float)Math.Round(i_count, 0);
                    j_count_rounded = (float)Math.Round(j_count, 0);
                    grid_x[Convert.ToInt32(i_count_rounded), Convert.ToInt32(j_count_rounded)] = mx1[i];
                    grid_y[Convert.ToInt32(i_count_rounded), Convert.ToInt32(j_count_rounded)] = my[i];
                    grid_z[Convert.ToInt32(i_count_rounded), Convert.ToInt32(j_count_rounded)] = mz[i];
                }
            }

            //initialize horsraw and horsrawi 
            for (int counter_x = 0; counter_x < Model.Nx; counter_x++)
            {
                for (int counter_y = 0; counter_y < Model.Ny; counter_y++)
                {
                    Model.horsraw[counter_x, counter_y, no_files] = grid_z[counter_x, counter_y];

                    input_xyz[counter_x, counter_y, no_files, 0] = grid_x[counter_x, counter_y];
                    input_xyz[counter_x, counter_y, no_files, 1] = grid_y[counter_x, counter_y];
                    input_xyz[counter_x, counter_y, no_files, 2] = grid_z[counter_x, counter_y];
                }
            }
            no_files++;
        }
        */



        //}

        //private class SetParam : Surface_model
        //{
        //public SetParam()
        //{
        //}
        //}
    }


}
