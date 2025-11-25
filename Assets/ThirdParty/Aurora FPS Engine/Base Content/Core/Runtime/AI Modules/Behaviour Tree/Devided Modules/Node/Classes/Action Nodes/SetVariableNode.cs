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
using AuroraFPSRuntime.AIModules.BehaviourTree.Variables;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Nodes
{
    [TreeNodeContent("Set Variable", "Actions/Set Variable [Experimental]")]
    [HideScriptField]
    public class SetVariableNode : ActionNode
    {
        [SerializeField]
        [TreeVariable(null)]
        private string keyVariable;

        [SerializeReference]
        [CustomView(ViewGUI = "OnVariableGUI", ViewHeight = "OnVariableHeight")]
        private TreeVariable value;

        protected override State OnUpdate()
        {
            if (tree.TryGetVariable(keyVariable, out TreeVariable variable))
            {
                tree.AddVariable(keyVariable, value);
                return State.Success;
            }
            return State.Failure;
        }

#if UNITY_EDITOR
        private string lastKey;

        private void OnVariableGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.y += EditorGUIUtility.standardVerticalSpacing;
            position.height -= EditorGUIUtility.standardVerticalSpacing;

            if (lastKey != keyVariable)
            {
                if (tree.TryGetVariable(keyVariable, out TreeVariable variable))
                {
                    property.managedReferenceValue = Activator.CreateInstance(variable.GetType());
                }
                lastKey = keyVariable;
            }

            if (value != null)
            {
                EditorGUI.PropertyField(position, property.FindPropertyRelative("value"), label);
            }
        }

        private float OnVariableHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty valueProperty = property.FindPropertyRelative("value");
            if (valueProperty != null)
            {
                return EditorGUI.GetPropertyHeight(valueProperty) + EditorGUIUtility.standardVerticalSpacing;
            }
            else
            {
                return 0f;
            }
        }
#endif
    }
}