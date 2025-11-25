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
    public class GameObjectListVariable : TreeVariable<List<GameObject>>
    {
        public static implicit operator GameObjectListVariable(List<GameObject> value)
        {
            GameObjectListVariable variable = new GameObjectListVariable();
            variable.SetValue(value);
            return variable;
        }

        public static implicit operator List<GameObject>(GameObjectListVariable variable)
        {
            return variable.GetValue();
        }
    }
}