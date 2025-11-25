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
    [TreeNodeContent("Parallel", "Composites/Parallels/Parallel")]
    [HideScriptField]
    public class ParallelNode : CompositeNode
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
            bool stillRunning = false;
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
                            stillRunning = true;
                            break;
                        case State.Success:
                            break;
                        case State.Failure:
                            AbortRunningChildren();
                            return State.Failure;
                    }
                    
                    childrenLeftToExecute[i] = status;
                }
            }

            return stillRunning ? State.Running : State.Success;
        }

        private void AbortRunningChildren()
        {
            for (int i = 0; i < childrenLeftToExecute.Count; i++)
            {
                if (childrenLeftToExecute[i] == State.Running)
                {
                    TreeNode child = GetChild(i);
                    if (child != null)
                    {
                        child.Abort();
                    }
                }
            }
        }
    }
}