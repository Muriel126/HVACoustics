using NestedDictionaryLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xbim.Ifc4.Interfaces;

namespace HVACoustics
{
    class Helper
    {
        public void CreateSortedListWithoutDuplicatesFromDict(Dictionary<double, string> evaluatedComponents)
        {
            var listEvaluatedComponents = evaluatedComponents.ToList();
            var hashSetList = new HashSet<KeyValuePair<double, string>>(listEvaluatedComponents);
            var listWithoutDuplicates = hashSetList.ToList();
            listWithoutDuplicates.Sort((pair1, pair2) => pair1.Key.CompareTo(pair2.Key));      
        }

        public static void PrintNestedDictionary(NestedDictionary<string, double, double> dictionary)
        {
            Console.WriteLine();
            foreach (KeyValuePair<string, NestedDictionary<double, double>> kvp1 in dictionary)
                foreach (KeyValuePair<double, double> kvp2 in kvp1.Value)
                    Console.WriteLine($"{kvp1.Key}\t-\t{kvp2.Key}\t-\t{kvp2.Value}");
            Console.WriteLine();
        }
        public static void WriteHeadLine(string text)
        {
            Helper.WriteColor(text , ConsoleColor.DarkGray);
           /* Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(text);
            Console.BackgroundColor = ConsoleColor.Black;*/
        }
        static void WriteColor(string message, ConsoleColor color)
        {
            var pieces = Regex.Split(message, @"(\[[^\]]*\])");

            for (int i = 0; i < pieces.Length; i++)
            {
                string piece = pieces[i];

                if (piece.StartsWith("[") && piece.EndsWith("]"))
                {
                    Console.BackgroundColor = color;
                    piece = piece.Substring(1, piece.Length - 2);
                }

                Console.Write(piece);
                Console.ResetColor();
            }

            Console.WriteLine();
        }

        public void CreateSortedListWithoutDuplicates(List<double> listEvaluatedComponents)
        {
            //var listEvaluatedComponents = evaluatedComponents.ToList();
           // var hashSetList = new HashSet<KeyValuePair<string, int>>(listEvaluatedComponents);
           // var listWithoutDuplicates = hashSetList.ToList();
            //listWithoutDuplicates.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

            /*foreach (var value in listWithoutDuplicates)
            {
                string part1 = value.ToString().Split("[")[1];
                string laufnummer = part1.Split(",")[0];
                string part2 = part1.Split(" ")[1];
                string prozentsatz = part2.Substring(0, part2.Length - 1);

                Console.WriteLine("Similar component {0}, {1} % accordance.", laufnummer, prozentsatz);
            }*/
        }
    }
}
