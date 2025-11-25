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
    public class TransformVariable : TreeVariable<Transform>
    {
        public static implicit operator TransformVariable(Transform value)
        {
            TransformVariable variable = new TransformVariable();
            variable.SetValue(value);
            return variable;
        }

        public static implicit operator Transform(TransformVariable variable)
        {
            return variable.GetValue();
        }
    }
}