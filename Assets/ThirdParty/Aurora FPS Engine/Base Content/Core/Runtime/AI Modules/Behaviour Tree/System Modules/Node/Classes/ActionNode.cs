/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Nodes
{
    public abstract class ActionNode : TreeNode
    {
        [SerializeField]
        [Order(899)]
        private bool singleCall;

        private bool isCalled;

        public override State Update()
        {
            if (!singleCall || (singleCall && !isCalled))
            {
                isCalled = true;
                return base.Update();
            }

            return State.Success;
        }
    }
}
