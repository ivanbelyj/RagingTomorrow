/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;
using AuroraFPSRuntime.Attributes;

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Nodes
{
    public abstract class DecoratorNode : TreeNode
    {
        [SerializeReference]
        [HideInInspector]
        protected TreeNode child;

        public sealed override State Update()
        {
            return child == null || child.mute ? State.Running : base.Update();
        }

        /// <summary>
        /// Implements cloning for DecorateNode.
        /// </summary>
        public override TreeNode Clone()
        {
            DecoratorNode node = Instantiate(this);
            node.child = child.Clone();
            return node;
        }

        #region [Getter / Setter]
        public TreeNode GetChild()
        {
            return child;
        }

        public void SetChild(TreeNode node)
        {
            child = node;
        }
        #endregion
    }
}