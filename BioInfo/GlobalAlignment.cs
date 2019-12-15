using System;

namespace BioInfo
{
    public class GlobalAlignment
    {
        public static (string uResult, string wResult) OptimalWithSimilarity(double[,] doubles, string u, string w)
            => GetOptimalGlobalAlignment(doubles, Math.Max, u, w);

        public static (string uResult, string wResult) OptimalWithDistance(double[,] doubles, string u, string w)
            => GetOptimalGlobalAlignment(doubles, Math.Min, u, w);


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

            for (var i = 0; i < w.Length + 1; i++)
            {
                for (var j = 0; j < u.Length + 1; j++) 
                    Console.Write(doubles[i, j]);
                Console.WriteLine();
            }

            var uCharArray = uResult.ToCharArray();
            Array.Reverse(uCharArray);
            var wCharArray = wResult.ToCharArray();
            Array.Reverse(wCharArray);

            return (new string(uCharArray), new string(wCharArray));
        }
    }
}