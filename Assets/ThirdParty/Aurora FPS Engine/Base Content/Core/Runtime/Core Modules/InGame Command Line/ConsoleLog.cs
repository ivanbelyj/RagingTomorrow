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
    public sealed class ConsoleLog
    {
        public enum LogType
        {
            Error = UnityEngine.LogType.Error,
            Assert = UnityEngine.LogType.Assert,
            Warning = UnityEngine.LogType.Warning,
            Message = UnityEngine.LogType.Log,
            Exception = UnityEngine.LogType.Exception,
            Input,
            ShellMessage
        }

        public struct LogItem
        {
            public LogType type;
            public string message;
            public string stackTrace;
        }

        public readonly List<LogItem> Logs = new List<LogItem>();
        private int maxItems;

        public ConsoleLog(int maxItems)
        {
            this.maxItems = maxItems;
        }

        public void HandleLog(string message, LogType type)
        {
            HandleLog(message, "", type);
        }

        public void HandleLog(string message, string stackTrace, LogType type)
        {
            LogItem log = new LogItem()
            {
                message = message,
                stackTrace = stackTrace,
                type = type
            };

            Logs.Add(log);

            if (Logs.Count > maxItems)
            {
                Logs.RemoveAt(0);
            }
        }

        public void Clear()
        {
            Logs.Clear();
        }

        #region [Getter / Setter]
        public int GetMaxItems()
        {
            return maxItems;
        }

        public void SetMaxItems(int value)
        {
            maxItems = value;
        }
        #endregion
    }
}
