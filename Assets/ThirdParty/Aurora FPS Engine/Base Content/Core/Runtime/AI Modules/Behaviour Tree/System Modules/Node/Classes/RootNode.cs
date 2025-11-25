/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.AIModules.BehaviourTree.Attributes;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Nodes
{
    [TreeNodeContent("Start", "Start", Hide = true)]
    [HideScriptField]
    public sealed class RootNode : TreeNode
    {
        [SerializeReference]
        [HideInInspector]
        private TreeNode child;

        public override State Update()
        {
            return child == null ? State.Failure : base.Update();
        }

        /// <summary>
        /// Updates the child node.
        /// </summary>
        protected override State OnUpdate()
        {
            return child.Update();
        }

        /// <summary>
        /// Implements cloning for RootNode.
        /// </summary>
        public override TreeNode Clone()
        {
            RootNode node = Instantiate(this);
            if (child != null)
            {
                node.child = child.Clone();
            }
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