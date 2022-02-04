using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DFNGenerator_SharedCode;

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

            // code to add surfaces and populate them with initial data

            Model.Backstrip();

            // code to extract data from surface model and write it to file

        }
    }
}
