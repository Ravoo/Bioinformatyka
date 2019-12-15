using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BioInfo
{
    public class RNAToAcidsMapper
    {
        public static string GetAcids(string rna)
        {
            var threes = ChunkIntoThrees(rna);
            var acids = new StringBuilder();
            foreach (var three in threes)
            {
                var acid = RNAToAcids[three];
                acids.Append(acid);
            }

            return acids.ToString();
        }

        public static string GetRNA(string acids)
        {
            var rna = new StringBuilder();

            foreach (var acid in acids) 
                rna.Append(AcidToRNA[acid.ToString()]);

            return rna.ToString();
        }

        private static IEnumerable<string> ChunkIntoThrees(string rna) =>
            Enumerable.Range(0, rna.Length / 3)
                .Select(i => rna.Substring(i * 3, 3));

        public static Dictionary<string, string> RNAToAcids = new Dictionary<string, string>
        {
            {"UUU", "F"},
            {"UUC", "F"},
            {"UUA", "L"},
            {"UUG", "L"},
            {"UCU", "S"},
            {"UCC", "S"},
            {"UCA", "S"},
            {"UCG", "S"},
            {"UAU", "Y"},
            {"UAC", "Y"}, 
            {"UAA", ""}, // zamienic na pustego stringa?
            {"UAG", ""},
            {"UGU", "C"},
            {"UGC", "C"},
            {"UGA", ""},
            {"UGG", "W"},
            {"CUU", "L"},
            {"CUC", "L"},
            {"CUA", "L"},
            {"CUG", "L"},
            {"AUU", "I"},
            {"AUC", "I"},
            {"AUA", "I"},
            {"AUG", "M"},
            {"GUU", "V"},
            {"GUC", "V"},
            {"GUA", "V"},
            {"GUG", "V"},
            {"CCU", "P"},
            {"CCC", "P"},
            {"CCA", "P"},
            {"CCG", "P"},
            {"CAU", "H"},
            {"CAC", "H"},
            {"CAA", "Q"},
            {"CAG", "Q"},
            {"CGU", "R"},
            {"CGC", "R"},
            {"CGA", "R"},
            {"CGG", "R"},
            {"ACU", "T"},
            {"ACC", "T"},
            {"ACA", "T"},
            {"ACG", "T"},
            {"AGU", "S"},
            {"AGC", "S"},
            {"AGA", "R"},
            {"AGG", "R"},
            {"GCU", "A"},
            {"GCC", "A"},
            {"GCA", "A"},
            {"GCG", "A"},
            {"GAU", "D"},
            {"GAC", "D"},
            {"GAA", "E"},
            {"GAG", "E"},
            {"GGU", "G"},
            {"GGC", "G"},
            {"GGA", "G"},
            {"GGG", "G"},
        };

        public static Dictionary<string, string> AcidToRNA = new Dictionary<string, string>
        {
            {"F", "UUU"},
            {"F", "UUC"},
            {"L", "UUA"},
            {"L", "UUG"},
            {"S", "UCU"},
            {"S", "UCC"},
            {"S", "UCA"},
            {"S", "UCG"},
            {"Y", "UAU"},
            {"Y", "UAC"},
            {"C", "UGU"},
            {"C", "UGC"},
            {"W", "UGG"},
            {"L", "CUU"},
            {"L", "CUC"},
            {"L", "CUA"},
            {"L", "CUG"},
            {"I", "AUU"},
            {"I", "AUC"},
            {"I", "AUA"},
            {"M", "AUG"},
            {"V", "GUU"},
            {"V", "GUC"},
            {"V", "GUA"},
            {"V", "GUG"},
            {"P", "CCU"},
            {"P", "CCC"},
            {"P", "CCA"},
            {"P", "CCG"},
            {"H", "CAU"},
            {"H", "CAC"},
            {"Q", "CAA"},
            {"Q", "CAG"},
            {"R", "CGU"},
            {"R", "CGC"},
            {"R", "CGA"},
            {"R", "CGG"},
            {"T", "ACU"},
            {"T", "ACC"},
            {"T", "ACA"},
            {"T", "ACG"},
            {"S", "AGU"},
            {"S", "AGC"},
            {"R", "AGA"},
            {"R", "AGG"},
            {"A", "GCU"},
            {"A", "GCC"},
            {"A", "GCA"},
            {"A", "GCG"},
            {"D", "GAU"},
            {"D", "GAC"},
            {"E", "GAA"},
            {"E", "GAG"},
            {"G", "GGU"},
            {"G", "GGC"},
            {"G", "GGA"},
            {"G", "GGG"},
        };
    }
}

