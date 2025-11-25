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
    public class IntVariable : TreeVariable<int>
    {
        public static implicit operator IntVariable(int value)
        {
            IntVariable variable = new IntVariable();
            variable.SetValue(value);
            return variable;
        }

        public static implicit operator int(IntVariable variable)
        {
            return variable.GetValue();
        }
    }
}