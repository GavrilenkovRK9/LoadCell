using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Input;
using BL.Sobol;

namespace BL
{
    public interface ISolution
    {
        List<double> VariableValues { get; set; }
        List<double> CriterionValues { get; }
        void SetCriterions(FeaConnector.FEAConnector connector);
    }

    public class Solution: ISolution
    {
        public Solution(IEnumerable<Variable> variables, IEnumerable<double> decisionVariableValues)
        {
            var values = decisionVariableValues.ToArray();
            variableValues = new List<double>();

            int i = 0;
            foreach (var variable in variables)
            {
                if (variable.IsDecisionVariable)
                {
                    variableValues.Add(values[i]);
                    i++;
                }
                else
                    variableValues.Add(variable.InitValue);
            }
        }

        public Solution()
        {
            variableValues = new List<double>();
        }

        public List<double> VariableValues
        {
            get
            {
                return variableValues;
            }
            set
            {
                variableValues = value;
            }
        }

        public List<double> CriterionValues
        {
            get
            {
                return criterionValues;
            }
        }

        public void SetCriterions(FeaConnector.FEAConnector connector)
        {

        }

        private List<double> variableValues;
        private List<double> criterionValues;
    }

    public static class SolutionGenerator
    {
        public static List<Solution> solutions(IEnumerable<Variable> vars, IEnumerable<FunConstraint> constraints, int pointCount)
        {
            List<Solution> solutions = new List<Solution>();
            var decisionVariables = vars.Where(f => f.IsDecisionVariable);
            int dimension = decisionVariables.Count();
            var nonDecisionVariables = vars.Where(f => !f.IsDecisionVariable);
            var sobol = new SobolAB(pointCount, dimension, decisionVariables.Select(f => f.Lo), decisionVariables.Select(f => f.Hi));
            for (int i = 0; i < pointCount; i++)
            {
                var generatedValues = sobol.getNextSobolAB();
                if (ConstraintsSatisfied(constraints, generatedValues, decisionVariables.Select(f => f.Name)))
                    solutions.Add(new Solution(vars, generatedValues));
            }
            return solutions;
        }


        static bool ConstraintsSatisfied(IEnumerable<FunConstraint> constraints, IEnumerable<double> values, IEnumerable<string> names)
        {
            
            foreach (var constraint in constraints)
            {
                if (!constraint.ConstraintSatisfied(names.ToArray(), values.ToArray()))
                    return false;
                    
            }
            return true;
        }

    }
}
