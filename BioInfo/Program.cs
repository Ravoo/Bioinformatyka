using System;
using System.IO;
using System.Text;

namespace BioInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            var matrix = new double [5, 5];
            string measureType;
            string u, w;
            
            var lines = File.ReadAllLines("DistMatrix.txt", Encoding.UTF8);

            measureType = lines[0];
            u = lines[1];
            w = lines[2];
            for(var i = 3; i< lines.Length; i++)
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
                ?  GetOptimalGlobalAlignment(d, Math.Min, u, w)
                :  GetOptimalGlobalAlignment(d, Math.Max, u, w);
            
            
            Console.WriteLine(measure);
            Console.WriteLine(optimalGlobalAlignment.uResult + " " + optimalGlobalAlignment.wResult);
        }

              private static (string uResult, string wResult) GetOptimalGlobalAlignment(double[,] doubles, Func<double,double, double> optimizationFunction, string u, string w)
        {
            var uResult = string.Empty;
            var wResult = string.Empty;
            
            for (var i = doubles.GetLength(0) - 1; i >= 0;)
            {
                var n2 = double.MinValue;
                for (var j = doubles.GetLength(1)-1; j >= 0;)
                {

                    var n1 = double.MinValue;
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
                            if(i > 0)
                                wResult += w[i-1];
                            if(j > 0) 
                                uResult += u[j-1];
                            
                        }else if (n2 <= n1 && n2 <= n3)
                        {
                            j -= 1;
                            if(i > 0)
                                uResult += u[j-1];
                            wResult += '-';
                        }
                        else
                        {
                            i--;
                            if(j > 0)
                                wResult += w[i-1];
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

            for (int i = 0; i < w.Length+1; i++)
            {
                for (int j = 0; j < u.Length+1; j++)
                {
                    Console.Write(doubles[i,j]);
                }

                Console.WriteLine();
            }
            
            var uCharArray = uResult.ToCharArray();
            Array.Reverse( uCharArray );
            var wCharArray = wResult.ToCharArray();
            Array.Reverse(wCharArray);
            
            return (new string(uCharArray), new string(wCharArray));
        }


        static (double result, double[,] d) CalculateMeasure(string u, string w, double[,] matrix ,Func<double,double, double> optimizationFunction) //optimizationFunction - Math.Min lub Math.Max w zależności czy distance czy similarity
        {
            int m, n;
            double [,]d;
            m = u.Length;
            n = w.Length;
 
            d = new double[n+1, m+1];

            d[0, 0] = 0;
            double prev = 0;
            for (var j = 1; j <= m; j++)
            {
                d[0, j] = prev + GetValue(matrix, u[j-1], '-');
                prev += GetValue(matrix, u[j-1], '-');
            }

            prev = 0;
            for (var i = 1; i <= n; i++)
            {
                d[i, 0] = prev + GetValue(matrix, '-', w[i-1]);
                prev += GetValue(matrix, '-', w[i-1]);
            }
 
            for (var i=1; i<=n; i++)
            for (var j = 1; j <= m; j++)
            {
                var case1 = d[i - 1, j - 1] + GetValue(matrix, u[j-1], w[i-1]);
                var case2 = d[i, j - 1] + GetValue(matrix, u[j-1], '-');
                var case3 = d[i - 1, j] + GetValue(matrix, '-', w[i-1]);

                d[i, j] = optimizationFunction(case1, optimizationFunction(case2, case3));
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
            return matrix[i,j];
        }
    }
}