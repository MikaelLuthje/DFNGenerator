using System;
using System.Collections.Generic;
using System.Text;

namespace DFNGenerator_SharedCode
{
    class SurfaceTimestep
    {
        public surface_point_data[,] SurfaceData;

        public SurfaceTimestep(int i, int j) {
            SurfaceData = new surface_point_data[i, j];
        }
    }

    class Surface
    {
        public List<SurfaceTimestep> surface;

        public Surface(int i, int j)
        {
            surface = new List<SurfaceTimestep> ();

            SurfaceTimestep InitialSurface = new SurfaceTimestep(i,j);
            surface.Add(InitialSurface);
        }


    }

    /*class x_data
    {
        public surface_point_data[] SurfaceData;

        public SurfaceTimestep(int x)
        {
            SurfaceData = new surface_point_data[i, j];
        }
    }

    class Mx
    {
        public List<SurfaceTimestep> surface;

        public Surface(int i, int j)
        {
            surface = new List<SurfaceTimestep>();

            SurfaceTimestep InitialSurface = new SurfaceTimestep(i, j);
            surface.Add(InitialSurface);
        }


    }
    */
    //class SetPara
    //{
    //
    //}
}
