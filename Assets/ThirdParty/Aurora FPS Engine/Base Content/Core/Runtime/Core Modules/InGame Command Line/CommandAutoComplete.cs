/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright ? 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using System.Collections.Generic;

namespace AuroraFPSRuntime.CoreModules.CommandLine
{
    public sealed class CommandAutoComplete
    {
        private List<string> knownWords = new List<string>();
        private List<string> buffer = new List<string>();

        public void Register(string word)
        {
            knownWords.Add(word.ToLower());
        }

        public string[] Complete(ref string text, ref int formatWidth)
        {
            string partialWord = EatLastWord(ref text).ToLower();
            string known;

            for (int i = 0; i < knownWords.Count; i++)
            {
                known = knownWords[i];

                if (known.StartsWith(partialWord))
                {
                    buffer.Add(known);

                    if (known.Length > formatWidth)
                    {
                        formatWidth = known.Length;
                    }
                }
            }

            string[] completions = buffer.ToArray();
            buffer.Clear();

            text += PartialWord(completions);
            return completions;
        }

        private string EatLastWord(ref string text)
        {
            int lastSpace = text.LastIndexOf(' ');
            string result = text.Substring(lastSpace + 1);

            text = text.Substring(0, lastSpace + 1);
            return result;
        }

        private string PartialWord(string[] words)
        {
            if (words.Length == 0)
            {
                return "";
            }

            string firstMatch = words[0];
            int partialLength = firstMatch.Length;

            if (words.Length == 1)
            {
                return firstMatch;
            }

            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                if (partialLength > word.Length)
                {
                    partialLength = word.Length;
                }

                for (int j = 0; j < partialLength; j++)
                {
                    if (word[j] != firstMatch[j])
                    {
                        partialLength = j;
                    }
                }
            }
            return firstMatch.Substring(0, partialLength);
        }
    }
}
