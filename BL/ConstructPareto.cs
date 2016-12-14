using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public static class ConstructPareto
    {
        public static List<int> ParetoSolIndices(double[][] points)
        {
            List<int> paretoSolsInd = new List<int>();
            for (int i = 0; i < points.Length; i++)
            {
                bool isPareto = true;
                for (int j = 0; j < points.Length; j++)
                {
                    if (i != j && dominates_A_B(points[j], points[i]))
                    {
                        isPareto = false;
                        break;
                    }
                }
                if (isPareto)
                    paretoSolsInd.Add(i);

            }
            return paretoSolsInd;
        }

        static bool dominates_A_B(double[] pointA, double[] pointB)
        {
            bool isBetter = false;
            bool isWorse = false;

            for (int i = 0; i < pointA.Count(); i++)
            {
                bool currentNotWorse;
                bool currentBetter;
                currentNotWorse = (pointA[i] <= pointB[i]) ? true : false;
                currentBetter = (pointA[i] < pointB[i]) ? true : false;

                if (!currentNotWorse)
                {
                    isWorse = true;
                    break;
                }
                if (currentBetter)
                    isBetter = true;
                
            }
            
            if (isBetter && !isWorse)
                return true;
            else
                return false;
        }
    }
}
