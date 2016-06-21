using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Asciigame
{
    class HighScore
    {
        public Dictionary<string, int> scores = new Dictionary<string,int>();
        public Dictionary<char, int> typoCharacters = new Dictionary<char, int>();

        public bool updateScore(string textName, int newScore)
        {
            int oldScore = 0;

            if (scores.ContainsKey(textName))
            {
                oldScore = scores[textName];
                scores[textName] = Math.Max(newScore, oldScore);
            }
            else
            {
                scores.Add(textName, newScore);
            }

            return newScore > oldScore;
        }

        public int getScore(string textName)
        {
            if (scores.ContainsKey(textName))
            {
                return (scores[textName]);
            }
            else
            {
                return 0;
            }
        }

        public bool readScoresFromFile()
        {
            if (File.Exists(TypingTest.path + "\\scores.txt"))
            {
                Debug.WriteLine("scores file exists");
                return true;
            }
            else
            {
                Debug.WriteLine("No scores file");
                return false;
            }
        }

        public void addTyposToCharDictionary(Dictionary<char, int> newTypos)
        {
            foreach (KeyValuePair<char, int> typo in newTypos)
            {
                if (typoCharacters.ContainsKey(typo.Key))
                {
                    typoCharacters[typo.Key] += typo.Value;
                }
                else
                {
                    typoCharacters.Add(typo.Key, typo.Value);
                }
            }
        }

        public void displayTypoCharacters(Dictionary<char, int> typoDictionary, int left, int bottom, int maxHeight)
        {
            int charsOnCurrentLine = 0;
            int xPosition = 0;
            //int yPosition = 0;
            SortedDictionary<char, int> sortedDict = new SortedDictionary<char, int>(typoDictionary);
            //Dictionary<char, int> orderedDict = (Dictionary<char, int>) typoDictionary.OrderByDescending(kv => kv.Value);
            Debug.WriteLine("typoDictionary: " + sortedDict.Keys.Count);

            //find highest typo character occurence
            int highestTypoValue = 1; //set to 1 to avoid div0 error, just in case
            foreach (KeyValuePair<char, int> typo in sortedDict)
            {
                if (typo.Value > highestTypoValue) highestTypoValue = typo.Value;
            }

            foreach (KeyValuePair<char, int> typo in sortedDict)
            {
                if (xPosition < Console.WindowWidth)
                {
                    Console.SetCursorPosition(left + xPosition, bottom - 1);
                    Console.Write(typo.Key);
                    float adjustedHeight = ((float)typo.Value / (float)highestTypoValue) * (float)maxHeight;
                    for (int i = 0; i < adjustedHeight; i++)
                    {
                        Console.SetCursorPosition(left + xPosition, bottom - i - 2);
                        Console.Write("^");
                    }
                }
                xPosition++;
            }
        }
    }
}
