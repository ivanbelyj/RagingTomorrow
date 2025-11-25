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
    [TreeNodeContent("Set Bool", "Actions/Variables/Set Bool")]
    [HideScriptField]
    public class SetBoolNode : ActionNode
    {
        [SerializeField]
        [TreeVariable(typeof(bool))]
        private string boolVariable;

        [SerializeField]
        private bool value;

        protected override State OnUpdate()
        {
            if (tree.TryGetVariable<BoolVariable>(boolVariable, out BoolVariable variable))
            {
                variable.SetValue(value);
                return State.Success;
            }

            return State.Failure;
        }
    }
}