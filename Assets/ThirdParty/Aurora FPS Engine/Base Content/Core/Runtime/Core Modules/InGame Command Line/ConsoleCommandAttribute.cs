using System;

namespace AuroraFPSRuntime.CoreModules.CommandLine
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ConsoleCommandAttribute : Attribute
    {
        public readonly string Name;

        public ConsoleCommandAttribute()
        {
            MinArgCount = 0;
            MaxArgCount = -1;
        }

        public ConsoleCommandAttribute(string name) : base()
        {
            Name = name;
        }

        #region [Optional Properties]
        public string Help { get; set; }
        public string Hint { get; set; }
        public int MinArgCount { get; set; }
        public int MaxArgCount { get; set; }
        #endregion
    }
}
