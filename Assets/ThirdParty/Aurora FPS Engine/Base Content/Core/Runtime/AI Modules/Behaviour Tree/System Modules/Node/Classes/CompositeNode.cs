/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Nodes
{
    public abstract class CompositeNode : TreeNode
    {
        [SerializeReference]
        [HideInInspector]
        protected List<TreeNode> children = new List<TreeNode>();

        public sealed override State Update()
        {
            if (children.Count == 0)
            {
                return State.Failure;
            }

            return children.All(c => c.mute) ? State.Failure : base.Update();
        }

        /// <summary>
        /// Adding a link to a node.
        /// </summary>
        public void AddChild(TreeNode node)
        {
            children.Add(node);
        }

        /// <summary>
        /// Removes the link to the node.
        /// </summary>
        public virtual bool RemoveChild(TreeNode node)
        {
            return children.Remove(node);
        }

        /// <summary>
        /// Removes the link to the node by its index.
        /// </summary>
        public void RemoveChild(int index)
        {
            children.RemoveAt(index);
        }

        /// <summary>
        /// Implements cloning for CompositeNode.
        /// </summary>
        public override TreeNode Clone()
        {
            CompositeNode node = Instantiate(this);
            node.children = children.ConvertAll(c => c.Clone());
            return node;
        }

        #region [Getter / Setter]
        public List<TreeNode> GetChildren()
        {
            return children;
        }

        public void SetChildren(List<TreeNode> children)
        {
            this.children = children;
        }

        public virtual TreeNode GetChild(int index)
        {
            if (index < children.Count)
            {
                return children[index];
            }
            return null;
        }

        public void SetChild(int index, TreeNode node)
        {
            children[index] = node;
        }

        public int GetChildCount()
        {
            return children.Count;
        }
        #endregion
    }
}