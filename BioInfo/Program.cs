using System;
using System.IO;
using System.Text;

namespace BioInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("RNASimMatrix.txt", Encoding.UTF8);
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
                
                PrintResult("Global", "Distance", uRNA, newRnaU, wRNA, newRnaW,measure);
                PrintResult("Global", "Distance", u, optimalGlobalAlignment.uResult, w, optimalGlobalAlignment.wResult,measure);
            }
            else
            {
                var (measure, d) = MatrixBuilder.GlobalForSimilarityRNA(u, w, matrix);
                var optimalGlobalAlignment = GlobalAlignment.OptimalWithSimilarity(d, u, w);
                var newRnaU = RNAToAcidsMapper.GetRNA(optimalGlobalAlignment.uResult, uRNA);
                var newRnaW = RNAToAcidsMapper.GetRNA(optimalGlobalAlignment.wResult, wRNA);
                PrintResult("Global", "Similarity", uRNA, newRnaU, wRNA, newRnaW,measure);
                PrintResult("Global", "Similarity", u, optimalGlobalAlignment.uResult, w, optimalGlobalAlignment.wResult,measure);
                
                var (measureLocal, dLocal) = MatrixBuilder.LocalSimilarityRNA(u, w, matrix);
                var (uResult, wResult) = LocalAlignment.MaximalLocal(dLocal, u, w);
                var localRnaU = RNAToAcidsMapper.GetRNA(uResult, uRNA);
                var localRnaW = RNAToAcidsMapper.GetRNA(wResult, wRNA);
                
                PrintResult("Local", "Similarity", uRNA, localRnaU, wRNA, localRnaW,measureLocal);
            }
        }


        public static void PrintResult(string alignmentType,string measureType, string u, string uResult, string w, string wResult, double measure)
        {
            Console.WriteLine();
            Console.WriteLine(alignmentType);
            Console.WriteLine($"{measureType}: {measure}");
            Console.WriteLine($"u: {u} \nw: {w} \nuResult: {uResult} {uResult.Length} \nwResult: {wResult} {wResult.Length}");
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
            

            if (measureType == "d")
            {
                var (measure, d) = MatrixBuilder.GlobalForDistance(u, w, matrix);
                var optimalGlobalAlignment = GlobalAlignment.OptimalWithDistance(d, u, w);
                PrintResult("Global", "Distance", u, optimalGlobalAlignment.uResult, w, optimalGlobalAlignment.wResult,
                    measure);
            }
            else
            {
                var (measure, d) = MatrixBuilder.GlobalForSimilarity(u, w, matrix);
                var optimalGlobalAlignment =GlobalAlignment.OptimalWithSimilarity(d, u, w);
                PrintResult("Global", "Similarity", u, optimalGlobalAlignment.uResult, w, optimalGlobalAlignment.wResult,
                    measure);
            }

            if (measureType == "s")
            {
                var (measureLocal, dLocal) = MatrixBuilder.LocalSimilarity(u, w, matrix);
                var (uResult, wResult) = LocalAlignment.MaximalLocal(dLocal, u, w);
                PrintResult("Local", "Similarity", u, uResult, w, wResult,measureLocal);
            }
        }
    }
}