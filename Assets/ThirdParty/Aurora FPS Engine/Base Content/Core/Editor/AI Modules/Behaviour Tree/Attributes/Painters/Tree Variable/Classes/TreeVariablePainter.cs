/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSEditor.Attributes;
using AuroraFPSRuntime.AIModules.BehaviourTree;
using AuroraFPSRuntime.AIModules.BehaviourTree.Attributes;
using AuroraFPSRuntime.AIModules.BehaviourTree.Nodes;
using AuroraFPSRuntime.AIModules.BehaviourTree.Variables;
using AuroraFPSRuntime.Attributes;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AuroraFPSEditor.AIModules.Tree.Attributes
{
    [ViewTarget(typeof(TreeVariableAttribute))]
    public sealed class TreeVariablePainter : PropertyView
    {
        private readonly GUIContent buttonContent = EditorGUIUtility.IconContent("align_vertically_center");
        private GUIStyle buttonStyle;
        private TreeVariableAttribute treeVariableAttribute;

        private SerializedProperty targetProperty;
        private SerializedProperty constProperty;
        private SerializedProperty variableProperty;
        private SerializedProperty toggleProperty;

        private string fieldName;
        private string displayName;

        public override void OnInitialize(SerializedProperty property, ViewAttribute attribute, GUIContent label)
        {
            treeVariableAttribute = attribute as TreeVariableAttribute;

            targetProperty = property;
            constProperty = property.serializedObject.FindProperty(targetProperty.name.Replace("Variable", ""));
            variableProperty = property.serializedObject.FindProperty(targetProperty.name.Replace("Variable", "") + "Variable");
            toggleProperty = property.serializedObject.FindProperty(targetProperty.name.Replace("Variable", "") + "Toggle");

            fieldName = property.name.Replace("Variable", "");
            displayName = property.displayName.Replace("Variable", "");
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (buttonStyle == null)
            {
                buttonStyle = new GUIStyle("IconButton");
            }

            if (Validate(true))
            {
                if (IsVariable())
                {
                    position.y += 2;
                }

                EditorGUI.BeginDisabledGroup(IsVariable());
                Rect propertyPosition = new Rect(position.x, position.y, position.width - 16, position.height);
                EditorGUI.PropertyField(propertyPosition, property, new GUIContent(displayName));
                EditorGUI.EndDisabledGroup();

                Rect buttonRect = new Rect(propertyPosition.x + propertyPosition.width + 1, propertyPosition.y + 1, 16, 16);
                if (GUI.Button(buttonRect, buttonContent, buttonStyle))
                {
                    DropdownVariable();
                }
            }
        }

        private void DropdownVariable()
        {
            TreeNode treeNode = targetProperty.serializedObject.targetObject as TreeNode;
            BehaviourTreeAsset tree = treeNode.GetBehaviourTree();

            System.Type[] propertyTypes = treeVariableAttribute.Types;
            if (!string.IsNullOrEmpty(treeVariableAttribute.Variable))
            {
                string variableName = targetProperty.serializedObject.FindProperty(treeVariableAttribute.Variable).stringValue;
                if (tree.TryGetVariable(variableName, out TreeVariable variable))
                {
                    propertyTypes = new System.Type[] { variable.GetValueObject().GetType().GenericTypeArguments[0] };
                }
            }

            GenericMenu menu = new GenericMenu();

            if (constProperty != null)
            {
                bool on = !toggleProperty?.boolValue ?? true;
                menu.AddItem(new GUIContent("Manual"), on, () =>
                {
                    if (variableProperty != null)
                    {
                        variableProperty.stringValue = string.Empty;
                    }
                    if (toggleProperty != null)
                    {
                        toggleProperty.boolValue = false;
                    }
                    targetProperty.serializedObject.ApplyModifiedProperties();
                });
            }

            if (constProperty != null && variableProperty != null)
            {
                for (int i = 0; i < propertyTypes.Length; i++)
                {
                    if (tree.LocalVariables.Any(n => n.Value.GetValueObject().GetType() == propertyTypes[i]))
                    {
                        menu.AddSeparator("");
                        break;
                    }
                }
            }

            {
                if (variableProperty != null)
                {
                    foreach (KeyValuePair<string, TreeVariable> variable in tree.LocalVariables)
                    {
                        if (propertyTypes == null)
                        {
                            bool on = variableProperty.stringValue == "Local/" + variable.Key;
                            menu.AddItem(new GUIContent("Local/" + variable.Key), on, () =>
                            {
                                variableProperty.stringValue = "Local/" + variable.Key;
                                if (toggleProperty != null)
                                {
                                    toggleProperty.boolValue = true;
                                }
                                targetProperty.serializedObject.ApplyModifiedProperties();
                            });
                        }
                        else
                        {
                            if (variable.Value != null)
                            {
                                System.Type variableType = variable.Value.GetValueObject().GetType();
                                for (int i = 0; i < propertyTypes.Length; i++)
                                {
                                    System.Type propertyType = propertyTypes[i];
                                    if (propertyType == null ||
                                        variableType == propertyType ||
                                        (propertyType.IsGenericType && propertyType.Name == variableType.Name && (propertyType.GenericTypeArguments.Length == 0 || (propertyType.GenericTypeArguments.Length != 0 && variableType.GenericTypeArguments[0] == propertyType.GenericTypeArguments[0]))))
                                    {
                                        bool on = variableProperty.stringValue == "Local/" + variable.Key;
                                        menu.AddItem(new GUIContent("Local/" + variable.Key), on, () =>
                                        {
                                            variableProperty.stringValue = "Local/" + variable.Key;
                                            if (toggleProperty != null)
                                            {
                                                toggleProperty.boolValue = true;
                                            }
                                            targetProperty.serializedObject.ApplyModifiedProperties();
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            {
                foreach (KeyValuePair<string, TreeVariable> variable in tree.GlobalVariables)
                {
                    if (propertyTypes == null)
                    {
                        bool on = variableProperty.stringValue == "Global/" + variable.Key;
                        menu.AddItem(new GUIContent("Global/" + variable.Key), on, () =>
                        {
                            variableProperty.stringValue = "Global/" + variable.Key;
                            if (toggleProperty != null)
                            {
                                toggleProperty.boolValue = true;
                            }
                            targetProperty.serializedObject.ApplyModifiedProperties();
                        });
                    }
                    else
                    {
                        if (variable.Value != null)
                        {
                            System.Type variableType = variable.Value.GetValueObject().GetType();
                            for (int i = 0; i < propertyTypes.Length; i++)
                            {
                                System.Type propertyType = propertyTypes[i];
                                if (propertyType == null ||
                                        variableType == propertyType ||
                                        (propertyType.IsGenericType && propertyType.Name == variableType.Name && (propertyType.GenericTypeArguments.Length == 0 || (propertyType.GenericTypeArguments.Length != 0 && variableType.GenericTypeArguments[0] == propertyType.GenericTypeArguments[0]))))
                                {
                                    bool on = variableProperty.stringValue == "Global/" + variable.Key;
                                    menu.AddItem(new GUIContent("Global/" + variable.Key), on, () =>
                                    {
                                        variableProperty.stringValue = "Global/" + variable.Key;
                                        if (toggleProperty != null)
                                        {
                                            toggleProperty.boolValue = true;
                                        }
                                        targetProperty.serializedObject.ApplyModifiedProperties();
                                    });
                                }
                            }
                        }
                    }
                }
            }

            menu.ShowAsContext();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (Validate())
            {
                return base.GetPropertyHeight(property, label);
            }
            else
            {
                return 0f;
            }
        }

        private bool IsVisible()
        {
            if (toggleProperty != null)
            {
                if (targetProperty.name.IndexOf("Variable") == -1)
                {
                    return !toggleProperty.boolValue;
                }
                else
                {
                    return toggleProperty.boolValue;
                }
            }

            return true;
        }

        private bool IsVariable()
        {
            return targetProperty.name.IndexOf("Variable") != -1;
        }

        private bool Validate(bool notify = false)
        {
            if (!IsVisible())
            {
                return false;
            }

            if (constProperty != null && variableProperty != null && toggleProperty == null)
            {
                if (notify && IsVariable())
                {
                    EditorGUILayout.HelpBox($"Create a {fieldName + "Toggle"} boolean field", MessageType.Error);
                }
                return false;
            }

            return true;
        }
    }
}