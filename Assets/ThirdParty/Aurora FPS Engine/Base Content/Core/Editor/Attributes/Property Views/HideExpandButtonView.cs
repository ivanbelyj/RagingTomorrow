/* ==================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AuroraFPSEditor.Attributes
{
    [ViewTarget(typeof(HideExpandButtonAttribute))]
    internal sealed class HideExpandButtonView : PropertyView
    {
        private List<ApexSerializedField> children;
        private List<ApexField> buttons;

        public override void OnInitialize(SerializedProperty property, ViewAttribute viewAttribute, GUIContent label)
        {
            List<SerializedProperty> copyChildren = property.CopyVisibleChildren();
            ApexEditor.CreateApexSerializedField(copyChildren, out children);
            ApexEditor.LayoutApexProperties(ref children);
            ApexEditor.CreateApexButtons(property.serializedObject.targetObject, out buttons);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(children != null && children.Count > 0)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    ApexField child = children[i];

                    if (child.IsVisible())
                    {
                        Rect childPosition = new Rect(position.x, position.y, position.width, child.GetFieldHeight());
                        child.DrawField(childPosition);
                        position.y += childPosition.height + EditorGUIUtility.standardVerticalSpacing;
                    }
                }
            }

            if (buttons != null && buttons.Count > 0)
            {
                for (int i = 0; i < buttons.Count; i++)
                {
                    ApexField child = buttons[i];

                    Rect childPosition = new Rect(position.x, position.y, position.width, child.GetFieldHeight());
                    child.DrawField(childPosition);
                    position.y += childPosition.height + EditorGUIUtility.standardVerticalSpacing;
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = 0;
            if (children != null && children.Count > 0)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    ApexField child = children[i];
                    if (child.IsVisible())
                    {
                        height += child.GetFieldHeight() + EditorGUIUtility.standardVerticalSpacing;
                    }
                }
            }

            if (buttons != null && buttons.Count > 0)
            {
                for (int i = 0; i < buttons.Count; i++)
                {
                    height += buttons[i].GetFieldHeight() + EditorGUIUtility.standardVerticalSpacing;
                }
            }
            return height;
        }
    }
}