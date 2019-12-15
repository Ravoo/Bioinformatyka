using System;

namespace BioInfo
{
    public class MatrixBuilder
    {
        public static (double result, double[,] d) GlobalForSimilarity(string u, string w, double[,] matrix)
            => CalculateMeasure(u, w, matrix, Math.Max, GetValue);

        public static (double result, double[,] d) GlobalForDistance(string u, string w, double[,] matrix)
            => CalculateMeasure(u, w, matrix, Math.Min, GetValue);

        public static (double result, double[,] d) GlobalForDistanceRNA(string u, string w, double[,] matrix)
            => CalculateMeasure(u, w, matrix, Math.Min, GetValueForAcids);

        public static (double result, double[,] d) GlobalForSimilarityRNA(string u, string w, double[,] matrix)
            => CalculateMeasure(u, w, matrix, Math.Max, GetValueForAcids);
        
        public static (double result, double[,] d) LocalSimilarityRNA(string u, string w, double[,] matrix)
            => Local(u, w, matrix, GetValueForAcids);
        public static (double result, double[,] d) LocalSimilarity(string u, string w, double[,] matrix)
            => Local(u, w, matrix, GetValue);

        private static (double result, double[,] d) CalculateMeasure(string u, string w, double[,] matrix, 
            Func<double, double, double> optimizationFunction, Func<double[,], char, char, double> getValue)
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
                d[0, j] = prev + getValue(matrix, u[j - 1], '-');
                prev += getValue(matrix, u[j - 1], '-');
            }

            prev = 0;
            for (var i = 1; i <= n; i++)
            {
                d[i, 0] = prev + getValue(matrix, '-', w[i - 1]);
                prev += getValue(matrix, '-', w[i - 1]);
            }

            for (var i = 1; i <= n; i++)
                for (var j = 1; j <= m; j++)
                {
                    var case1 = d[i - 1, j - 1] + getValue(matrix, u[j - 1], w[i - 1]);
                    var case2 = d[i, j - 1] + getValue(matrix, u[j - 1], '-');
                    var case3 = d[i - 1, j] + getValue(matrix, '-', w[i - 1]);

                    d[i, j] = optimizationFunction(case1, optimizationFunction(case2, case3));
                }



            return (d[n, m], d);
        }

        public static (double result, double[,] d) Local(string u, string w, double[,] matrix, Func<double[,], char, char, double> getValue)
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
                    var case1 = d[i - 1, j - 1] + getValue(matrix, u[j - 1], w[i - 1]);
                    var case2 = d[i, j - 1] + getValue(matrix, u[j - 1], '-');
                    var case3 = d[i - 1, j] + getValue(matrix, '-', w[i - 1]);
                    const int case4 = 0;

                    d[i, j] = Math.Max(Math.Max(case1, case4), Math.Max(case2, case3));
                }

            return (d[n, m], d);
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

        static double GetValueForAcids(double[,] matrix, char u, char w)
        {
            int i, j;
            i = u switch
            {
                '-' => 0,
                'A' => 1,
                'R' => 2,
                'D' => 3,
                'C' => 4,
                'E' => 5,
                'Q' => 6,
                'G' => 7,
                'H' => 8,
                'I' => 9,
                'L' => 10,
                'K' => 11,
                'M' => 12,
                'F' => 13,
                'P' => 14,
                'N' => 15,
                'S' => 16,
                'T' => 17,
                'W' => 18,
                'Y' => 19,
                'V' => 20,
            };
            j = w switch
            {
                '-' => 0,
                'A' => 1,
                'R' => 2,
                'D' => 3,
                'C' => 4,
                'E' => 5,
                'Q' => 6,
                'G' => 7,
                'H' => 8,
                'I' => 9,
                'L' => 10,
                'K' => 11,
                'M' => 12,
                'F' => 13,
                'P' => 14,
                'N' => 15,
                'S' => 16,
                'T' => 17,
                'W' => 18,
                'Y' => 19,
                'V' => 20,
            };
            return matrix[i, j];
        }
    }
}