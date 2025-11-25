/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.AIModules.BehaviourTree.Attributes;
using AuroraFPSRuntime.AIModules.BehaviourTree.Variables;
using AuroraFPSRuntime.Attributes;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Nodes
{
    [TreeNodeContent("Random Range", "Actions/Unity/Random Range")]
    [HideScriptField]
    public class RandomRangeNode : ActionNode
    {
        [SerializeField]
        [TreeVariable(typeof(Vector2))]
        private string minMaxVariable;

        [SerializeField]
        [TreeVariable(typeof(Vector2))]
        private Vector2 minMax;


        [SerializeField]
        [TreeVariable(typeof(float))]
        private string storageVariable;

        #region [Variables Toggle]
#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        private bool minMaxToggle;
#endif
        #endregion

        protected override State OnUpdate()
        {
            if (!string.IsNullOrEmpty(minMaxVariable) && tree.TryGetVariable<Vector2Variable>(minMaxVariable, out Vector2Variable vector2Variable))
            {
                minMax = vector2Variable;
            }

            if (tree.TryGetVariable<FloatVariable>(storageVariable, out FloatVariable floatVariable))
            {
                float randomValue = Random.Range(minMax.x, minMax.y);
                floatVariable.SetValue(randomValue);
                return State.Success;
            }
            return State.Failure;
        }
    }
}