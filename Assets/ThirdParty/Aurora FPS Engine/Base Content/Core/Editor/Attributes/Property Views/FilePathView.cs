/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEditor;
using UnityEngine;
using FilePathAttribute = AuroraFPSRuntime.Attributes.FilePathAttribute;

namespace AuroraFPSEditor.Attributes
{
    [ViewTarget(typeof(FilePathAttribute))]
    public sealed class FilePathView : PropertyView, IPropertyValidatorReceiver
    {
        private FilePathAttribute attribute;
        private Texture icon;

        /// <summary>
        /// Called once when initializing PropertyView.
        /// </summary>
        /// <param name="property">Serialized property with ViewAttribute.</param>
        /// <param name="viewAttribute">ViewAttribute of serialized property.</param>
        /// <param name="label">Label of serialized property.</param>
        public override void OnInitialize(SerializedProperty property, ViewAttribute viewAttribute, GUIContent label)
        {
            attribute = viewAttribute as FilePathAttribute;
            icon = EditorGUIUtility.IconContent("Folder Icon").image;
        }

        /// <summary>
        /// Called every inspector update to draw property.
        /// </summary>
        /// <param name="position">Position of the serialized property.</param>
        /// <param name="property">Serialized property with ViewAttribute.</param>
        /// <param name="label">Label of serialized property.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth += 17.0f;
            EditorGUI.PropertyField(position, property, label);

            Rect iconPosition = new Rect(position.x + (previousLabelWidth + 2), position.y + 1, 25, 25);
            if(GUI.Button(iconPosition, icon, "IconButton"))
            {
                string selectedPath = EditorUtility.OpenFilePanel(attribute.Title, attribute.Directory, attribute.Extension);
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    if (attribute.RelativePath && selectedPath.Contains("Assets"))
                    {
                        selectedPath = selectedPath.Remove(0, selectedPath.IndexOf("Assets"));
                    }
                    property.stringValue = selectedPath;
                }
            }
            EditorGUIUtility.labelWidth = previousLabelWidth;
        }

        public bool IsValidProperty(SerializedProperty property, GUIContent label)
        {
            return property.propertyType == SerializedPropertyType.String;
        }
    }
}