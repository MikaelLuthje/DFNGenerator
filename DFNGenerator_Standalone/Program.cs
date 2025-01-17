﻿// Switch this flag off to use hardcoded values for all parameters
// This should be done for debugging only
// The flag should be set to generate release versions of the standalone code
#define READINPUTFROMFILE
// Set this flag to output detailed information on input parameters and properties for each gridblock
// Use for debugging only; will significantly increase runtime
//#define DEBUG_FRACS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DFNGenerator_SharedCode;

namespace DFNGenerator_Standalone
{
    // Enumerators used throughout code

    class Program
    {
        static void Main(string[] args)
        {
#if READINPUTFROMFILE
            // Find input file; if it does not exist then create a dummy file and abort
            string inputfile_name;
            if (args.Length > 0)
                inputfile_name = args[0];
            else
                inputfile_name = "DFNGenerator_configuration.txt";
            if (!File.Exists(inputfile_name))
            {
                StreamWriter input_file = new StreamWriter(inputfile_name);

                input_file.WriteLine(string.Format("% Model {0}\n", inputfile_name));
                input_file.WriteLine("% Replace defaults with required values");
                input_file.WriteLine("% Use % for comment lines");
                input_file.WriteLine();

                input_file.WriteLine("% Main properties");
                input_file.WriteLine("% Grid size; all lengths in metres");
                input_file.WriteLine("NoRows 3");
                input_file.WriteLine("NoCols 3");
                input_file.WriteLine("% Gridblock size");
                input_file.WriteLine("Width_EW 20");
                input_file.WriteLine("Length_NS 20");
                input_file.WriteLine("LayerThickness 1");
                input_file.WriteLine("% Model location");
                input_file.WriteLine("% Use the origin offset to set the absolute XY coordinates of the SW corner of the bottom left gridblock");
                input_file.WriteLine("OriginXOffset 0");
                input_file.WriteLine("OriginYOffset 0");
                input_file.WriteLine("% Current depth of burial in metres, positive downwards");
                input_file.WriteLine("Depth 2000");
                input_file.WriteLine("% Minimum strain orientatation (i.e. direction of maximum extension) in radians, clockwise from North");
                input_file.WriteLine("EhminAzi 0");
                input_file.WriteLine("% Set VariableStrainOrientation false to have the same minimum strain orientatation in all cells");
                input_file.WriteLine("% Set VariableStrainOrientation true to have laterally variable strain orientation controlled by EhminAzi and EhminCurvature");
                input_file.WriteLine("% EhminCurvature is the difference in strain orientation between adjacent blocks in radians");
                input_file.WriteLine("VariableStrainOrientation false");
                input_file.WriteLine("EhminCurvature 0.1963495");
                input_file.WriteLine("% Strain rates; units determined by ModelTimeUnits (i.e. strain/s, strain/year or strain/ma)");
                input_file.WriteLine("% Set to negative value for extensional strain - this is necessary in at least one direction to generate fractures");
                input_file.WriteLine("% With no strain relaxation, strain rate will control rate of horizontal stress increase");
                input_file.WriteLine("% With strain relaxation, ratio of strain rate to strain relaxation time constants will control magnitude of constant horizontal stress");
                input_file.WriteLine("% EhminRate is most tensile (i.e. most negative) horizontal strain rate");
                input_file.WriteLine("EhminRate -0.01");
                input_file.WriteLine("% Set EhmaxRate to 0 for uniaxial strain; set to between 0 and EhminRate for anisotropic fracture pattern; set to EhminRate for isotropic fracture pattern");
                input_file.WriteLine("EhmaxRate 0");
                input_file.WriteLine("% Set VariableStrainMagnitude to add random variation to the input strain rates");
                input_file.WriteLine("% Strain rates for each gridblock will vary randomly from 0 to 2x specified values");
                input_file.WriteLine("VariableStrainMagnitude false");
                input_file.WriteLine("% Set time units to ma, year or second");
                input_file.WriteLine("ModelTimeUnits ma");
                input_file.WriteLine();

                input_file.WriteLine("% Mechanical properties");
                input_file.WriteLine("% Young's Modulus in Pa");
                input_file.WriteLine("YoungsMod 1E+10");
                input_file.WriteLine("% Set VariableYoungsMod true to have laterally variable Young's Modulus");
                input_file.WriteLine("VariableYoungsMod false");
                input_file.WriteLine("PoissonsRatio 0.25");
                input_file.WriteLine("BiotCoefficient 1");
                input_file.WriteLine("FrictionCoefficient 0.5");
                input_file.WriteLine("% Set VariableFriction true to have laterally variable friction coefficient");
                input_file.WriteLine("VariableFriction false");
                input_file.WriteLine("% Crack surface energy in J/m2");
                input_file.WriteLine("CrackSurfaceEnergy 1000");
                input_file.WriteLine("% Set VariableCSE true to have laterally variable crack surface energy");
                input_file.WriteLine("VariableCSE false");
                input_file.WriteLine("% Strain relaxation time constants");
                input_file.WriteLine("% Units are determined by ModelTimeUnits setting");
                input_file.WriteLine("% Set RockStrainRelaxation to 0 for no strain relaxation and steadily increasing horizontal stress; set it to >0 for constant horizontal stress determined by ratio of strain rate and relaxation rate");
                input_file.WriteLine("RockStrainRelaxation 0");
                input_file.WriteLine("% Set FractureRelaxation to >0 and RockStrainRelaxation to 0 to apply strain relaxation to the fractures only");
                input_file.WriteLine("FractureRelaxation 0");
                input_file.WriteLine("% Density of initial microfractures");
                input_file.WriteLine("InitialMicrofractureDensity 0.001");
                input_file.WriteLine("% Size distribution of initial microfractures - increase for larger ratio of small:large initial microfractures");
                input_file.WriteLine("InitialMicrofractureSizeDistribution 2");
                input_file.WriteLine("% Subritical fracture propagation index; <5 for slow subcritical propagation, 5-15 for intermediate, >15 for rapid critical propagation");
                input_file.WriteLine("SubcriticalPropIndex 10");
                input_file.WriteLine("% Critical fracture propagation rate in m/s");
                input_file.WriteLine("CriticalPropagationRate 2000");
                input_file.WriteLine();

                input_file.WriteLine("% Stress state");
                input_file.WriteLine("% Stress distribution scenario - use to turn on or off stress shadow effect");
                input_file.WriteLine("% Options are EvenlyDistributedStress or StressShadow");
                input_file.WriteLine("% Do not use DuctileBoundary as this is not yet implemented");
                input_file.WriteLine("StressDistributionScenario StressShadow");
                input_file.WriteLine("% Depth at the time of deformation (in metres, positive downwards) - this will control stress state");
                input_file.WriteLine("% If DepthAtDeformation is specified, this will be used to calculate effective vertical stress instead of the current depth");
                input_file.WriteLine("% If DepthAtDeformation is <=0 or NaN, OverwriteDepth will be set to false and DepthAtFracture will not be used");
                input_file.WriteLine("DepthAtDeformation -1");
                input_file.WriteLine("% Mean density of overlying sediments and fluid in kg/m3");
                input_file.WriteLine("MeanOverlyingSedimentDensity 2250");
                input_file.WriteLine("FluidDensity 1000");
                input_file.WriteLine("% Fluid overpressure in Pa");
                input_file.WriteLine("InitialOverpressure 0");
                input_file.WriteLine("% InitialStressRelaxation controls the initial horizontal stress, prior to the application of horizontal strain");
                input_file.WriteLine("% Set InitialStressRelaxation to 1 to have initial horizontal stress = vertical stress (viscoelastic equilibrium)");
                input_file.WriteLine("% Set InitialStressRelaxation to 0 to have initial horizontal stress = v/(1-v) * vertical stress (elastic equilibrium)");
                input_file.WriteLine("% Set InitialStressRelaxation to -1 for initial horizontal stress = Mohr-Coulomb failure stress (critical stress state)");
                input_file.WriteLine("InitialStressRelaxation 1");
                input_file.WriteLine();

                input_file.WriteLine("% Outputs");
                input_file.WriteLine("% Output to file");
                input_file.WriteLine("% These must be set to true for stand-alone version or no output will be generated");
                input_file.WriteLine("WriteImplicitDataFiles true");
                input_file.WriteLine("WriteDFNFiles true");
                input_file.WriteLine("% Output file type for explicit DFN data: ASCII or FAB (NB FAB files can be loaded directly into Petrel)");
                input_file.WriteLine("OutputDFNFileType ASCII");
                input_file.WriteLine("% Output DFN at intermediate stages of fracture growth");
                input_file.WriteLine("NoIntermediateOutputs 0");
                input_file.WriteLine("% Flag to control interval between output of intermediate stage DFNs; if true, they will be output at equal intervals of time, if false they will be output at approximately regular intervals of total fracture area");
                input_file.WriteLine("OutputAtEqualTimeIntervals false");
                input_file.WriteLine("% Flag to output the macrofracture centrepoints as a polyline, in addition to the macrofracture cornerpoints");
                input_file.WriteLine("OutputCentrepoints false");
                input_file.WriteLine("% Flag to output the bulk rock compliance tensor");
                input_file.WriteLine("OutputComplianceTensor false");
                input_file.WriteLine("% Fracture porosity control parameters");
                input_file.WriteLine("% Flag to calculate and output fracture porosity");
                input_file.WriteLine("CalculateFracturePorosity true");
                input_file.WriteLine("% Flag to determine method used to determine fracture aperture - used in porosity and permeability calculation");
                input_file.WriteLine("% Set to Uniform, SizeDependent, Dynamic, or BartonBandis");
                input_file.WriteLine("FractureApertureControl Uniform");
                input_file.WriteLine("% Fracture aperture control parameters: Uniform fracture aperture");
                input_file.WriteLine("% Fixed aperture for Mode 1 fractures striking perpendicular to hmin in the uniform aperture case (m)");
                input_file.WriteLine("Mode1HMin_UniformAperture 0.0005");
                input_file.WriteLine("% Fixed aperture for Mode 2 fractures striking perpendicular to hmin in the uniform aperture case (m)");
                input_file.WriteLine("Mode2HMin_UniformAperture 0.0005");
                input_file.WriteLine("% Fixed aperture for Mode 1 fractures striking perpendicular to hmax in the uniform aperture case (m)");
                input_file.WriteLine("Mode1HMax_UniformAperture 0.0005");
                input_file.WriteLine("% Fixed aperture for Mode 2 fractures striking perpendicular to hmax in the uniform aperture case (m)");
                input_file.WriteLine("Mode2HMax_UniformAperture 0.0005");
                input_file.WriteLine("% Fracture aperture control parameters: SizeDependent fracture aperture");
                input_file.WriteLine("% Size-dependent aperture multiplier for Mode 1 fractures striking perpendicular to hmin - layer-bound fracture aperture is given by layer thickness times this multiplier");
                input_file.WriteLine("Mode1HMin_SizeDependentApertureMultiplier 1E-5");
                input_file.WriteLine("% Size-dependent aperture multiplier for Mode 2 fractures striking perpendicular to hmin - layer-bound fracture aperture is given by layer thickness times this multiplier");
                input_file.WriteLine("Mode2HMin_SizeDependentApertureMultiplier 1E-5");
                input_file.WriteLine("% Size-dependent aperture multiplier for Mode 1 fractures striking perpendicular to hmax - layer-bound fracture aperture is given by layer thickness times this multiplier");
                input_file.WriteLine("Mode1HMax_SizeDependentApertureMultiplier 1E-5");
                input_file.WriteLine("% Size-dependent aperture multiplier for Mode 2 fractures striking perpendicular to hmax - layer-bound fracture aperture is given by layer thickness times this multiplier");
                input_file.WriteLine("Mode2HMax_SizeDependentApertureMultiplier 1E-5");
                input_file.WriteLine("% Fracture aperture control parameters: Dynamic fracture aperture");
                input_file.WriteLine("% Multiplier for dynamic aperture");
                input_file.WriteLine("DynamicApertureMultiplier 1");
                input_file.WriteLine("% Fracture aperture control parameters: Barton-Bandis model for fracture aperture");
                input_file.WriteLine("% Joint Roughness Coefficient");
                input_file.WriteLine("JRC 10");
                input_file.WriteLine("% Compressive strength ratio; ratio of unconfined compressive strength of unfractured rock to fractured rock");
                input_file.WriteLine("UCSRatio 2");
                input_file.WriteLine("% Initial normal strength on fracture");
                input_file.WriteLine("InitialNormalStress 2E+5");
                input_file.WriteLine("% Stiffness normal to the fracture, at initial normal stress");
                input_file.WriteLine("FractureNormalStiffness 2.5E+9");
                input_file.WriteLine("% Maximum fracture closure (m)");
                input_file.WriteLine("MaximumClosure 0.0005");
                input_file.WriteLine();

                input_file.WriteLine("% Calculation control parameters");
                input_file.WriteLine("% Number of fracture sets");
                input_file.WriteLine("% Set to 1 to generate a single fracture set, perpendicular to ehmin");
                input_file.WriteLine("% Set to 2 to generate two orthogonal fracture sets, perpendicular to the minimum and maximum horizontal strain directions; this is typical of a single stage of tectonic deformation in intact rock");
                input_file.WriteLine("% Set to 6 to model polygonal or strike-slip fractures, or multiple deformation episodes where there are pre-existing fractures oblique to the principal horizontal stresses");
                input_file.WriteLine("NoFractureSets 2");
                input_file.WriteLine("% Fracture mode: set these to force only Mode 1 (dilatant) or only Mode 2 (shear) fractures; otherwise model will include both, depending on which is energetically optimal");
                input_file.WriteLine("Mode1Only false");
                input_file.WriteLine("Mode2Only false");
                input_file.WriteLine("% Flag to check microfractures against stress shadows of all macrofractures, regardless of set: can be set to None, All or Automatic");
                input_file.WriteLine("% Flag to control whether to search adjacent gridblocks for stress shadow interaction: can be set to All, None or Automatic; if set to Automatic, this will be determined independently for each gridblock based on the gridblock geometry");
                input_file.WriteLine("% If None, microfractures will only be deactivated if they lie in the stress shadow zone of parallel macrofractures");
                input_file.WriteLine("% If All, microfractures will also be deactivated if they lie in the stress shadow zone of oblique or perpendicular macrofractures, depending on the strain tensor");
                input_file.WriteLine("% If Automatic, microfractures in the stress shadow zone of oblique or perpendicular macrofractures will be deactivated only if there are more than two fracture sets");
                input_file.WriteLine("CheckAlluFStressShadows Automatic");
                input_file.WriteLine("% Cutoff value to use the isotropic method for calculating cross-fracture set stress shadow and exclusion zone volumes");
                input_file.WriteLine("AnisotropyCutoff 1");
                input_file.WriteLine("% Flag to allow reverse fractures; if set to false, fracture dipsets with a reverse displacement vector will not be allowed to accumulate displacement or grow");
                input_file.WriteLine("AllowReverseFractures false");
                input_file.WriteLine("% Maximum duration for individual timesteps; set to -1 for no maximum timestep duration");
                input_file.WriteLine("MaxTimestepDuration -1");
                input_file.WriteLine("% Maximum increase in MFP33 allowed in each timestep - controls the optimal timestep duration");
                input_file.WriteLine("% Increase this to run calculation faster, with fewer but longer timesteps");
                input_file.WriteLine("MaxTimestepMFP33Increase 0.002");
                input_file.WriteLine("% Minimum radius for microfractures to be included in implicit fracture density and porosity calculations (in metres)");
                input_file.WriteLine("% If this is set to 0 (i.e. include all microfractures) then it will not be possible to calculate volumetric microfracture density as this will be infinite");
                input_file.WriteLine("% If this is set to -1 the maximum radius of the smallest bin will be used (i.e. exclude the smallest bin from the microfracture population)");
                input_file.WriteLine("MinImplicitMicrofractureRadius -1");
                input_file.WriteLine("% Number of bins used in numerical integration of uFP32");
                input_file.WriteLine("% This controls accuracy of numerical calculation of microfracture populations - increase this to increase accuracy of the numerical integration at expense of runtime");
                input_file.WriteLine("No_r_bins 10");
                input_file.WriteLine("% Flag to calculate implicit fracture population distribution functions");
                input_file.WriteLine("CalculatePopulationDistribution true");
                input_file.WriteLine("% Number of macrofracture length values to calculate for each of the implicit fracture population distribution functions");
                input_file.WriteLine("No_l_indexPoints 20");
                input_file.WriteLine("% MaxHMinLength and MaxHMaxLength control the range of macrofracture lengths to calculate for the implicit fracture population distribution functions for fractures striking perpendicular to hmin and hmax respectively");
                input_file.WriteLine("% Set these values to the approximate maximum length of macrofractures generated (in metres), or 0 if this is not known; 0 will default to maximum potential length - but this may be much greater than actual maximum length");
                input_file.WriteLine("MaxHMinLength 0");
                input_file.WriteLine("MaxHMaxLength 0");
                input_file.WriteLine("% Calculation termination controls");
                input_file.WriteLine("% The calculation is set to stop automatically when fractures stop growing");
                input_file.WriteLine("% This can be defined in one of three ways:");
                input_file.WriteLine("%      - When the total volumetric ratio of active (propagating) half-macrofractures (a_MFP33) drops below a specified proportion of the peak historic value");
                input_file.WriteLine("%      - When the total volumetric density of active (propagating) half-macrofractures (a_MFP30) drops below a specified proportion of the total (propagating and non-propagating) volumetric density (MFP30)");
                input_file.WriteLine("%      - When the total clear zone volume (the volume in which fractures can nucleate without falling within or overlapping a stress shadow) drops below a specified proportion of the total volume");
                input_file.WriteLine("% Increase these cutoffs to reduce the sensitivity and stop the calculation earlier");
                input_file.WriteLine("% Use this to prevent a long calculation tail - i.e. late timesteps where fractures have stopped growing so they have no impact on fracture populations, just increase runtime");
                input_file.WriteLine("% To stop calculation while fractures are still growing reduce the DeformationStageDuration_in or maxTimesteps_in limits");
                input_file.WriteLine("% Ratio of current to peak active macrofracture volumetric ratio at which fracture sets are considered inactive; set to negative value to switch off this control");
                input_file.WriteLine("Current_HistoricMFP33TerminationRatio -1");
                input_file.WriteLine("% Ratio of active to total macrofracture volumetric density at which fracture sets are considered inactive; set to negative value to switch off this control");
                input_file.WriteLine("Active_TotalMFP30TerminationRatio -1");
                input_file.WriteLine("% Minimum required clear zone volume in which fractures can nucleate without stress shadow interactions (as a proportion of total volume); if the clear zone volume falls below this value, the fracture set will be deactivated");
                input_file.WriteLine("MinimumClearZoneVolume 0.01");
                input_file.WriteLine("% Use the deformation stage duration and maximum timestep limits to stop the calculation before fractures have finished growing");
                input_file.WriteLine("% Set DeformationStageDuration to -1 to continue until fracture saturation is reached");
                input_file.WriteLine("DeformationStageDuration -1");
                input_file.WriteLine("MaxTimesteps 1000");
                input_file.WriteLine("% DFN geometry controls");
                input_file.WriteLine("% Flag to generate explicit DFN; if set to false only implicit fracture population functions will be generated");
                input_file.WriteLine("GenerateExplicitDFN true");
                input_file.WriteLine("% Set false to allow fractures to propagate outside of the outer grid boundary");
                input_file.WriteLine("CropAtBoundary true");
                input_file.WriteLine("% Set true to link fractures that terminate due to stress shadow interaction into one long fracture, via a relay segment");
                input_file.WriteLine("LinkStressShadows false");
                input_file.WriteLine("% Maximum variation in fracture propagation azimuth allowed across gridblock boundary; if the orientation of the fracture set varies across the gridblock boundary by more than this, the algorithm will seek a better matching set");
                input_file.WriteLine("% Set to Pi/4 radians (45 degrees) by default");
                input_file.WriteLine("MaxConsistencyAngle 0.78539816");
                input_file.WriteLine("% Layer thickness cutoff (in metres): explicit DFN will not be calculated for gridblocks thinner than this value");
                input_file.WriteLine("% Set this to prevent the generation of excessive numbers of fractures in very thin gridblocks where there is geometric pinch-out of the layers");
                input_file.WriteLine("MinimumLayerThickness 0");
                input_file.WriteLine("% Allow fracture nucleation to be controlled probabilistically, if the number of fractures nucleating per timestep is less than the specified value - this will allow fractures to nucleate when gridblocks are small");
                input_file.WriteLine("% Set to 0 to disable probabilistic fracture nucleation");
                input_file.WriteLine("% Set to -1 for automatic (probabilistic fracture nucleation will be activated whenever searching neighbouring gridblocks is also active; if SearchNeighbouringGridblocks is set to automatic, this will be determined independently for each gridblock based on the gridblock geometry)");
                input_file.WriteLine("ProbabilisticFractureNucleationLimit -1");
                input_file.WriteLine("% Flag to control the order in which fractures are propagated within each timestep: if true, fractures will be propagated in order of nucleation time regardless of fracture set; if false they will be propagated in order of fracture set");
                input_file.WriteLine("% Propagating in strict order of nucleation time removes bias in fracture lengths between sets, but will add a small overhead to calculation time");
                input_file.WriteLine("PropagateFracturesInNucleationOrder true");
                input_file.WriteLine("% Flag to control whether to search adjacent gridblocks for stress shadow interaction: can be set to All, None or Automatic; if set to Automatic, this will be determined independently for each gridblock based on the gridblock geometry");
                input_file.WriteLine("SearchNeighbouringGridblocks Automatic");
                input_file.WriteLine("% Minimum radius for microfractures to be included in explicit DFN (in metres)");
                input_file.WriteLine("% Set this to 0 to exclude microfractures from DFN; set to between 0 and half layer thickness to include larger microfractures in the DFN");
                input_file.WriteLine("MinExplicitMicrofractureRadius 0");
                input_file.WriteLine("% Number of cornerpoints defining the microfracture polygons in the explicit DFN");
                input_file.WriteLine("% Set to zero to output microfractures as just a centrepoint and radius; set to 3 or greater to output microfractures as polygons defined by a list of cornerpoints");
                input_file.WriteLine("Number_uF_Points 8");
                input_file.WriteLine();

                input_file.WriteLine("% Add property and geometry overrides for individual gridblocks here");
                input_file.WriteLine("% Overrides for individual gridblocks should be nested between a Gridblock Col Row statement and an End Gridblock statement, e.g.:");
                input_file.WriteLine("% E.g. to override the properties in the gridblock in column 1 row 2, use:");
                input_file.WriteLine("% Gridblock 1 2");
                input_file.WriteLine("%   PropertyA ValueA");
                input_file.WriteLine("%   PropertyB ValueB");
                input_file.WriteLine("%   CornerpointA Xcoord Ycoord Zcoord");
                input_file.WriteLine("%   CornerpointB Xcoord Ycoord Zcoord");
                input_file.WriteLine("% End Gridblock");
                input_file.WriteLine("% Properties that can be overridden are EhminAzi, EhminRate, EhmaxRate,");
                input_file.WriteLine("% YoungsMod, PoissonsRatio, BiotCoefficient, CrackSurfaceEnergy, FrictionCoefficient,");
                input_file.WriteLine("% SubcriticalPropIndex, RockStrainRelaxation, FractureRelaxation, InitialMicrofractureDensity, InitialMicrofractureSizeDistribution");
                input_file.WriteLine("% Cornerpoints that can be overridden are SETopCorner, SEBottomCorner, NETopCorner, NEBottomCorner,");
                input_file.WriteLine("% NWTopCorner, NWBottomCorner, SWTopCorner, SWBottomCorner");
                input_file.WriteLine("% Z coordinates should be specified positive downwards");
                input_file.WriteLine("% NB the cornerpoints of adjacent gridblocks will automatically be adjusted when a gridblock cornerpoint is overridden,");
                input_file.WriteLine("% but there is no sanity check to ensure the resulting grid geometry is consistent (e.g. checking for negative gridblock volumes, etc.); this must be done beforehand");
                input_file.WriteLine("");

                input_file.WriteLine("% Overrides for individual properties can be done by specifying Include files, using the statement:");
                input_file.WriteLine("% Include Filename");
                input_file.WriteLine("% The Include file should follow the format");
                input_file.WriteLine("% #PropertyA");
                input_file.WriteLine("% Gridblock_1_1_value Gridblock_2_1_value Gridblock_3_1_value");
                input_file.WriteLine("% Gridblock_1_2_value Gridblock_2_2_value Gridblock_3_2_value");
                input_file.WriteLine("% Gridblock_1_3_value Gridblock_2_3_value Gridblock_3_3_value");
                input_file.WriteLine("% #PropertyB");
                input_file.WriteLine("% Gridblock_1_1_value Gridblock_1_2_value Gridblock_1_3_value");
                input_file.WriteLine("% Gridblock_1_2_value Gridblock_2_2_value Gridblock_3_2_value");
                input_file.WriteLine("% Gridblock_1_3_value NA Gridblock_3_3_value");
                input_file.WriteLine("% Include files can include values for multiple properties, in separate blocks; any property in the list above can be overridden");
                input_file.WriteLine("% Each new property must start on a new line, but within each property block, the values can be separated by either spaces or line returns; the layout of the data is not significant");
                input_file.WriteLine("% However the values must be given in order shown above, looping first through rows and then through columns");
                input_file.WriteLine("% Use NA instead of a specifying a value to revert to the default value for a specific gridblock (as has been done for PropertyB in Gridblock 3,2 in the example above)");
                input_file.WriteLine("% ");
                input_file.WriteLine("% Overrides for geometry can be also done using Include files");
                input_file.WriteLine("% However in this case the Include file format is slightly different:");
                input_file.WriteLine("% #Geometry");
                input_file.WriteLine("% Gridblock_1_1_SWTopCornerpoint_X Gridblock_1_1_SWTopCornerpoint_Y Gridblock_1_1_SWTopCornerpoint_Z Gridblock_1_1_SWBottomCornerpoint_X Gridblock_1_1_SWBottomCornerpoint_Y Gridblock_1_1_SWBottomCornerpoint_Z");
                input_file.WriteLine("% Gridblock_2_1_SWTopCornerpoint_X Gridblock_2_1_SWTopCornerpoint_Y Gridblock_2_1_SWTopCornerpoint_Z Gridblock_2_1_SWBottomCornerpoint_X Gridblock_2_1_SWBottomCornerpoint_Y Gridblock_2_1_SWBottomCornerpoint_Z");
                input_file.WriteLine("% Gridblock_3_1_SWTopCornerpoint_X Gridblock_3_1_SWTopCornerpoint_Y Gridblock_3_1_SWTopCornerpoint_Z Gridblock_3_1_SWBottomCornerpoint_X Gridblock_3_1_SWBottomCornerpoint_Y Gridblock_3_1_SWBottomCornerpoint_Z");
                input_file.WriteLine("% Gridblock_3_1_SETopCornerpoint_X Gridblock_3_1_SETopCornerpoint_Y Gridblock_3_1_SETopCornerpoint_Z Gridblock_3_1_SEBottomCornerpoint_X Gridblock_3_1_SEBottomCornerpoint_Y Gridblock_3_1_SEBottomCornerpoint_Z");
                input_file.WriteLine("% Gridblock_1_2_SWTopCornerpoint_X Gridblock_1_2_SWTopCornerpoint_Y Gridblock_1_2_SWTopCornerpoint_Z Gridblock_1_2_SWBottomCornerpoint_X Gridblock_1_2_SWBottomCornerpoint_Y Gridblock_1_2_SWBottomCornerpoint_Z");
                input_file.WriteLine("% Gridblock_2_2_SWTopCornerpoint_X Gridblock_2_2_SWTopCornerpoint_Y Gridblock_2_2_SWTopCornerpoint_Z Gridblock_2_2_SWBottomCornerpoint_X Gridblock_2_2_SWBottomCornerpoint_Y Gridblock_2_2_SWBottomCornerpoint_Z");
                input_file.WriteLine("% Gridblock_3_2_SWTopCornerpoint_X Gridblock_3_2_SWTopCornerpoint_Y Gridblock_3_2_SWTopCornerpoint_Z Gridblock_3_2_SWBottomCornerpoint_X Gridblock_3_2_SWBottomCornerpoint_Y Gridblock_3_2_SWBottomCornerpoint_Z");
                input_file.WriteLine("% Gridblock_3_2_SETopCornerpoint_X Gridblock_3_2_SETopCornerpoint_Y Gridblock_3_2_SETopCornerpoint_Z Gridblock_3_2_SEBottomCornerpoint_X Gridblock_3_2_SEBottomCornerpoint_Y Gridblock_3_2_SEBottomCornerpoint_Z");
                input_file.WriteLine("% Gridblock_1_3_SWTopCornerpoint_X Gridblock_1_3_SWTopCornerpoint_Y Gridblock_1_3_SWTopCornerpoint_Z Gridblock_1_3_SWBottomCornerpoint_X Gridblock_1_3_SWBottomCornerpoint_Y Gridblock_1_3_SWBottomCornerpoint_Z");
                input_file.WriteLine("% Gridblock_2_3_SWTopCornerpoint_X Gridblock_2_3_SWTopCornerpoint_Y Gridblock_2_3_SWTopCornerpoint_Z Gridblock_2_3_SWBottomCornerpoint_X Gridblock_2_3_SWBottomCornerpoint_Y Gridblock_2_3_SWBottomCornerpoint_Z");
                input_file.WriteLine("% Gridblock_3_3_SWTopCornerpoint_X Gridblock_3_3_SWTopCornerpoint_Y Gridblock_3_3_SWTopCornerpoint_Z Gridblock_3_3_SWBottomCornerpoint_X Gridblock_3_3_SWBottomCornerpoint_Y Gridblock_3_3_SWBottomCornerpoint_Z");
                input_file.WriteLine("% Gridblock_3_3_SETopCornerpoint_X Gridblock_3_3_SETopCornerpoint_Y Gridblock_3_3_SETopCornerpoint_Z Gridblock_3_3_SEBottomCornerpoint_X Gridblock_3_3_SEBottomCornerpoint_Y Gridblock_3_3_SEBottomCornerpoint_Z");
                input_file.WriteLine("% Gridblock_1_3_NWTopCornerpoint_X Gridblock_1_3_NWTopCornerpoint_Y Gridblock_1_3_NWTopCornerpoint_Z Gridblock_1_3_NWBottomCornerpoint_X Gridblock_1_3_NWBottomCornerpoint_Y Gridblock_1_3_NWBottomCornerpoint_Z");
                input_file.WriteLine("% Gridblock_2_3_NWTopCornerpoint_X Gridblock_2_3_NWTopCornerpoint_Y Gridblock_2_3_NWTopCornerpoint_Z Gridblock_2_3_NWBottomCornerpoint_X Gridblock_2_3_NWBottomCornerpoint_Y Gridblock_2_3_NWBottomCornerpoint_Z");
                input_file.WriteLine("% Gridblock_3_3_NWTopCornerpoint_X Gridblock_3_3_NWTopCornerpoint_Y Gridblock_3_3_NWTopCornerpoint_Z Gridblock_3_3_NWBottomCornerpoint_X Gridblock_3_3_NWBottomCornerpoint_Y Gridblock_3_3_NWBottomCornerpoint_Z");
                input_file.WriteLine("% Gridblock_3_3_NETopCornerpoint_X Gridblock_3_3_NETopCornerpoint_Y Gridblock_3_3_NETopCornerpoint_Z Gridblock_3_3_NEBottomCornerpoint_X Gridblock_3_3_NEBottomCornerpoint_Y Gridblock_3_3_NEBottomCornerpoint_Z");
                input_file.WriteLine("% The values can be separated by either spaces or line returns; the layout of the data is not significant");
                input_file.WriteLine("% However the values must be given in order shown above, i.e. looping through each pillar in the grid, first in order of row and then in order of column");
                input_file.WriteLine("% NB there is no sanity check to ensure the resulting grid geometry is consistent (e.g. checking for negative gridblock volumes, etc.); this must be done beforehand");

                input_file.Close();

                Console.WriteLine("\nDFNGenerator configuration file did not exist. An empty file has been created. Please enter the required values, SAVE it and press ENTER! ");
                Console.ReadKey();
            }

            // Get path for output files
            string folderPath = "";
            if (args.Length > 0)
            {
                folderPath = inputfile_name.Replace(".txt", "") + "_output" + @"\";
                // If the output folder does not exist, create it
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);
            }
#else
            // Get path for output files
            string fullHomePath = "";
            string folderPath = "";

