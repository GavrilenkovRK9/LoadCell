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
        public Surface(double[] coord, double[][] axialStrain, double[][] tangentialStrain)
        {
            int stepCount = axialStrain.Length;
            int nodeCount = coord.Length;
            nodes = new List<Node>();
            for (int i = 0; i < nodeCount; i++)
                nodes.Add(new Node(coord[i], Utils.getCol(axialStrain, i), Utils.getCol(axialStrain, i)));
            minCoord = coord.Min();
            maxCoord = coord.Max();
        }
        
        public double GetMeanStrain(double r, double theta, int stepIndex, StrainGauge gauge)
        {
            double baseSide = (gauge.Width - gauge.GridWidth) / 2;
            double startCoord = r + baseSide;
            double gridPitch = gauge.GridWidth / gauge.GridCount;
            var points_interp = nodes.Where(f => (f.X > r && f.X < r + gauge.GridWidth));
            var coord = nodes.Select(f => f.X);
            if (theta == 0)
            {
                var strain = nodes.Select(f => f.EpsR[stepIndex]);
                var interpol = Interpolate.Linear(coord, strain);
                double endCoord = startCoord + gauge.GridWidth;
                return interpol.Integrate(startCoord, endCoord);
            }
            else//theta equals 90 degrees
            {
                var strain = nodes.Select(f => f.EpsT[stepIndex]);
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
        
        public double minCoord { get; set; }
        public double maxCoord { get; set; }
               
        List<Node> nodes;
    }

    public class Node
    {
        public Node(double x, double[] epsR, double[] epsT)
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
