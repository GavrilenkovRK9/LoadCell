using Microsoft.VisualStudio.TestTools.UnitTesting;
using BL.FeaConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BL.Input;

namespace BL.FeaConnector.Tests
{
    [TestClass()]
    public class StrainReaderTests
    {
        [TestMethod()]
        public void SurfacesTest()
        {
            var reader = new StrainReader(File.ReadAllLines("strain_123.txt"));
            var surfaces = reader.Surfaces;
            var gauge = new StrainGauge(7e-3, 5e-3, 3e-3, 3e-3, 1e-3, 30, 0.2e-3, 2e-3);
            double qqq = surfaces.First().GetMeanStrain(0.045, 0, 6, gauge);
            Assert.AreEqual(0.015, surfaces.Last().minCoord);
            Assert.AreEqual(0.027, surfaces.Last().maxCoord);
            Assert.AreEqual(0.030, surfaces.First().minCoord);
            Assert.AreEqual(0.059, surfaces.First().maxCoord);
            
        }
    }
}