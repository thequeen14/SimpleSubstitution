using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace SimpleSubstitution
{
    class Program
    {
        #region RussianAlfabet numberOfLetter
        public static Dictionary<string, int> numberOfLetter = new Dictionary<string, int>()
    {
        {"а", 0},
        {"б", 1},
        {"в", 2},
        {"г", 3},
        {"д", 4},
        {"е", 5},
        {"ё", 6},
        {"ж", 7},
        {"з", 8},
        {"и", 9},
        {"к", 10},
        {"л", 11},
        {"м", 12},
        {"н", 13},
        {"о", 14},
        {"п", 15},
        {"р", 16},
        {"с", 17},
        {"т", 18},
        {"у", 19},
        {"ф", 20},
        {"х", 21},
        {"ц", 22},
        {"ч", 23},
        {"ш", 24},
        {"щ", 25},
        {"ъ", 26},
        {"ы", 27},
        {"ь", 28},
        {"э", 29},
        {"ю", 30},
        {"я", 31},
        {"й", 32}
    };
        #endregion

        static void Main(string[] args)
        {
            //var sr = new StreamReader("twograms.txt");

            var onegramsPlaintextStatistics = new Dictionary<string, double>();
            using (var sr = new StreamReader("onegrams.txt"))
            {
                var onegramsStatisticsFromFile = new List<string>();
                var line = sr.ReadLine();

                while (line != null)
                {
                    onegramsStatisticsFromFile.Add(line);
                    line = sr.ReadLine();
                }

                onegramsPlaintextStatistics = ReadStatisticsPlaintext(onegramsStatisticsFromFile);
                sr.Close();
            }

            var ciphertextOnegramsStatistics = new Dictionary<string, double>();
            var ciphertextFromFile = string.Empty;
            using (var sr = new StreamReader("task1.txt"))
            {
                ciphertextFromFile = sr.ReadToEnd();
                ciphertextOnegramsStatistics = CountStatisticsCiphertext(ciphertextFromFile, 1);
                sr.Close();
            }

            var initialSubstitution = DefineInitialSubstitution(onegramsPlaintextStatistics, ciphertextOnegramsStatistics);
            var possibleDecipheredText = TryInitialSubstitution(initialSubstitution, ciphertextFromFile);
            using (var sw = new StreamWriter("decipherUsingOnegrams.txt"))
            {
                sw.WriteLine(possibleDecipheredText);
                sw.Close();
            }

            var twograms = new Dictionary<string, double>();
            using (var sr = new StreamReader("twograms.txt"))
            {
                var twogramsStatisticsFromFile = new List<string>();
                var line = sr.ReadLine();

                while (line != null)
                {
                    twogramsStatisticsFromFile.Add(line);
                    line = sr.ReadLine();
                }

                twograms = ReadStatisticsPlaintext(twogramsStatisticsFromFile);
                sr.Close();
            }
            var matrixPlain = FillTheMatrix(twograms);

            var ciphertextTwograms = new Dictionary<string, double>();
            using (var sr = new StreamReader("task1.txt"))
            {
                ciphertextFromFile = sr.ReadToEnd();
                ciphertextTwograms = CountStatisticsCiphertext(ciphertextFromFile, 2);
                sr.Close();
            }
            var matrixCipher = FillTheMatrix(ciphertextTwograms);

        }

        public static KeyValuePair<string, double> ParseInputStatistics(string input)
        {
            var newPair = input.Split(new char[] { '#', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return new KeyValuePair<string, double>(newPair[0], Convert.ToDouble(newPair[1]));
        }
        public static Dictionary<string, double> ReadStatisticsPlaintext(List<string> plainText)
        {
            var result = new Dictionary<string, double>();
            foreach (var ps in plainText)
            {
                var KVPair = ParseInputStatistics(ps);
                result.Add(KVPair.Key, KVPair.Value);
            }
            return result;
        }

        public static Dictionary<string, double> CountStatisticsCiphertext(string text, int mGrams)
        {
            var statistic = new Dictionary<string, double>();
            var HowManyPairs = 0;
            for (int i = 0; i < text.Length - 2; i++)
            {
                var newSubString = text.Substring(i, mGrams);
                if (statistic.ContainsKey(newSubString))
                {
                    statistic[newSubString]++;
                    HowManyPairs++;
                }
                else
                {
                    statistic.Add(newSubString, 1);
                    HowManyPairs++;
                }
            }
            var finalStatistics = new Dictionary<string, double>();
            foreach (var p in statistic)
            {
                var newValue = p.Value / HowManyPairs;
                finalStatistics.Add(p.Key, newValue);
            }
            return finalStatistics;
        }

        public static double[,] FillTheMatrix(Dictionary<string, double> twograms)
        {
            var result = new double[33, 33];
            foreach (var c in twograms)
            {
                var iLetter = c.Key[0];
                var jLetter = c.Key[1];

                var i = numberOfLetter[iLetter.ToString()];
                var j = numberOfLetter[jLetter.ToString()];

                result[i, j] = c.Value;
            }

            return result;
        }

        public static Dictionary<string, string> DefineInitialSubstitution(Dictionary<string, double> plainText, Dictionary<string, double> cipherText)
        {
            var result = new Dictionary<string, string>();
            
            foreach (var l in cipherText)
            {
                double minLength = 1;
                var newSub = new KeyValuePair<string, string>();
                var newMinLength = (double)0;
                foreach (var pL in plainText)
                {                   
                    newMinLength = Math.Abs(l.Value - pL.Value);
                    if (newMinLength < minLength)
                    {
                        minLength = newMinLength;
                        newSub = new KeyValuePair<string, string>(l.Key, pL.Key);
                    }
                }
                result.Add(newSub.Key, newSub.Value);
                plainText[newSub.Value] = 2; //чтобы при вычитании в следующий раз по модулю вышло больше единицы и второй раз букву в подстановку не взяли бы
            }
            return result;
        }

        public static string TryInitialSubstitution(Dictionary<string, string> initialSubstitution, string ciphertext)
        {
            var result = new StringBuilder();
            foreach (var c in ciphertext)
            {
                result.AppendFormat(initialSubstitution[c.ToString()]);
            }
            return result.ToString();
        }

        public static void WorkWithTwograms()
        {
            var coefficientOfCloseness = 0;
            
        }

        public static void ChangeRandomPositions(ref double[,] matrix, ref Dictionary<string, string> substitution)
        {
            var random = new Random();
            var alpha = random.Next(0, 33);
            var betha = random.Next(0, 33);

            var firstToChange = substitution.ElementAt(alpha);
            var secondToChange = substitution.ElementAt(betha);

            substitution[firstToChange.Key] = secondToChange.Value;
            substitution[secondToChange.Key] = firstToChange.Value;

            var firstIndexToChangeInMatrix = numberOfLetter[firstToChange.Key];
            var secondIndexToChangeInMatrix = numberOfLetter[secondToChange.Key];

            var alphaLine = new double[33];
            for (int i = 0; i < 33; i++)
            {
                alphaLine[i] = matrix[firstIndexToChangeInMatrix, i];
            }

            for (int i = 0; i < 33; i++)
            {
                matrix[firstIndexToChangeInMatrix, i] = matrix[secondIndexToChangeInMatrix, i];
            }

            for (int i = 0; i < 33; i++)
            {
                matrix[secondIndexToChangeInMatrix, i] = alphaLine[i];
            }

            var alphaColumn = new double[33];
            for (int i = 0; i < 33; i++)
            {
                alphaColumn[i] = matrix[i, firstIndexToChangeInMatrix];
            }

            for (int i = 0; i < 33; i++)
            {
                matrix[i, firstIndexToChangeInMatrix] = matrix[i, secondIndexToChangeInMatrix];
            }

            for (int i = 0; i < 33; i++)
            {
                matrix[i, secondIndexToChangeInMatrix] = alphaColumn[i];
            }
        }
    }
}
