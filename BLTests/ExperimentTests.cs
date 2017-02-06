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
    public class ExperimentTests
    {
        [TestMethod()]
        public void RunExperimentTest()
        {
            string path = "forDebug.mac";
            var gauge = new StrainGauge(7e-3, 5e-3, 3e-3, 3e-3, 1e-3, 30, 0.2e-3, 2e-3);
            List<Variable> variables = new List<Variable>();
            variables.Add(new Variable("boss_r", 0.03, 0.025, 0.035, true));
            variables.Add(new Variable("boss_h", 0.04, 0.035, 0.045, true));
            variables.Add(new Variable("boss_h_2", 0.030, 0.035, 0.045, true));
            variables.Add(new Variable("intern_r_1", 0.062, 0.035, 0.045, true));
            variables.Add(new Variable("intern_h_1", 0.015, 0.035, 0.045, true));
            variables.Add(new Variable("intern_r_3", 0.052, 0.035, 0.045, true));
            variables.Add(new Variable("intern_r_4", 0.054, 0.035, 0.045, true));
            variables.Add(new Variable("intern_h_2", 0.01, 0.035, 0.045, true));
            foreach (var variable in variables)
            {
                variable.Lo = 0.9 * variable.InitValue;
                variable.Hi = 1.1 * variable.InitValue;
            }
            int N = 32;
            Experiment exp = new Experiment();
            exp.Variables = variables;
            exp.Criterions.Add(new Criterion("stress", true));
            exp.Criterions.Add(new Criterion("slip3", true));
            exp.Criterions.Add(new Criterion("gain", false));
            exp.Criterions.Add(new Criterion("NL", true));
            exp.Criterions.Add(new Criterion("mass", true));
            exp.fixedConstraints.Add(new FunConstraint("boss_h>boss_h_2"));
            exp.fixedConstraints.Add(new FunConstraint("boss_r<intern_r_3-10e-3"));
            exp.fixedConstraints.Add(new FunConstraint("intern_h_2<intern_h_1-2e-3"));
            exp.TensionGauge = gauge;
            exp.CompressionGauge = gauge;
            exp.NickelGauge = gauge;
            exp.PointCount = 8;
            exp.FilePath = path;
            exp.surfaceID = new List<int>{ 6,10};
            exp.RunExperiment();
            exp.CalculateFeasibleAndPareto();
            var qq1 = exp.ParetoSolutions;
            exp.Criterions.First().SetConstraint(1000);
            exp.CalculateFeasibleAndPareto();
            var qq2 = exp.ParetoSolutions;
            Assert.AreEqual(1, qq2.Count);
        }

        
    }
}