/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AuroraFPSEditor.Attributes
{
    [ViewTarget(typeof(DropdownReferenceAttribute))]
    public class DropdownReferenceView : PropertyView, IPropertyValidatorReceiver, IPropertyReceiver
    {
        private const string NullReferenceLabel = "None";

        private DropdownReferenceAttribute attribute;
        private ApexProperty apexProperty;
        private string storedReferenceLabel;

        public override void OnInitialize(SerializedProperty property, ViewAttribute viewAttribute, GUIContent label)
        {
            attribute = viewAttribute as DropdownReferenceAttribute;
            storedReferenceLabel = LoadReferenceName(property);
        }

        public void OnReceiveProperty(ApexProperty property)
        {
            apexProperty = property;
        }

        /// <summary>
        /// Called for drawing property view GUI.
        /// </summary>
        /// <param name="position">Position of the serialized property.</param>
        /// <param name="property">Serialized property with ViewAttribute.</param>
        /// <param name="label">Label of serialized property.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float standardVerticalSpacing = EditorGUIUtility.standardVerticalSpacing;

            UpdateLabel();

            Rect fixedPosition = new Rect(position.x, position.y, position.width, singleLineHeight);
            Rect popupPosition = EditorGUI.PrefixLabel(fixedPosition, label);
            FixPopupRect(ref popupPosition);
            if (GUI.Button(popupPosition, storedReferenceLabel, EditorStyles.popup))
            {
                DropdownAllReferences(property, popupPosition);
            }

            if(apexProperty.HasChildren())
            {
                Rect foldoutPosition = new Rect(fixedPosition.x, fixedPosition.y, position.width - popupPosition.width, singleLineHeight);
                property.isExpanded = attribute.FoldoutToggle ? EditorGUI.Foldout(foldoutPosition, property.isExpanded, GUIContent.none, true) : true;
                if (property.isExpanded && property.hasVisibleChildren)
                {
                    Rect childPosition = new Rect(foldoutPosition.x, foldoutPosition.yMax + standardVerticalSpacing, position.width, singleLineHeight);
                    childPosition = EditorGUI.IndentedRect(childPosition);
                    apexProperty.DrawChildren(childPosition);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            if (property.isExpanded)
            {
                height += apexProperty.GetChildrenHeight();
            }
            return height;
        }

        private void DropdownAllReferences(SerializedProperty property, Rect position)
        {
            GenericMenu genericMenu = new GenericMenu();
            Type type = ApexReflection.GetPropertyType(property);
            IEnumerable<Type> subclasses = ApexReflection.FindSubclassesOf(type);
            foreach (Type subclass in subclasses)
            {
                ReferenceContent referenceContent = subclass.GetCustomAttributes<ReferenceContent>().FirstOrDefault();
                if (referenceContent != null && !referenceContent.Hided)
                {
                    genericMenu.AddItem(new GUIContent(referenceContent.path), false, () =>
                    {
                        storedReferenceLabel = referenceContent.name;
                        property.managedReferenceValue = Activator.CreateInstance(subclass);
                        property.serializedObject.ApplyModifiedProperties();
                        apexProperty.ApplyChildren();
                    });
                }
            }

            genericMenu.AddSeparator("");
            genericMenu.AddItem(new GUIContent("Clear"), false, () =>
            {
                storedReferenceLabel = NullReferenceLabel;
                property.managedReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
                apexProperty.ApplyChildren();
            });
            genericMenu.DropDown(position);
        }

        private string LoadReferenceName(SerializedProperty property)
        {
            object reference = ApexReflection.GetObjectOfProperty(property);
            if (reference != null)
            {
                ReferenceContent referenceContent = reference.GetType().GetCustomAttributes<ReferenceContent>().FirstOrDefault();
                if (referenceContent != null)
                {
                    return referenceContent.name;
                }
            }
            return NullReferenceLabel;
        }

        private void FixPopupRect(ref Rect position)
        {
            if (apexProperty.HasChildren())
            {
                position.width += 4;
            }
        }

        private void UpdateLabel()
        {
            if(storedReferenceLabel != NullReferenceLabel && !apexProperty.HasChildren())
            {
                storedReferenceLabel = NullReferenceLabel;
            }
        }

        #region [IPropertyValidatorReceiver Implementation]
        /// <summary>
        /// Return true if this property valid the using with this attribute.
        /// If return false, this property attribute will be ignored.
        /// </summary>
        /// <param name="property">Reference of serialized property.</param>
        /// <param name="label">Display label of serialized property.</param>
        public bool IsValidProperty(SerializedProperty property, GUIContent label)
        {
            return property.propertyType == SerializedPropertyType.ManagedReference;
        }
        #endregion
    }
}