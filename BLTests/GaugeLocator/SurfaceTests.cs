using Microsoft.VisualStudio.TestTools.UnitTesting;
using BL.GaugeLocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Input;

namespace BL.GaugeLocator.Tests
{
    [TestClass()]
    public class SurfaceTests
    {
        [TestMethod()]
        public void GetMeanStrainTest()
        {
            var gauge = new StrainGauge(7e-3, 5e-3, 3e-3, 3e-3, 1e-3, 30, 0.2e-3, 2e-3);
            double[] coord = new double[] { 0, 0.002, 0.004, 0.006, 0.008, 0.01, 0.012, 0.014, 0.016, 0.018 };
            double[] axial05 = new double[] { 1e-3, 0.99e-3, 0.98e-3, 0.97e-3, 0.96e-3, 0.955e-3, 0.95e-3, 0.945e-3, 0.94e-3, 0.94e-3 };
            double[] axial10 = new double[10];
            for (int i = 0; i < 10; i++)
                axial10[i] = 2 * axial05[i];
            double[] trans05 = new double[] { 1e-3, 0.98e-3, 0.97e-3, 0.96e-3, 0.95e-3, 0.94e-3, 0.93e-3, 0.92e-3, 0.91e-3, 0.90e-3 };
            double[] trans10 = new double[10];
            for (int i = 0; i < 10; i++)
                trans10[i] = 2 * trans05[i];
            double[][] axial = new double[2][];
            double[][] trans = new double[2][];
            axial[0] = axial05;
            axial[1] = axial10;
            trans[0] = trans05;
            trans[1] = trans10;
            var surface = new Surface(coord, axial, trans);
            var qqq = surface.GetMeanStrain(0.005, 0, 1, gauge);
            Assert.Fail();
        }
    }
}