            try
            {
                var homeDrive = Environment.GetEnvironmentVariable("HOMEDRIVE");
                if (homeDrive != null)
                {
                    var homePath = Environment.GetEnvironmentVariable("HOMEPATH");
                    if (homePath != null)
                    {
                        fullHomePath = homeDrive + Path.DirectorySeparatorChar + homePath;
                        folderPath = Path.Combine(fullHomePath, "DFNFolder");
                        folderPath = folderPath + @"\";
                        // If the output folder does not exist, create it
                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);
                    }
                    else
                    {
                        throw new Exception("Environment variable error, there is no 'HOMEPATH'");
                    }
                }
                else
                {
                    throw new Exception("Environment variable error, there is no 'HOMEDRIVE'");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception thrown: " + e.Message);
                return;
            }
#endif

            // Set hardcoded default values for all parameters

            // Main properties
            // Grid size
            int NoRows = 3;
            int NoCols = 3;
            // Gridblock size
            double Width_EW = 20;
            double Length_NS = 20;
            double LayerThickness = 1;
            // Model location 
            // Use the origin offset to set the absolute XY coordinates of the SW corner of the bottom left gridblock
            double OriginXOffset = 0;
            double OriginYOffset = 0;
            double Depth = 2000;
            // Strain orientatation
            double EhminAzi = 0;
            // Set VariableStrainOrientation false to have N-S minimum strain orientatation in all cells
            // Set VariableStrainOrientation true to have laterally variable strain orientation controlled by EhminAzi and EhminCurvature
            bool VariableStrainOrientation = false;
            double EhminCurvature = Math.PI / 16;
            if (VariableStrainOrientation) 
                EhminAzi = Math.PI / 4;
            // Strain rates
            // Set to negative value for extensional strain - this is necessary in at least one direction to generate fractures
            // With no strain relaxation, strain rate will control rate of horizontal stress increase
            // With strain relaxation, ratio of strain rate to strain relaxation time constants will control magnitude of constant horizontal stress
            // Ehmin is most tensile (i.e. most negative) horizontal strain rate
            double EhminRate = -0.01;
            // Set EhmaxRate to 0 for uniaxial strain; set to between 0 and EhminRate for anisotropic fracture pattern; set to EhminRate for isotropic fracture pattern
            double EhmaxRate = 0;
            // Set VariableStrainMagnitude to add random variation to the input strain rates
            // Strain rates for each gridblock will vary randomly from 0 to 2x specified values
            bool VariableStrainMagnitude = false;
            // Set TestComplexGeometry true to generate a geometry with non-orthogonal gridblock boundaries and convergent stress orientations - use this to check if fracture generation algorithms can cope with complex geometry
            // TestComplexGeometry will override the normal strain orientation data, so VariableStrainOrientation must be set to false; however the variable strain magnitude should be set to true
            bool TestComplexGeometry = false;
            if (TestComplexGeometry)
            {
                VariableStrainOrientation = false;
                VariableStrainMagnitude = true;
            }
            // Time units set to ma
            TimeUnits ModelTimeUnits = TimeUnits.ma;

