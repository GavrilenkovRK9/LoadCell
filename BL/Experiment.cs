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
        }

        public void RunExperiment()
        {
            DecisionVariables = Variables.Where(f => f.IsDecisionVariable).ToList();
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
                    var reader = new StrainReader(connector.GetSurfaceData(i));
                    var locationProblem = new GaugeLocator.LocateGauges(reader.Surfaces(), RequiredGain, TensionGauge,
                        CompressionGauge, reader.TimeCurve);
                    solutions[i].CriterionValues.Concat(locationProblem.GetGF);
                    
                }
            }
            solutions.RemoveAll(f => f.Feasible == false);
            if(runCount==0)
            {
                Criterions.Add(new Criterion("NL", true));
                Criterions.Add(new Criterion("Gain", false));
                Criterions.Last().SetConstraint(RequiredGain);
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

        void CalculateFeasibleAndPareto()
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
            //определение множества Парето-оптимальных решений
            for (int i = 0; i < FeasibleSolutions.Count(); i++)
            {
                bool isPareto = true;
                for (int j = 0; j < FeasibleSolutions.Count(); j++)
                {
                    if (i!=j && dominatesA_B(FeasibleSolutions[j], FeasibleSolutions[i]))
                    {
                        isPareto = false;
                        break;
                    }
                }
                if (isPareto)
                    ParetoSolutions.Add(FeasibleSolutions[i]);
            }
            
        }

        bool dominatesA_B(Solution A, Solution B)
        {
            bool[] notWorse = new bool[A.CriterionValues.Count()];
            bool[] better = new bool[A.CriterionValues.Count()];
            bool isBetter = false;
            bool isWorse = false;
            for (int i = 0; i < Criterions.Count(); i++)
            {
                bool currentNotWorse;
                bool currentBetter;
                if(Criterions[i].isMinimized)
                {
                    currentNotWorse = (A.CriterionValues[i] <= B.CriterionValues[i]) ? true : false;
                    currentBetter = (A.CriterionValues[i] < B.CriterionValues[i]) ? true : false;
                }
                else
                {
                    currentNotWorse = (A.CriterionValues[i] >= B.CriterionValues[i]) ? true : false;
                    currentBetter = (A.CriterionValues[i] > B.CriterionValues[i]) ? true : false;
                }
                if(!currentNotWorse)
                {
                    isWorse = true;
                    break;
                }
                if(currentBetter)
                {
                    isBetter = true;
                }
            }
            if (isBetter && !isWorse)
                return true;
            else
                return false;
        }

        List<Solution> solutions;//решения данного прогона
        List<Solution> archivedSolutions;//решения ото всех прогонов
        List<Solution> ParetoSolutions { get; set; }
        List<Solution> FeasibleSolutions { get; set; }


        int runCount;
    }
}
