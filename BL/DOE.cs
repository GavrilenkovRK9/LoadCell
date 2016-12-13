using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Sobol;
using BL.FeaConnector;
using Constr = BL.Input.FunConstraint;
using Variable = BL.Input.Variable;

namespace BL
{
    public class DOE
    {
        public DOE(string filePath, IEnumerable<Variable> vars, IEnumerable<Constr> constraints)
        {
            variables = vars;
            this.filePath = filePath;
            this.constraints = constraints;
        }

        public int CalculateN(int requiredNPrime)
        {
            int n0 = getInitialEstimate(requiredNPrime);
            int n0Prime = getNPrime(n0);
            if (n0Prime >= requiredNPrime && n0Prime < 1.1 * requiredNPrime)
                return n0Prime;

            while(n0Prime <= 0)
            {
                int difference = (requiredNPrime - n0Prime);
                n0 += 2 * difference;
                n0Prime = getNPrime(n0);
                
            }
            int n1 = n0 + 2 * (requiredNPrime - n0Prime) + 1;
            int n1Prime = getNPrime(n1);
            while(n1Prime < requiredNPrime)
            {
                int difference = (requiredNPrime - n1Prime);
                n1 += 2 * difference;
                n1Prime = getNPrime(n1);
                
            }
            return (int)(n0 + (n1- n0) * (requiredNPrime - n0Prime) / (double)(n1Prime - n0Prime));
        }

        int getInitialEstimate(int N)
        {
            int n0 = N;
            int n0Prime = SolutionGenerator.solutions(variables, constraints, n0).Count();
            while (n0Prime <= 0)
            {
                int difference = (N - n0Prime);
                n0 += 2 * difference;
                n0Prime = SolutionGenerator.solutions(variables, constraints, n0).Count();

            }
            int n1 = n0 + 2 * (N - n0Prime);
            int n1Prime = SolutionGenerator.solutions(variables, constraints, n1).Count();
            while (n1Prime < N)
            {
                int difference = (N - n1Prime);
                n1 += 2 * difference;
                n1Prime = SolutionGenerator.solutions(variables, constraints, n1).Count();

            }
            var result = (int)(n0 + (n1 - n0) * (N - n0Prime) / (double)(n1Prime - n0Prime));
            return result;

        }


        int getNPrime(int N)
        {
            var names = variables.Select(f => f.Name);
            var generator = new MacroManager(filePath);
            var generatedSolutions = SolutionGenerator.solutions(variables, constraints, N);
            generator.CreateMacros(names, generatedSolutions);
            var connector = new FEAConnector(filePath);
            connector.ConnectFeaDOE(generatedSolutions, names.ToList());
            connector.CollectResults();
            return connector.GetSuccessCount();
            
        }

        string filePath;
        IEnumerable<Variable> variables;
        IEnumerable<Constr> constraints;
        
    }
}
