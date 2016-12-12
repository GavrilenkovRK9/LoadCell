using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Input;
using MathNet.Numerics;
using System.Numerics;


namespace BL.GaugeLocator
{
    public class Surface
    {
        

        public double GetMeanStrain(double r, double theta, int stepIndex, StrainGauge gauge)
        {
            double baseSide = (gauge.Width - gauge.GridWidth) / 2;
            double startCoord = r + baseSide;
            double gridPitch = gauge.GridWidth / gauge.GridCount;
            var points_interp = points.Where(f => (f.X > r && f.X < r + gauge.GridWidth));
            var coord = points.Select(f => f.X);
            if (theta == 0)
            {
                var strain = points.Select(f => f.EpsR[stepIndex]);
                var interpol = Interpolate.Linear(coord, strain);
                double endCoord = startCoord + gauge.GridWidth;
                return interpol.Integrate(startCoord, endCoord);
            }
            else//theta equals 90 degrees
            {
                var strain = points.Select(f => f.EpsT[stepIndex]);
                var interpol = Interpolate.Linear(coord, strain);
                double totalStrain = 0;
                double currentCoord = startCoord;
                for (int i = 0; i < gauge.GridCount; i++)
                {
                    totalStrain += interpol.Interpolate(currentCoord);
                    currentCoord += gridPitch;
                }
                return totalStrain / gauge.GridCount;
                


            }
            
        }

        void setSurfaceOrientation()
        {
            var firstPointX = points.Select(f => f.X).Min();
            var secondPointX = points.Select(f => f.X).Max();
            if(firstPointX.AlmostEqual(secondPointX))
            {
                isHorizontal = true;
            }
        }

        private bool isHorizontal;
        List<SurfacePoint> points;
    }

    public class SurfacePoint
    {
        public SurfacePoint(double x, double[] epsR, double[] epsT)
        {
            X = x;
            EpsR = epsR;
            EpsT = epsT;
        }

        public double X { get; set; }
        public double[] EpsR { get; set; }
        public double[] EpsT { get; set; }
        


    }

}
