using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace BL.FeaConnector
{
    public class BatchManager
    {
        public BatchManager()
        {
            readConfigurations();
            directory = Utils.GetTempDir();
        }

        public void RunOnceGUI()
        {
            var launchCommand = getLaunchSequenceGUI("solution_0.mac");
            File.WriteAllText("launcher.bat", launchCommand);
            run();
        }


        public void RunBatch(int solutionCount)
        {
            
            List<string> launchCommands = new List<string>();
            for (int i = 0; i < solutionCount; i++)
                launchCommands.Add(getLaunchSequence(string.Format("solution_{0}.mac", i)));
            File.WriteAllLines("launcher.bat", launchCommands);
            run();
        }

        void run()
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "launcher.bat";
            process.Start();
            process.WaitForExit();
            File.Delete("launcher.bat");
        }

        string getLaunchSequence(string input_file)
        {
            string cmd_line = string.Format(stringFormatBatchMode, fea_executable_path,
                directory + string.Format("\\{0}", input_file), license_type, numberOfCores, directory, "makrel", memory_allocation, (int)(memory_allocation / 2), directory + "\\stanok.txt");
            return cmd_line;
        }

        string getLaunchSequenceGUI(string input_file)
        {
            string cmd_line = string.Format(stringFormatGuiMode,
                fea_executable_path, license_type, numberOfCores, directory,
                "makrel");
            return cmd_line;
        }


        void readConfigurations()
        {
            if (File.Exists("configs.txt"))
            {
                string[] lines = File.ReadAllLines("configs.txt");
                try
                {
                    if (lines.Length != 4)
                        throw new Exception("Данные конфигурации введены неполностью или с ошибками");
                    fea_executable_path = lines.First();
                    license_type = lines[1];
                    numberOfCores = int.Parse(lines[2]);
                    memory_allocation = int.Parse(lines[3]);

                }
                catch
                {
                    throw new Exception("Данные конфигурации введены неполностью или с ошибками");
                }
            }
            else
                throw new Exception("Файла конфигурации configs.txt не существует.\n Его нужно создать.");
        }

        string fea_executable_path;
        string license_type;
        int numberOfCores;
        int memory_allocation;
        string directory;
        const string stringFormatBatchMode = @"""{0}"" -b -i ""{1}"" -p {2} -np {3} -dir ""{4}"" -j ""{5}"" - s read -m {6} -db {7} - l en - us - t - d win32 -o ""{8}""";
        const string stringFormatGuiMode = @"""{0}""  -g -p {1} -np {2} -dir ""{3}"" -j ""{4}"" -s read -l en-us -t -d win32";
    }
}
