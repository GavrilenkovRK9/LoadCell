using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Input;
using BL.Sobol;

namespace BL.GaugeLocator
{
    public class LocateGauges
    {
        public LocateGauges(List<Surface> surfaces, double requiredGain, 
            StrainGauge TensionGauge, StrainGauge CompressionGauge,
            double [] timeCurve)
        {
            gauge = TensionGauge;
            get05Index(timeCurve);
            this.surfaces = surfaces;
            createRouletteToChooseSurface();
            calculatePointCount();
            findLocations(requiredGain);
            

        }


        void findLocations(double requiredGain)
        {
            double[] bestScheme = null;
            double minNl = 100;
            if (surfaceCount == 1)
                generateVariantsOneSurface();
            else
                generateVariantsMultipleSurfaces();
            for (int i = 0; i < pointCount; i++)
                evalGF(i);
            var paretoSolutions = ConstructPareto.ParetoSolIndices(goalFunctions);
            int qqq = 3;


        }

        void calculatePointCount()
        {
            pointCount = 0;
            foreach (var surface in surfaces)
                pointCount += (int)((surface.maxCoord - surface.minCoord) / positioningAccuracy);
            pointCount *= 4;
            tentativeLocations = new double[pointCount][];
            goalFunctions = new double[pointCount][];
            for (int i = 0; i < pointCount; i++)
            {
                tentativeLocations[i] = new double[decisionVariableCount+1];
                goalFunctions[i] = new double[2];
            }
                
            
            
            
        }

        void generateVariantsMultipleSurfaces()
        {
            var sobol = new Sobol01(pointCount, decisionVariableCount);
            for (int i = 0; i < pointCount; i++)
            {
                var params_ = sobol.getNextSobol01();
                tentativeLocations[i][0] = getSurfaceNoFromLP_tau(params_[0]);
                tentativeLocations[i][1] = getSurfaceNoFromLP_tau(params_[1]);
                tentativeLocations[i][2] = getLocationRFromLP_Tau(params_[2], gauge, 0);
                tentativeLocations[i][3] = getLocationRFromLP_Tau(params_[3], gauge, 0);
                tentativeLocations[i][4] = getLocationThetaFromLP_Tau(params_[4]);
                tentativeLocations[i][5] = 90- getLocationThetaFromLP_Tau(params_[4]);
            }

        }


        void generateVariantsOneSurface()
        {
            var sobol = new Sobol01(pointCount, decisionVariableCountOneSurface);
            for (int i = 0; i < pointCount; i++)
            {
                var params_ = sobol.getNextSobol01();
                tentativeLocations[i][2] = getLocationRFromLP_Tau(params_[0], gauge, 0);
                tentativeLocations[i][3] = getLocationRFromLP_Tau(params_[1], gauge, 0);
                tentativeLocations[i][4] = getLocationThetaFromLP_Tau(params_[2]);
                tentativeLocations[i][5] = getLocationThetaFromLP_Tau(params_[3]);
            }

        }

        void evalGF(int i)
        {
            int s_id_1 = (int)tentativeLocations[i][0];
            int s_id_2 = (int)tentativeLocations[i][1];
            double r1 = tentativeLocations[i][2];
            double r2 = tentativeLocations[i][3];
            double theta1 = tentativeLocations[i][4];
            double theta2 = tentativeLocations[i][5];
            var deltaR_Tension05 = getDeltaR(surfaces[s_id_1].GetMeanStrain(r1, theta1, index05, gauge),gauge);
            var deltaR_Tension10 = getDeltaR(surfaces[s_id_1].GetMeanStrain(r1, theta1, index10, gauge), gauge);
            var deltaR_Compression05 = getDeltaR(surfaces[s_id_2].GetMeanStrain(r2, theta2, index05, gauge), gauge);
            var deltaR_Compression10 = getDeltaR(surfaces[s_id_2].GetMeanStrain(r2, theta2, index10, gauge), gauge);
            var gain05 = getGain(deltaR_Tension05, deltaR_Compression05);
            var gain10 = getGain(deltaR_Tension10, deltaR_Compression10);
            goalFunctions[i][0] = Math.Abs((gain05 - timeForNl* gain10) / gain10 * 100);
            goalFunctions[i][1] = 1/(1+Math.Abs(gain10));

        }

        double getDeltaR(double strain, StrainGauge gauge)
        {
            double naturalStrain = Math.Log(strain + 1);
            return Math.Exp(gauge.GaugeFactor * naturalStrain) - 1;
        }

        double getGain(double deltaR1, double deltaR2)
        {
            
            if (deltaR1 * deltaR2 > 0)
                return 0;
            else
            {
                return 1000*(deltaR1 - deltaR2) / (2 + deltaR1 + deltaR2);
            }
            
        }
        

        void get05Index(double[] timeCurve)
        {
            var list = timeCurve;
            double number = 0.5;
            double closest = list.Aggregate((x, y) => Math.Abs(x - number) < Math.Abs(y - number) ? x : y);
            index05 = Array.IndexOf(timeCurve, closest);
            index10 = timeCurve.Length - 1;
            timeForNl = timeCurve[index05];
        }
        
        
        void createRouletteToChooseSurface()
        {
            surfaceCount = surfaces.Count();
            surfacesRoulette = new double[surfaceCount + 1];
            double surfaceRouletteSpan = 1 / surfaceCount;
            for (int i = 0; i < surfaceCount; i++)
            {
                surfacesRoulette[i] = i * surfaceRouletteSpan;
            }
            surfacesRoulette[surfaceCount - 1] = 1;
        }

        
        double getSurfaceNoFromLP_tau(double input)
        {
            if (input == 0)
                return 0;
            for (int i = 0; i < surfaceCount; i++)
            {
                if (input > surfacesRoulette[i] && input <= surfacesRoulette[i + 1])
                    return i;
            }
            return 0;
        }

        double getLocationThetaFromLP_Tau(double input)
        {
            if (input <= 0.5)
                return 0;
            else
                return 90;
        }

        double getLocationRFromLP_Tau(double input, StrainGauge gauge, int surfaceID)
        {
            double leftLimit = surfaces[surfaceID].minCoord + gauge.Length;
            double rightLimit = surfaces[surfaceID].maxCoord - 2 * gauge.Length;
            return leftLimit + input * (rightLimit - leftLimit);
        } 


        double[] surfacesRoulette;

        int pointCount;
        int index05;
        double timeForNl;
        int index10;
        List<Surface> surfaces;
        int surfaceCount;
        double requiredGain;
        double resultingGain;
        double resultingNonlinearity;
        double[][] tentativeLocations;
        double[][] goalFunctions;
        
        StrainGauge gauge;



        public List<double> GetGF
        {
            get
            {
                var result = new List<double>();
                //result.Add(resultingNonlinearity);
                //result.Add(resultingGain);
                result.Add(0.005);
                result.Add(2.3);
                return result;
            }
        }

        const double positioningAccuracy = 0.1e-3;
        const int decisionVariableCount = 5;
        const int decisionVariableCountOneSurface = 4;


        
    }

    
}
