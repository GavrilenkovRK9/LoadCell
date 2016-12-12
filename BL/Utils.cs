using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace BL
{
    /// <summary>
    /// свалка полезных статических методов
    /// </summary>
    public static class Utils
    {
        public static List<double> ParseCSV(string input)
        {
            var delimited = input.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            return delimited.Select(f => double.Parse(f)).ToList();
        }

        public static string GetDir()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static string GetTempDir()
        {
            return GetDir() + "\\temp\\";
        }

        public static string GetScriptsDir()
        {
            return GetDir() + "\\scripts\\";
        }


        public static void ShowError(string message)
        {
            MessageBox.Show(message, "ЕГГОГ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void CleanDir(string Path)
        {
            DirectoryInfo folder = new DirectoryInfo(Path);
            foreach (FileInfo file in folder.GetFiles())
            {
                try
                {
                    if (!file.Name.Contains(".mac"))
                        file.Delete();
                }
                catch
                {

                }
            }
        }
    }

    
}
