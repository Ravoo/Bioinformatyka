using System;
using System.Collections.Generic;
using System.Linq;

namespace BioInfo
{
    public class LocalAlignment
    {
        public static (string uResult, string wResult) MaximalLocal(double[,] doubles, string u, string w)
        {
            var alignments = GetLocalAlignments(doubles, u, w).ToList();
            var longestLength = alignments.Max(x => x.uResult.Length);
            return alignments.First(x => x.uResult.Length == longestLength);
        }

        private static IEnumerable<(string uResult, string wResult)> GetLocalAlignments(double[,] doubles, string u, string w)
        {
            var maxValue = doubles.Cast<double>().Max();
            if (maxValue == 0) 
                Console.WriteLine("Something's wrong? Max value in matrix is 0.");
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
                            Console.WriteLine($"Starting from max val at position [{max_i},{max_j}] got to point when doubles[{i},{j}] is 0.");

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

        public static IEnumerable<Tuple<int, int>> CoordinatesOf(double[,] matrix, double value)
        {
            var w = matrix.GetLength(0);
            var h = matrix.GetLength(1);

            for (var x = 0; x < w; ++x)
            for (var y = 0; y < h; ++y)
                if (matrix[x, y].Equals(value))
                    yield return Tuple.Create(x, y);
        }
    }
}