            // Mechanical properties
            double YoungsMod = 1E+10;
            // Set VariableYoungsMod true to have laterally variable Young's Modulus
            bool VariableYoungsMod = false;
            double PoissonsRatio = 0.25;
            double BiotCoefficient = 1;
            double FrictionCoefficient = 0.5;
            // Set VariableFriction true to have laterally variable friction coefficient
            bool VariableFriction = false;
            double CrackSurfaceEnergy = 1000;
            // Set VariableCSE true to have laterally variable crack surface energy
            bool VariableCSE = false;
            // Strain relaxation data
            // Set RockStrainRelaxation to 0 for no strain relaxation and steadily increasing horizontal stress; set it to >0 for constant horizontal stress determined by ratio of strain rate and relaxation rate
            double RockStrainRelaxation = 0;
            // Set FractureRelaxation to >0 and RockStrainRelaxation to 0 to apply strain relaxation to the fractures only
            double FractureRelaxation = 0;
            // Density of initial microfractures
            double InitialMicrofractureDensity = 0.001;
            // Size distribution of initial microfractures - increase for larger ratio of small:large initial microfractures
            double InitialMicrofractureSizeDistribution = 2;
            // Subritical fracture propagation index; <5 for slow subcritical propagation, 5-15 for intermediate, >15 for rapid critical propagation
            double SubcriticalPropIndex = 10;
            double CriticalPropagationRate = 2000;

            // Stress state
            // Stress distribution scenario - use to turn on or off stress shadow effect
            // Do not use DuctileBoundary as this is not yet implemented
            StressDistribution StressDistributionScenario = StressDistribution.StressShadow;
            // Depth at the time of deformation - will control stress state
            // If DepthAtDeformation is specified, use this to calculate effective vertical stress instead of the current depth
            // If DepthAtDeformation is <=0 or NaN, OverwriteDepth will be set to false and DepthAtFracture will not be used
            double DepthAtDeformation = -1;
            bool OverwriteDepth = (DepthAtDeformation > 0);
            // Mean density of overlying sediments and fluid (kg/m3)
            double MeanOverlyingSedimentDensity = 2250;
            double FluidDensity = 1000;
            // Fluid overpressure (Pa)
            double InitialOverpressure = 0;
            // InitialStressRelaxation controls the initial horizontal stress, prior to the application of horizontal strain
            // Set InitialStressRelaxation to 1 to have initial horizontal stress = vertical stress (viscoelastic equilibrium)
            // Set InitialStressRelaxation to 0 to have initial horizontal stress = v/(1-v) * vertical stress (elastic equilibrium)
            // Set InitialStressRelaxation to -1 for initial horizontal stress = Mohr-Coulomb failure stress (critical stress state)
            double InitialStressRelaxation = 1;

            // Outputs
            // Output to file
            // These must be set to true for stand-alone version or no output will be generated
            bool WriteImplicitDataFiles = true;
            bool WriteDFNFiles = true;
            // Output file type for explicit DFN data: ASCII or FAB (NB FAB files can be loaded directly into Petrel)
            DFNFileType OutputDFNFileType = DFNFileType.ASCII;
            // Output DFN at intermediate stages of fracture growth
            int NoIntermediateOutputs = 0;
            // Flag to control interval between output of intermediate stage DFNs; if true, they will be output at equal intervals of time, if false they will be output at approximately regular intervals of total fracture area 
            bool OutputAtEqualTimeIntervals = false;
            // Flag to output the macrofracture centrepoints as a polyline, in addition to the macrofracture cornerpoints
            bool OutputCentrepoints = false;
            // Flag to output the bulk rock compliance tensor
            bool OutputComplianceTensor = false;
            // Fracture porosity control parameters
            // Flag to calculate and output fracture porosity
            bool CalculateFracturePorosity = true;
            // Flag to determine method used to determine fracture aperture - used in porosity and permeability calculation
            FractureApertureType FractureApertureControl = FractureApertureType.Uniform;
            // Fracture aperture control parameters: Uniform fracture aperture
            // Fixed aperture for Mode 1 fractures striking perpendicular to hmin in the uniform aperture case (m)
            double Mode1HMin_UniformAperture = 0.0005;
            // Fixed aperture for Mode 2 fractures striking perpendicular to hmin in the uniform aperture case (m)
            double Mode2HMin_UniformAperture = 0.0005;
            // Fixed aperture for Mode 1 fractures striking perpendicular to hmax in the uniform aperture case (m)
            double Mode1HMax_UniformAperture = 0.0005;
            // Fixed aperture for Mode 2 fractures striking perpendicular to hmax in the uniform aperture case (m)
            double Mode2HMax_UniformAperture = 0.0005;
            // Fracture aperture control parameters: SizeDependent fracture aperture
            // Size-dependent aperture multiplier for Mode 1 fractures striking perpendicular to hmin - layer-bound fracture aperture is given by layer thickness times this multiplier
            double Mode1HMin_SizeDependentApertureMultiplier = 1E-5;
            // Size-dependent aperture multiplier for Mode 2 fractures striking perpendicular to hmin - layer-bound fracture aperture is given by layer thickness times this multiplier
            double Mode2HMin_SizeDependentApertureMultiplier = 1E-5;
            // Size-dependent aperture multiplier for Mode 1 fractures striking perpendicular to hmax - layer-bound fracture aperture is given by layer thickness times this multiplier
            double Mode1HMax_SizeDependentApertureMultiplier = 1E-5;
            // Size-dependent aperture multiplier for Mode 2 fractures striking perpendicular to hmax - layer-bound fracture aperture is given by layer thickness times this multiplier
            double Mode2HMax_SizeDependentApertureMultiplier = 1E-5;
            // Fracture aperture control parameters: Dynamic fracture aperture
            // Multiplier for dynamic aperture
            double DynamicApertureMultiplier = 1;
            // Fracture aperture control parameters: Barton-Bandis model for fracture aperture
            // Joint Roughness Coefficient
            double JRC = 10;
            // Compressive strength ratio; ratio of unconfined compressive strength of unfractured rock to fractured rock
            double UCSRatio = 2;
            // Initial normal strength on fracture
            double InitialNormalStress = 2E+5;
            // Stiffness normal to the fracture, at initial normal stress
            double FractureNormalStiffness = 2.5E+9;
            // Maximum fracture closure (m)
            double MaximumClosure = 0.0005;

