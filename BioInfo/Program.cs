using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BioInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            var matrix = new double[5, 5];
            string measureType;
            string u, w;

            var lines = File.ReadAllLines("SimMatrix.txt", Encoding.UTF8);

            measureType = lines[0];
            u = lines[1];
            w = lines[2];
            for (var i = 3; i < lines.Length; i++)
            {
                var line = lines[i].Split(',');
                for (var j = 0; j < 5; j++)
                {
                    matrix[i - 3, j] = double.Parse(line[j]);
                }
            }


            var (measure, d) = measureType == "d"
                ? CalculateMeasure(u, w, matrix, Math.Min)
                : CalculateMeasure(u, w, matrix, Math.Max);

            var optimalGlobalAlignment = measureType == "d"
                ? GetOptimalGlobalAlignment(d, Math.Min, u, w)
                : GetOptimalGlobalAlignment(d, Math.Max, u, w);

            if (measureType == "s")
            {
                var (measureLocal, dLocal) = CalculateMeasureForLocalMax(u, w, matrix, Math.Max);
                var optimalLocal = GetOptimalLocalAlignmentForMax(dLocal, u, w);
                Console.WriteLine(measureLocal);
                foreach (var (uResult, wResult) in optimalLocal)
                    Console.WriteLine(uResult + " " + wResult);
            }

            Console.WriteLine(measure);
            Console.WriteLine(optimalGlobalAlignment.uResult + " " + optimalGlobalAlignment.wResult);
        }


        static (double result, double[,] d) CalculateMeasure(string u, string w, double[,] matrix, Func<double, double, double> optimizationFunction) //optimizationFunction - Math.Min lub Math.Max w zależności czy distance czy similarity
        {
            int m, n;
            double[,] d;
            m = u.Length;
            n = w.Length;

            d = new double[n + 1, m + 1];

            d[0, 0] = 0;
            double prev = 0;
            for (var j = 1; j <= m; j++)
            {
                d[0, j] = prev + GetValue(matrix, u[j - 1], '-');
                prev += GetValue(matrix, u[j - 1], '-');
            }

            prev = 0;
            for (var i = 1; i <= n; i++)
            {
                d[i, 0] = prev + GetValue(matrix, '-', w[i - 1]);
                prev += GetValue(matrix, '-', w[i - 1]);
            }

            for (var i = 1; i <= n; i++)
                for (var j = 1; j <= m; j++)
                {
                    var case1 = d[i - 1, j - 1] + GetValue(matrix, u[j - 1], w[i - 1]);
                    var case2 = d[i, j - 1] + GetValue(matrix, u[j - 1], '-');
                    var case3 = d[i - 1, j] + GetValue(matrix, '-', w[i - 1]);

                    d[i, j] = optimizationFunction(case1, optimizationFunction(case2, case3));
                }

            return (d[n, m], d);
        }

        static (double result, double[,] d) CalculateMeasureForLocalMax(string u, string w, double[,] matrix, Func<double, double, double> optimizationFunction) //optimizationFunction - Math.Min lub Math.Max w zależności czy distance czy similarity
        {
            int m, n;
            double[,] d;
            m = u.Length;
            n = w.Length;

            d = new double[n + 1, m + 1];

            d[0, 0] = 0; // na sztywno brzegi wypelniane zerami, nie ma to sensu z punktu widzenia minimalizacji
            for (var j = 1; j <= m; j++) d[0, j] = 0;

            for (var i = 1; i <= n; i++) d[i, 0] = 0;

            for (var i = 1; i <= n; i++)
                for (var j = 1; j <= m; j++)
                {
                    var case1 = d[i - 1, j - 1] + GetValue(matrix, u[j - 1], w[i - 1]);
                    var case2 = d[i, j - 1] + GetValue(matrix, u[j - 1], '-');
                    var case3 = d[i - 1, j] + GetValue(matrix, '-', w[i - 1]);
                    const int case4 = 0;

                    d[i, j] = optimizationFunction(optimizationFunction(case1, case4), optimizationFunction(case2, case3));
                }

            return (d[n, m], d);
        }

        private static (string uResult, string wResult) GetOptimalGlobalAlignment(double[,] doubles, Func<double, double, double> optimizationFunction, string u, string w)
        {
            var uResult = string.Empty;
            var wResult = string.Empty;

            for (var i = doubles.GetLength(0) - 1; i >= 0;)
            {
                for (var j = doubles.GetLength(1) - 1; j >= 0;)
                {
                    var n1 = double.MinValue;
                    var n2 = double.MinValue;
                    var n3 = double.MinValue;
                    if (optimizationFunction == Math.Min)
                    {
                        n1 = double.MaxValue;
                        n2 = double.MaxValue;
                        n3 = double.MaxValue;
                    }

                    if (i == 0 && j > 0)
                    {
                        n2 = doubles[i, j - 1];
                    }
                    else if (j == 0 && i > 0)
                    {
                        n3 = doubles[i - 1, j];
                    }
                    else if (j == 0 && i == 0)
                    {
                        i--;
                        j--;
                        break;
                    }
                    else
                    {
                        n1 = doubles[i - 1, j - 1];
                        n2 = doubles[i, j - 1];
                        n3 = doubles[i - 1, j];
                    }

                    if (optimizationFunction == Math.Min)
                    {
                        if (n1 <= n2 && n1 <= n3)
                        {
                            i -= 1;
                            j -= 1;
                            if (i > 0)
                                wResult += w[i - 1];
                            if (j > 0)
                                uResult += u[j - 1];

                        }
                        else if (n2 <= n1 && n2 <= n3)
                        {
                            j -= 1;
                            if (i > 0)
                                uResult += u[j - 1];
                            wResult += '-';
                        }
                        else
                        {
                            i--;
                            if (j > 0)
                                wResult += w[i - 1];
                            uResult += '-';
                        }
                    }
                    else
                    {
                        if (n1 >= n2 && n1 >= n3)
                        {
                            i -= 1;
                            j -= 1;
                            if (i > 0)
                                wResult += w[i - 1];
                            if (j > 0)
                                uResult += u[j - 1];

                        }
                        else if (n2 >= n1 && n2 >= n3)
                        {
                            j -= 1;
                            if (i > 0)
                                uResult += u[j - 1];
                            wResult += '-';
                        }
                        else
                        {
                            i--;
                            if (j > 0)
                                wResult += w[i - 1];
                            uResult += '-';
                        }
                    }
                }
            }

            for (int i = 0; i < w.Length + 1; i++)
            {
                for (int j = 0; j < u.Length + 1; j++)
                {
                    Console.Write(doubles[i, j]);
                }

                Console.WriteLine();
            }

            var uCharArray = uResult.ToCharArray();
            Array.Reverse(uCharArray);
            var wCharArray = wResult.ToCharArray();
            Array.Reverse(wCharArray);

            return (new string(uCharArray), new string(wCharArray));
        }

        private static IEnumerable<(string uResult, string wResult)> GetOptimalLocalAlignmentForMax(double[,] doubles, string u, string w)
        {
            var maxValue = doubles.Cast<double>().Max();
            if (maxValue == 0)
            {
                Console.WriteLine("Something's wrong? Max value in matrix is 0.");
            }
            var maximumPoints = CoordinatesOf(doubles, maxValue);

            foreach (var (max_i, max_j) in maximumPoints)
            {
                var uResult = string.Empty;
                var wResult = string.Empty;
                for (var i = max_i; i >= 0;)
                {
                    for (var j = max_j; j >= 0;)
                    {
                        if (doubles[i, j] == 0)
                        {
                            Console.WriteLine($"Starting from max val at position [{max_i},{max_j}] got to point when doubles[{i},{j}] is 0.");
                        }

                        var n1 = double.MinValue;
                        var n2 = double.MinValue;
                        var n3 = double.MinValue;

                        if (i == 0 && j > 0)
                        {
                            n2 = doubles[i, j - 1];
                        }
                        else if (j == 0 && i > 0)
                        {
                            n3 = doubles[i - 1, j];
                        }
                        else if (j == 0 && i == 0)
                        {
                            i--;
                            j--;
                            break;
                        }
                        else
                        {
                            n1 = doubles[i - 1, j - 1];
                            n2 = doubles[i, j - 1];
                            n3 = doubles[i - 1, j];
                        }

                        if (n1 >= n2 && n1 >= n3)
                        {
                            i -= 1;
                            j -= 1;
                            if (i > 0)
                                wResult += w[i - 1];
                            if (j > 0)
                                uResult += u[j - 1];

                        }
                        else if (n2 >= n1 && n2 >= n3)
                        {
                            j -= 1;
                            if (i > 0)
                                uResult += u[j - 1];
                            wResult += '-';
                        }
                        else
                        {
                            i--;
                            if (j > 0)
                                wResult += w[i - 1];
                            uResult += '-';
                        }
                    }
                }

                var uCharArray = uResult.ToCharArray();
                Array.Reverse(uCharArray);
                var wCharArray = wResult.ToCharArray();
                Array.Reverse(wCharArray);

                yield return (new string(uCharArray), new string(wCharArray));
            }
        }

        static double GetValue(double[,] matrix, char u, char w)
        {
            int i, j;
            i = u switch
            {
                '-' => 0,
                'A' => 1,
                'G' => 2,
                'C' => 3,
                'T' => 4,
            };
            j = w switch
            {
                '-' => 0,
                'A' => 1,
                'G' => 2,
                'C' => 3,
                'T' => 4,
            };
            return matrix[i, j];
        }

        public static IEnumerable<Tuple<int, int>> CoordinatesOf(double[,] matrix, double value)
        {
            int w = matrix.GetLength(0); // width
            int h = matrix.GetLength(1); // height

            for (int x = 0; x < w; ++x)
            {
                for (int y = 0; y < h; ++y)
                {
                    if (matrix[x, y].Equals(value))
                        yield return Tuple.Create(x, y);
                }
            }
        }
    }
}