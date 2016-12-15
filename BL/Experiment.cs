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
            archivedSolutions = new List<Solution>();
            Criterions = new List<Criterion>();
            softConstraints = new List<FunConstraint>();
            fixedConstraints = new List<FunConstraint>();
        }

        public void RunExperiment()
        {
            DecisionVariables = Variables.Where(f => f.IsDecisionVariable).ToList();
            solutions = SolutionGenerator.solutions(Variables, fixedConstraints, PointCount);
            var macroGenerator = new MacroManager(FilePath, Criterions.Select(f => f.Name), surfaceID);
            macroGenerator.CreateMacros(Variables.Select(f => f.Name), solutions);
            var connector = new FEAConnector(FilePath, Criterions.Select(f=>f.Name).ToList(), surfaceID);
            connector.ConnectToFea(solutions, Variables.Select(f => f.Name).ToList());
            connector.CollectResults();
            for (int i = 0; i < solutions.Count(); i++)
            {
                if(connector.IsSuccess(i))
                {
                    solutions[i].Feasible = connector.IsSuccess(i);
                    solutions[i].CriterionValues = connector.GetGF(i);
                    var reader = new StrainReader(connector.GetSurfaceData(i));
                    var locationProblem = new GaugeLocator.LocateGauges(reader.Surfaces, RequiredGain, TensionGauge,
                        CompressionGauge, reader.TimeCurve);
                    solutions[i].CriterionValues.Concat(locationProblem.GetGF);
                    
                }
            }
            solutions.RemoveAll(f => f.Feasible == false);
            if(runCount==0)
            {
                //Criterions.Add(new Criterion("NL", true));
                //Criterions.Add(new Criterion("Gain", false));
                //Criterions.Last().SetConstraint(RequiredGain);
                softConstraints.Add(new FunConstraint("true"));
            }
            runCount++;
            archivedSolutions.Concat(solutions);
        }

        
        public void RunLocalSearch(List<double> initialVariableValues, List<string> designPars, double deviationFromStart)
        {

        }

        List<Variable> DecisionVariables;
        
        public double RequiredGain { get; set; }
        public int PointCount { get; set; }
        public List<Variable> Variables { get; set; }
        public StrainGauge TensionGauge { get; set; }
        public StrainGauge CompressionGauge { get; set; }
        public StrainGauge NickelGauge { get; set; }
        public List<FunConstraint> fixedConstraints { get; set; }
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
        public List<Solution> ParetoSolutions { get; set; }
        public List<Solution> FeasibleSolutions { get; set; }

        public void CalculateFeasibleAndPareto()
        {
            //Допустимым считается то решение, которое прошло
            //мягкие функциональные ограничения и критериальные
            //ограничения
            ParetoSolutions = new List<Solution>();
            FeasibleSolutions = new List<Solution>();
            //определение множества допустимых решений
            foreach (var solution in solutions)
            {
                //проверка по функциональным ограничениям
                bool critConstrSat = true;
                for (int i = 0; i < Criterions.Count(); i++)
                {
                    if(!Criterions[i].ConstraintSatisfied(solution.CriterionValues[i]))
                    {
                        critConstrSat = false;
                        break;
                    }

                }
                if (!critConstrSat)//если по критериальным ограничения не проходит
                    continue;//то оставить решение в покое
                bool softConstrSat = true;
                for (int i = 0; i < softConstraints.Count(); i++)
                    if (!softConstraints[i].ConstraintSatisfied(Variables
                        .Select(f=>f.Name).ToArray(), solution.VariableValues.ToArray())  )                   
                    {
                        softConstrSat = false;
                        break;
                    }
                if (softConstrSat)
                    FeasibleSolutions.Add(solution);
                
            }

            double[][] points = new double[FeasibleSolutions.Count][];
            for (int i = 0; i < FeasibleSolutions.Count; i++)
            {
                points[i] = new double[Criterions.Count];
                for (int j = 0; j < Criterions.Count; j++)
                {
                    if (!Criterions[j].isMinimized)
                        points[i][j] = 1 / (1 + solutions[i].CriterionValues[j]);
                }
            }
            var pareto = ConstructPareto.ParetoSolIndices(points);
            foreach (var paretoSolIndex in pareto)
                ParetoSolutions.Add(FeasibleSolutions[paretoSolIndex]);
        }

        List<Solution> solutions;//решения данного прогона
        List<Solution> archivedSolutions;//решения ото всех прогонов
        


        int runCount;
    }
}
