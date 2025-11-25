/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Variables
{
    [System.Serializable]
    public class StringVariable : TreeVariable<string>
    {
        public static implicit operator StringVariable(string value)
        {
            StringVariable variable = new StringVariable();
            variable.SetValue(value);
            return variable;
        }

        public static implicit operator string(StringVariable variable)
        {
            return variable.GetValue();
        }
    }
}