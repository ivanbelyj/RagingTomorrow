/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System.Collections.Generic;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Variables
{
    [System.Serializable]
    public class TransformListVariable : TreeVariable<List<Transform>>
    {
        public static implicit operator TransformListVariable(List<Transform> value)
        {
            TransformListVariable variable = new TransformListVariable();
            variable.SetValue(value);
            return variable;
        }

        public static implicit operator List<Transform>(TransformListVariable variable)
        {
            return variable.GetValue();
        }
    }
}