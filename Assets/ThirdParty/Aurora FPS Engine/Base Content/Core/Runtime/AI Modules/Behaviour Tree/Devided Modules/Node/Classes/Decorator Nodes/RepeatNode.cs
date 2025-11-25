/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.AIModules.BehaviourTree.Attributes;
using AuroraFPSRuntime.Attributes;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Nodes
{
    [TreeNodeContent("Repeat", "Decorators /Repeat")]
    [HideScriptField]
    public class RepeatNode : DecoratorNode
    {
        [SerializeField]
        private bool fixedCount;

        [SerializeField]
        [VisibleIf("fixedCount", true)]
        [MinValue(1)]
        private int count;

        // Stored required properties.
        private int current;
        private State lastState;

        protected override void OnEntry()
        {
            current = 0;
        }

        protected override State OnUpdate()
        {
            if (child == null || child.mute)
            {
                return State.Failure;
            }

            if (fixedCount)
            {
                lastState = child.Update();
                if (lastState != State.Running)
                {
                    current++;
                }
                return current == count ? lastState : State.Running;
            }
            else
            {
                child.Update();
                return State.Running;
            }
        }
    }
}