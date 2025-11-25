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
    public sealed class CommandHistory
    {
        private List<string> historyCache = new List<string>();
        private int position;

        public void Push(string commandString)
        {
            if (commandString == "")
            {
                return;
            }

            historyCache.Add(commandString);
            position = historyCache.Count;
        }

        public string Next()
        {
            position++;

            if (position >= historyCache.Count)
            {
                position = historyCache.Count;
                return "";
            }

            return historyCache[position];
        }

        public string Previous()
        {
            if (historyCache.Count == 0)
            {
                return "";
            }

            position--;

            if (position < 0)
            {
                position = 0;
            }

            return historyCache[position];
        }

        public int Count()
        {
            return historyCache.Count;
        }

        public void Clear()
        {
            historyCache.Clear();
            position = 0;
        }

        public IEnumerable<string> Commands
        {
            get
            {
                return historyCache;
            }
        }
    }
}
