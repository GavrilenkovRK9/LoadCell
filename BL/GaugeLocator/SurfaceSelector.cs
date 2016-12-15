using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using BL.FeaConnector;

namespace BL.GaugeLocator
{
    public class SurfaceSelector
    {
        public SurfaceSelector()
        {
            SurfaceID = new List<int>();
        }

        public void Select(string feaFilePath)
        {

            string folder = AppDomain.CurrentDomain.BaseDirectory + "temp\\";
            Utils.CleanDir(folder);
            string pickerMacro = AppDomain.CurrentDomain.BaseDirectory + "scripts\\linePicker.mac";
            string mainMacro = folder + "solution_0.mac";
            //создать аббревиатуру:
            var writer = new StreamWriter(folder + "start140.ans");
            writer.WriteLine("*abbr, vibrat_resultat, solution_0.mac");
            writer.Close();
            //скопировать основной макрос
            File.Copy(feaFilePath, mainMacro);
            var auxMacro = File.ReadAllLines(pickerMacro);
            writer = new StreamWriter(mainMacro, true);
            writer.WriteLine();
            foreach (var line in auxMacro)
                writer.WriteLine(line);
            writer.Close();

            var manager = new BatchManager();
            manager.RunOnceGUI();

            var lines = File.ReadAllLines(folder + "lineList.txt");
            foreach (var line in lines)
            {
                SurfaceID.Add(int.Parse(line.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0]));
            }

        }

        public List<int> SurfaceID
        {
            get; set;
        }
    }
}
