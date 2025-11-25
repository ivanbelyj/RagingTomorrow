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
    public class Vector3ListVariable : TreeVariable<List<Vector3>>
    {
        public static implicit operator Vector3ListVariable(List<Vector3> value)
        {
            Vector3ListVariable variable = new Vector3ListVariable();
            variable.SetValue(value);
            return variable;
        }

        public static implicit operator List<Vector3>(Vector3ListVariable variable)
        {
            return variable.GetValue();
        }
    }
}