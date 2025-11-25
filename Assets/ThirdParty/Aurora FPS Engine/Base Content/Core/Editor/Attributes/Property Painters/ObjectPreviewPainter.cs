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

namespace AuroraFPSEditor.Attributes
{
    [PainterTarget(typeof(ObjectPreviewAttribute))]
    public sealed class ObjectPreviewPainter : PropertyPainter, IPropertyValidatorReceiver, IPropertyPositionModifyReceiver
    {
        private readonly static Padding FoldoutButtonPadding = new Padding(0.0f, 0.0f, 0.0f, 0.0f);

        private const float Space = 2.0f;
        private const float SpaceAfter = 2.0f;
        private const float BorderWidth = 1.0f;

        private ObjectPreviewAttribute attribute;
        private Editor objectEditor;
        private int currentInstanceID;
        private float previousLabelWidth;
        private bool hasPreview;

        public override void OnInitialize(SerializedProperty property, PainterAttribute painterAttribute, GUIContent label)
        {
            attribute = painterAttribute as ObjectPreviewAttribute;
            hasPreview = true;
            property.isExpanded = !attribute.Expandable;
            if (objectEditor != null)
                objectEditor = Editor.CreateEditor(property.objectReferenceValue);
        }

        public void ModifyPropertyPosition(ref Rect position)
        {
            if (attribute.Expandable && objectEditor != null)
            {
                previousLabelWidth = EditorGUIUtility.labelWidth;
                position = FoldoutButtonPadding.PaddingRect(position);
                EditorGUIUtility.labelWidth -= FoldoutButtonPadding.left;
            }
        }

        /// <summary>
        /// Called for drawing GUI elements.
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the painter GUI.</param>
        /// <param name="property">Reference of serialized property painter attribute.</param>
        /// <param name="label">Display label of serialized property.</param>
        public override void OnPainterGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (objectEditor == null && property.objectReferenceValue != null || (GUI.changed && property.objectReferenceValue != null && currentInstanceID != property.objectReferenceValue.GetInstanceID()))
            {
                objectEditor = Editor.CreateEditor(property.objectReferenceValue);
                currentInstanceID = property.objectReferenceValue.GetInstanceID();

                GameObject gameObject = property.objectReferenceValue as GameObject;
                if (gameObject.GetComponent<Terrain>())
                {
                    hasPreview = false;
                }
            }
            else if (objectEditor != null && property.objectReferenceValue == null)
            {
                objectEditor = null;
            }


            Rect propertyPosition = EditorGUI.IndentedRect(GetPropertyPosition());
            if (attribute.Expandable && objectEditor != null)
            {
                Rect foldoutPosition = new Rect(position.x, propertyPosition.y, 5, propertyPosition.height);
                property.isExpanded = EditorGUI.Foldout(foldoutPosition, property.isExpanded, GUIContent.none, true);
            }

            if (property.isExpanded && objectEditor != null)
            {
                Rect previewRect = new Rect(propertyPosition.x, position.y + Space, propertyPosition.width - BorderWidth, position.height);
                if (hasPreview)
                {
                    objectEditor.DrawPreview(previewRect);
                }
                else
                {
                    Rect messagePosition = new Rect(previewRect.x, previewRect.y + (EditorGUIUtility.singleLineHeight / 2.25f), previewRect.width, EditorGUIUtility.singleLineHeight);
                    GUI.Label(messagePosition, "This object does not support preview...", EditorStyles.centeredGreyMiniLabel);
                }

                Rect topBorder = new Rect(previewRect.x, previewRect.y, previewRect.width, BorderWidth);
                EditorGUI.DrawRect(topBorder, Color.black);

                Rect bottomBorder = new Rect(previewRect.x, previewRect.y + previewRect.height, previewRect.width, BorderWidth);
                EditorGUI.DrawRect(bottomBorder, Color.black);

                Rect leftBorder = new Rect(previewRect.x, previewRect.y, BorderWidth, previewRect.height);
                EditorGUI.DrawRect(leftBorder, Color.black);

                Rect rightBorder = new Rect(previewRect.x + previewRect.width, previewRect.y, BorderWidth, previewRect.height);
                EditorGUI.DrawRect(rightBorder, Color.black);
            }
            EditorGUIUtility.labelWidth = previousLabelWidth;
        }

        public override float GetPainterHeight(SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue != null)
            {
                if (attribute.Expandable && property.isExpanded)
                {
                    return attribute.Height + Space + 1;
                }
                else if (!attribute.Expandable)
                {
                    return attribute.Height + Space + 1 + SpaceAfter;
                }
            }
            return 0;
        }

        public bool IsValidProperty(SerializedProperty property, GUIContent label)
        {
            return property.propertyType == SerializedPropertyType.ObjectReference;
        }
    }
}