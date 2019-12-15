using System;
using System.IO;
using System.Text;

namespace BioInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("SimMatrix.txt", Encoding.UTF8);
            var measureType = lines[0];

            if (measureType == "r")
                RNAMode(lines);
            else
                StandardMode(measureType, lines);
        }

        public static void RNAMode(string[] lines)
        {
            var uRNA = lines[1];
            var wRNA = lines[2];
            // rzucic wyjatek jesli nie sa podzielne przez 3?
            var matrix = new double[21, 21];

            for (var i = 3; i < lines.Length; i++)
            {
                var line = lines[i].Split(',');
                for (var j = 0; j < 5; j++)
                    matrix[i - 3, j] = double.Parse(line[j]);
            }

            var u = RNAToAcidsMapper.GetAcids(uRNA);
            var w = RNAToAcidsMapper.GetAcids(wRNA);


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