            // Calculation control parameters
            // Number of fracture sets
            // Set to 1 to generate a single fracture set, perpendicular to ehmin
            // Set to 2 to generate two orthogonal fracture sets, perpendicular to ehmin and ehmax; this is typical of a single stage of deformation in intact rock
            // Set to 6 or more to generate oblique fractures; this is typical of multiple stages of deformation with fracture reactivation, or transtensional strain
            int NoFractureSets = 2;
            // Fracture mode: set these to force only Mode 1 (dilatant) or only Mode 2 (shear) fractures; otherwise model will include both, depending on which is energetically optimal
            bool Mode1Only = false;
            bool Mode2Only = false;
            // Flag to check microfractures against stress shadows of all macrofractures, regardless of set
            // If None, microfractures will only be deactivated if they lie in the stress shadow zone of parallel macrofractures
            // If All, microfractures will also be deactivated if they lie in the stress shadow zone of oblique or perpendicular macrofractures, depending on the strain tensor
            // If Automatic, microfractures in the stress shadow zone of oblique or perpendicular macrofractures will be deactivated only if there are more than two fracture sets
            AutomaticFlag CheckAlluFStressShadows = AutomaticFlag.Automatic;
            // Cutoff value to use the isotropic method for calculating cross-fracture set stress shadow and exclusion zone volumes
            // For now we will set this to 1 (always use isotropic method) as this seems to give more reliable results
            double AnisotropyCutoff = 1;// 0.5;
            // Flag to allow reverse fractures; if set to false, fracture dipsets with a reverse displacement vector will not be allowed to accumulate displacement or grow
            bool AllowReverseFractures = false;
            // Maximum duration for individual timesteps; set to -1 for no maximum timestep duration
            double MaxTimestepDuration = -1;
            // Maximum increase in MFP33 allowed in each timestep - controls the optimal timestep duration
            // Increase this to run calculation faster, with fewer but longer timesteps
            double MaxTimestepMFP33Increase = 0.002;
            // Minimum radius for microfractures to be included in implicit fracture density and porosity calculations
            // If this is set to 0 (i.e. include all microfractures) then it will not be possible to calculate volumetric microfracture density as this will be infinite
            // If this is set to -1 the maximum radius of the smallest bin will be used (i.e. exclude the smallest bin from the microfracture population)
            double MinImplicitMicrofractureRadius = -1;
            // Number of bins used in numerical integration of uFP32
            // This controls accuracy of numerical calculation of microfracture populations - increase this to increase accuracy of the numerical integration at expense of runtime 
            int No_r_bins = 10;
            // Flag to calculate implicit fracture population distribution functions
            bool CalculatePopulationDistribution = true;
            // Number of macrofracture length values to calculate for each of the implicit fracture population distribution functions
            int No_l_indexPoints = 20;
            // MaxHMinLength and MaxHMaxLength control the range of macrofracture lengths to calculate for the implicit fracture population distribution functions for fractures striking perpendicular to hmin and hmax respectively
            // Set these values to the approximate maximum length of fractures generated, or 0 if this is not known; 0 will default to maximum potential length - but this may be much greater than actual maximum length
            double MaxHMinLength = 0;
            double MaxHMaxLength = 0;
            // Calculation termination controls
            // The calculation is set to stop automatically when fractures stop growing
            // This can be defined in one of three ways:
            //      - When the total volumetric ratio of active (propagating) half-macrofractures (a_MFP33) drops below a specified proportion of the peak historic value
            //      - When the total volumetric density of active (propagating) half-macrofractures (a_MFP30) drops below a specified proportion of the total (propagating and non-propagating) volumetric density (MFP30)
            //      - When the total clear zone volume (the volume in which fractures can nucleate without falling within or overlapping a stress shadow) drops below a specified proportion of the total volume
            // Increase these cutoffs to reduce the sensitivity and stop the calculation earlier
            // Use this to prevent a long calculation tail - i.e. late timesteps where fractures have stopped growing so they have no impact on fracture populations, just increase runtime
            // To stop calculation while fractures are still growing reduce the DeformationStageDuration_in or maxTimesteps_in limits
            // Ratio of current to peak active macrofracture volumetric ratio at which fracture sets are considered inactive; set to negative value to switch off this control
            double Current_HistoricMFP33TerminationRatio = -1;// 0.01;
            // Ratio of active to total macrofracture volumetric density at which fracture sets are considered inactive; set to negative value to switch off this control
            double Active_TotalMFP30TerminationRatio = -1;// 0.01;
            // Minimum required clear zone volume in which fractures can nucleate without stress shadow interactions (as a proportion of total volume); if the clear zone volume falls below this value, the fracture set will be deactivated
            double MinimumClearZoneVolume = 0.01;
            // Use the deformation stage duration and maximum timestep limits to stop the calculation before fractures have finished growing
            // Set DeformationStageDuration to -1 to continue until fracture saturation is reached
            double DeformationStageDuration = -1;
            int MaxTimesteps = 1000;
            // DFN geometry controls
            // Flag to generate explicit DFN; if set to false only implicit fracture population functions will be generated
            bool GenerateExplicitDFN = true;
            // Set false to allow fractures to propagate outside of the outer grid boundary
            bool CropAtBoundary = true;
            // Set true to link fractures that terminate due to stress shadow interaction into one long fracture, via a relay segment
            bool LinkStressShadows = false;
            // Maximum variation in fracture propagation azimuth allowed across gridblock boundary; if the orientation of the fracture set varies across the gridblock boundary by more than this, the algorithm will seek a better matching set 
            // Set to Pi/4 rad (45 degrees) by default
            double MaxConsistencyAngle = Math.PI / 4;
            // Layer thickness cutoff: explicit DFN will not be calculated for gridblocks thinner than this value
            // Set this to prevent the generation of excessive numbers of fractures in very thin gridblocks where there is geometric pinch-out of the layers
            double MinimumLayerThickness = 0;
            // Allow fracture nucleation to be controlled probabilistically, if the number of fractures nucleating per timestep is less than the specified value - this will allow fractures to nucleate when gridblocks are small
            // Set to 0 to disable probabilistic fracture nucleation
            // Set to -1 for automatic (probabilistic fracture nucleation will be activated whenever searching neighbouring gridblocks is also active; if SearchNeighbouringGridblocks is set to automatic, this will be determined independently for each gridblock based on the gridblock geometry)
            double ProbabilisticFractureNucleationLimit = -1;
            // Flag to control the order in which fractures are propagated within each timestep: if true, fractures will be propagated in order of nucleation time regardless of fracture set; if false they will be propagated in order of fracture set
            // Propagating in strict order of nucleation time removes bias in fracture lengths between sets, but will add a small overhead to calculation time
            bool PropagateFracturesInNucleationOrder = true;
            // Flag to control whether to search adjacent gridblocks for stress shadow interaction; if set to automatic, this will be determined independently for each gridblock based on the gridblock geometry
            AutomaticFlag SearchNeighbouringGridblocks = AutomaticFlag.Automatic;
            // Minimum radius for microfractures to be included in explicit DFN
            // Set this to 0 to exclude microfractures from DFN; set to between 0 and half layer thickness to include larger microfractures in the DFN
            double MinExplicitMicrofractureRadius = 0;
            // Number of cornerpoints defining the microfracture polygons in the explicit DFN
            // Set to zero to output microfractures as just a centrepoint and radius; set to 3 or greater to output microfractures as polygons defined by a list of cornerpoints
            int Number_uF_Points = 8;
            // Not yet implemented - keep this at 0
            double MinDFNMacrofractureLength = 0;

