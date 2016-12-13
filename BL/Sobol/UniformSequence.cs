using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Sobol
{
    public class Sobol01
    {

        public Sobol01(int numberOfPoints_, int Dimension_)
        {
            Dimension = Dimension_;
            numberOfPoints = numberOfPoints_;


            L = (int)Math.Ceiling(Math.Log((double)numberOfPoints_) / Math.Log(2.0));
            computeIndexZeroBit();
            computeFirstColumn();
            X_Previous = new uint[Dimension - 1];
            X_current = new uint[Dimension - 1];//начальная точка для нулевой строки, где все столбцы равны нулю
            vectorNumber = 1;
            callCount = 0;
            directionNumbers = DirectionNumbers.directionsArray();


        }

        public double[] getNextSobol01()
        {
            double[] uniform = new double[Dimension];
            directionRowIndex = 0;
            if (callCount == 0)
            {
                callCount++;
                return uniform;//первый вектор чисел нулевой
            }
            else
            {
                //значение для 1 столбца уже вычислено:
                uniform[0] = firstColumn[vectorNumber];

                for (int j = 1; j < Dimension; j++)
                {
                    uint d, s, a;
                    uint[] numbersFromFile = readDirectionNumbers();
                    d = numbersFromFile[0];
                    s = numbersFromFile[1];
                    a = numbersFromFile[2];
                    uint[] m = new uint[s + 1];
                    for (int i = 1; i < s + 1; i++) m[i] = numbersFromFile[2 + i];
                    //рассчитать направления от 1 до L, смасштабированные по 2^32
                    uint[] v1 = new uint[L + 1];
                    //Случай, когда L меньше размерности полинома:
                    if (L <= s)
                    {
                        for (int i = 0; i < L + 1; i++)
                        {
                            v1[i] = m[i] << (32 - i);
                        }
                    }
                    else
                    {
                        for (uint i = 0; i < s + 1; i++) v1[i] = m[i] << (32 - (int)i);
                        for (uint i = s + 1; i < L + 1; i++)
                        {
                            v1[i] = v1[i - s] ^ (v1[i - s] >> (int)s);
                            for (int k = 1; k <= s - 1; k++)
                                v1[i] ^= (((a >> ((int)s - 1 - k)) & 1) * v1[i - k]);
                        }
                    }
                    X_current[j - 1] = X_Previous[j - 1] ^ v1[indexZeroBit[vectorNumber - 1]];
                    uniform[j] = (double)X_current[j - 1] / Math.Pow(2.0, 32);
                }

                for (int i = 0; i < X_current.Length; i++)
                {
                    X_Previous[i] = X_current[i];

                }

                vectorNumber++;
                callCount++;
                return uniform;
            }

        }


        private void computeFirstColumn()
        {
            uint[] v = new uint[L + 1];


            uint one = 1;
            for (int i = 1; i < L + 1; i++)
            {
                v[i] = one << (32 - i);
            }

            uint[] X = new uint[numberOfPoints];//память под первую колонку
            firstColumn = new double[numberOfPoints];
            //Рассчитать значения для первой колонки (i=0)
            //та колонка, которая самая левая, кароч.
            firstColumn[0] = 0;
            for (int i = 1; i < numberOfPoints; i++)
            {
                X[i] = X[i - 1] ^ v[indexZeroBit[i - 1]];
                firstColumn[i] = (double)X[i] / Math.Pow(2.0, 32);

            }
        }

        private void computeIndexZeroBit()
        {
            indexZeroBit = new uint[numberOfPoints];
            indexZeroBit[0] = 1;
            for (uint i = 1; i < numberOfPoints; i++)
            {
                indexZeroBit[i] = 1;
                uint value = i;
                while ((value & 1) > 0)
                {
                    value >>= 1;
                    indexZeroBit[i]++;
                }

            }
        }
        private double[] firstColumn;
        private uint[] X_Previous;
        private uint[] X_current;
        private uint[] indexZeroBit;
        private int vectorNumber;
        private int callCount;
        protected int Dimension;
        private int numberOfPoints;
        private int L;
        private int directionRowIndex;//строка массива, который считывается

        private uint[][] directionNumbers;


        private uint[] readDirectionNumbers()
        {
            var temp = directionNumbers[directionRowIndex];
            directionRowIndex++;
            return temp;
        }


        public void printSequence()
        {
            double[] print = getNextSobol01();
            for (int i = 0; i < Dimension; i++)
            {
                Console.Write(print[i] + " ");
            }
            Console.Write("\n");
        }


    }

    public class SobolAB : Sobol01
    {
        public SobolAB(int pointCount, int dimension, double[] lo, double[] hi) : base(pointCount, dimension)
        {
            this.lo = lo;
            this.hi = hi;
        }
        double[] lo;
        double[] hi;

        public double[] getNextSobolAB()
        {
            double[] result = new double[Dimension];
            double[] sobol01 = getNextSobol01();
            for (int i = 0; i < Dimension; i++)
            {
                result[i] = (lo[i] + sobol01[i] * (hi[i] - lo[i]));
            }
            return result;
        }
    }
}
