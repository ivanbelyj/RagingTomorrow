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
    public class Vector3Variable : TreeVariable<Vector3>
    {
        public static implicit operator Vector3Variable(Vector3 value)
        {
            Vector3Variable variable = new Vector3Variable();
            variable.SetValue(value);
            return variable;
        }

        public static implicit operator Vector3(Vector3Variable variable)
        {
            return variable.GetValue();
        }
    }
}