using Microsoft.VisualStudio.TestTools.UnitTesting;
using BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BL;

namespace BL.Tests
{
    [TestClass()]
    public class ConstructParetoTests
    {
        [TestMethod()]
        public void paretoSolIndicesTest()
        {
            double[][] points = new double[9][];
            points[0] = new double[] { 1, 0.15 };
            points[1] = new double[] { 1, 0.35 };
            points[2] = new double[] { 2.5, 0.25 };
            points[3] = new double[] { 4, 0.1 };
            points[4] = new double[] { 7, 0 };
            points[5] = new double[] { 6, 0.2 };
            points[6] = new double[] { 5, 0.35 };
            points[7] = new double[] { 2.5, 0.4 };
            points[8] = new double[] { 1, 0.9 };
            var pareto = ConstructPareto.ParetoSolIndices(points);
            Assert.AreEqual(3, pareto.Count);
            Assert.AreEqual(0, pareto[0]);
            Assert.AreEqual(3, pareto[1]);
            Assert.AreEqual(4, pareto[2]);
        }

        [TestMethod]
        public void testPareto4Dimensions()
        {
            readInputForSecondTest();
            var pareto = ConstructPareto.ParetoSolIndices(testPoint);
            Assert.AreEqual(truePareto.Length, pareto.Count());
            for (int i = 0; i < pareto.Count(); i++)
            {
                Assert.AreEqual(truePareto[i], pareto[i]);
            }
        }


        void readInputForSecondTest()
        {
            var lines = File.ReadAllLines("testInput.txt");
            testPoint = new double[lines.Count()][];
            for (int i = 0; i < lines.Count(); i++)
            {
                testPoint[i] = Utils.ParseCSV(lines[i]).ToArray();
            }
            var linesResult = File.ReadAllLines("truePareto.txt");
            truePareto = new int[linesResult.Count()];
            for (int i = 0; i < linesResult.Count(); i++)
            {
                truePareto[i] = int.Parse(linesResult[i]);
            }
        }

        double[][] testPoint;
        int[] truePareto;
    }
}