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
using UnityEditorInternal;
using UnityEngine;

namespace AuroraFPSEditor.Attributes
{
    [ViewTarget(typeof(TagPopupAttribute))]
    public sealed class TagPopupView : PropertyView, IPropertyValidatorReceiver
    {
        public override void OnInitialize(SerializedProperty property, ViewAttribute viewAttribute, GUIContent label)
        {
            ConvertPropertyToTag(property);
        }

        /// <summary>
        /// Called every inspector update to draw property.
        /// </summary>
        /// <param name="position">Position of the serialized property.</param>
        /// <param name="property">Serialized property with ViewAttribute.</param>
        /// <param name="label">Label of serialized property.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);

            string currentTag = !string.IsNullOrEmpty(property.stringValue) ? property.stringValue : "None";
            if (GUI.Button(position, currentTag, EditorStyles.popup))
            {
                SearchableMenu searchableMenu = new SearchableMenu();
                for (int i = 0; i < InternalEditorUtility.tags.Length; i++)
                {
                    string tag = InternalEditorUtility.tags[i];
                    searchableMenu.AddItem(new GUIContent(tag), true, () =>
                    {
                        property.stringValue = tag;
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }
                searchableMenu.ShowAsDropdown(position, new Vector2(position.width, 250));
            }
        }

        public void ConvertPropertyToTag(SerializedProperty property)
        {
            for (int i = 0; i < InternalEditorUtility.tags.Length; i++)
            {
                if (property.stringValue == InternalEditorUtility.tags[i])
                {
                    return;
                }
            }
            property.stringValue = "None";
            property.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Return true if this property valid the using with this attribute.
        /// If return false, this property attribute will be ignored.
        /// </summary>
        /// <param name="property">Reference of serialized property decorator attribute.</param>
        /// <param name="label">Display label of serialized property.</param>
        public bool IsValidProperty(SerializedProperty property, GUIContent label)
        {
            return property.propertyType == SerializedPropertyType.String;
        }
    }
}