            // Create a random number generator for randomising properties, if required
            Random RandomNumberGenerator = new Random();

#if READINPUTFROMFILE
            // Read default values from input file
            Console.WriteLine("Reading the DFNGenerator configuration file...");
            string[] inputfile_lines = File.ReadAllLines(inputfile_name);
            // NB we will create a list of gridblock overrides and include files - they will be processed later
            List<List<string>> GBoverrides = new List<List<string>>();
            List<string> CurrentGBoverride = new List<string>();
            List<string> IncludeFiles = new List<string>();
            bool GBoverride = false;
            foreach (string line in inputfile_lines)
            {
                // Ignore comment lines
                if (line.StartsWith("%"))
                    continue;

                // Split the line into components and echo to console
                // All lines should have at least 2 components; if not we can ignore them
                string[] line_split = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (line_split.Length < 2)
                    continue;

                // Check if we are in a gridblock override block, and if so add the line to the list of gridblock overrides - we will deal with these later
                if (GBoverride)
                {
                    CurrentGBoverride.Add(line);
                    if (line_split[0] == "End")
                    {
                        GBoverride = false;
                        GBoverrides.Add(CurrentGBoverride);
                    }

                    continue;
                }
                else if (line_split[0] == "Gridblock")
                {
                    GBoverride = true;
                    CurrentGBoverride = new List<string>();
                    CurrentGBoverride.Add(line);
                    continue;
                }
                else
                {
                    Console.WriteLine(line_split[0] + " " + line_split[1]);
                }

                // Check if this is an include statement and if so add to the list - we will deal with this later
                if (line_split[0] == "Include")
                {
                    IncludeFiles.Add(line_split[1]);
                    continue;
                }

                // Now we can process specified universal or default values
                // Catch any exceptions due to invalid data formats
                try
                {
                    switch (line_split[0])
                    {
                        // Main properties
                        // Grid size
                        case "NoRows":
                            NoRows = Convert.ToInt32(line_split[1]);
                            break;
                        case "NoCols":
                            NoCols = Convert.ToInt32(line_split[1]);
                            break;
                        // Gridblock size
                        case "Width_EW":
                        case "width_EW": // For backwards compatibility
                            Width_EW = Convert.ToDouble(line_split[1]);
                            break;
                        case "Length_NS":
                        case "length_NS": // For backwards compatibility
                            Length_NS = Convert.ToDouble(line_split[1]);
                            break;
                        case "LayerThickness":
                            LayerThickness = Convert.ToDouble(line_split[1]);
                            break;
                        // Model location 
                        // Use the origin offset to set the absolute XY coordinates of the SW corner of the bottom left gridblock
                        case "OriginXOffset":
                            OriginXOffset = Convert.ToDouble(line_split[1]);
                            break;
                        case "OriginYOffset":
                            OriginYOffset = Convert.ToDouble(line_split[1]);
                            break;
                        case "Depth":
                            Depth = Convert.ToDouble(line_split[1]);
                            break;
                        // Strain orientatation
                        case "EhminAzi":
                        case "Epsilon_hmin_azimuth_in": // For backwards compatibility
                            EhminAzi = Convert.ToDouble(line_split[1]);
                            break;
                        // Set VariableStrainOrientation false to have N-S minimum strain orientatation in all cells
                        // Set VariableStrainOrientation true to have laterally variable strain orientation controlled by EhminAzi and EhminCurvature
                        case "VariableStrainOrientation":
                            VariableStrainOrientation = (line_split[1] == "true");
                            break;
                        case "EhminCurvature":
                        case "Epsilon_hmin_curvature_in": // For backwards compatibility
                            EhminCurvature = Convert.ToDouble(line_split[1]);
                            break;
                        // Strain rates
                        // Set to negative value for extensional strain - this is necessary in at least one direction to generate fractures
                        // With no strain relaxation, strain rate will control rate of horizontal stress increase
                        // With strain relaxation, ratio of strain rate to strain relaxation time constants will control magnitude of constant horizontal stress
                        // Ehmin is most tensile (i.e. most negative) horizontal strain rate
                        case "EhminRate":
                        case "Epsilon_hmin_dashed_in": // For backwards compatibility
                            EhminRate = Convert.ToDouble(line_split[1]);
                            break;
                        // Set EhmaxRate to 0 for uniaxial strain; set to between 0 and EhminRate for anisotropic fracture pattern; set to EhminRate for isotropic fracture pattern
                        case "EhmaxRate":
                        case "Epsilon_hmax_dashed_in": // For backwards compatibility
                            EhmaxRate = Convert.ToDouble(line_split[1]);
                            break;
                        // Set VariableStrainMagnitude to add random variation to the input strain rates
                        // Strain rates for each gridblock will vary randomly from 0 to 2x specified values
                        case "VariableStrainMagnitude":
                            VariableStrainMagnitude = (line_split[1] == "true");
                            break;
                        // TestComplexGeometry is only used for testing and cannot be set in an input file
                        // Time units set to ma
                        case "ModelTimeUnits":
                        case "timeUnits_in": // For backwards compatibility
                            {
                                if (line_split[1] == "ma")
                                    ModelTimeUnits = TimeUnits.ma;
                                else if (line_split[1] == "year")
                                    ModelTimeUnits = TimeUnits.year;
                                else if (line_split[1] == "second")
                                    ModelTimeUnits = TimeUnits.second;
                            }
                            break;

                        // Mechanical properties
                        case "YoungsMod":
                            YoungsMod = Convert.ToDouble(line_split[1]);
                            break;
                        // Set VariableYoungsMod true to have laterally variable Young's Modulus
                        case "VariableYoungsMod":
                            VariableYoungsMod = (line_split[1] == "true");
                            break;
                        case "PoissonsRatio":
                            PoissonsRatio = Convert.ToDouble(line_split[1]);
                            break;
                        case "BiotCoefficient":
                            BiotCoefficient = Convert.ToDouble(line_split[1]);
                            break;
                        case "FrictionCoefficient":
                            FrictionCoefficient = Convert.ToDouble(line_split[1]);
                            break;
                        // Set VariableFriction true to have laterally variable friction coefficient
                        case "VariableFriction":
                            VariableFriction = (line_split[1] == "true");
                            break;
                        case "CrackSurfaceEnergy":
                            CrackSurfaceEnergy = Convert.ToDouble(line_split[1]);
                            break;
                        // Set VariableCSE true to have laterally variable crack surface energy
                        case "VariableCSE":
                            VariableCSE = (line_split[1] == "true");
                            break;
                        // Strain relaxation data
                        // Set RockStrainRelaxation to 0 for no strain relaxation and steadily increasing horizontal stress; set it to >0 for constant horizontal stress determined by ratio of strain rate and relaxation rate
                        case "RockStrainRelaxation":
                            RockStrainRelaxation = Convert.ToDouble(line_split[1]);
                            break;
                        // Set FractureRelaxation to >0 and RockStrainRelaxation to 0 to apply strain relaxation to the fractures only
                        case "FractureRelaxation":
                            FractureRelaxation = Convert.ToDouble(line_split[1]);
                            break;
                        // Density of initial microfractures
                        case "InitialMicrofractureDensity":
                        case "B": // For backwards compatibility
                            InitialMicrofractureDensity = Convert.ToDouble(line_split[1]);
                            break;
                        // Size distribution of initial microfractures - increase for larger ratio of small:large initial microfractures
                        case "InitialMicrofractureSizeDistribution":
                        case "c": // For backwards compatibility
                            InitialMicrofractureSizeDistribution = Convert.ToDouble(line_split[1]);
                            break;
                        // Subritical fracture propagation index; <5 for slow subcritical propagation, 5-15 for intermediate, >15 for rapid critical propagation
                        case "SubcriticalPropIndex":
                        case "b": // For backwards compatibility
                            SubcriticalPropIndex = Convert.ToDouble(line_split[1]);
                            break;
                        case "CriticalPropagationRate":
                            CriticalPropagationRate = Convert.ToDouble(line_split[1]);
                            break;

                        // Stress state
                        // Stress distribution scenario - use to turn on or off stress shadow effect
                        // Do not use DuctileBoundary as this is not yet implemented
                        case "StressDistributionScenario":
                        case "StressDistribution_in": // For backwards compatibility
                            {
                                if (line_split[1] == "EvenlyDistributedStress")
                                    StressDistributionScenario = StressDistribution.EvenlyDistributedStress;
                                else if (line_split[1] == "StressShadow")
                                    StressDistributionScenario = StressDistribution.StressShadow;
                                else if (line_split[1] == "DuctileBoundary")
                                    StressDistributionScenario = StressDistribution.DuctileBoundary;
                            }
                            break;
                        // Depth at the time of deformation - will control stress state
                        // If DepthAtDeformation is specified, use this to calculate effective vertical stress instead of the current depth
                        // If DepthAtDeformation is <=0 or NaN, OverwriteDepth will be set to false and DepthAtFracture will not be used
                        case "DepthAtDeformation":
                        case "DepthAtFracture": // For backwards compatibility
                            {
                                DepthAtDeformation = Convert.ToDouble(line_split[1]);
                                OverwriteDepth = (DepthAtDeformation > 0);
                            }
                            break;
                        // Mean density of overlying sediments and fluid (kg/m3)
                        case "MeanOverlyingSedimentDensity":
                        case "mean_overlying_sediment_density": // For backwards compatibility
                            MeanOverlyingSedimentDensity = Convert.ToDouble(line_split[1]);
                            break;
                        case "FluidDensity":
                        case "fluid_density": // For backwards compatibility
                            FluidDensity = Convert.ToDouble(line_split[1]);
                            break;
                        // Fluid overpressure (Pa)
                        case "InitialOverpressure":
                        case "fluid_overpressure": // For backwards compatibility
                            InitialOverpressure = Convert.ToDouble(line_split[1]);
                            break;
                        // InitialStressRelaxation controls the initial horizontal stress, prior to the application of horizontal strain
                        // Set InitialStressRelaxation to 1 to have initial horizontal stress = vertical stress (viscoelastic equilibrium)
                        // Set InitialStressRelaxation to 0 to have initial horizontal stress = v/(1-v) * vertical stress (elastic equilibrium)
                        // Set InitialStressRelaxation to -1 for initial horizontal stress = Mohr-Coulomb failure stress (critical stress state)
                        case "InitialStressRelaxation":
                            InitialStressRelaxation = Convert.ToDouble(line_split[1]);
                            break;

                        // Outputs
                        // Output to file
                        // LogCalculation and WriteDFNFiles must be set to true or no output will be generated
                        case "WriteImplicitDataFiles":
                        case "LogCalculation": // For backwards compatibility
                            WriteImplicitDataFiles = true;
                            break;
                        case "WriteDFNFiles":
                            WriteDFNFiles = true;
                            break;
                        // Output file type for explicit DFN data: ASCII or FAB (NB FAB files can be loaded directly into Petrel)
                        case "OutputDFNFileType":
                        case "OutputFileType": // For backwards compatibility
                            {
                                if (line_split[1] == "ASCII")
                                    OutputDFNFileType = DFNFileType.ASCII;
                                else if (line_split[1] == "FAB")
                                    OutputDFNFileType = DFNFileType.FAB;
                            }
                            break;
                        // Output DFN at intermediate stages of fracture growth
                        case "NoIntermediateOutputs":
                        case "noIntermediateOutputs": // For backwards compatibility
                            NoIntermediateOutputs = Convert.ToInt32(line_split[1]);
                            break;
                        // Flag to control interval between output of intermediate stage DFNs; if true, they will be output at equal intervals of time, if false they will be output at approximately regular intervals of total fracture area 
                        case "OutputAtEqualTimeIntervals":
                        case "separateIntermediateOutputsByTime": // For backwards compatibility
                            OutputAtEqualTimeIntervals = (line_split[1] == "true");
                            break;
                        // Flag to output the macrofracture centrepoints as a polyline, in addition to the macrofracture cornerpoints
                        case "OutputCentrepoints":
                        case "outputCentrepoints": // For backwards compatibility
                            OutputCentrepoints = (line_split[1] == "true");
                            break;
                        // Flag to output the bulk rock compliance tensor
                        case "OutputComplianceTensor":
                            OutputComplianceTensor = (line_split[1] == "true");
                            break;
                        // Fracture porosity control parameters
                        // Flag to calculate and output fracture porosity
                        case "CalculateFracturePorosity":
                        case "CalculateFracturePorosity_in": // For backwards compatibility
                            CalculateFracturePorosity = (line_split[1] == "true");
                            break;
                        // Flag to determine method used to determine fracture aperture - used in porosity and permeability calculation
                        case "FractureApertureControl":
                        case "FractureApertureControl_in": // For backwards compatibility
                            {
                                if (line_split[1] == "Uniform")
                                    FractureApertureControl = FractureApertureType.Uniform;
                                else if (line_split[1] == "SizeDependent")
                                    FractureApertureControl = FractureApertureType.SizeDependent;
                                else if (line_split[1] == "Dynamic")
                                    FractureApertureControl = FractureApertureType.Dynamic;
                                else if (line_split[1] == "BartonBandis")
                                    FractureApertureControl = FractureApertureType.BartonBandis;
                            }
                            break;
                        // Fracture aperture control parameters: Uniform fracture aperture
                        // Fixed aperture for Mode 1 fractures striking perpendicular to hmin in the uniform aperture case (m)
                        case "Mode1HMin_UniformAperture":
                        case "Mode1HMin_UniformAperture_in": // For backwards compatibility
                            Mode1HMin_UniformAperture = Convert.ToDouble(line_split[1]);
                            break;
                        // Fixed aperture for Mode 2 fractures striking perpendicular to hmin in the uniform aperture case (m)
                        case "Mode2HMin_UniformAperture":
                        case "Mode2HMin_UniformAperture_in": // For backwards compatibility
                            Mode2HMin_UniformAperture = Convert.ToDouble(line_split[1]);
                            break;
                        // Fixed aperture for Mode 1 fractures striking perpendicular to hmax in the uniform aperture case (m)
                        case "Mode1HMax_UniformAperture":
                        case "Mode1HMax_UniformAperture_in": // For backwards compatibility
                            Mode1HMax_UniformAperture = Convert.ToDouble(line_split[1]);
                            break;
                        // Fixed aperture for Mode 2 fractures striking perpendicular to hmax in the uniform aperture case (m)
                        case "Mode2HMax_UniformAperture":
                        case "Mode2HMax_UniformAperture_in": // For backwards compatibility
                            Mode2HMax_UniformAperture = Convert.ToDouble(line_split[1]);
                            break;
                        // Fracture aperture control parameters: SizeDependent fracture aperture
                        // Size-dependent aperture multiplier for Mode 1 fractures striking perpendicular to hmin - layer-bound fracture aperture is given by layer thickness times this multiplier
                        case "Mode1HMin_SizeDependentApertureMultiplier":
                        case "Mode1HMin_SizeDependentApertureMultiplier_in": // For backwards compatibility
                            Mode1HMin_SizeDependentApertureMultiplier = Convert.ToDouble(line_split[1]);
                            break;
                        // Size-dependent aperture multiplier for Mode 2 fractures striking perpendicular to hmin - layer-bound fracture aperture is given by layer thickness times this multiplier
                        case "Mode2HMin_SizeDependentApertureMultiplier":
                        case "Mode2HMin_SizeDependentApertureMultiplier_in": // For backwards compatibility
                            Mode2HMin_SizeDependentApertureMultiplier = Convert.ToDouble(line_split[1]);
                            break;
                        // Size-dependent aperture multiplier for Mode 1 fractures striking perpendicular to hmax - layer-bound fracture aperture is given by layer thickness times this multiplier
                        case "Mode1HMax_SizeDependentApertureMultiplier":
                        case "Mode1HMax_SizeDependentApertureMultiplier_in": // For backwards compatibility
                            Mode1HMax_SizeDependentApertureMultiplier = Convert.ToDouble(line_split[1]);
                            break;
                        // Size-dependent aperture multiplier for Mode 2 fractures striking perpendicular to hmax - layer-bound fracture aperture is given by layer thickness times this multiplier
                        case "Mode2HMax_SizeDependentApertureMultiplier":
                        case "Mode2HMax_SizeDependentApertureMultiplier_in": // For backwards compatibility
                            Mode2HMax_SizeDependentApertureMultiplier = Convert.ToDouble(line_split[1]);
                            break;
                        // Fracture aperture control parameters: Dynamic fracture aperture
                        // Multiplier for dynamic aperture
                        case "DynamicApertureMultiplier":
                        case "DynamicApertureMultiplier_in": // For backwards compatibility
                            DynamicApertureMultiplier = Convert.ToDouble(line_split[1]);
                            break;
                        // Fracture aperture control parameters: Barton-Bandis model for fracture aperture
                        // Joint Roughness Coefficient
                        case "JRC":
                        case "JRC_in": // For backwards compatibility
                            JRC = Convert.ToDouble(line_split[1]);
                            break;
                        // Compressive strength ratio; ratio of unconfined compressive strength of unfractured rock to fractured rock
                        case "UCSRatio":
                        case "UCS_ratio_in": // For backwards compatibility
                            UCSRatio = Convert.ToDouble(line_split[1]);
                            break;
                        // Initial normal strength on fracture
                        case "InitialNormalStress":
                        case "InitialNormalStress_in": // For backwards compatibility
                            InitialNormalStress = Convert.ToDouble(line_split[1]);
                            break;
                        // Stiffness normal to the fracture, at initial normal stress
                        case "FractureNormalStiffness":
                        case "FractureNormalStiffness_in": // For backwards compatibility
                            FractureNormalStiffness = Convert.ToDouble(line_split[1]);
                            break;
                        // Maximum fracture closure (m)
                        case "MaximumClosure":
                        case "MaximumClosure_in": // For backwards compatibility
                            MaximumClosure = Convert.ToDouble(line_split[1]);
                            break;

                        // Calculation control parameters
                        // Number of fracture sets
                        // Set to 1 to generate a single fracture set, perpendicular to ehmin
                        // Set to 2 to generate two orthogonal fracture sets, perpendicular to ehmin and ehmax; this is typical of a single stage of deformation in intact rock
                        // Set to 6 or more to generate oblique fractures; this is typical of multiple stages of deformation with fracture reactivation, or transtensional strain
                        case "NoFractureSets":
                            NoFractureSets = Convert.ToInt32(line_split[1]);
                            break;
                        // Fracture mode: set these to force only Mode 1 (dilatant) or only Mode 2 (shear) fractures; otherwise model will include both, depending on which is energetically optimal
                        case "Mode1Only":
                            Mode1Only = (line_split[1] == "true");
                            break;
                        case "Mode2Only":
                            Mode2Only = (line_split[1] == "true");
                            break;
                        // Flag to check microfractures against stress shadows of all macrofractures, regardless of set
                        // If None, microfractures will only be deactivated if they lie in the stress shadow zone of parallel macrofractures
                        // If All, microfractures will also be deactivated if they lie in the stress shadow zone of oblique or perpendicular macrofractures, depending on the strain tensor
                        // If Automatic, microfractures in the stress shadow zone of oblique or perpendicular macrofractures will be deactivated only if there are more than two fracture sets
                        case "CheckAlluFStressShadows":
                            {
                                if (line_split[1] == "All")
                                    CheckAlluFStressShadows = AutomaticFlag.All;
                                else if (line_split[1] == "None")
                                    CheckAlluFStressShadows = AutomaticFlag.None;
                                else if (line_split[1] == "Automatic")
                                    CheckAlluFStressShadows = AutomaticFlag.Automatic;
                            }
                            break;
                        // Cutoff value to use the isotropic method for calculating cross-fracture set stress shadow and exclusion zone volumes
                        case "AnisotropyCutoff":
                            AnisotropyCutoff = Convert.ToDouble(line_split[1]);
                            break;
                        // Flag to allow growth of reverse fractures; if set to false, fracture sets with reverse displacement will be deactivated
                        case "AllowReverseFractures":
                            AllowReverseFractures = (line_split[1] == "true");
                            break;
                        // Maximum duration for individual timesteps; set to -1 for no maximum timestep duration
                        case "MaxTimestepDuration":
                        case "maxTimestepDuration_in": // For backwards compatibility
                            MaxTimestepDuration = Convert.ToDouble(line_split[1]);
                            break;
                        // Maximum increase in MFP33 allowed in each timestep - controls the optimal timestep duration
                        // Increase this to run calculation faster, with fewer but longer timesteps
                        case "MaxTimestepMFP33Increase":
                        case "max_TS_MFP33_increase_in": // For backwards compatibility
                            MaxTimestepMFP33Increase = Convert.ToDouble(line_split[1]);
                            break;
                        // Minimum radius for microfractures to be included in implicit fracture density and porosity calculations
                        // If this is set to 0 (i.e. include all microfractures) then it will not be possible to calculate volumetric microfracture density as this will be infinite
                        // If this is set to -1 the maximum radius of the smallest bin will be used (i.e. exclude the smallest bin from the microfracture population)
                        case "MinImplicitMicrofractureRadius":
                        case "minMicrofractureRadius_in": // For backwards compatibility
                            MinImplicitMicrofractureRadius = Convert.ToDouble(line_split[1]);
                            break;
                        // Number of bins used in numerical integration of uFP32
                        // This controls accuracy of numerical calculation of microfracture populations - increase this to increase accuracy of the numerical integration at expense of runtime 
                        case "No_r_bins":
                        case "no_r_bins_in": // For backwards compatibility
                            No_r_bins = Convert.ToInt32(line_split[1]);
                            break;
                        // Flag to calculate implicit fracture population distribution functions
                        case "CalculatePopulationDistribution":
                        case "CalculatePopulationDistribution_in": // For backwards compatibility
                            CalculatePopulationDistribution = (line_split[1] == "true");
                            break;
                        // Number of macrofracture length values to calculate for each of the implicit fracture population distribution functions
                        case "No_l_indexPoints":
                        case "no_l_indexPoints_in": // For backwards compatibility
                            No_l_indexPoints = Convert.ToInt32(line_split[1]);
                            break;
                        // MaxHMinLength and MaxHMaxLength control the range of macrofracture lengths to calculate for the implicit fracture population distribution functions for fractures striking perpendicular to hmin and hmax respectively
                        // Set these values to the approximate maximum length of fractures generated, or 0 if this is not known; 0 will default to maximum potential length - but this may be much greater than actual maximum length
                        case "MaxHMinLength":
                        case "maxHMinLength": // For backwards compatibility
                            MaxHMinLength = Convert.ToDouble(line_split[1]);
                            break;
                        case "MaxHMaxLength":
                        case "maxHMaxLength": // For backwards compatibility
                            MaxHMaxLength = Convert.ToDouble(line_split[1]);
                            break;
                        // Calculation termination controls
                        // The calculation is set to stop automatically when fractures stop growing
                        // This can be defined in one of three ways:
                        //      - When the total volumetric ratio of active (propagating) half-macrofractures (a_MFP33) drops below a specified proportion of the peak historic value
                        //      - When the total volumetric density of active (propagating) half-macrofractures (a_MFP30) drops below a specified proportion of the total (propagating and non-propagating) volumetric density (MFP30)
                        //      - When the total clear zone volume (the volume in which fractures can nucleate without falling within or overlapping a stress shadow) drops below a specified proportion of the total volume
                        // Increase these cutoffs to reduce the sensitivity and stop the calculation earlier
                        // Use this to prevent a long calculation tail - i.e. late timesteps where fractures have stopped growing so they have no impact on fracture populations, just increase runtime
                        // To stop calculation while fractures are still growing reduce the DeformationStageDuration_in or maxTimesteps_in limits
                        // Ratio of current to peak active macrofracture volumetric ratio at which fracture sets are considered inactive; set to negative value to switch off this control
                        case "Current_HistoricMFP33TerminationRatio":
                        case "d_historic_MFP33_termination_ratio_in": // For backwards compatibility
                            Current_HistoricMFP33TerminationRatio = Convert.ToDouble(line_split[1]);
                            break;
                        // Ratio of active to total macrofracture volumetric density at which fracture sets are considered inactive; set to negative value to switch off this control
                        case "Active_TotalMFP30TerminationRatio":
                        case "active_total_MFP30_termination_ratio_in": // For backwards compatibility
                            Active_TotalMFP30TerminationRatio = Convert.ToDouble(line_split[1]);
                            break;
                        // Minimum required clear zone volume in which fractures can nucleate without stress shadow interactions (as a proportion of total volume); if the clear zone volume falls below this value, the fracture set will be deactivated
                        case "MinimumClearZoneVolume":
                        case "minimum_ClearZone_Volume_in": // For backwards compatibility
                            MinimumClearZoneVolume = Convert.ToDouble(line_split[1]);
                            break;
                        // Use the deformation stage duration and maximum timestep limits to stop the calculation before fractures have finished growing
                        // Set DeformationStageDuration to -1 to continue until fracture saturation is reached
                        case "DeformationStageDuration":
                        case "DeformationStageDuration_in": // For backwards compatibility
                            DeformationStageDuration = Convert.ToDouble(line_split[1]);
                            break;
                        case "MaxTimesteps":
                        case "maxTimesteps_in": // For backwards compatibility
                            MaxTimesteps = Convert.ToInt32(line_split[1]);
                            break;
                        // DFN geometry controls
                        // Flag to generate explicit DFN; if set to false only implicit fracture population functions will be generated
                        case "GenerateExplicitDFN":
                            GenerateExplicitDFN = (line_split[1] == "true");
                            break;
                        // Set false to allow fractures to propagate outside of the outer grid boundary
                        case "CropAtBoundary":
                        case "cropAtBoundary": // For backwards compatibility
                            CropAtBoundary = (line_split[1] == "true");
                            break;
                        // Set true to link fractures that terminate due to stress shadow interaction into one long fracture, via a relay segment
                        case "LinkStressShadows":
                        case "linkStressShadows": // For backwards compatibility
                            LinkStressShadows = (line_split[1] == "true");
                            break;
                        // Maximum variation in fracture propagation azimuth allowed across gridblock boundary; if the orientation of the fracture set varies across the gridblock boundary by more than this, the algorithm will seek a better matching set 
                        // Set to Pi/4 rad (45 degrees) by default
                        case "MaxConsistencyAngle":
                            MaxConsistencyAngle = Convert.ToDouble(line_split[1]);
                            break;
                        // Layer thickness cutoff: explicit DFN will not be calculated for gridblocks thinner than this value
                        // Set this to prevent the generation of excessive numbers of fractures in very thin gridblocks where there is geometric pinch-out of the layers
                        case "MinimumLayerThickness":
                            MinimumLayerThickness = Convert.ToDouble(line_split[1]);
                            break;
                        // Allow fracture nucleation to be controlled probabilistically, if the number of fractures nucleating per timestep is less than the specified value - this will allow fractures to nucleate when gridblocks are small
                        // Set to 0 to disable probabilistic fracture nucleation
                        // Set to -1 for automatic (probabilistic fracture nucleation will be activated whenever searching neighbouring gridblocks is also active; if SearchNeighbouringGridblocks is set to automatic, this will be determined independently for each gridblock based on the gridblock geometry)
                        case "ProbabilisticFractureNucleationLimit":
                        case "probabilisticFractureNucleationLimit": // For backwards compatibility
                            ProbabilisticFractureNucleationLimit = Convert.ToDouble(line_split[1]);
                            break;
                        // Flag to control the order in which fractures are propagated within each timestep: if true, fractures will be propagated in order of nucleation time regardless of fracture set; if false they will be propagated in order of fracture set
                        // Propagating in strict order of nucleation time removes bias in fracture lengths between sets, but will add a small overhead to calculation time
                        case "PropagateFracturesInNucleationOrder":
                        case "propagateFracturesInNucleationOrder": // For backwards compatibility
                            PropagateFracturesInNucleationOrder = (line_split[1] == "true");
                            break;
                        // Flag to control whether to search adjacent gridblocks for stress shadow interaction; if set to automatic, this will be determined independently for each gridblock based on the gridblock geometry
                        case "SearchNeighbouringGridblocks":
                            {
                                if (line_split[1] == "All")
                                    SearchNeighbouringGridblocks = AutomaticFlag.All;
                                else if (line_split[1] == "None")
                                    SearchNeighbouringGridblocks = AutomaticFlag.None;
                                else if (line_split[1] == "Automatic")
                                    SearchNeighbouringGridblocks = AutomaticFlag.Automatic;
                            }
                            break;
                        // Minimum radius for microfractures to be included in explicit DFN
                        // Set this to 0 to exclude microfractures from DFN; set to between 0 and half layer thickness to include larger microfractures in the DFN
                        case "MinExplicitMicrofractureRadius":
                        case "MinDFNMicrofractureRadius": // For backwards compatibility
                            MinExplicitMicrofractureRadius = Convert.ToDouble(line_split[1]);
                            break;
                        // Number of cornerpoints defining the microfracture polygons in the explicit DFN
                        // Set to zero to output microfractures as just a centrepoint and radius; set to 3 or greater to output microfractures as polygons defined by a list of cornerpoints
                        case "Number_uF_Points":
                        case "number_uF_Points": // For backwards compatibility
                            Number_uF_Points = Convert.ToInt32(line_split[1]);
                            break;
                        // Not yet implemented - keep this at 0
                        case "MinDFNMacrofractureLength":
                            MinDFNMacrofractureLength = 0;
                            break;

                        // If this is an include statement, the file name to the list - we will deal with this later
                        case "Include":
                            IncludeFiles.Add(line_split[1]);
                            break;
                        // If the property name is not recognised, give a warning 
                        default:
                            Console.WriteLine(string.Format("Warning! Property name {0} is not recognised", line_split[0]));
                            break;
                    }
                }
                catch (System.FormatException)
                {
                    Console.WriteLine(string.Format("Warning! {0} is an invalid format for {1}", line_split[1], line_split[0]));
                    Console.WriteLine("Data will be ignored");
                }
            }
#endif
            // Create arrays for variable parameters and populate them
            double[,] Epsilon_hmin_azimuth_array = new double[NoRows, NoCols];
            double[,] Epsilon_hmin_rate_array = new double[NoRows, NoCols];
            double[,] Epsilon_hmax_rate_array = new double[NoRows, NoCols];
            double[,] YoungsMod_array = new double[NoRows, NoCols];
            double[,] PoissonsRatio_array = new double[NoRows, NoCols];
            double[,] BiotCoefficient_array = new double[NoRows, NoCols];
            double[,] CrackSurfaceEnergy_array = new double[NoRows, NoCols];
            double[,] FrictionCoefficient_array = new double[NoRows, NoCols];
            double[,] SubcriticalPropIndex_array = new double[NoRows, NoCols];
            double[,] RockStrainRelaxation_array = new double[NoRows, NoCols];
            double[,] FractureRelaxation_array = new double[NoRows, NoCols];
            double[,] InitialMicrofractureDensity_array = new double[NoRows, NoCols];
            double[,] InitialMicrofractureSizeDistribution_array = new double[NoRows, NoCols];
            for (int RowNo = 0; RowNo < NoRows; RowNo++)
                for (int ColNo = 0; ColNo < NoCols; ColNo++)
                {
                    if (TestComplexGeometry)
                    {
                        double local_Epsilon_hmin_azimuth_in = 0;
                        if ((ColNo + RowNo) > 4)
                            local_Epsilon_hmin_azimuth_in += Math.PI / 2;
                        Epsilon_hmin_azimuth_array[RowNo, ColNo] = ((ColNo + RowNo) % 2 == 1 ? local_Epsilon_hmin_azimuth_in - EhminCurvature : local_Epsilon_hmin_azimuth_in + EhminCurvature);
                    }
                    else if (VariableStrainOrientation)
                    {
                        Epsilon_hmin_azimuth_array[RowNo, ColNo] = EhminAzi + ((double)(ColNo - RowNo) * (EhminCurvature));
                    }
                    else
                    {
                        Epsilon_hmin_azimuth_array[RowNo, ColNo] = EhminAzi;
                    }
                    if (VariableStrainMagnitude)
                    {
                        //Epsilon_hmin_rate_array[RowNo, ColNo] = Epsilon_hmin_dashed_in * (1 + ((double)(ColNo + RowNo) / 10));
                        //Epsilon_hmax_rate_array[RowNo, ColNo] = Epsilon_hmax_dashed_in * (1 + ((double)(ColNo + RowNo) / 10));
                        double strainMultiplier = RandomNumberGenerator.NextDouble() * 2;
                        Epsilon_hmin_rate_array[RowNo, ColNo] = EhminRate * strainMultiplier;
                        Epsilon_hmax_rate_array[RowNo, ColNo] = EhmaxRate * strainMultiplier;
                    }
                    else
                    {
                        Epsilon_hmin_rate_array[RowNo, ColNo] = EhminRate;
                        Epsilon_hmax_rate_array[RowNo, ColNo] = EhmaxRate;
                    }
                    if (VariableYoungsMod)
                        YoungsMod_array[RowNo, ColNo] = YoungsMod * (1 + (((double)(NoCols - ColNo) / (double)NoCols) * 0.05));
                    else
                        YoungsMod_array[RowNo, ColNo] = YoungsMod;
                    PoissonsRatio_array[RowNo, ColNo] = PoissonsRatio;
                    BiotCoefficient_array[RowNo, ColNo] = BiotCoefficient;
                    if (VariableCSE)
                        CrackSurfaceEnergy_array[RowNo, ColNo] = CrackSurfaceEnergy * (1 + (((double)(ColNo) / (double)NoCols) * 1));
                    else
                        CrackSurfaceEnergy_array[RowNo, ColNo] = CrackSurfaceEnergy;

                    if (VariableFriction)
                        FrictionCoefficient_array[RowNo, ColNo] = FrictionCoefficient * (1 + (((double)(ColNo) / (double)NoCols) * 1));
                    else
                        FrictionCoefficient_array[RowNo, ColNo] = FrictionCoefficient;
                    SubcriticalPropIndex_array[RowNo, ColNo] = SubcriticalPropIndex;
                    RockStrainRelaxation_array[RowNo, ColNo] = RockStrainRelaxation;
                    FractureRelaxation_array[RowNo, ColNo] = FractureRelaxation;
                    InitialMicrofractureDensity_array[RowNo, ColNo] = InitialMicrofractureDensity;
                    InitialMicrofractureSizeDistribution_array[RowNo, ColNo] = InitialMicrofractureSizeDistribution;
                }

