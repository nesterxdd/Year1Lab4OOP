using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics.Eventing.Reader;
using System.CodeDom.Compiler;

namespace L44
{
    internal class Program
    {
        const int CMax = 100;
        const string CFn = "L4_10T.txt";
        const string CRn = "Results.txt";
        const string analysis = "Analysis.txt";
        static void Main(string[] args)
        {
            Process(CFn, CRn, analysis);
        }

        static void Process(string filename, string resultfile, string analysisfile)
        {
            string line;
            int numberofline = 0;
            using (StreamReader reader = new StreamReader(filename))
            {
                using (StreamWriter result = new StreamWriter(resultfile))
                {
                    using (StreamWriter analysis = new StreamWriter(analysisfile))
                    {
                        analysis.WriteLine("-------------------------------------------");
                        analysis.WriteLine("|Number of line|       Longest word       |");
                        analysis.WriteLine("-------------------------------------------");
                        while ((line = reader.ReadLine()) != null)
                        {
                            numberofline++;
                            string[] words = Regex.Split(line, "[^a-zA-Z0-9]+");

                            int counterLongests = 0;
                            string longestsInLine = "";
                            string[] longestsWords = FindOnlyLongestWords(words, ref counterLongests, out longestsInLine);
                            longestsWords = DeleteRepetitions(longestsWords, ref counterLongests);

                            int counterRequired = 0;
                            string[] requiredWords = FillingRequiredWords(words, longestsWords, counterLongests, ref counterRequired);

                            requiredWords = DeleteRepetitions(requiredWords, ref counterRequired);
                            DeleteLongestsWords(ref requiredWords, ref counterRequired, longestsWords, counterLongests);                           
                           
                            line = Modified(line, requiredWords, counterRequired);
                            
                            line = line.Insert(line.IndexOf(requiredWords[0]), "|");
                            result.WriteLine(line);
                            string longestsInLine1 = "";
                            for (int i = 0; i < counterLongests; i++)
                                longestsInLine1 += longestsWords[i] + " ";
                            analysis.WriteLine("|{0,14}|{1,26}|", numberofline, longestsInLine1);
                        }
                        analysis.WriteLine("-------------------------------------------");
                    }
                }
            }
        }
        static void DeleteLongestsWords(ref string[] required, ref int counter, string[] longestwords, int counterlongests)
        {
            int temp = counter;

            for (int i = 0; i < counter; i++)
            {
                for (int j = 0; j < counterlongests; j++)
                {

                    if (required[i] == longestwords[j])
                    {
                        required = ShiftToTheLeft(required, ref counter, i);
                    }
                }
            }
        }

        static string[] ShiftToTheLeft(string[] array, ref int counter, int index)
        {
            for (int j = index; j < counter; j++)
            {
                array[j] = array[j + 1];
            }
            counter--;
            return array;
        }

        static string Modified(string line, string[] requiredwords, int counterofrequired)
        {
            string delimeters = new string(line.Where(c => char.IsPunctuation(c)).ToArray());
            line = new string(line.Where(c => !char.IsPunctuation(c)).ToArray());
            for (int i = 0; i < counterofrequired; i++)
            {
                
                    string patternEnd = @"$";
                    string patternStart = @"^";
                    line = Regex.Replace(line, String.Format(" " + requiredwords[i] + " "), " ");
                    line = Regex.Replace(line, String.Format(" " + requiredwords[i] + patternEnd), " ");
                    line = Regex.Replace(line, String.Format(patternStart + requiredwords[i] + " "), " ");
                    line = line.Insert(line.Length, " " + requiredwords[i]);
                

            }

            //line = line.Insert(line.Length, delimeters);

            line = Regex.Replace(line.Trim(), "[ ]+", " ") + delimeters;
            return line;
        }


        static char[] GetCharactersLongestWords(string[] longestsWords, int counterLongests)
        {
            string characters = "";
            for (int i = 0; i < counterLongests; i++)
            {
                characters += longestsWords[i];
            }
            return characters.ToCharArray();
        }

        static string[] FillingRequiredWords(string[] words, string[] longestWords, int counterLongests, ref int counterRequired)
        {
            char[] characters = GetCharactersLongestWords(longestWords, counterLongests);
            string requiredwords = "";
            for (int j = 0; j < words.Length; j++)
            {
                for (int i = 0; i < characters.Length; i++)
                {
                    if (words[j].ToUpper().Contains(characters[i]) || words[j].ToLower().Contains(characters[i]))
                    {
                        requiredwords += words[j] + ";";
                        counterRequired++;
                    }
                }
            }

            return requiredwords.Split(';');
        }

        static string[] DeleteRepetitions(string[] requiredwords, ref int k)
        {
            string[] temp = new string[CMax];
            int counterRequired = k;
            k = 0;
            for (int i = 0; i < counterRequired; i++)
            {
                bool contains = false;
                for (int j = i + 1; j < counterRequired; j++)
                {
                    if (requiredwords[i] == requiredwords[j])
                    {
                        contains = true;
                    }
                }

                if (contains == false)
                {
                    temp[k++] = requiredwords[i];
                }

            }

            return temp;
        }


        static int FindLongestLength(string[] words)
        {
            int maxlength = -1;
            for (int i = 0; i < words.Length; i++)
            {
                if (maxlength < words[i].Length)
                {
                    maxlength = words[i].Length;
                }
            }
            return maxlength;
        }

        static string[] FindOnlyLongestWords(string[] words, ref int count, out string longestsinline)
        {
            string[] longests = new string[CMax];
            longestsinline = "";
            count = 0;

            for (int i = 0; i < words.Length; i++)
            {


                if (words[i].Length == FindLongestLength(words))
                {
                    longests[count] = words[i];
                    longestsinline += longests[count] + " ";
                    count++;
                }

            }
            return longests;
        }

    }
}
