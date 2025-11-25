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

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Nodes
{
    [TreeNodeContent("Sequencer", "Composites/Sequencer")]
    [HideScriptField]
    public class SequencerNode : CompositeNode
    {
        private int current;

        protected override void OnEntry()
        {
            current = 0;
        }

        protected override State OnUpdate()
        {
            for (int i = current; i < children.Count; i++)
            {
                current = i;
                TreeNode child = GetChild(current);
                if (child == null) return State.Failure;
                if (child.mute) continue;

                switch (child.Update())
                {
                    case State.Running:
                        return State.Running;
                    case State.Success:
                        continue;
                    case State.Failure:
                        return State.Failure;
                }
            }

            return State.Success;
        }
    }
}