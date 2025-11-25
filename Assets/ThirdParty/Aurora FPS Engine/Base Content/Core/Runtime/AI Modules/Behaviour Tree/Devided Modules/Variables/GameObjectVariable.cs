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
    public class GameObjectVariable : TreeVariable<GameObject>
    {
        public static implicit operator GameObjectVariable(GameObject value)
        {
            GameObjectVariable variable = new GameObjectVariable();
            variable.SetValue(value);
            return variable;
        }

        public static implicit operator GameObject(GameObjectVariable variable)
        {
            return variable.GetValue();
        }
    }
}