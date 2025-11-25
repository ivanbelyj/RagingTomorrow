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
    [TreeNodeContent("Wait", "Actions/Wait")]
    [HideScriptField]
    public class WaitNode : ActionNode
    {
        [SerializeField]
        [TreeVariable(typeof(float))]
        private string durationVariable;

        [SerializeField]
        [TreeVariable(typeof(float))]
        [Min(0.01f)]
        private float duration = 1f;

        #region [Variables Toggle]
#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        private bool durationToggle;
#endif
        #endregion

        // Stored required properties.
        private float startTime;

        protected override void OnEntry()
        {
            if (!string.IsNullOrEmpty(durationVariable) && tree.TryGetVariable<FloatVariable>(durationVariable, out FloatVariable floatVariable))
            {
                duration = floatVariable;
            }

            startTime = Time.time;
        }

        protected override State OnUpdate()
        {
            if (Time.time - startTime > duration)
            {
                return State.Success;
            }
            return State.Running;
        }
    }
}