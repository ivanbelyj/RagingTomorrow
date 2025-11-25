/* ==================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================== */

using AuroraFPSEditor.Attributes;
using UnityEngine;
using UnityEditor;

namespace AuroraFPSEditor.AI
{
    internal sealed class BehaviourEditorWindow : EditorWindow
    {
        // Stored serialized target properties.
        private Object target;
        private SerializedObject serializedObject;
        private ApexProperty behaviour;

        // Stored required properties.
        private string behaviourName;
        private Vector2 scrollPosition;
        private bool isInitialized;
        private GUIStyle titleStyle;

        internal void InitializeBehaviour(string behaviourName, Object target, string propertyPath)
        {
            this.behaviourName = behaviourName;
            this.target = target;
            serializedObject = new SerializedObject(target);
            behaviour = new ApexProperty(serializedObject.FindProperty(propertyPath));
            isInitialized = true;
        }

        private void OnEnable()
        {
            if (isInitialized && (target == null || serializedObject == null || behaviour == null || (behaviour != null && behaviour.TargetSerializedProperty == null)))
            {
                Close();
            }
        }

        private void OnGUI()
        {
            if (isInitialized && (target == null || serializedObject == null || behaviour == null || (behaviour != null && behaviour.TargetSerializedProperty == null)))
            {
                Close();
            }

            float standardVerticalSpacing = EditorGUIUtility.standardVerticalSpacing;

            Rect titlePosition = GUILayoutUtility.GetRect(0, 35);

            if(titleStyle == null)
            {
                titleStyle = new GUIStyle("In BigTitle");
                titleStyle.fontSize = 18;
                titleStyle.alignment = TextAnchor.MiddleCenter;
            }
            GUI.Label(titlePosition, new GUIContent(behaviourName + " Behaviour Settings"), titleStyle);

            GUILayout.Space(3);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            float height = behaviour.GetChildrenHeight() + standardVerticalSpacing;
            Rect fieldsPosition = GUILayoutUtility.GetRect(0, height);
            Rect childrenPosition = new Rect(fieldsPosition.x + 3, fieldsPosition.y + standardVerticalSpacing, fieldsPosition.width - 6, fieldsPosition.height);
            serializedObject.Update();
            behaviour.DrawChildren(childrenPosition);
            serializedObject.ApplyModifiedProperties();

            GUILayout.Space(3);
            GUILayout.EndScrollView();
        }

        #region [Static Methods]
        public static void Open(string behaviourName, Object target, string propertyPath)
        {
            BehaviourEditorWindow window = GetWindow<BehaviourEditorWindow>();
            window.titleContent = new GUIContent("Behaviour Editor");
            window.InitializeBehaviour(behaviourName, target, propertyPath);
            window.Show();
        }
        #endregion
    }
}