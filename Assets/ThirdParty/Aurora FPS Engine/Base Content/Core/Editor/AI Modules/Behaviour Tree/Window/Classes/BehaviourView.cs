/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.AIModules.BehaviourTree;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AuroraFPSEditor.AIModules.BehaviourTree
{
    public class BehaviourView : BehaviourTreeEditorView
    {
        public new class UxmlFactory : UxmlFactory<BehaviourView, VisualElement.UxmlTraits> { }

        private Editor editor;

        public BehaviourView()
        {
            UpdateBehaviour(null);
        }

        public void UpdateBehaviour(BehaviourTreeAsset behaviourTree)
        {
            Clear();

            Object.DestroyImmediate(editor);
            if (behaviourTree != null)
            {
                editor = Editor.CreateEditor(behaviourTree);
            }

            Add(new IMGUIContainer(() => OnGUI(behaviourTree)));
        }

        private void OnGUI(BehaviourTreeAsset behaviourTree)
        {
            if (behaviourTree == null)
            {
                EditorGUILayout.HelpBox("No behaviour tree selected. Create a new behavior tree or select one of the created ones.", MessageType.Info);

                if (GUILayout.Button("Create new Behaviour tree"))
                {
                    behaviourTreeEditor.CreateBehaviourTree("New Behaviour Tree");
                }
            }

            if (behaviourTree != null)
            {
                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 100;

                GUILayout.Space(2);

                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(2);
                    EditorGUILayout.BeginVertical();
                    {
                        editor.OnInspectorGUI();
                    }
                    EditorGUILayout.EndVertical();
                    GUILayout.Space(2);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUIUtility.labelWidth = labelWidth;
            }
        }
    }
}