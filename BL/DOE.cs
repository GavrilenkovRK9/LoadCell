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
            
        }

        public int CalculateN(int requiredNPrime)
        {
            int n0 = requiredNPrime;
            int n0Prime = getNPrime(n0);
            while(n0Prime <= 0)
            {
                int k = n0 * n0 / n0Prime;
                n0Prime = getNPrime(k);
                n0 = k;
            }
            int n1 = n0 * n0 / n0Prime;
            int n1Prime = getNPrime(n1);
            while(n1Prime < requiredNPrime)
            {
                int m = n1 * n1 / n1Prime;
                n1Prime = getNPrime(m);
                n1 = m;
            }
            return (int)(n0 + (n1Prime- n0Prime) * (requiredNPrime - n0Prime) / (double)(n1Prime - n0Prime));
        }

        private int getNPrime(int N)
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
        int dimension;
    }
}
