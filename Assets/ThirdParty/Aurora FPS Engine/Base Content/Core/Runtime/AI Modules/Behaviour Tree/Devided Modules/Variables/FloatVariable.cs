/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Variables
{
    [System.Serializable]
    public class FloatVariable : TreeVariable<float>
    {
        public static implicit operator FloatVariable(float value)
        {
            FloatVariable variable = new FloatVariable();
            variable.SetValue(value);
            return variable;
        }

        public static implicit operator float(FloatVariable variable)
        {
            return variable.GetValue();
        }
    }
}