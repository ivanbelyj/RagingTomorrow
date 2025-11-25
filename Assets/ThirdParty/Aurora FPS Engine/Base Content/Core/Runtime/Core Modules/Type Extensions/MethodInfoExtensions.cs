/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;
using System.Reflection;

namespace AuroraFPSRuntime.CoreModules.TypeExtensions
{
    public static class MethodInfoExtensions
    {
        public static string ToText(this MethodInfo method, bool includeParameters = true)
        {
            if (method != null)
            {
                string name = method.Name;
                name = name.ToTitle();
                name = name.LettersOnly();

                name += "(";

                if (includeParameters)
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters != null)
                    {
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            ParameterInfo parameter = parameters[i];
                            name += string.Format("{0} {1}", parameter.ParameterType.Name, parameter.Name);

                            if (i + 1 < parameters.Length)
                            {
                                name += ", ";
                            }
                        }
                    }
                }
                name += ")";
                return name;
            }
            return null;
        }

        public static GUIContent ToContent(this MethodInfo method, bool includeParameters = true)
        {
            string text = method.ToText(includeParameters);
            if(text != null)
            {
                return new GUIContent(text);
            }
            return null;
        }
    }
}