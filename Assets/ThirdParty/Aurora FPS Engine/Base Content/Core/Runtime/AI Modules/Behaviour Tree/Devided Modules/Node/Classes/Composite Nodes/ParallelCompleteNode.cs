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
using System.Collections.Generic;

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Nodes
{
    [TreeNodeContent("Parallel Complete", "Composites/Parallels/Parallel Complete")]
    [HideScriptField]
    public class ParallelCompleteNode : CompositeNode
    {
        List<State> childrenLeftToExecute = new List<State>();

        protected override void OnEntry()
        {
            childrenLeftToExecute.Clear();
            children.ForEach(c =>
            {
                childrenLeftToExecute.Add(State.Running);
            });
        }

        protected override State OnUpdate()
        {
            for (int i = 0; i < childrenLeftToExecute.Count; i++)
            {
                TreeNode child = GetChild(i);
                if (child.mute) continue;

                if (childrenLeftToExecute[i] == State.Running)
                {
                    if (child == null)
                    {
                        AbortRunningChildren();
                        return State.Failure;
                    }

                    State status = child.Update();
                    switch (status)
                    {
                        case State.Running:
                            break;
                        case State.Success:
                            AbortRunningChildren();
                            return State.Success;
                        case State.Failure:
                            AbortRunningChildren();
                            return State.Failure;
                    }

                    childrenLeftToExecute[i] = status;
                }
            }

            return State.Running;
        }

        private void AbortRunningChildren()
        {
            for (int i = 0; i < childrenLeftToExecute.Count; i++)
            {
                TreeNode child = GetChild(i);
                if (childrenLeftToExecute[i] == State.Running)
                {
                    if (child != null)
                    {
                        child.Abort();
                    }
                }
            }
        }
    }
}