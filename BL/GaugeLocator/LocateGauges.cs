using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Input;

namespace BL.GaugeLocator
{
    public class LocateGauges
    {
        public LocateGauges(List<Surface> surfaces, double requiredGain, 
            StrainGauge TensionGauge, StrainGauge CompressionGauge,
            double [] timeCurve)
        {            
            createRouletteToChooseSurface();
        }

        
        void get05Index(double[] timeCurve)
        {
            var list = timeCurve;
            double number = 0.5;
            double closest = list.Aggregate((x, y) => Math.Abs(x - number) < Math.Abs(y - number) ? x : y);
            index05 = Array.IndexOf(timeCurve, closest);
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

        void calculateGoalFunctions(int locationID)
        {

        }

        double getSurfaceNoFromLP_tau(double input)
        {
            return 1;
        }

        double[] surfacesRoulette;

        int index05;
        List<Surface> surfaces;
        int surfaceCount;
        double requiredGain;
        double resultingGain;
        double resultingNonlinearity;
        double[][] tentativeLocations;
        double[][] goalFunctions;

        public List<double> GetGF
        {
            get
            {
                var result = new List<double>();
                result.Add(resultingGain);
                result.Add(resultingNonlinearity);
                return result;
            }
        }


        
    }

    
}
