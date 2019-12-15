using System;
using System.IO;
using System.Text;

namespace BioInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("RNADistMatrix.txt", Encoding.UTF8);
            var measureType = lines[0];

            if (measureType == "rd" || measureType == "rs")
                RNAMode(measureType, lines);
            else
                StandardMode(measureType, lines);
        }

        public static void RNAMode(string measureType, string[] lines)
        {
            var uRNA = lines[1];
            var wRNA = lines[2];
            if (uRNA.Length % 3 != 0 || wRNA.Length % 3 != 0)
            {
                Console.WriteLine("Sekwencje RNA muszą byc podzielne przez 3.");
                return;
            }

            var matrix = new double[21, 21];

            for (var i = 3; i < lines.Length; i++)
            {
                var line = lines[i].Split(',');
                for (var j = 0; j < 21; j++)
                    matrix[i - 3, j] = double.Parse(line[j]);
            }

            var u = RNAToAcidsMapper.GetAcids(uRNA);
            var w = RNAToAcidsMapper.GetAcids(wRNA);

            if (measureType == "rd")
            {
                var (measure, d) = MatrixBuilder.GlobalForDistanceRNA(u, w, matrix);
                var optimalGlobalAlignment = GlobalAlignment.OptimalWithDistance(d, u, w);
                var newRnaU = RNAToAcidsMapper.GetRNA(optimalGlobalAlignment.uResult, uRNA);
                var newRnaW = RNAToAcidsMapper.GetRNA(optimalGlobalAlignment.wResult, wRNA);
                Console.WriteLine("\nGlobal");
                Console.WriteLine(measure);
                Console.WriteLine(optimalGlobalAlignment.uResult + " " + optimalGlobalAlignment.wResult);
                Console.WriteLine(newRnaU + " " + newRnaW);
            }
            else
            {
                var (measure, d) = MatrixBuilder.GlobalForSimilarityRNA(u, w, matrix);
                var optimalGlobalAlignment = GlobalAlignment.OptimalWithSimilarity(d, u, w);
                Console.WriteLine("\nGlobal");
                Console.WriteLine(measure);
                Console.WriteLine(optimalGlobalAlignment.uResult + " " + optimalGlobalAlignment.wResult);
            }
        }

        public static void StandardMode(string measureType, string[] lines)
        {
            var u = lines[1];
            var w = lines[2];
            var matrix = new double[5, 5];
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