            // Create arrays for the top and bottom of the pillars and populate them
            PointXYZ[,] PillarTops = new PointXYZ[NoRows + 1, NoCols + 1];
            PointXYZ[,] PillarBottoms = new PointXYZ[NoRows + 1, NoCols + 1];
            for (int RowNo = 0; RowNo <= NoRows; RowNo++)
                for (int ColNo = 0; ColNo <= NoCols; ColNo++)
                {
                    PointXYZ PillarTop, PillarBottom;
                    if (TestComplexGeometry)
                    {
                        PillarTop = new PointXYZ(((double)ColNo * Width_EW) - ((RowNo) % 2 == 1 ? 2 : -2) + OriginXOffset, ((double)RowNo * Length_NS) - ((ColNo) % 2 == 1 ? 2 : -2) + OriginYOffset, Depth + ((RowNo) % 2 == 1 ? 0.1 : -0.1));
                        PillarBottom = new PointXYZ(((double)ColNo * Width_EW) + OriginXOffset, ((double)RowNo * Length_NS) + OriginYOffset, Depth + LayerThickness + ((ColNo) % 2 == 1 ? 0.1 : -0.1));
                    }
                    else
                    {
                        PillarTop = new PointXYZ(((double)ColNo * Width_EW) + OriginXOffset, ((double)RowNo * Length_NS) + OriginYOffset, Depth);
                        PillarBottom = new PointXYZ(((double)ColNo * Width_EW) + OriginXOffset, ((double)RowNo * Length_NS) + OriginYOffset, Depth + LayerThickness);
                    }
                    PillarTops[RowNo, ColNo] = PillarTop;
                    PillarBottoms[RowNo, ColNo] = PillarBottom;
                }

#if READINPUTFROMFILE
            // Read include files and set property and geometry overrides
            foreach (string includefile_name in IncludeFiles)
            {
                // Check if the file actually exists
                if (!File.Exists(includefile_name))
                {
                    Console.WriteLine(string.Format("Warning! Could not find include file {0}", includefile_name));
                    continue;
                }
                else
                {
                    Console.WriteLine(string.Format("Reading include file {0}", includefile_name));

                    // Read the file data and sort it into properties
                    List<List<string>> Properties = new List<List<string>>();
                    List<string> PropertyData = new List<string>();
                    string[] includefile_lines = File.ReadAllLines(includefile_name);
                    foreach (string line in includefile_lines)
                    {
                        // Ignore comment lines
                        if (line.StartsWith("%"))
                            continue;

                        // Split the line into items
                        string[] line_split = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string item in line_split)
                        {
                            // Check if the item starts with a hash - this indicates a new property
                            if (item.StartsWith("#"))
                            {
                                // Add the current PropertyData list to the Properties list, create a new property list and add the property name to the start of it
                                Properties.Add(PropertyData);
                                PropertyData = new List<string>();
                                PropertyData.Add(item.TrimStart('#'));
                            }
                            // Otherwise just add the item to the current PropertyData list
                            else
                            {
                                PropertyData.Add(item);
                            }
                        }
                    }
                    // Add the final PropertyData list to the Properties list
                    Properties.Add(PropertyData);

                    // Loop through each of the properties
                    foreach (List<string> PropertyOverrideData in Properties)
                    {
                        if (PropertyOverrideData.Count < 1)
                            continue;

                        // Get the name of the property and remove it from the list
                        string propertyName = PropertyOverrideData[0];
                        PropertyOverrideData.RemoveAt(0);

                        // Geometry data must be processed differently
                        if (propertyName == "Geometry")
                        {
                            Console.WriteLine(string.Format("Reading values for grid geometry"));

                            // Get the number of data items, and give a warning if this does not match the number of gridblocks
                            int noDataItems = PropertyOverrideData.Count / 6;
                            int noPillars = (NoRows + 1) * (NoCols + 1);
                            if (noDataItems > noPillars)
                            {
                                Console.WriteLine(string.Format("Note: the number of specified pillar locations is greater than the number of pillars; excess values will be ignored"));
                                noDataItems = noPillars;
                            }
                            else if (noDataItems < noPillars)
                                Console.WriteLine(string.Format("Note: the number of specified pillar locations is less than the number of pillars; excess pillars will be set to default locations"));

                            // Loop through all the items in the data list
                            // NB each item consists of 6 values: PillarTopX, PillarTopY, PillarTopZ, PillarBottomX, PillarBottomY, PillarBottomZ
                            for (int itemNo = 0; itemNo < noDataItems; itemNo++)
                            {
                                // Get the row and column index for this item
                                int rowNo = itemNo / (NoCols + 1);
                                int colNo = itemNo % (NoCols + 1);

                                // Write the data item to the appropriate array
                                // Catch any exceptions due to invalid data formats
                                try
                                {
                                    // Get the data items
                                    double PillarTopX = Convert.ToDouble(PropertyOverrideData[(itemNo * 6)]);
                                    double PillarTopY = Convert.ToDouble(PropertyOverrideData[(itemNo * 6) + 1]);
                                    double PillarTopZ = Convert.ToDouble(PropertyOverrideData[(itemNo * 6) + 2]);
                                    double PillarBottomX = Convert.ToDouble(PropertyOverrideData[(itemNo * 6) + 3]);
                                    double PillarBottomY = Convert.ToDouble(PropertyOverrideData[(itemNo * 6) + 4]);
                                    double PillarBottomZ = Convert.ToDouble(PropertyOverrideData[(itemNo * 6) + 5]);

                                    PillarTops[rowNo, colNo] = new PointXYZ(PillarTopX, PillarTopY, PillarTopZ);
                                    PillarBottoms[rowNo, colNo] = new PointXYZ(PillarBottomX, PillarBottomY, PillarBottomZ);
                                }
                                catch (System.FormatException)
                                {
                                    Console.WriteLine(string.Format("Warning! Could not read data for pillar {0},{1}", colNo, rowNo));
                                    Console.WriteLine("Data will be ignored");
                                }
                            } // Next item in the data list  
                        }
                        // Process property array data
                        else
                        {
                            // Get a handle to the property data array for the specified property
                            // If the property name is not recognised, abort
                            double[,] propertyArray;
                            switch (propertyName)
                            {
                                case "EhminAzi":
                                case "Epsilon_hmin_azimuth_in": // For backwards compatibility
                                    propertyArray = Epsilon_hmin_azimuth_array;
                                    break;
                                case "EhminRate":
                                case "Epsilon_hmin_dashed_in": // For backwards compatibility
                                    propertyArray = Epsilon_hmin_rate_array;
                                    break;
                                case "EhmaxRate":
                                case "Epsilon_hmax_dashed_in": // For backwards compatibility
                                    propertyArray = Epsilon_hmax_rate_array;
                                    break;

                                case "YoungsMod":
                                    propertyArray = YoungsMod_array;
                                    break;
                                case "PoissonsRatio":
                                    propertyArray = PoissonsRatio_array;
                                    break;
                                case "BiotCoefficient":
                                    propertyArray = BiotCoefficient_array;
                                    break;
                                case "FrictionCoefficient":
                                    propertyArray = FrictionCoefficient_array;
                                    break;
                                case "CrackSurfaceEnergy":
                                    propertyArray = CrackSurfaceEnergy_array;
                                    break;
                                case "RockStrainRelaxation":
                                    propertyArray = RockStrainRelaxation_array;
                                    break;
                                case "FractureRelaxation":
                                    propertyArray = FractureRelaxation_array;
                                    break;
                                case "InitialMicrofractureDensity":
                                case "B": // For backwards compatibility
                                    propertyArray = InitialMicrofractureDensity_array;
                                    break;
                                case "InitialMicrofractureSizeDistribution":
                                case "c": // For backwards compatibility
                                    propertyArray = InitialMicrofractureSizeDistribution_array;
                                    break;
                                case "SubcriticalPropIndex":
                                case "b": // For backwards compatibility
                                    propertyArray = SubcriticalPropIndex_array;
                                    break;

                                default:
                                    Console.WriteLine(string.Format("Warning! Property name {0} is not recognised", propertyName));
                                    continue;
                            }

                            Console.WriteLine(string.Format("Reading values for property {0}", propertyName));

                            // Get the number of data items, and give a warning if this does not match the number of gridblocks
                            int noDataItems = PropertyOverrideData.Count;
                            int noGridblocks = NoRows * NoCols;
                            if (noDataItems > noGridblocks)
                            {
                                Console.WriteLine(string.Format("Warning! The number of specified {0} values is greater than the number of gridblocks; excess values will be ignored", propertyName));
                                noDataItems = noGridblocks;
                            }
                            else if (noDataItems < noGridblocks)
                            {
                                Console.WriteLine(string.Format("Warning! The number of specified {0} values is less than the number of gridblocks; excess gridblocks will be set to default values", propertyName));
                            }

                            // Loop through all the items in the data list
                            for (int itemNo = 0; itemNo < noDataItems; itemNo++)
                            {
                                // Get the data item, and check to see if it is set to the null value
                                string dataItem = PropertyOverrideData[itemNo];
                                if (dataItem == "NA")
                                    continue;

                                // Get the row and column index for this item
                                int rowNo = itemNo / NoCols;
                                int colNo = itemNo % NoCols;

                                // Write the data item to the appropriate array
                                // Catch any exceptions due to invalid data formats
                                try
                                {
                                    propertyArray[rowNo, colNo] = Convert.ToDouble(dataItem);
                                }
                                catch (System.FormatException)
                                {
                                    Console.WriteLine(string.Format("Warning! {0} is an invalid format for {1}", dataItem, propertyName));
                                    Console.WriteLine("Data will be ignored");
                                }
                            } // Next item in the data list
                        } // End process property array 
                    } // Next property array
                } // End check if file exists
            } // Next include file

