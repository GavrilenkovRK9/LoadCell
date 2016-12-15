using Microsoft.VisualStudio.TestTools.UnitTesting;
using BL.GaugeLocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.FeaConnector;
using System.IO;
using BL.Input;

namespace BL.GaugeLocator.Tests
{
    [TestClass()]
    public class LocateGaugesTests
    {
        [TestMethod()]
        public void LocateGaugesTest()
        {
            var reader = new StrainReader(File.ReadAllLines("strain_123.txt"));
            var surfaces = reader.Surfaces;
            var gauge = new StrainGauge(7e-3, 5e-3, 3e-3, 3e-3, 1e-3, 30, 0.2e-3, 2e-3);
            var locator = new LocateGauges(surfaces, 2, gauge, gauge, reader.TimeCurve);
            Assert.Fail();
        }
    }
}