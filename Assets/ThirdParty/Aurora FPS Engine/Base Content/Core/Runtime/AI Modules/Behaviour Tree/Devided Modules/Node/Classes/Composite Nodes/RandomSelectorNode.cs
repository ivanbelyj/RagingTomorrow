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
using System.Linq;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Nodes
{
    [TreeNodeContent("Random Selector", "Composites/Random Selector")]
    [HideScriptField]
    public class RandomSelectorNode : CompositeNode
    {
        private int current;
        private List<TreeNode> randomizedChildren;

        protected override void OnEntry()
        {
            current = 0;
            randomizedChildren = children.OrderBy(x => Random.value).ToList();
        }

        protected override State OnUpdate()
        {
            for (int i = current; i < children.Count; i++)
            {
                current = i;
                TreeNode child = GetChild(current);
                if (child == null || child.mute) continue;

                switch (child.Update())
                {
                    case State.Running:
                        return State.Running;
                    case State.Success:
                        return State.Success;
                    case State.Failure:
                        continue;
                }
            }

            return State.Failure;
        }

        public override bool RemoveChild(TreeNode node)
        {
            randomizedChildren.Remove(node);
            return base.RemoveChild(node);
        }

        public override TreeNode GetChild(int index)
        {
            if (index < randomizedChildren.Count)
            {
                return randomizedChildren[index];
            }
            return null;
        }
    }
}