/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.AIModules.BehaviourTree;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace AuroraFPSEditor.AIModules.BehaviourTree
{
    public class TreeGroupView : Group
    {
        private TreeGroup group;

        public TreeGroupView(TreeGroup group)
        {
            this.group = group;
            title = group.title;

            this.viewDataKey = group.GetInstanceID().ToString();
        }

        protected override void OnGroupRenamed(string oldName, string newName)
        {
            base.OnGroupRenamed(oldName, newName);
            group.title = newName;
        }

        protected override void OnElementsAdded(IEnumerable<GraphElement> elements)
        {
            base.OnElementsAdded(elements);

            foreach (var element in elements)
            {
                TreeNodeView nodeView = element as TreeNodeView;
                if (nodeView != null)
                {
                    if (!group.nodes.Contains(nodeView.GetNode()))
                    {
                        group.nodes.Add(nodeView.GetNode());
                    }
                }
            }
        }

        protected override void OnElementsRemoved(IEnumerable<GraphElement> elements)
        {
            base.OnElementsRemoved(elements);

            BehaviourTreeEditor behaviourTreeEditor = EditorWindow.focusedWindow as BehaviourTreeEditor;
            if (behaviourTreeEditor != null && behaviourTreeEditor.GetTreeView().GetTree() == group.tree)
            {
                foreach (var element in elements)
                {
                    TreeNodeView nodeView = element as TreeNodeView;
                    if (nodeView != null)
                    {
                        if (group.nodes.Contains(nodeView.GetNode()))
                        {
                            group.nodes.Remove(nodeView.GetNode());
                        }
                    }
                }
            }
        }

        #region [Getter / Setter]
        public TreeGroup GetGroup()
        {
            return group;
        }
        #endregion
    }
}