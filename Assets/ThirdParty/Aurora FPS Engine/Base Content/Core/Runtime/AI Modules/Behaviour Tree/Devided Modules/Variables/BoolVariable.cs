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
    public class BoolVariable : TreeVariable<bool>
    {
        public static implicit operator BoolVariable(bool value)
        {
            BoolVariable variable = new BoolVariable();
            variable.SetValue(value);
            return variable;
        }

        public static implicit operator bool(BoolVariable variable)
        {
            return variable.GetValue();
        }
    }
}