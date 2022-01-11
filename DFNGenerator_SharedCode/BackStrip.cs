using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BackStrip_SharedCode
{
    class Backstrip_functions
    {
        int IN(double i, double j, double I)
        {
            return Convert.ToInt32(j * I + i);
        }
        public double[,] revil_backstrip(double[,] bottomhist, double[] bottoms, int Nv_50, double phi0_sd, double phi0_sh, double[] phics_sd, double[] phics_sh, double[] svs, int[] modeltypes)
        {
            
            double beta_sd = 6e-8;
            double beta_sh = 4e-8;
            double rhog = 2650;
            double rhof = 1050;
            double g = 9.82;

            int Nl = bottoms.Length;

            Nv_50 = 9;
            double[] layermasses = new double[Nv_50];

            for (int i = 0; i < Nv_50; i++)
                layermasses[i] = 0 * bottoms[i];

            int dznom = 10; //Nominal integration step. Decrease for higher precision.
            double z = 0;
            int l = Nl; //to change
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

            double[,] bottomhist = new double[66, 66];

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
