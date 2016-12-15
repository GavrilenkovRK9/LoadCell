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
        string[] GetSurfaceData(int solutionId);
        bool IsSuccess(int solutionId);
    }

    public class FEAConnector : IConnector
    {
        public FEAConnector(string filePath)
        {
            this.filePath = filePath;
            isForDOE = true;
            logFilePath = Utils.GetTempDir() + "logs.txt";
        }

        public FEAConnector(string filePath, List<string> GFs, List<int> SurfaceID)
        {
            this.filePath = filePath;
            isForDOE = false;
            this.GFs = GFs;
            this.SurfaceID = SurfaceID;
            logFilePath = Utils.GetTempDir() + "logs.txt";
        }
                 
        public void ConnectToFea(List<Solution> solutions, List<string> varNames)
        {
            customGF = new List<double[]>(solutions.Count());
            strainData = new List<string[]>(solutions.Count());
            for (int i = 0; i < solutions.Count(); i++)
            {
                customGF.Add(new double[1]);
                strainData.Add(new string[2]);
            }
            isSuccess = new bool[solutions.Count()];
            var batchManager = new BatchManager();
            batchManager.RunBatch(solutions.Count());
        }

        public void ConnectFeaDOE(List<Solution> solutions, List<string> varNames)
        {
            isSuccess = new bool[solutions.Count()];
            var manager = new MacroManager(filePath);
            manager.CreateMacros(varNames, solutions);
            var batchManager = new BatchManager();
            batchManager.RunBatch(solutions.Count());
        }

        public void CollectResults()
        {
            if (!File.Exists(logFilePath))
                return;
            var regex = new Regex(@"(results\d+.txt|strain_\d+.txt)", RegexOptions.IgnoreCase);
            //сбор результатов из скалярного файла
            DirectoryInfo info = new DirectoryInfo(Utils.GetTempDir());
            var files = info.GetFiles().Select(f => f.Name).Where(f => regex.IsMatch(f)).ToList();
            var winnerIDs = File.ReadAllLines(logFilePath).Select(f => f.Substring(1))
                .Select(f => int.Parse(f)).ToList();
            foreach(var id in winnerIDs)
            {
                isSuccess[id] = true;
                if(!isForDOE)
                {
                    string customGFFile = Utils.GetTempDir() + files.Find(f => f.Equals(string.Format("results{0}.txt",
                      id)));
                    customGF[id] = Utils.ParseCSV(File.ReadAllText(customGFFile)).ToArray();
                    string strainFile = Utils.GetTempDir() + files.Find(f => f.Equals(string.Format("strain_{0}.txt",
                        id)));
                    strainData[id] = File.ReadAllLines(strainFile);
                }
                
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

        public string[] GetSurfaceData(int solutionId)
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

        public int GetSuccessCount()
        {
            return isSuccess.Where(f => f == true).Count();
        }

        bool[] isSuccess;
        List<double[]> customGF;
        List<string[]> strainData;
        List<string> GFs;
        string filePath;
        bool isForDOE;
        List<int> SurfaceID;
        string logFilePath;
    }
}
