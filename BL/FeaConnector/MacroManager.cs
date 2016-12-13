using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NCalc;

/// <summary>
/// пространство имен, содержащее классы для 
/// связи в МКЭ системой ANSYS
/// </summary>
namespace BL.FeaConnector
{
    public class MacroManager
    {
        public MacroManager(string filePath, bool isForDOE, IEnumerable<string> GFs, IEnumerable<int> SurfaceID)
        {
            macro = File.ReadAllLines(filePath).ToList();
            if(isForDOE)
            {
                truncateMacro(new string[] { "solve", "lssolve" });
                appendCommand("*set, id, 0");
                appendCommands(Utils.GetScriptsDir() + "doe.mac");
            }
            else
            {
                appendCommand("*set,id,0");
                addCommandsForGF_OutPut(GFs, SurfaceID);
            }
            appendCommands(Utils.GetScriptsDir() + "log.mac");
        }

        public void CreateMacros(List<string> variables, List<List<double>> values)
        {
            Utils.CleanDir(Utils.GetTempDir());
            int macroCount = values.Count();
            var temp1 = variables;
            temp1.Add("id");
            findDesignVariables(variables.ToArray());
            for (int i = 0; i < macroCount; i++)
            {
                var temp = values[i].ToList();
                temp.Add(i);
                updateChandingVariables(temp.ToArray(), temp1.ToArray());
                File.WriteAllLines(Utils.GetTempDir() + string.Format("solution_{0}.mac", i), macro);
            }
        }
        
        public static string[] GetVariables(string filePath)
        {
            var variables = new List<string>();
            var SetVarRegex = new Regex(@"\*set,.+,.+", RegexOptions.IgnoreCase);
            foreach (var line in File.ReadAllLines(filePath))
            {
                if (SetVarRegex.IsMatch(line))
                    variables.Add(line.Split(',')[1]);
            }
            return variables.ToArray();
        }

        public static double[] GetValuesOfVariables(string filePath, string[] variableNames)
        {
            //функция возвращает значения указанных переменных в данном файле
            variableNames = variableNames.Select(f => f.Replace(" ", string.Empty)).ToArray();
            var SetVarEx = new Regex(@"\*set,.+,.+", RegexOptions.IgnoreCase);
            double[] values = new double[variableNames.Count()];
            var feaScript = File.ReadAllLines(filePath);

            foreach (var line in feaScript)
            {
                if (SetVarEx.IsMatch(line))
                {
                    var nameSection = line.Replace(" ", string.Empty).Split(',')[1];
                    var expressionSection = line.Replace(" ", string.Empty).Split(',')[2];
                    if (nameIsInTheList(variableNames, nameSection))
                    {
                        int indexOf = Array.IndexOf(variableNames.ToArray(), nameSection);
                        Expression exp = new Expression(expressionSection);
                        try
                        {
                            values[indexOf] = (double)exp.Evaluate();
                        }
                        catch
                        {
                            values[indexOf] = double.NaN;//expression is not arithmetical
                        }


                    }
                }
            }
            return values;
        }

        static bool nameIsInTheList(IEnumerable<string> list, string name)
        {
            foreach (var item in list)
                if (item.Equals(name))
                    return true;
            return false;
        }

        void addCommandsForGF_OutPut(IEnumerable<string> GF, IEnumerable<int> surfaceID)
        {
            appendCommand(@"*cfopen,results%id%,txt");
            appendCommand(@"*cfwrite,," + string.Join(",", GF));
            appendCommand(@"*cfclos");
            appendCommand(string.Format(@"*dim,lines,array,{0}", surfaceID.Count()));
            appendCommand(string.Format("lineCount={0}", surfaceID.Count()));
            int i = 0;
            
            foreach (var surface in surfaceID)
            {
                appendCommand(string.Format(@"lines({0})={1}", i+1, surface));
                i++;
            }
            appendCommands(Utils.GetScriptsDir() + "strain.mac");


        }

        void findDesignVariables(string [] names)
        {
            var SetVarEx = new Regex(@"\*set,.+,.+", RegexOptions.IgnoreCase);
            variableLocationInMacro = new int[names.Length];
            int i = 0;
            foreach (var line in macro)
            {
                if (SetVarEx.IsMatch(line))
                {
                    var nameSection = line.Replace(" ", string.Empty).Split(',')[1];
                    foreach (var name in names)
                        if (nameSection.Equals(name))
                        {
                            int k = Array.IndexOf(names, name);
                            variableLocationInMacro[k] = i;
                            break;
                        }

                }
                i++;
            }


        }

        void updateChandingVariables(double[] values, string [] names)
        {
            for (int i = 0; i < values.Length; i++)
                macro[variableLocationInMacro[i]] = string.Format("*set,{0},{1}", names[i], values[i]);
        }

        void removeCommands(IEnumerable<string> keywords)
        {
            var regex = getRegexForKeywords(keywords);
            for (int i = 0; i < macro.Count; i++)
                if (regex.IsMatch(macro[i]))
                    macro[i] = "";
        }

        void appendCommands(string sourceFile)
        {
            appendCommands(File.ReadAllLines(sourceFile));
        }

        void appendCommands(IEnumerable<string> commands)
        {
            foreach (var command in commands)
                appendCommand(command);
        }

        void appendCommand(string command)
        {
            macro.Add(command);
        }
        
        void truncateMacro(IEnumerable<string> keywords)
        {
            var regex = getRegexForKeywords(keywords);
            int indexForRemoving = macro.FindIndex(f => regex.IsMatch(f) == true);
            macro = macro.Take(indexForRemoving).ToList();
        }



        Regex getRegexForKeywords(IEnumerable<string> keywords)
        {
            string regexString = "(" + string.Join("|", keywords) + ")";
            return new Regex(regexString, RegexOptions.IgnoreCase);
        }

        List<string> macro;
        int[] variableLocationInMacro;
    }
}