            // Set overrides for individual gridblocks
            foreach (List<string> NextGBoverride in GBoverrides)
            {
                // Check if there is any data; if not move on to the next gridblock override
                if (NextGBoverride.Count < 1)
                    continue;

                // Set the row and column numbers for the gridblock to override
                int RowNo = -1;
                int ColNo = -1;
                // Catch any exceptions due to invalid data formats
                try
                {
                    string[] firstline_split = NextGBoverride[0].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (firstline_split.Length < 3)
                    {
                        Console.WriteLine("Warning! Invalid format for gridblock coordinates");
                        Console.WriteLine("Data will be ignored");
                        continue;
                    }
                    else
                    {
                        // In the input data, the first coordinate is the column number (E-W) and the second coordinate is the row number (N-S)
                        // NB The order of the column and row coordinates is reversed in the Grid object interface
                        RowNo = Convert.ToInt32(firstline_split[2]);
                        ColNo = Convert.ToInt32(firstline_split[1]);
                    }
                    if ((RowNo < 1) || (RowNo > NoRows) || (ColNo < 1) || (ColNo > NoCols))
                    {
                        Console.WriteLine(string.Format("Warning! Gridblock coordinate {0},{1} is out of range", ColNo, RowNo));
                        Console.WriteLine("Data will be ignored");
                        continue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Overrides for gridblock {0},{1}", ColNo, RowNo));
                        // Convert to zero-based coordinates
                        RowNo--;
                        ColNo--;
                    }
                }
                catch (System.FormatException)
                {
                    Console.WriteLine("Warning! Invalid format for gridblock coordinates");
                    Console.WriteLine("Data will be ignored");
                    continue;
                }

                foreach (string line in NextGBoverride)
                {
                    // First find any gridblock override blocks
                    // Ignore comment lines
                    if (line.StartsWith("%"))
                        continue;

                    // Split the line into components and echo to console
                    // All lines should have at least 2 components; if not we can ignore them
                    string[] line_split = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (line_split.Length < 2)
                        continue;

                    // Check if this is an include statement and if so ignore
                    if (line_split[0] == "Include")
                        continue;

                    // Now we can process specified values
                    // Catch any exceptions due to invalid data formats
                    try
                    {
                        // Check for start and end block markers
                        if (line_split[0] == "Gridblock")
                            continue;
                        if (line_split[0] == "End")
                            break;

                        Console.WriteLine(line_split[0] + " " + line_split[1]);

                        switch (line_split[0])
                        {

                            case "EhminAzi":
                            case "Epsilon_hmin_azimuth_in": // For backwards compatibility
                                Epsilon_hmin_azimuth_array[RowNo, ColNo] = Convert.ToDouble(line_split[1]);
                                break;
                            case "EhminRate":
                            case "Epsilon_hmin_dashed_in": // For backwards compatibility
                                Epsilon_hmin_rate_array[RowNo, ColNo] = Convert.ToDouble(line_split[1]);
                                break;
                            case "EhmaxRate":
                            case "Epsilon_hmax_dashed_in": // For backwards compatibility
                                Epsilon_hmax_rate_array[RowNo, ColNo] = Convert.ToDouble(line_split[1]);
                                break;

                            case "YoungsMod":
                                YoungsMod_array[RowNo, ColNo] = Convert.ToDouble(line_split[1]);
                                break;
                            case "PoissonsRatio":
                                PoissonsRatio_array[RowNo, ColNo] = Convert.ToDouble(line_split[1]);
                                break;
                            case "BiotCoefficient":
                                BiotCoefficient_array[RowNo, ColNo] = Convert.ToDouble(line_split[1]);
                                break;
                            case "FrictionCoefficient":
                                FrictionCoefficient_array[RowNo, ColNo] = Convert.ToDouble(line_split[1]);
                                break;
                            case "CrackSurfaceEnergy":
                                CrackSurfaceEnergy_array[RowNo, ColNo] = Convert.ToDouble(line_split[1]);
                                break;
                            case "RockStrainRelaxation":
                                RockStrainRelaxation_array[RowNo, ColNo] = Convert.ToDouble(line_split[1]);
                                break;
                            case "FractureRelaxation":
                                FractureRelaxation_array[RowNo, ColNo] = Convert.ToDouble(line_split[1]);
                                break;
                            case "InitialMicrofractureDensity":
                            case "B": // For backwards compatibility
                                InitialMicrofractureDensity_array[RowNo, ColNo] = Convert.ToDouble(line_split[1]);
                                break;
                            case "InitialMicrofractureSizeDistribution":
                            case "c": // For backwards compatibility
                                InitialMicrofractureSizeDistribution_array[RowNo, ColNo] = Convert.ToDouble(line_split[1]);
                                break;
                            case "SubcriticalPropIndex":
                            case "b": // For backwards compatibility
                                SubcriticalPropIndex_array[RowNo, ColNo] = Convert.ToDouble(line_split[1]);
                                break;

                            case "SWTopCorner":
                                {
                                    if (line_split.Length < 4)
                                        Console.WriteLine(string.Format("Warning! Need to specify 3 coordinates for {0}", line_split[0]));
                                    else
                                        PillarTops[RowNo, ColNo] = new PointXYZ(Convert.ToDouble(line_split[1]), Convert.ToDouble(line_split[2]), Convert.ToDouble(line_split[3]));
                                }
                                break;
                            case "SWBottomCorner":
                                {
                                    if (line_split.Length < 4)
                                        Console.WriteLine(string.Format("Warning! Need to specify 3 coordinates for {0}", line_split[0]));
                                    else
                                        PillarBottoms[RowNo, ColNo] = new PointXYZ(Convert.ToDouble(line_split[1]), Convert.ToDouble(line_split[2]), Convert.ToDouble(line_split[3]));
                                }
                                break;
                            case "NWTopCorner":
                                {
                                    if (line_split.Length < 4)
                                        Console.WriteLine(string.Format("Warning! Need to specify 3 coordinates for {0}", line_split[0]));
                                    else
                                        PillarTops[RowNo + 1, ColNo] = new PointXYZ(Convert.ToDouble(line_split[1]), Convert.ToDouble(line_split[2]), Convert.ToDouble(line_split[3]));
                                }
                                break;
                            case "NWBottomCorner":
                                {
                                    if (line_split.Length < 4)
                                        Console.WriteLine(string.Format("Warning! Need to specify 3 coordinates for {0}", line_split[0]));
                                    else
                                        PillarBottoms[RowNo + 1, ColNo] = new PointXYZ(Convert.ToDouble(line_split[1]), Convert.ToDouble(line_split[2]), Convert.ToDouble(line_split[3]));
                                }
                                break;
                            case "NETopCorner":
                                {
                                    if (line_split.Length < 4)
                                        Console.WriteLine(string.Format("Warning! Need to specify 3 coordinates for {0}", line_split[0]));
                                    else
                                        PillarTops[RowNo + 1, ColNo + 1] = new PointXYZ(Convert.ToDouble(line_split[1]), Convert.ToDouble(line_split[2]), Convert.ToDouble(line_split[3]));
                                }
                                break;
                            case "NEBottomCorner":
                                {
                                    if (line_split.Length < 4)
                                        Console.WriteLine(string.Format("Warning! Need to specify 3 coordinates for {0}", line_split[0]));
                                    else
                                        PillarBottoms[RowNo + 1, ColNo + 1] = new PointXYZ(Convert.ToDouble(line_split[1]), Convert.ToDouble(line_split[2]), Convert.ToDouble(line_split[3]));
                                }
                                break;
                            case "SETopCorner":
                                {
                                    if (line_split.Length < 4)
                                        Console.WriteLine(string.Format("Warning! Need to specify 3 coordinates for {0}", line_split[0]));
                                    else
                                        PillarTops[RowNo, ColNo + 1] = new PointXYZ(Convert.ToDouble(line_split[1]), Convert.ToDouble(line_split[2]), Convert.ToDouble(line_split[3]));
                                }
                                break;
                            case "SEBottomCorner":
                                {
                                    if (line_split.Length < 4)
                                        Console.WriteLine(string.Format("Warning! Need to specify 3 coordinates for {0}", line_split[0]));
                                    else
                                        PillarBottoms[RowNo, ColNo + 1] = new PointXYZ(Convert.ToDouble(line_split[1]), Convert.ToDouble(line_split[2]), Convert.ToDouble(line_split[3]));
                                }
                                break;

                            default:
                                Console.WriteLine(string.Format("Warning! Property name {0} is not recognised", line_split[0]));
                                continue;
                        }
                    }
                    catch (System.FormatException)
                    {
                        Console.WriteLine(string.Format("Warning! {0} is an invalid format for {1}", line_split[1], line_split[0]));
                        Console.WriteLine("Data will be ignored");
                    }
                }
            }
#endif

            // Progress reporter object - write progress updates to Console
            ConsoleProgressReporter progReporter = new ConsoleProgressReporter(10, true);

