using System;
using System.IO;
using System.Text;

namespace BioInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            var matrix = new double[5, 5];

            var lines = File.ReadAllLines("SimMatrix.txt", Encoding.UTF8);

            var measureType = lines[0];
            var u = lines[1];
            var w = lines[2];
            for (var i = 3; i < lines.Length; i++)
            {
                var line = lines[i].Split(',');
                for (var j = 0; j < 5; j++) 
                    matrix[i - 3, j] = double.Parse(line[j]);
            }


            var (measure, d) = measureType == "d"
                ? MatrixBuilder.GlobalForDistance(u, w, matrix)
                : MatrixBuilder.GlobalForSimilarity(u, w, matrix);

            var optimalGlobalAlignment = measureType == "d"
                ? GlobalAlignment.OptimalWithDistance(d, u, w)
                : GlobalAlignment.OptimalWithSimilarity(d, u, w);

            Console.WriteLine("\nGlobal");
            Console.WriteLine(measure);
            Console.WriteLine(optimalGlobalAlignment.uResult + " " + optimalGlobalAlignment.wResult);

            if (measureType == "s")
            {
                Console.WriteLine("\nLocal");

                var (measureLocal, dLocal) = MatrixBuilder.Local(u, w, matrix);
                var (uResult, wResult) = LocalAlignment.MaximalLocal(dLocal, u, w);
                Console.WriteLine(measureLocal);
                Console.WriteLine(uResult + " " + wResult);
            }
        }
    }
}