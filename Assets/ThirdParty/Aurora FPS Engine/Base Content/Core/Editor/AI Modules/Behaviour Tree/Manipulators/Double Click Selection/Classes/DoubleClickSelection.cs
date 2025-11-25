/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEditor;
using UnityEngine.UIElements;
using AuroraFPSEditor.AIModules.Tree;
using AuroraFPSRuntime.AIModules.BehaviourTree;

namespace AuroraFPSEditor.AIModules.BehaviourTree
{
    public sealed class DoubleClickSelection : MouseManipulator
    {
        private double timer;
        private double doubleClickDeltaTime = 0.3;

        public DoubleClickSelection()
        {
            timer = EditorApplication.timeSinceStartup;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            var treeView = target as TreeView;
            if (treeView == null) return;

            double delta = EditorApplication.timeSinceStartup - timer;
            if (delta < doubleClickDeltaTime)
            {
                SelectChildren(evt);
            }

            timer = EditorApplication.timeSinceStartup;
        }


        private void SelectChildren(MouseDownEvent evt)
        {
            var treeView = target as TreeView;
            if (treeView == null || !CanStopManipulation(evt)) return;

            TreeNodeView clickedNode = evt.target as TreeNodeView;
            if (clickedNode == null)
            {
                var ve = evt.target as VisualElement;
                clickedNode = ve.GetFirstAncestorOfType<TreeNodeView>();
                if (clickedNode == null)
                    return;
            }

            // Add children to selection so the root element can be moved
            BehaviourTreeAsset.Traverse(clickedNode.GetNode(), (node) =>
            {
                var view = treeView.FindNodeView(node);
                treeView.AddToSelection(view);
            });
        }
    }
}