            // Create a grid and populate it with identical gridblocks
            FractureGrid ModelGrid = new FractureGrid(NoRows, NoCols);
            for (int RowNo = 0; RowNo < NoRows; RowNo++)
            {
                for (int ColNo = 0; ColNo < NoCols; ColNo++)
                {
                    // Get the cornerpoints for the new gridblock
                    PointXYZ SWtop, NWtop, NEtop, SEtop;
                    PointXYZ SWbottom, NWbottom, NEbottom, SEbottom;
                    SWtop = PillarTops[RowNo, ColNo];
                    NWtop = PillarTops[RowNo + 1, ColNo];
                    NEtop = PillarTops[RowNo + 1, ColNo + 1];
                    SEtop = PillarTops[RowNo, ColNo + 1];
                    SWbottom = PillarBottoms[RowNo, ColNo];
                    NWbottom = PillarBottoms[RowNo + 1, ColNo];
                    NEbottom = PillarBottoms[RowNo + 1, ColNo + 1];
                    SEbottom = PillarBottoms[RowNo, ColNo + 1];

                    // Calculate the mean depth of top surface and mean layer thickness at time of deformation - assume that these are equal to the current mean depth and layer thickness, unless the depth at the time of fracturing has been specified
                    double local_Depth = (OverwriteDepth ? DepthAtDeformation : (SWtop.Z + NWtop.Z + NEtop.Z + SEtop.Z) / 4);
                    double local_LayerThickness = ((SWbottom.Z - SWtop.Z) + (NWbottom.Z - NWtop.Z) + (NEbottom.Z - NEtop.Z) + (SEbottom.Z - SEtop.Z)) / 4;

                    // Create a new gridblock object containing the required number of fracture sets
                    GridblockConfiguration gc = new GridblockConfiguration(local_LayerThickness, local_Depth, NoFractureSets);

                    // Set the gridblock cornerpoints
                    gc.setGridblockCorners(SWtop, SWbottom, NWtop, NWbottom, NEtop, NEbottom, SEtop, SEbottom);

                    // Get the mechanical properties for the gridblock
                    double local_YoungsMod = YoungsMod_array[RowNo, ColNo];
                    double local_PoissonsRatio = PoissonsRatio_array[RowNo, ColNo];
                    double local_BiotCoefficient = BiotCoefficient_array[RowNo, ColNo];
                    double local_CrackSurfaceEnergy = CrackSurfaceEnergy_array[RowNo, ColNo];
                    double local_FrictionCoefficient = FrictionCoefficient_array[RowNo, ColNo];
                    double local_RockStrainRelaxation = RockStrainRelaxation_array[RowNo, ColNo];
                    double local_FractureRelaxation = FractureRelaxation_array[RowNo, ColNo];
                    double local_SubcriticalPropIndex = SubcriticalPropIndex_array[RowNo, ColNo];

                    // Set the mechanical properties for the gridblock
                    gc.MechProps.setMechanicalProperties(local_YoungsMod, local_PoissonsRatio, local_BiotCoefficient, local_CrackSurfaceEnergy, local_FrictionCoefficient, local_RockStrainRelaxation, local_FractureRelaxation, CriticalPropagationRate, local_SubcriticalPropIndex, ModelTimeUnits);

                    // Set the fracture aperture control properties
                    gc.MechProps.setFractureApertureControlData(DynamicApertureMultiplier, JRC, UCSRatio, InitialNormalStress, FractureNormalStiffness, MaximumClosure);

                    // Set the initial stress and strain
                    // If the initial stress relaxation value is negative, set it to the required value for a critical initial stress state
                    double local_InitialStressRelaxation = InitialStressRelaxation;
                    if (InitialStressRelaxation < 0)
                    {
                        double opt_dip = ((Math.PI / 2) + Math.Atan(local_FrictionCoefficient)) / 2;
                        local_InitialStressRelaxation = (((1 - PoissonsRatio) * ((Math.Sin(opt_dip) * Math.Cos(opt_dip)) - (local_FrictionCoefficient * Math.Pow(Math.Cos(opt_dip), 2)))) - PoissonsRatio) / (1 - (2 * PoissonsRatio));
                    }
                    double lithostatic_stress = (MeanOverlyingSedimentDensity * 9.81 * Depth);
                    double fluid_pressure = (FluidDensity * 9.81 * Depth) + InitialOverpressure;
                    gc.StressStrain.SetInitialStressStrainState(lithostatic_stress, fluid_pressure, local_InitialStressRelaxation);

                    // Get the strain azimuth and strain rates
                    double local_Epsilon_hmin_azimuth_in = Epsilon_hmin_azimuth_array[RowNo, ColNo];
                    double local_Epsilon_hmin_rate_in = Epsilon_hmin_rate_array[RowNo, ColNo];
                    double local_Epsilon_hmax_rate_in = Epsilon_hmax_rate_array[RowNo, ColNo];

                    // Calculate the minimum microfracture radius from the layer thickness, if required
                    double local_minImplicitMicrofractureRadius = (MinImplicitMicrofractureRadius < 0 ? local_LayerThickness / (double)No_r_bins : MinImplicitMicrofractureRadius);

                    // Determine whether to check for stress shadows from other fracture sets
                    bool local_checkAlluFStressShadows;
                    switch (CheckAlluFStressShadows)
                    {
                        case AutomaticFlag.None:
                            local_checkAlluFStressShadows = false;
                            break;
                        case AutomaticFlag.All:
                            local_checkAlluFStressShadows = true;
                            break;
                        case AutomaticFlag.Automatic:
                            local_checkAlluFStressShadows = (NoFractureSets > 2);
                            break;
                        default:
                            local_checkAlluFStressShadows = false;
                            break;
                    }

                    // Set the propagation control data for the gridblock
                    gc.PropControl.setPropagationControl(CalculatePopulationDistribution, No_l_indexPoints, MaxHMinLength, MaxHMaxLength, false, OutputComplianceTensor, StressDistributionScenario, MaxTimestepMFP33Increase, Current_HistoricMFP33TerminationRatio, Active_TotalMFP30TerminationRatio,
                        MinimumClearZoneVolume, DeformationStageDuration, MaxTimesteps, MaxTimestepDuration, No_r_bins, local_minImplicitMicrofractureRadius, local_checkAlluFStressShadows, AnisotropyCutoff, WriteImplicitDataFiles, local_Epsilon_hmin_azimuth_in, local_Epsilon_hmin_rate_in, local_Epsilon_hmax_rate_in, ModelTimeUnits, CalculateFracturePorosity, FractureApertureControl);

                    // Set folder path for output files
                    gc.PropControl.FolderPath = folderPath;

                    // Create the fracture sets
                    double local_InitialMicrofractureDensity = InitialMicrofractureDensity_array[RowNo, ColNo];
                    double local_InitialMicrofractureSizeDistribution = InitialMicrofractureSizeDistribution_array[RowNo, ColNo];
                    if (Mode1Only)
                        gc.resetFractures(local_InitialMicrofractureDensity, local_InitialMicrofractureSizeDistribution, FractureMode.Mode1, AllowReverseFractures);
                    else if (Mode2Only)
                        gc.resetFractures(local_InitialMicrofractureDensity, local_InitialMicrofractureSizeDistribution, FractureMode.Mode2, AllowReverseFractures);
                    else
                        gc.resetFractures(local_InitialMicrofractureDensity, local_InitialMicrofractureSizeDistribution, AllowReverseFractures);

                    // Set the fracture aperture control data for fracture porosity calculation
                    for (int fs_index = 0; fs_index < NoFractureSets; fs_index++)
                    {
                        double Mode1_UniformAperture_in, Mode2_UniformAperture_in, Mode1_SizeDependentApertureMultiplier_in, Mode2_SizeDependentApertureMultiplier_in;
                        if (fs_index == 0)
                        {
                            Mode1_UniformAperture_in = Mode1HMin_UniformAperture;
                            Mode2_UniformAperture_in = Mode2HMin_UniformAperture;
                            Mode1_SizeDependentApertureMultiplier_in = Mode1HMin_SizeDependentApertureMultiplier;
                            Mode2_SizeDependentApertureMultiplier_in = Mode2HMin_SizeDependentApertureMultiplier;
                        }
                        else if ((fs_index == (NoFractureSets / 2)) && ((NoFractureSets % 2) == 0))
                        {
                            Mode1_UniformAperture_in = Mode1HMax_UniformAperture;
                            Mode2_UniformAperture_in = Mode2HMax_UniformAperture;
                            Mode1_SizeDependentApertureMultiplier_in = Mode1HMax_SizeDependentApertureMultiplier;
                            Mode2_SizeDependentApertureMultiplier_in = Mode2HMax_SizeDependentApertureMultiplier;
                        }
                        else
                        {
                            double relativeAngle = Math.PI * ((double)fs_index / (double)NoFractureSets);
                            double HMinComponent = Math.Pow(Math.Cos(relativeAngle), 2);
                            double HMaxComponent = Math.Pow(Math.Sin(relativeAngle), 2);
                            Mode1_UniformAperture_in = (Mode1HMin_UniformAperture * HMinComponent) + (Mode1HMax_UniformAperture * HMaxComponent);
                            Mode2_UniformAperture_in = (Mode2HMin_UniformAperture * HMinComponent) + (Mode2HMax_UniformAperture * HMaxComponent);
                            Mode1_SizeDependentApertureMultiplier_in = (Mode1HMin_SizeDependentApertureMultiplier * HMinComponent) + (Mode1HMax_SizeDependentApertureMultiplier * HMaxComponent);
                            Mode2_SizeDependentApertureMultiplier_in = (Mode2HMin_SizeDependentApertureMultiplier * HMinComponent) + (Mode2HMax_SizeDependentApertureMultiplier * HMaxComponent);
                        }
                        gc.FractureSets[fs_index].SetFractureApertureControlData(Mode1_UniformAperture_in, Mode2_UniformAperture_in, Mode1_SizeDependentApertureMultiplier_in, Mode2_SizeDependentApertureMultiplier_in);
                    }

                    // Add to grid
                    ModelGrid.AddGridblock(gc, RowNo, ColNo);

#if DEBUG_FRACS
                    Console.WriteLine(string.Format("Cell {0} {1} ", RowNo, ColNo));
                    Console.WriteLine(string.Format("SWtop {0} {1} {2}", SWtop.X, SWtop.Y, SWtop.Z));
                    Console.WriteLine(string.Format("NWtop {0} {1} {2}", NWtop.X, NWtop.Y, NWtop.Z));
                    Console.WriteLine(string.Format("NEtop {0} {1} {2}", NEtop.X, NEtop.Y, NEtop.Z));
                    Console.WriteLine(string.Format("SEtop {0} {1} {2}", SEtop.X, SEtop.Y, SEtop.Z));
                    Console.WriteLine(string.Format("SWbottom {0} {1} {2}", SWbottom.X, SWbottom.Y, SWbottom.Z));
                    Console.WriteLine(string.Format("NWbottom {0} {1} {2}", NWbottom.X, NWbottom.Y, NWbottom.Z));
                    Console.WriteLine(string.Format("NEbottom {0} {1} {2}", NEbottom.X, NEbottom.Y, NEbottom.Z));
                    Console.WriteLine(string.Format("SEbottom {0} {1} {2}", SEbottom.X, SEbottom.Y, SEbottom.Z));
                    Console.WriteLine(string.Format("LayerThickness = {0}; Depth = {1};", local_LayerThickness, local_Depth));
                    Console.WriteLine(string.Format("local_Epsilon_hmin_azimuth_in = {0}; Epsilon_hmin_rate_in = {1}; Epsilon_hmax_rate_in = {2};", local_Epsilon_hmin_azimuth_in, local_Epsilon_hmin_rate_in, local_Epsilon_hmax_rate_in));
                    Console.WriteLine(string.Format("sv' {0}", gc.StressStrain.Sigma_v_eff));
                    Console.WriteLine(string.Format("Young's Mod: {0}, Poisson's ratio: {1}, Biot coefficient {2}, Crack surface energy:{3}, Friction coefficient:{4}", local_YoungsMod, local_PoissonsRatio, local_BiotCoefficient, local_CrackSurfaceEnergy, local_FrictionCoefficient));
                    Console.WriteLine(string.Format("gc = new GridblockConfiguration({0}, {1}, {2});", local_LayerThickness, local_Depth, NoFractureSets));
                    Console.WriteLine(string.Format("gc.MechProps.setMechanicalProperties({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, TimeUnits.{9});", local_YoungsMod, local_PoissonsRatio, local_BiotCoefficient, local_CrackSurfaceEnergy, local_FrictionCoefficient, local_RockStrainRelaxation, local_FractureRelaxation, CriticalPropagationRate, local_SubcriticalPropIndex, ModelTimeUnits));
                    Console.WriteLine(string.Format("gc.MechProps.setFractureApertureControlData({0}, {1}, {2}, {3}, {4}, {5});", DynamicApertureMultiplier, JRC, UCSRatio, InitialNormalStress, FractureNormalStiffness, MaximumClosure));
                    Console.WriteLine(string.Format("gc.StressStrain.setStressStrainState({0}, {1}, {2});", lithostatic_stress, fluid_pressure, InitialStressRelaxation));
                    Console.WriteLine(string.Format("gc.PropControl.setPropagationControl({0}, {1}, {2}, {3}, {4}, {5}, StressDistribution.{6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}, {21}, TimeUnits.{22}, {23}, {24}); ",
                        CalculatePopulationDistribution, No_l_indexPoints, MaxHMinLength, MaxHMaxLength, false, OutputComplianceTensor, StressDistributionScenario, MaxTimestepMFP33Increase, Current_HistoricMFP33TerminationRatio, Active_TotalMFP30TerminationRatio,
                        MinimumClearZoneVolume, DeformationStageDuration, MaxTimesteps, MaxTimestepDuration, No_r_bins, local_minImplicitMicrofractureRadius, local_checkAlluFStressShadows, AnisotropyCutoff, LogCalculation, local_Epsilon_hmin_azimuth_in, local_Epsilon_hmin_rate_in, local_Epsilon_hmax_rate_in, ModelTimeUnits, CalculateFracturePorosity, FractureApertureControl));
                    Console.WriteLine(string.Format("gc.resetFractures({0}, {1}, {2}, {3});", local_InitialMicrofractureDensity, local_InitialMicrofractureSizeDistribution, (Mode1Only ? "Mode1" : (Mode2Only ? "Mode2" : "NoModeSpecified")), AllowReverseFractures));
                    Console.WriteLine(string.Format("ModelGrid.AddGridblock(gc, {0}, {1});", RowNo, ColNo));
#endif
                }
            }
            // Set the DFN generation data
            DFNGenerationControl dfn_control = new DFNGenerationControl(GenerateExplicitDFN, MinExplicitMicrofractureRadius, MinDFNMacrofractureLength, -1, MinimumLayerThickness, MaxConsistencyAngle, CropAtBoundary, LinkStressShadows, Number_uF_Points, NoIntermediateOutputs, OutputAtEqualTimeIntervals, WriteDFNFiles, OutputDFNFileType, OutputCentrepoints, ProbabilisticFractureNucleationLimit, SearchNeighbouringGridblocks, PropagateFracturesInNucleationOrder, ModelTimeUnits);
#if DEBUG_FRACS
            Console.WriteLine(string.Format("DFNGenerationControl dfn_control = new DFNGenerationControl({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, DFNFileType.{12}, {13}, {14}, {15}, {16}, TimeUnits.{17});", GenerateExplicitDFN, MinExplicitMicrofractureRadius, MinDFNMacrofractureLength, -1, MinimumLayerThickness, MaxConsistencyAngle, CropAtBoundary, LinkStressShadows, Number_uF_Points, NoIntermediateOutputs, OutputAtEqualTimeIntervals, WriteDFNFiles, OutputDFNFileType, OutputCentrepoints, ProbabilisticFractureNucleationLimit, SearchNeighbouringGridblocks, PropagateFracturesInNucleationOrder, ModelTimeUnits));
#endif
            dfn_control.FolderPath = folderPath;
            ModelGrid.DFNControl = dfn_control;

            Console.WriteLine("Grid created");

            // Run the calculation for all gridblocks
            ModelGrid.CalculateAllFractureData(progReporter);
            Console.WriteLine("Implicit fracture data calculated");
            ModelGrid.GenerateDFN(progReporter);
            if (dfn_control.GenerateExplicitDFN)
                Console.WriteLine("DFN Generated");
            Console.WriteLine("Calculation completed!");

#if !READINPUTFROMFILE
            // If running from hardcoded data, require a user key press before terminating and closing the console window
            Console.ReadKey();
#endif
        }
    }
}
