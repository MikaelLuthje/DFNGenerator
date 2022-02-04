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
        int IN(double i, double j, double I)
        {
            return Convert.ToInt32(j * I + i);
        }

        double getuz(double[] bottomhist, double t, double dint, int Nv)
        {
            double uzout = 0, uzabove, uzbelow, currenthor, abovehor, belowhor, dly;
            double indmin, indmax, mid;

            indmin = 1; indmax = t + 1;

            while (indmax - indmin > 1)
            {
                double temp = 0.5 * (indmax + indmin);
                mid = Math.Floor(temp);
                int temp_to_IN = IN(mid - 1, t - 1, Nv);
                currenthor = bottomhist[temp_to_IN];

                if (currenthor > dint)
                {
                    indmin = mid;
                }
                else indmax = mid;
            }

            if (dint < bottomhist[IN(0, t - 1, Nv)])
            {
                abovehor = bottomhist[IN(indmax - 1, t - 1, Nv)];
                belowhor = bottomhist[IN(indmin - 1, t - 1, Nv)];

                if (Math.Abs(belowhor - abovehor) > 1e-5)
                {
                    dly = (dint - abovehor) / (belowhor - abovehor);
                }
                else dly = 0;

                uzabove = bottomhist[IN(indmax - 1, t, Nv)] - bottomhist[IN(indmax - 1, t - 1, Nv)];
                uzbelow = bottomhist[IN(indmin - 1, t, Nv)] - bottomhist[IN(indmin - 1, t - 1, Nv)];
                uzout = uzabove * (1 - dly) + uzbelow * dly;
            }

            else uzout = bottomhist[IN(0, t, Nv)] - bottomhist[IN(0, t - 1, Nv)];
            return uzout;

        }
        public double[] getstraincolumn3d1_F31(double[,] bottomhist_W, double[,] bottomhist_C, double[,] bottomhist_E, double[,] bottomhist_S, double[,] bottomhist_N, float dx, float dy, int lmin, int lmax, int tmax, int Nv)
        {

            double dint, dz, uzup, uzcenter, uzwest, uzeast, uzsouth, uznorth, Finc31, Finc32, Finc33, F31, F32, F33, Facc;
            double[] F31out = new double[Nv - 1];

            double[] rhs = new double[1];
            double[] lhs = new double[3];

            dz = 1e-3;

            double[] bottomhistcenter = new double[Nv * Nv];
            double[] bottomhistwest = new double[Nv * Nv];
            double[] bottomhisteast = new double[Nv * Nv];
            double[] bottomhistnorth = new double[Nv * Nv];
            double[] bottomhistsouth = new double[Nv * Nv];
            int iii = 0;
            for (int ii = 0; ii < Nv; ii++)
            {
                for (int jj = 0; jj < Nv; jj++)
                {
                    bottomhistcenter[iii] = bottomhist_C[ii, jj];
                    bottomhisteast[iii] = bottomhist_E[ii, jj];
                    bottomhistwest[iii] = bottomhist_W[ii, jj];
                    bottomhistnorth[iii] = bottomhist_N[ii, jj];
                    bottomhistsouth[iii] = bottomhist_S[ii, jj];
                    iii++;
                }
            }

            for (int l = (int)lmin; l <= (int)lmax; l++)
            {
                F31 = 0.0; F32 = 0.0; F33 = 1.0; Facc = 1.0;

                for (int t = l - 1; t <= tmax; t++)
                {
                    dint = bottomhist_C[t - 1, l - 1];

                    int matrix_size = lmax * 9;

                    dint = bottomhistcenter[IN(l - 1, t - 1, Nv)];

                    uzcenter = getuz(bottomhistcenter, t, dint, Nv);
                    uzup = getuz(bottomhistcenter, t, dint - dz, Nv);

                    uzwest = getuz(bottomhistwest, t, dint, Nv);

                    uzeast = getuz(bottomhisteast, t, dint, Nv);

                    uzsouth = getuz(bottomhistsouth, t, dint, Nv);

                    uznorth = getuz(bottomhistnorth, t, dint, Nv);

                    Finc31 = (uzeast - uzwest) / dx; Finc32 = (uznorth - uzsouth) / dy; Finc33 = 1.0 + (uzcenter - uzup) / dz;

                    F31 = Finc31 + Finc33 * F31;
                    F32 = Finc32 + Finc33 * F32;
                    F33 = Finc33 * F33;
                }

                F31out[l - lmin] = F31;
            }

            return F31out;
        }

        public double[] getstraincolumn3d1_F32(double[,] bottomhist_W, double[,] bottomhist_C, double[,] bottomhist_E, double[,] bottomhist_S, double[,] bottomhist_N, float dx, float dy, int lmin, int lmax, int tmax, int Nv)
        {
            double dint, dz, uzup, uzcenter, uzwest, uzeast, uzsouth, uznorth, Finc31, Finc32, Finc33, F31, F32, F33, Facc;
            double[] F32out = new double[Nv - 1];

            double[] rhs = new double[1];
            double[] lhs = new double[3];

            dz = 1e-3;

            double[] bottomhistcenter = new double[Nv * Nv];
            double[] bottomhistwest = new double[Nv * Nv];
            double[] bottomhisteast = new double[Nv * Nv];
            double[] bottomhistnorth = new double[Nv * Nv];
            double[] bottomhistsouth = new double[Nv * Nv];
            int iii = 0;
            for (int ii = 0; ii < Nv; ii++)
            {
                for (int jj = 0; jj < Nv; jj++)
                {
                    bottomhistcenter[iii] = bottomhist_C[ii, jj];
                    bottomhisteast[iii] = bottomhist_E[ii, jj];
                    bottomhistwest[iii] = bottomhist_W[ii, jj];
                    bottomhistnorth[iii] = bottomhist_N[ii, jj];
                    bottomhistsouth[iii] = bottomhist_S[ii, jj];
                    iii++;
                }
            }

            for (int l = (int)lmin; l <= (int)lmax; l++)
            {
                F31 = 0.0; F32 = 0.0; F33 = 1.0; Facc = 1.0;

                for (int t = l - 1; t <= tmax; t++)
                {
                    dint = bottomhist_C[t - 1, l - 1];

                    int matrix_size = lmax * 9;

                    dint = bottomhistcenter[IN(l - 1, t - 1, Nv)];

                    uzcenter = getuz(bottomhistcenter, t, dint, Nv);
                    uzup = getuz(bottomhistcenter, t, dint - dz, Nv);

                    uzwest = getuz(bottomhistwest, t, dint, Nv);

                    uzeast = getuz(bottomhisteast, t, dint, Nv);

                    uzsouth = getuz(bottomhistsouth, t, dint, Nv);

                    uznorth = getuz(bottomhistnorth, t, dint, Nv);

                    Finc31 = (uzeast - uzwest) / dx;
                    Finc32 = (uznorth - uzsouth) / dy;
                    Finc33 = 1.0 + (uzcenter - uzup) / dz;

                    F31 = Finc31 + Finc33 * F31;
                    F32 = Finc32 + Finc33 * F32;
                    F33 = Finc33 * F33;
                }
                F32out[l - lmin] = F32;

            }
            return F32out;
        }

        public double[] getstraincolumn3d1_F33(double[,] bottomhist_W, double[,] bottomhist_C, double[,] bottomhist_E, double[,] bottomhist_S, double[,] bottomhist_N, float dx, float dy, int lmin, int lmax, int tmax, int Nv)
        {
            double dint, dz, uzup, uzcenter, uzwest, uzeast, uzsouth, uznorth, Finc31, Finc32, Finc33, F31, F32, F33, Facc;
            double[] F33out = new double[Nv - 1];

            double[] rhs = new double[1];
            double[] lhs = new double[3];

            dz = 1e-3;

            double[] bottomhistcenter = new double[Nv * Nv];
            double[] bottomhistwest = new double[Nv * Nv];
            double[] bottomhisteast = new double[Nv * Nv];
            double[] bottomhistnorth = new double[Nv * Nv];
            double[] bottomhistsouth = new double[Nv * Nv];
            int iii = 0;
            for (int ii = 0; ii < Nv; ii++)
            {
                for (int jj = 0; jj < Nv; jj++)
                {
                    bottomhistcenter[iii] = bottomhist_C[ii, jj];
                    bottomhisteast[iii] = bottomhist_E[ii, jj];
                    bottomhistwest[iii] = bottomhist_W[ii, jj];
                    bottomhistnorth[iii] = bottomhist_N[ii, jj];
                    bottomhistsouth[iii] = bottomhist_S[ii, jj];
                    iii++;
                }
            }

            for (int l = (int)lmin; l <= (int)lmax; l++)
            {
                F31 = 0.0; F32 = 0.0; F33 = 1.0; Facc = 1.0;

                for (int t = l - 1; t <= tmax; t++)
                {
                    dint = bottomhist_C[t - 1, l - 1];

                    int matrix_size = lmax * 9;

                    dint = bottomhistcenter[IN(l - 1, t - 1, Nv)];

                    uzcenter = getuz(bottomhistcenter, t, dint, Nv);
                    uzup = getuz(bottomhistcenter, t, dint - dz, Nv);

                    uzwest = getuz(bottomhistwest, t, dint, Nv);

                    uzeast = getuz(bottomhisteast, t, dint, Nv);

                    uzsouth = getuz(bottomhistsouth, t, dint, Nv);
                    ;
                    uznorth = getuz(bottomhistnorth, t, dint, Nv);

                    Finc31 = (uzeast - uzwest) / dx;
                    Finc32 = (uznorth - uzsouth) / dy;
                    Finc33 = 1.0 + (uzcenter - uzup) / dz;

                    F31 = Finc31 + Finc33 * F31;
                    F32 = Finc32 + Finc33 * F32;
                    F33 = Finc33 * F33;

                }
                F33out[l - lmin] = F33;

            }
            return F33out;
        }

        public double[] getstraincolumn3d1_Facc(double[,] bottomhist_W, double[,] bottomhist_C, double[,] bottomhist_E, double[,] bottomhist_S, double[,] bottomhist_N, float dx, float dy, int lmin, int lmax, int tmax, int Nv)
        {
            int t, l;
            double dint, dz, uzup, uzcenter, uzwest, uzeast, uzsouth, uznorth, Finc31, Finc32, Finc33, F31, F32, F33, Facc;

            double[] rhs = new double[1];
            double[] lhs = new double[3];
            double[] Faccout = new double[Nv - 1];

            Facc = 0;

            dz = 1e-3;
            for (l = (int)lmin; l <= (int)lmax; l++)
            {
                F31 = 0.0; F32 = 0.0; F33 = 1.0; Facc = 1.0;

                for (t = l - 1; t <= tmax; t++)
                {
                    dint = bottomhist_C[l - 1, t - 1];

                    int matrix_size = Nv * Nv;
                    double[] bottomhistcenter = new double[matrix_size];
                    bottomhistcenter[IN(l - 1, t - 1, Nv)] = bottomhist_C[l - 1, t - 1];
                    uzcenter = getuz(bottomhistcenter, t, dint, Nv);
                    uzup = getuz(bottomhistcenter, t, dint - dz, Nv);

                    double[] bottomhistwest = new double[matrix_size];
                    bottomhistwest[IN(l - 1, t - 1, Nv)] = bottomhist_W[l - 1, t - 1];
                    uzwest = getuz(bottomhistwest, t, dint, Nv);

                    double[] bottomhisteast = new double[matrix_size];
                    bottomhisteast[IN(l - 1, t - 1, Nv)] = bottomhist_E[l - 1, t - 1];
                    uzeast = getuz(bottomhisteast, t, dint, Nv);

                    double[] bottomhistsouth = new double[matrix_size];
                    bottomhistsouth[IN(l - 1, t - 1, Nv)] = bottomhist_S[l - 1, t - 1];
                    uzsouth = getuz(bottomhistsouth, t, dint, Nv);

                    double[] bottomhistnorth = new double[matrix_size];
                    bottomhistnorth[IN(l - 1, t - 1, Nv)] = bottomhist_N[l - 1, t - 1];
                    uznorth = getuz(bottomhistnorth, t, dint, Nv);

                    Finc31 = (uzeast - uzwest) / dx; Finc32 = (uznorth - uzsouth) / dy; Finc33 = 1.0 + (uzcenter - uzup) / dz;

                    F31 = Finc31 + Finc33 * F31;
                    F32 = Finc32 + Finc33 * F32;
                    F33 = Finc33 * F33;
                }

                Faccout[l - lmin] = Facc;
            }
            return Faccout;
        }
        public double[,] revil_backstrip(double[,] bottomhist, double[] bottoms, int Nv_50, double phi0_sd, double phi0_sh, double[] phics_sd, double[] phics_sh, double[] svs, int[] modeltypes)
        {

            double beta_sd = 6e-8;
            double beta_sh = 4e-8; //Values from Revil paper
            double rhog = 2650;
            double rhof = 1050;
            double g = 9.82;

            int Nl = bottoms.Length;

            Nv_50 = bottoms.Length;
            double[] layermasses = new double[Nv_50];

            for (int i = 0; i < Nv_50; i++)
                layermasses[i] = 0 * bottoms[i];

            int dznom = 10; //Nominal integration step. Decrease for higher precision.
            double z = 0;
            int l = Nl;
            double phi_sd = phi0_sd;
            double phi_sh = phi0_sh;
            double dz;
            int changelayer;
            double sv = svs[0];

            double phi_b = getbulkpor(phi_sd, phi_sh, sv, rhog, modeltypes[l - 1], 0);

            double phi_sd_dsig = 0;
            double phi_sd_dsigold = 0;
            double phi_sh_dsig = 0;
            double phi_sh_dsigold = 0;
            double phi_sd2_dsig2 = 0;
            double phi_sh2_dsig2 = 0;
            double dsigold = 0;

            double localmass = 0;

            double phic_sd;
            double phic_sh;

            while (l >= 1)
            {
                phic_sd = phics_sd[l - 1];
                phic_sh = phics_sh[l - 1];
                sv = svs[l - 1];

                if (z + dznom <= bottoms[l - 1])
                {
                    dz = dznom;
                    changelayer = 0;
                }
                else
                {
                    dz = bottoms[l - 1] - z;
                    changelayer = 1;
                }

                localmass = localmass + (1 - phi_b) * dz;


                double dsig = (rhog - rhof) * (1 - phi_b) * g * dz;
                //Change in normal stress ('pressure') over dz

                if (dsig > 0) //Find derivative of porosity with respect to normal stress (dphi_dsig)
                {
                    phi_sd_dsig = -(phi_sd - phic_sd) * beta_sd;
                    phi_sh_dsig = -(phi_sh - phic_sh) * beta_sh;
                }

                if (dsigold > 0)
                {
                    phi_sd2_dsig2 = (phi_sd_dsig - phi_sd_dsigold) / dsigold;
                    phi_sh2_dsig2 = (phi_sh_dsig - phi_sh_dsigold) / dsigold;
                }
                phi_sd = phi_sd + phi_sd_dsig * dsig + phi_sd2_dsig2 * dsig * dsig * 0.5;
                phi_sh = phi_sh + phi_sh_dsig * dsig + phi_sh2_dsig2 * dsig * dsig * 0.5;
                phi_b = getbulkpor(phi_sd, phi_sh, sv, rhog, modeltypes[l - 1], z + dz);
                dsigold = dsig;
                phi_sd_dsigold = phi_sd_dsig;
                phi_sh_dsigold = phi_sh_dsig;

                if (changelayer == 1)
                {
                    layermasses[l - 1] = localmass;
                    localmass = 0;
                    l = l - 1;
                }
                z = z + dz;
            }

            for (int t = 0; t < Nl; t++)  // Time loop
            {
                z = 0;
                l = t;

                sv = svs[1];

                phi_sd = phi0_sd;
                phi_sh = phi0_sh;

                phi_b = getbulkpor(phi_sd, phi_sh, sv, rhog, modeltypes[l], 0);

                phi_sd_dsig = 0;
                phi_sd_dsigold = 0;
                phi_sh_dsig = 0;
                phi_sh_dsigold = 0;
                phi_sd2_dsig2 = 0;
                phi_sh2_dsig2 = 0;
                dsigold = 0;

                localmass = 0;
                while (l >= 0)  // layer loop
                {
                    phic_sd = phics_sd[l];
                    phic_sh = phics_sh[l];
                    sv = svs[l];

                    double newmass = localmass + (1 - phi_b) * dznom;

                    if (newmass <= layermasses[l])
                    {
                        dz = dznom;
                        localmass = newmass;
                        changelayer = 0;
                    }
                    else
                    {
                        dz = (layermasses[l] - localmass) / (newmass - localmass) * dznom;
                        changelayer = 1;
                    }

                    double dsig = (rhog - rhof) * (1 - phi_b) * g * dz;

                    if (dsig > 0)
                    {
                        phi_sd_dsig = -(phi_sd - phic_sd) * beta_sd;
                        phi_sh_dsig = -(phi_sh - phic_sh) * beta_sh;
                    }

                    if (dsigold > 0)
                    {
                        phi_sd2_dsig2 = (phi_sd_dsig - phi_sd_dsigold) / dsigold;
                        phi_sh2_dsig2 = (phi_sh_dsig - phi_sh_dsigold) / dsigold;
                    }
                    phi_sd = phi_sd + phi_sd_dsig * dsig + phi_sd2_dsig2 * dsig * dsig * 0.5;
                    phi_sh = phi_sh + phi_sh_dsig * dsig + phi_sh2_dsig2 * dsig * dsig * 0.5;
                    phi_b = getbulkpor(phi_sd, phi_sh, sv, rhog, modeltypes[l], z + dz);
                    dsigold = dsig;
                    phi_sd_dsigold = phi_sd_dsig;
                    phi_sh_dsigold = phi_sh_dsig;

                    if (changelayer == 1)
                    {
                        bottomhist[l, t] = z + dz;
                        localmass = 0;
                        l = l - 1;
                    }
                    z = z + dz;
                }

            }
            return bottomhist;

        }

        public double getbulkpor(double phi_sd, double phi_sh, double sv, double rhog, int mtype, double depth)
        {
            //mtype indicates model type. 0 is the revil model - 1-4 are from Sclater and Christie (1980): 1: Shale, 2: Sandstone, 3: Chalk, 4: Shaly sandstone
            //if sv<phi_sd phi_b=phi_sd-sv*(1-phi_sh); else phi_b=sv*phi_sh; end
            //Revil reference: https://agupubs.onlinelibrary.wiley.com/doi/full/10.1029/2001JB000318
            double svw;
            double svwcr;
            double phi_b = 0;

            if (mtype == 0)
            {
                if (sv < phi_sd)
                {
                    svw = sv * (1 - phi_sh) * rhog / (sv * (1 - phi_sh) * rhog + (1 - phi_sd) * rhog);
                }
                else
                {
                    svw = sv * (1 - phi_sh) * rhog / (sv * (1 - phi_sh) * rhog + (1 - sv) * rhog);
                }

                svwcr = phi_sd * (1 - phi_sh) / (1 - phi_sd * phi_sh);

                if (svw < svwcr)
                {
                    phi_b = phi_sd - (1 - phi_sd) * svw / (1 - svw);
                    return phi_b;
                }
                else
                {
                    phi_b = phi_sh * svw / (svw + (1 - phi_sh) * (1 - svw));
                    return phi_b;
                }
            }

            else if (mtype == 1)
            {
                phi_b = 0.63 * Math.Exp(-0.51e-3 * depth);
                return phi_b;
            }

            else if (mtype == 2)
            {
                phi_b = 0.49 * Math.Exp(-0.27e-3 * depth);
                return phi_b;
            }

            else if (mtype == 3)
            {
                phi_b = 0.70 * Math.Exp(-0.71e-3 * depth);
                return phi_b;
            }

            else if (mtype == 4)
            {
                phi_b = 0.56 * Math.Exp(-0.39e-3 * depth);
                return phi_b;
            }

            return phi_b;
        }


        public double[] backstrip_refine_hors1dfine(double[] hors1dfine, double[,,] horsrawi, int[] sublayers, int i, int j, int Nhorsraw)
        {

            int Nv = sublayers.Length;
            double rawtop = 0;
            double rawbot = 0;
            int hfine = 0;
            int bound;


            for (int h = 0; h < Nhorsraw; h++)
            {
                rawbot = horsrawi[i, j, h];
                if (h < Nhorsraw - 1)
                {
                    rawtop = horsrawi[i, j, h + 1];
                }
                else
                    rawtop = 0;
                bound = sublayers[h];
                for (int hfinelocal = 0; hfinelocal < bound; hfinelocal++)
                {
                    hors1dfine[hfine] = rawbot - (rawbot - rawtop) * (hfinelocal) / sublayers[h];
                    hfine++;
                }
            }

            return hors1dfine;

        }

        public double[] backstrip_refine_agesfine(double[] agesfine, double[] agesraw, int[] sublayers, int i, int j, int Nhorsraw)
        {

            double rawtop = 0;
            double rawbot = 0;
            int hfine = 0;
            int bound;

            hfine = 0;

            for (int h = 0; h < Nhorsraw; h++)
            {
                rawtop = agesraw[h];
                if (h == 0)
                {
                    rawbot = rawtop;
                }
                else
                    rawbot = agesraw[h - 1];
                bound = sublayers[h];
                for (int hfinelocal = 0; hfinelocal < bound; hfinelocal++)
                {
                    agesfine[hfine] = rawbot - (rawbot - rawtop) * (hfinelocal + 1) / sublayers[h];
                    hfine++;
                }
            }

            return agesfine;

        }


        public double[,] backstrip_refine(double[,] bottomhist, double[] hors1dfine, int Nv_50, double phi0_sd, double phi0_sh, double[] phics_sdfine, double[] phics_shfine, double[] svsfine, int[] modeltypesfine)
        {

            var bsf = new Backstrip_functions();
            bottomhist = (bsf.revil_backstrip(bottomhist, hors1dfine, Nv_50, phi0_sd, phi0_sh, phics_sdfine, phics_shfine, svsfine, modeltypesfine));
            return bottomhist;
        }
    }
}