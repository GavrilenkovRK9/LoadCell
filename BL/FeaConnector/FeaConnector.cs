using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;

namespace BL.FeaConnector
{
    public interface IConnector
    {
        List<double> GetGF(int solutionId);
        List<double[][]> GetStrainData(int solutionId);
        bool IsSuccess(int solutionId);
    }

    public class FeaConnector : IConnector
    {
        public FeaConnector(string filePath, bool isForDOE, List<string> GFs, List<int> SurfaceID)
        {
            this.filePath = filePath;
            this.isForDOE = isForDOE;
            this.GFs = GFs;
            this.SurfaceID = SurfaceID;
        }
                 
        public void ConnectToFea(List<Solution> solutions, List<string> varNames)
        {
            customGF = new List<double[]>(solutions.Count());
            isSuccess = new bool[solutions.Count()];
            var manager = new MacroManager(filePath, isForDOE, GFs, SurfaceID);
            manager.CreateMacros(varNames, solutions.Select(f => f.VariableValues).ToList());
            var batchManager = new BatchManager();
            batchManager.RunBatch(solutions.Count());
        }

        public void CollectResults()
        {
            var regex = new Regex(@"results\d+.txt", RegexOptions.IgnoreCase);
            //сбор результатов из скалярного файла
            DirectoryInfo info = new DirectoryInfo(Utils.GetTempDir());
            var files = info.GetFiles().Select(f => f.Name).Where(f => regex.IsMatch(f)).ToList();
            var winnerIDs = File.ReadAllLines(Utils.GetTempDir() + "log.txt").Select(f => f.Substring(1))
                .Select(f => int.Parse(f));
            foreach(var id in winnerIDs)
            {
                isSuccess[id] = true;
                customGF[id] = Utils.ParseCSV(files.Find(f => f.Equals(string.Format("results{0}.txt",
                    id)))).ToArray();

            }
            Utils.CleanDir(Utils.GetTempDir());  

        }

        public List<double> GetGF(int solutionId)
        {
            if (!isSuccess[solutionId])
                return null;
            else
                return customGF[solutionId].ToList();
        }

        public List<double[][]> GetStrainData(int solutionId)
        {
            if (!isSuccess[solutionId])
                return null;
            else
                return strainData[solutionId];
        }

        public bool IsSuccess(int solutionId)
        {
            return isSuccess[solutionId];
        }

        bool[] isSuccess;
        List<double[]> customGF;
        List<List<double[][]>> strainData;
        List<string> GFs;
        string filePath;
        bool isForDOE;
        List<int> SurfaceID;
    }
}
