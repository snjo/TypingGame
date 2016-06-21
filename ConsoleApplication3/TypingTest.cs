using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Asciigame
{
    class TypingTest : GameMode
    {
        public string text;
        public textError[] errorArray;
        public int selectedTextNumber = 0;
        private List<string> textLines;

        private int currentTextPosition = 0;
        private int currentCharPos = 0;
        private int currentLinePos = 0;

        private int paddingTop = 2;

        DateTime clock = new DateTime();
        bool clockRunning = false;
        double secondsSinceStart = 0;

        public static string exeLocation;
        public static string path;

        private HighScore highScore = new HighScore();
        private string currentTextName = string.Empty;

        private TextScore textScore;

        public enum textError
        {
            Blank,
            Correct,
            WasCorrected,
            Typo,
        }

        private static TypingTest _instance;
        public static TypingTest Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TypingTest();
                }
                return _instance;
            }
        }

        public override void Start(Game _game)
        {
            base.Start(_game);
            Console.OutputEncoding = System.Text.Encoding.Unicode;            
            exeLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            path = System.IO.Path.GetDirectoryName(exeLocation);

            Console.Clear();

            highScore.readScoresFromFile();
            highScore.displayTypoCharacters(highScore.typoCharacters, 0, Console.WindowHeight, 10);
            
            userSelectText();

            resetCounters();
            
            Console.Clear();
            Console.SetCursorPosition(0, paddingTop);
            loadText(selectedTextNumber);            
            formatText();
            displayText();
            //Console.ReadKey(false);
        }

        private void resetCounters()
        {
            currentTextPosition = 0;
            currentCharPos = 0;
            currentLinePos = 0;
            secondsSinceStart = 0;
            clockRunning = false;
            textScore = new TextScore(currentTextName);
        }

        private void userSelectText()
        {           
            //Console.ReadKey();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Select Text by typing its name and pressing Enter, Q to quit");
            Console.WriteLine();
            ListFilesInFolder();            
            //listTextsInVariables();

            string userInput = Console.ReadLine();
            int selection = 0;
            if (userInput == "Q" || userInput == "q")
            {
                game.terminateApplication = true;
            }
            else if (int.TryParse(userInput, out selection))
            {
                selectedTextNumber = selection;
            }
            else
            {
                readTextFromFile(userInput);
            }
        }

        private void listTextsInVariables()
        {
            for (int i = 0; i < 100; i++)
            {
                string newText = loadText(i);
                if (newText == string.Empty || newText == "No text loaded ")
                    break;
                Console.WriteLine(i + ": " + previewText(newText) + "...");
            }
        }

        private string previewText(string longText)
        {
            return longText.Substring(0,Math.Min(Math.Max(0, longText.Length),40));
        }

        private void ListFilesInFolder()
        {
            string textFolder = path + "\\text";
            if (Directory.Exists(textFolder))
            {
                string[] filesInFolder = Directory.GetFiles(textFolder, "*.txt");
                foreach (string fileName in filesInFolder)
                {
                    string shortName = Path.GetFileNameWithoutExtension(fileName);
                    Console.Write(shortName);
                    Console.Write(": " + previewText(File.ReadAllLines(fileName)[0]) + "...");
                    Console.WriteLine(" " + highScore.getScore(shortName));
                }
            }
            else
            {
                Console.WriteLine("No folder called " + textFolder);
            }
        }

        private void readTextFromFile(string userInput)
        {            
            Debug.WriteLine("paths: " + exeLocation + " --- " + path);            
            string filepath = path + "\\text\\" + userInput + ".txt";
            if (File.Exists(filepath))
            {
                Debug.WriteLine("File exists");
                text = File.ReadAllText(filepath);
                if (text.Length < 1) selectedTextNumber = 0;
                selectedTextNumber = -1;
                currentTextName = userInput;
            }
            else
            {
                Debug.WriteLine("File missing: " + filepath);
                selectedTextNumber = 0;
                currentTextName = "0";
            }
        }

        public override void Update()
        {
            base.Update();

            if (clockRunning) updateClock();
            updateStatusText();
            if (exitGameMode) return;
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            char key = keyInfo.KeyChar;
            
            char expectedKey = getExpectedKey(currentCharPos);            
            if (key == expectedKey)
            {
                if (atEndOfText)
                {
                    endOfTextUpdate();
                    return;
                }
                
                if (errorArray[currentTextPosition] == textError.Typo || errorArray[currentTextPosition] == textError.WasCorrected)
                {
                    errorArray[currentTextPosition] = textError.WasCorrected;
                }
                else
                {
                    errorArray[currentTextPosition] = textError.Correct;
                }

                Console.SetCursorPosition(currentCharPos, currentLinePos + paddingTop);
                if (errorArray[currentTextPosition] == textError.Correct)
                {
                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                }
                Console.Write(key);

                

                typeHeadForward();
                if (!clockRunning)
                {
                    Debug.Write("Starting Clock");
                    startClock();
                }
            }
            else if (key == (char)8)
            {
                typeHeadBack();
                Console.BackgroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(currentCharPos, currentLinePos + paddingTop);
                Console.Write(getExpectedKey(currentCharPos));                    
            }
            else if (key == (char)27)
            {
                stopClock();
                Start(game);
                //RequestTermination();
            }
            else
            {
                if (atEndOfText)
                {
                    endOfTextUpdate();
                    return;
                }
                errorArray[currentTextPosition] = textError.Typo;
                Console.BackgroundColor = ConsoleColor.DarkRed;
                textScore.addTypoCharacter(expectedKey);
                Console.SetCursorPosition(currentCharPos, currentLinePos + paddingTop);
                Console.Write(expectedKey);
                typeHeadForward();
            }
        }

        private void endOfTextUpdate()
        {
            stopClock();
            highScore.updateScore(currentTextName, GetScore());
            Console.SetCursorPosition(0, 10);
            highScore.displayTypoCharacters(textScore.typoCharacters, 0, Console.WindowHeight, 10);
            highScore.addTyposToCharDictionary(textScore.typoCharacters);
        }

        private bool atEndOfText
        {
            get
            {
                return (currentLinePos == textLines.Count - 1 && currentCharPos == textLines[currentLinePos].Length - 1);
            }
        }

        private void typeHeadForward()
        {
            currentTextPosition++;
            currentCharPos++;
            if (currentCharPos > textLines[currentLinePos].Length - 1)
            {
                if (currentLinePos < textLines.Count - 1)
                {
                    currentCharPos = 0;
                    currentLinePos++;
                }
                else
                {
                    currentTextPosition--;
                    currentCharPos--;
                }
            }
        }

        private void typeHeadBack()
        {
            currentTextPosition--;
            currentCharPos--;
            if (currentCharPos < 0)
            {
                if (currentLinePos > 0)
                {
                    currentLinePos--;
                    currentCharPos = textLines[currentLinePos].Length - 1;
                }
                else
                {
                    currentCharPos = 0;
                }
            }
        }

        private char getExpectedKey(int position)
        {
            char expectedKey = textLines[currentLinePos].Substring(position, 1).ToCharArray()[0];
            return expectedKey;
        }

        private string loadText(int textNumber) // make sure the text has a trailing space
        {
            switch (textNumber)
            {
                case -1:
                    //got text from file
                    break;
                case 0:
                    text = "A typewriter is a mechanical or electromechanical machine for writing in characters similar to those produced by printer's movable type by means of keyboard-operated types striking a ribbon to transfer ink. ";
                    break;
                case 1:
                    text = "Fare for vådeskudd. Det at politiet tar ladegrep når de skal gå inn i en skarp situasjon og dermed har et skuddklart våpen, er jo helt greit. Men at politiet skal gå rundt med skuddklare våpen i Oslos og Bergens gater i påvente av at noe dramatisk skal skje, er å gå altfor langt. Til det er faren for vådeskudd og uheldige hendelser altfor høy, sier Berg-Knutsen. ";
                    break;
                case 2:
                    text = "Ber politiet endre praksis. Nå viser det seg i tillegg at en politimann i april i år avfyrte et vådeskudd inne på politistasjonen i Kristiansund. Det er VG som melder dette tirsdag kveld. Hendelsen er under etterforskning av spesialenheten for politisaker. ";
                    break;
                case 3:
                    text = "\"The Brink\" er en ny HBO-komiserie som fokuserer på en geopolitisk krise og dens effekt på tre uforenelige og desperate menn: Walter Larson, USAs utenriksminister, Alex Talbott, en diplomat stasjonert i Islamabad, og Zeke \"Z-Pak\" Tilson, en jagerpilot med en lukrativ sidegeskjeft hvor han selger reseptbelagte medisiner. Denne episke mørke komiserien begynner med at det er fare for kupp i Pakistan. En kjeltring av en general tar kontroll over landet og atomvåpnene der, noe som gjør at verden må stole på disse tre utypiske amerikanske heltene. Fra de turbulente gatene i Midtøsten til Det Hvite Hus og et krigsskip i Rødehavet: \"The Brink\" tar oss med på en vill reise gjennom mange tidssoner for å vise oss hvordan svakhetene, egoene og rivaliseringen til politiske ledere kan føre oss til randen av en ny verdenskrig. ";
                    break;
                default:
                    text = "No text loaded ";
                    break;
            }
            errorArray = new textError[text.Length];
            return text;
        }

        private void formatText()
        {
            Debug.WriteLine("Formatting string: " + text);
            textLines = new List<string>();
            int bufferWidth = 62;//game.bufferSize.x;
            for (int i = 0; i < text.Length; )
            {
                string finalString = string.Empty;
                int adjustedWidth = bufferWidth;
                char[] line;
                if (text.Length > i + bufferWidth)
                    line = text.Substring(i, bufferWidth).ToCharArray();
                else
                    line = text.Substring(i).ToCharArray();

                for (int j = line.Length - 1; j >= 0; j--)
                {
                    Debug.WriteLine("i = " + i + ", j = " + j);
                    if (line[j] == ' ' || line[j] == ',' || line[j] == '.' || line[j] == '-')
                    {
                        adjustedWidth = j+1;
                        finalString = new string(line).Substring(0, adjustedWidth); ;
                        break;
                    }
                }
                if (adjustedWidth < 1) adjustedWidth = bufferWidth;
                i += adjustedWidth;
                //int width = bufferWidth;
                //if (i + adjustedWidth > text.Length)
                //    adjustedWidth = text.Length % adjustedWidth;
                textLines.Add(finalString);
            }
        }

        private void displayText()
        {
            for (int i = 0; i < textLines.Count; i++)
            {
                Console.WriteLine(textLines[i]);
            }
        }

        private void startClock()
        {
            clock = DateTime.Now;            
            clockRunning = true;
            secondsSinceStart = 0;
        }

        private void stopClock()
        {
            clockRunning = false;
        }

        private void updateClock()
        {
            secondsSinceStart = (DateTime.Now - clock).TotalSeconds;
        }

        private void updateStatusText()
        {
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write("WPM: " + GetWordsPerMinute() + ", Chars written: " + GetCharactersWritten() + "               ");            
            Console.Write("Score: " + GetScore());
            Console.Write("  " + GetStarRating());
        }

        private string GetStarRating()
        {
            int score = GetScore();
            string starText = "     ";
            if (score >= 1500) starText = "*    ";
            if (score >= 2000) starText = "**   ";
            if (score >= 2500) starText = "***  ";
            if (score >= 3000) starText = "**** ";
            if (score >= 4000) starText = "*****";

            return starText;
        }

        private int GetScore()
        {
            float baseScore = errorArray.Length * 2;
            Debug.WriteLine("bs1: " + baseScore);
            for (int i = 0; i < errorArray.Length; i++)
            {
                if (errorArray[i] == textError.Blank)
                    baseScore -= 2;
                else
                    baseScore -= (int)errorArray[i] - 1;
            }
            Debug.WriteLine("bs2: " + baseScore);
            baseScore = (baseScore / 2f) / errorArray.Length;
            Debug.WriteLine("Basescore = " + baseScore);
            return (int)(baseScore * GetWordsPerMinute() * 100);
        }

        private int GetWordsPerMinute()
        {
            Debug.WriteLine(clock + " / " + secondsSinceStart);
            if (secondsSinceStart > 1)
                return (int)((GetCharactersWritten() / 5) / (secondsSinceStart / 60d));
            else return 0;
        }

        private int GetCharactersWritten()
        {
            int result = 0;
            for (int i = 0; i < currentLinePos; i++)
            {
                result += textLines[i].Length;
            }
            result += currentCharPos;
            return result;
        }
    }
}
