/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System.Text;

namespace AuroraFPSRuntime.CoreModules.TypeExtensions
{
    public static class StringExtensions
    {
        public static string ToTitle(this string source)
        {
            if (!string.IsNullOrEmpty(source))
            {
                StringBuilder stBuilder = new StringBuilder(source);
                for (int i = 0; i < stBuilder.Length; i++)
                {
                    if (i == 0)
                    {
                        stBuilder[0] = char.ToUpper(stBuilder[0]);
                    }
                    else
                    {
                        if (!char.IsLetter(stBuilder[i - 1]))
                        {
                            stBuilder[i] = char.ToUpper(stBuilder[i]);
                        }
                    }
                }
                source = stBuilder.ToString();
            }
            return source;
        }

        public static string LettersOnly(this string source)
        {
            StringBuilder stBuilder = new StringBuilder(source);
            for (int i = 0; i < stBuilder.Length; i++)
            {
                if (!char.IsLetter(stBuilder[i]))
                {
                    stBuilder.Remove(i, 1);
                }
            }
            return stBuilder.ToString();
        }
    }
}