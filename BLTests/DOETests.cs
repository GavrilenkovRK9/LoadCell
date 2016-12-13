using Microsoft.VisualStudio.TestTools.UnitTesting;
using BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Input;

namespace BL.Tests
{
    [TestClass()]
    public class DOETests
    {
        [TestMethod()]
        public void CalculateNTest()
        {
            string path = Utils.GetDir() + "\\current.mac";
            int required = 32;
            List<Variable> vars = new List<Variable>();
            vars.Add(new Variable());
            vars.Last().Name = "boss_r";
            vars.Last().InitValue = 0.03;
            vars.Last().Lo = 0.02;
            vars.Last().Hi = 0.04;
            vars.Add(new Variable());
            vars.Last().Name = "intern_r_1";
            vars.Last().InitValue = 0.062;
            vars.Last().Lo = 0.05;
            vars.Last().Hi = 0.08;
            vars.Add(new Variable());
            vars.Last().Name = "foot_dia_mid";
            vars.Last().InitValue = 0.064;
            vars.Last().Lo = 0.06;
            vars.Last().Hi = 0.08;
            vars.Add(new Variable());
            vars.Last().Name = "flat_foot";
            vars.Last().InitValue = 0.006;
            vars.Last().Lo = 0.006;
            vars.Last().Hi = 0.012;
            foreach (var variable in vars)
                variable.IsDecisionVariable = true;
            List<FunConstraint> constraints = new List<FunConstraint>();
            constraints.Add(new FunConstraint("foot_dia_mid+flat_foot<0.078"));
            constraints.Add(new FunConstraint("intern_r_1<0.078"));
            var doe = new DOE(path, vars, constraints);
            int N = doe.CalculateN(16);
            var testConnector = new FeaConnector.FEAConnector(path);
            testConnector.ConnectFeaDOE(SolutionGenerator.solutions(vars, constraints, N), vars.Select(f => f.Name).ToList());
            testConnector.CollectResults();
            int N_actual = testConnector.GetSuccessCount();
            Assert.AreEqual(16, N_actual, 2);
            
        }
    }
}