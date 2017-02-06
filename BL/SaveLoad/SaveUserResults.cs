using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using BL.Input;

namespace BL.SaveLoad
{
    public static class SaveUserResults
    {
        public static void Save(Experiment exp)
        {
            var writer = new StreamWriter(exp.FilePath);
            foreach (var variable in exp.Variables)
            {
                writer.WriteLine(exp.FilePath);
                writer.WriteLine(string.Format("{0};{1};{2};{3};{4}"),
                    variable.Name, variable.InitValue, variable.Lo,
                    variable.Hi, (variable.IsDecisionVariable) ? "Да" : "Нет");
                writer.WriteLine(string.Join(";", exp.softConstraints.Select(f=>f.ConstraintEquation)));
                writer.WriteLine(string.Join(";", exp.fixedConstraints.Select(f => f.ConstraintEquation)));
                writer.WriteLine(string.Join(";", exp.Criterions.Select(f => f.ConstraintEqn)));
                writer.WriteLine(string.Join(";", exp.Criterions.Select(f => f.Name)));
                writer.WriteLine(string.Join(";", exp.Criterions.Select(f=>(f.isMinimized)?"Да":"Нет")));
                writer.WriteLine(string.Join(";", exp.surfaceID));

                writeGaugeInfo(exp.TensionGauge, writer);
                writeGaugeInfo(exp.CompressionGauge, writer);
                writeGaugeInfo(exp.NickelGauge, writer);

            }

        }

        static void writeSolutionInfo()
        {

        }

        static void writeGaugeInfo(StrainGauge gauge, StreamWriter pushkin)
        {
            pushkin.WriteLine(string.Format("{0};{1};{2};{3};{4};{5};{6};{7}",
                    gauge.Length, gauge.Width,
                    gauge.GridLength, gauge.GridWidth,
                    gauge.EndLoopLength, gauge.GaugeFactor, 
                    gauge.GridCount, gauge.BaseFront));
        }
    }
}
