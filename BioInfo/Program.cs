using System;
using System.IO;
using System.Text;

namespace BioInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("DistMatrix.txt", Encoding.UTF8);
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

                var similarityMatrix = ConvertDistanceToSimilarity(matrix);
                var (measureLocal, dLocal) = MatrixBuilder.LocalSimilarity(u, w, similarityMatrix);
                var (uResult, wResult) = LocalAlignment.MaximalLocal(dLocal, u, w);
                PrintResult("Local", "Distance", u, uResult, w, wResult, measureLocal);
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

                var similarityMatrix = ConvertDistanceToSimilarity(matrix);
                //PrintMatrix(similarityMatrix);
                var (measureLocal, dLocal) = MatrixBuilder.LocalSimilarity(u, w, similarityMatrix);
                var (uResult, wResult) = LocalAlignment.MaximalLocal(dLocal, u, w);
                PrintResult("Local", "Distance", u, uResult, w, wResult, measureLocal);
            }
            else
            {
                var (measure, d) = MatrixBuilder.GlobalForSimilarity(u, w, matrix);
                var optimalGlobalAlignment =GlobalAlignment.OptimalWithSimilarity(d, u, w);
                PrintResult("Global", "Similarity", u, optimalGlobalAlignment.uResult, w, optimalGlobalAlignment.wResult,
                    measure);

                var (measureLocal, dLocal) = MatrixBuilder.LocalSimilarity(u, w, matrix);
                var (uResult, wResult) = LocalAlignment.MaximalLocal(dLocal, u, w);
                PrintResult("Local", "Similarity", u, uResult, w, wResult, measureLocal);
            } 
        }

        static double[,] ConvertDistanceToSimilarity(double[,] distanceMatrix)
        {
            var similarityMatrix = new double[distanceMatrix.GetLength(0), distanceMatrix.GetLength(1)];
            for (var i = 0; i < distanceMatrix.GetLength(0); i++)
            for (var j = 0; j < distanceMatrix.GetLength(1); j++)
                similarityMatrix[i, j] = 1 / (1 + distanceMatrix[i, j]);

            return similarityMatrix;
        }

        static void PrintMatrix(double[,] matrix)
        {
            Console.WriteLine("Converted Matrix");
            for (var i = 0; i < matrix.GetLength(0); i++)
            {
                for (var j = 0; j < matrix.GetLength(1); j++)
                    Console.Write($"{matrix[i, j]},");
                Console.WriteLine();
            }
        }
    }
}