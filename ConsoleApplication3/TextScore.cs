using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asciigame
{
    class TextScore
    {
        public string textName = string.Empty;
        public Dictionary<char, int> typoCharacters;

        public TextScore(string _textName)
        {
            textName = _textName;
            typoCharacters = new Dictionary<char, int>();
        }

        public void addTypoCharacter(char badCharacter)
        {
            if (typoCharacters.ContainsKey(badCharacter))
            {
                typoCharacters[badCharacter] ++;
            }
            else
            {
                typoCharacters.Add(badCharacter, 1);
            }
            Debug.WriteLine("Typos for " + badCharacter + ": " + getTyposForCharacter(badCharacter));
        }

        public int getTyposForCharacter(char queryChar)
        {
            if (typoCharacters.ContainsKey(queryChar))
            {
                return typoCharacters[queryChar];
            }
            else
            {
                return 0;
            }
        }
    }
}
