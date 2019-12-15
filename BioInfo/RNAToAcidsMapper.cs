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

        public static string GetRNA(string acids, string rnaOriginal)
        {
            var threes = ChunkIntoThrees(rnaOriginal).ToList();
            var rnaNew = new StringBuilder();

            foreach (var acid in acids)
            {
                var matchingCodons = AcidToRNA[acid.ToString()];
                var matchedThree = threes.First(matchingCodons.Contains);
                threes.Remove(matchedThree);
            }

            return rnaNew.ToString();
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
            {"AAA", "K" },
            {"AAG", "K" },
            {"AAU", "N" },
            {"AAC", "N" }
        };

        public static Dictionary<string, List<string>> AcidToRNA = new Dictionary<string, List<string>>
        {
            {"F", new List<string>{"UUU", "UUC"}},
            {"L", new List<string>{ "UUA", "UUG", "CUU", "CUC", "CUA", "CUG"}},
            {"S", new List<string>{ "UCU", "UCU", "UCA", "UCG", "AGU", "AGC"}},
            {"Y", new List<string>{ "UAU", "UAC"}},
            {"C", new List<string>{ "UGU", "UGC"}},
            {"I", new List<string>{ "AUU", "AUC", "AUA"}},
            {"W", new List<string>{ "UGG"}},
            {"M", new List<string>{ "AUG"}},
            {"V", new List<string>{ "GUU", "GUC", "GUA", "GUG"}},
            {"P", new List<string>{ "CCU", "CCC", "CCA", "CCG"}},
            {"H", new List<string>{ "CAU", "CAC"}},
            {"Q", new List<string>{ "CAA", "CAG"}},
            {"R", new List<string>{ "CGU", "CGC", "CGA", "CGG", "AGA", "AGG"}},
            {"T", new List<string>{ "ACU", "ACC", "ACA", "ACG"}},
            {"A", new List<string>{ "GCU", "GCC", "GCA", "GCG"}},
            {"D", new List<string>{ "GAU", "GAC"}},
            {"E", new List<string>{ "GAA", "GAG"}},
            {"G", new List<string>{ "GGU", "GGC", "GGA", "GGG"}},
            {"K", new List<string>{ "AAA", "AAG"}},
            {"N", new List<string>{ "AAU", "AAC"}},
        };
    }
}

