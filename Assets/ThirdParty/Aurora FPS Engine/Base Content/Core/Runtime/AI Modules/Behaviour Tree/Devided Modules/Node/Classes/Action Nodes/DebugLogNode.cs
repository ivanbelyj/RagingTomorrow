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
    [TreeNodeContent("Debug Log", "Actions/Unity/Debug Log")]
    [HideScriptField]
    public class DebugLogNode : ActionNode
    {
        [SerializeField]
        [TreeVariable(typeof(string))]
        private string messageVariable;

        [SerializeField]
        [TreeVariable(typeof(string))]
        private string message;

        #region [Variables Toggle]
#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        private bool messageToggle;
#endif
        #endregion

        protected override State OnUpdate()
        {
            if (!string.IsNullOrEmpty(messageVariable) && tree.TryGetVariable<StringVariable>(messageVariable, out StringVariable variable))
            {
                message = variable;
            }

            Debug.Log($"[Behaviour Tree Logger]: {message}");
            return State.Success;
        }
    }
}