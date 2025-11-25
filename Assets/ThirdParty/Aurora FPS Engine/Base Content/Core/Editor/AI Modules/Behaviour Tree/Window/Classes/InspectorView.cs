/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AuroraFPSEditor.AIModules.BehaviourTree
{
    public class InspectorView : BehaviourTreeEditorView
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

        private Editor editor;

        public InspectorView()
        {
            UpdateSelection(null);
        }

        public void UpdateSelection(TreeNodeView nodeView)
        {
            Clear();

            Object.DestroyImmediate(editor);
            if (nodeView != null)
            {
                editor = Editor.CreateEditor(nodeView.GetNode());
            }

            Add(new IMGUIContainer(() => OnGUI(nodeView)));
        }

        private void OnGUI(TreeNodeView nodeView)
        {
            if (nodeView == null)
            {
                EditorGUILayout.HelpBox("Select a node from tree to view its properties.", MessageType.Info);
            }

            if (nodeView != null && editor != null && editor.target != null)
            {
                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 100;

                GUILayout.Space(3);

                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(3);
                    EditorGUILayout.BeginVertical();
                    {
                        editor.OnInspectorGUI();
                    }
                    EditorGUILayout.EndVertical();
                    GUILayout.Space(3);
                }
                EditorGUILayout.EndHorizontal();

                nodeView.OnInspectorUpdate();

                EditorGUIUtility.labelWidth = labelWidth;
            }
        }
    }
}