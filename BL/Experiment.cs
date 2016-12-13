using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Input;
using BL.FeaConnector;

namespace BL
{
    public class Experiment
    {
        public Experiment()
        {

        }

        public void RunExperiment()
        {
            locationProblems = new List<GaugeLocator.LocateGauges>();
            solutions = SolutionGenerator.solutions(Variables, fixedConstraints, PointCount);
            var macroGenerator = new MacroManager(FilePath, Criterions.Select(f => f.Name), surfaceID);
            macroGenerator.CreateMacros(Variables.Select(f => f.Name), solutions);
            var connector = new FEAConnector(FilePath);
            connector.ConnectToFea(solutions, Variables.Select(f => f.Name).ToList());
            connector.CollectResults();
            for (int i = 0; i < solutions.Count(); i++)
            {
                if(connector.IsSuccess(i))
                {
                    solutions[i].Feasible = connector.IsSuccess(i);
                    solutions[i].CriterionValues = connector.GetGF(i);
                    var reader = new StrainReader()
                    locationProblems.Add(new GaugeLocator.LocateGauges(null, RequiredGain, TensionGauge,
                        CompressionGauge, null));
                }
            }

            solutions.RemoveAll(f => f.Feasible == false);
        }

        
        public double RequiredGain { get; set; }
        public int PointCount { get; set; }
        public List<Variable> Variables { get; set; }
        public StrainGauge TensionGauge { get; set; }
        public StrainGauge CompressionGauge { get; set; }
        public StrainGauge NickelGauge { get; set; }
        public List<FunConstraint> fixedConstraints { get; set; }
        public List<FunConstraint> CriterionContraints { get; set; }
        public List<FunConstraint> softConstraints { get; set; }
        public List<Criterion> Criterions { get; set; }
        public List<int> surfaceID { get; set; }
        public string FilePath { get; set; }
        public List<Solution> RawSolutions
        {
            get
            {
                return solutions;
            }
            set
            {
                solutions = value;
            }
        }
        List<Solution> solutions;
        List<GaugeLocator.LocateGauges> locationProblems;
    }
}
