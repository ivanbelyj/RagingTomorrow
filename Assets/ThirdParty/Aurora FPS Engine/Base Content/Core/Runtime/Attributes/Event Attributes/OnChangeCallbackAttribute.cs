using System;

namespace AuroraFPSRuntime.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class OnChangeCallbackAttribute : ApexBaseAttribute
    {
        public readonly string callback;

        public OnChangeCallbackAttribute(string callback)
        {
            this.callback = callback;
        }
    }
}