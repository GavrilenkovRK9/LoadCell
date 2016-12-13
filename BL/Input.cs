using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCalc;

/// <summary>
/// пространство для классов,
/// которые являеются входными данными для эксперимента по ИПП
/// Это управляющие переменные, ограничения функциональные
/// </summary>
namespace BL.Input
{
    public class FunConstraint
    {
        public FunConstraint(string ConstraintEquation)
        {
            constraint = new Expression(ConstraintEquation);
        }

        /// <summary>
        /// Функция проверяет, правильно ли записано условие
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public bool ConstrainDefinitionValid(List<string> parameters)
        {
            Random rnd = new Random();
            List<double> values = new List<double>();
            for (int i = 0; i < parameters.Count; i++)
                values.Add(rnd.NextDouble());
            return ConstraintDefinitionValid(parameters, values);
        }

        public bool ConstraintDefinitionValid(List<string> parameters, List<double> values)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                constraint.Parameters[parameters[i]] = values[i];
            }
            try
            {
                var result = (bool)constraint.Evaluate();
                //проверка во-первых, на синтаксическую правильность
                //во-вторых, на тип результата - должна быть булевская переменная
                //в-третьих, на то, что были заданы правильные параметры
                //любое нарушение вызовет исключение
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// проверка, выполнено ли ограничение
        /// </summary>
        /// <param name="names"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool ConstraintSatisfied(string[] names, double[] values)
        {
            for (int i = 0; i < values.Length; i++)
                constraint.Parameters[names[i]] = values[i];
            return (bool)constraint.Evaluate();
        }

        private Expression constraint;

    }

    public class Variable
    {
        public double InitValue { get; set; }
        public double Hi { get; set; }
        public double Lo { get; set; }
        public string Name { get; set; }
        public bool IsDecisionVariable { get; set; }
    }

    public class StrainGauge
    {
        public StrainGauge(double length, double width, double gridLength, double gridWidth, double baseFront, int gridCount
            , double endLoopLength, double gaugeFactor)
        {
            Length = length;
            GridLength = gridLength;
            GridWidth = gridWidth;
            Width = width;
            BaseFront = baseFront;
            GridCount = gridCount;
            EndLoopLength = endLoopLength;
            GaugeFactor = gaugeFactor;
        }

        public double EndLoopLength { get; set; }
        public double GaugeFactor { get; set; }
        public double Length { get; set; }
        public double GridLength { get; set; }
        public double Width { get; set; }
        public double GridWidth { get; set; }
        public double BaseFront { get; set; }
        public double GridCount { get; set; }
    }

    public class Surface
    {
        public int ID { get; set; }
        public bool Horizontal { get; set; }
    }

    public class Criterion
    {
        public Criterion(string Name, bool isMinimized)
        {
            this.Name = Name;
            this.isMinimized = isMinimized;
        }
        public string Name { get; set; }
        public bool isMinimized { get; set; }
    }
}
