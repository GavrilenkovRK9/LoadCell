using Microsoft.VisualStudio.TestTools.UnitTesting;
using BL.FeaConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.FeaConnector.Tests
{
    [TestClass()]
    public class FeaConnectorTests
    {
        public FeaConnectorTests()
        {
            
            var GFs = new string[] { "str_max", "eps_center", "eps_top" };
            var surfaceIds = new int[] { 6, 8, 11 };
            string path = Utils.GetDir() + "\\current.mac";
            connector = new FeaConnector(path, false, GFs.ToList(), surfaceIds.ToList());

        }

        [TestMethod()]
        public void CollectResultsTest()
        {
            testSolutions = new List<Solution>();
            testSolutions.Add(new Solution());
            testSolutions.Last().VariableValues.Add(0.06);
            testSolutions.Last().VariableValues.Add(0.015);
            testSolutions.Add(new Solution());
            testSolutions.Last().VariableValues.Add(0.1);
            testSolutions.Last().VariableValues.Add(0.015);
            testSolutions.Add(new Solution());
            testSolutions.Last().VariableValues.Add(0.058);
            testSolutions.Last().VariableValues.Add(0.017);
            testSolutions.Add(new Solution());
            testSolutions.Last().VariableValues.Add(0.062);
            testSolutions.Last().VariableValues.Add(0.02);
            string[] parameterNames = new string[] { "intern_r_1", "groove_inner_h" };
            connector.ConnectToFea(testSolutions, parameterNames.ToList());
            connector.CollectResults();
            Assert.AreEqual(1099591561, connector.GetGF(0)[0]);
            Assert.AreEqual(0.001262932854, connector.GetGF(0)[1],0.0000001);
            Assert.AreEqual(-0.001281287818, connector.GetGF(0)[2], 0.0000001);
            Assert.AreEqual(1099048031, connector.GetGF(2)[0], 0.0000001);
            Assert.AreEqual(0.001184181929, connector.GetGF(2)[1], 0.0000001);
            Assert.AreEqual(-0.001302087002, connector.GetGF(2)[2], 0.0000001);
            Assert.AreEqual(1236576888, connector.GetGF(3)[0], 0.0000001);
            Assert.AreEqual(0.001210236395, connector.GetGF(3)[1], 0.0000001);
            Assert.AreEqual(-0.001798645784, connector.GetGF(3)[2], 0.0000001);
            Assert.AreEqual(true, connector.IsSuccess(0));
            Assert.AreEqual(false, connector.IsSuccess(1));
            Assert.AreEqual(true, connector.IsSuccess(2));
            Assert.AreEqual(true, connector.IsSuccess(3));
        }

        [TestMethod()]
        public void TestConnectorWithDOEAllFail()
        {
            var GFs = new string[] { "str_max", "eps_center", "eps_top" };
            var surfaceIds = new int[] { 6, 8, 11 };
            string path = Utils.GetDir() + "\\current.mac";
            testSolutions = new List<Solution>();
            testSolutions.Add(new Solution());
            testSolutions.Last().VariableValues.Add(0.06);
            testSolutions.Last().VariableValues.Add(1);
            testSolutions.Add(new Solution());
            testSolutions.Last().VariableValues.Add(2);
            testSolutions.Last().VariableValues.Add(3);
            testSolutions.Add(new Solution());
            testSolutions.Last().VariableValues.Add(23);
            testSolutions.Last().VariableValues.Add(0);
            testSolutions.Add(new Solution());
            testSolutions.Last().VariableValues.Add(92);
            testSolutions.Last().VariableValues.Add(0.02);
            string[] parameterNames = new string[] { "intern_r_1", "groove_inner_h" };
            connector = new FeaConnector(path, true, GFs.ToList(), surfaceIds.ToList());
            connector.ConnectToFea(testSolutions, parameterNames.ToList());
            connector.CollectResults();
            Assert.AreEqual(false, connector.IsSuccess(0));
            Assert.AreEqual(false, connector.IsSuccess(1));
            Assert.AreEqual(false, connector.IsSuccess(2));
            Assert.AreEqual(false, connector.IsSuccess(3));

        }

        [TestMethod()]
        public void TestConnectorWithDOEFirstLastFail()
        {
            var GFs = new string[] { "str_max", "eps_center", "eps_top" };
            var surfaceIds = new int[] { 6, 8, 11 };
            string path = Utils.GetDir() + "\\current.mac";
            testSolutions = new List<Solution>();
            testSolutions.Add(new Solution());
            testSolutions.Last().VariableValues.Add(0.09);
            testSolutions.Last().VariableValues.Add(0.015);
            testSolutions.Add(new Solution());
            testSolutions.Last().VariableValues.Add(0.06);
            testSolutions.Last().VariableValues.Add(0.015);
            testSolutions.Add(new Solution());
            testSolutions.Last().VariableValues.Add(0.058);
            testSolutions.Last().VariableValues.Add(0.017);
            testSolutions.Add(new Solution());
            testSolutions.Last().VariableValues.Add(0.062);
            testSolutions.Last().VariableValues.Add(0.00);
            string[] parameterNames = new string[] { "intern_r_1", "groove_inner_h" };
            connector = new FeaConnector(path, true, GFs.ToList(), surfaceIds.ToList());
            connector.ConnectToFea(testSolutions, parameterNames.ToList());
            connector.CollectResults();
            Assert.AreEqual(false, connector.IsSuccess(0));
            Assert.AreEqual(true, connector.IsSuccess(1));
            Assert.AreEqual(true, connector.IsSuccess(2));
            Assert.AreEqual(false, connector.IsSuccess(3));
        }


        FeaConnector connector;
        List<Solution> testSolutions;
    }
}