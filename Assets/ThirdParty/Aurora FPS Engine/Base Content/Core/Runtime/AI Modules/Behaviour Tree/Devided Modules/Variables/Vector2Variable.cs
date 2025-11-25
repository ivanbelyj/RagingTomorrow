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
    public class Vector2Variable : TreeVariable<Vector2>
    {
        public static implicit operator Vector2Variable(Vector2 value)
        {
            Vector2Variable variable = new Vector2Variable();
            variable.SetValue(value);
            return variable;
        }

        public static implicit operator Vector2(Vector2Variable variable)
        {
            return variable.GetValue();
        }
    }
}