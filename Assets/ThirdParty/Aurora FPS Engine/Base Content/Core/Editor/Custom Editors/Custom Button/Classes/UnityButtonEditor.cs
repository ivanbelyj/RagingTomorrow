/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSEditor.Attributes;
using AuroraFPSRuntime.UIModules.UIElements;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UI;

namespace AuroraFPSEditor
{
    [CustomEditor(typeof(UnityButton), true)]
    [CanEditMultipleObjects]
    public sealed class UnityButtonEditor : SelectableEditor
    {
        private ApexProperty onClick;
        private List<ApexSerializedField> properties;

        protected override void OnEnable()
        {
            base.OnEnable();

            onClick = new ApexProperty(serializedObject.FindProperty("onClick"));

            List<SerializedProperty> declaredProperties = new List<SerializedProperty>();
            using (SerializedProperty iterator = serializedObject.GetIterator())
            {
                if (iterator.NextVisible(true))
                {
                    do
                    {
                        string propertyPath = iterator.propertyPath;
                        FieldInfo fieldInfo = target.GetType().GetField(propertyPath, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.CreateInstance);
                        if(fieldInfo != null)
                        {
                            declaredProperties.Add(iterator.Copy());
                        }
                    }
                    while (iterator.NextVisible(false));
                }
            }

            if(declaredProperties.Count > 0)
            {
                ApexEditor.CreateApexSerializedField(declaredProperties, out properties);
                ApexEditor.OrderSerializedProperties(ref properties);
                ApexEditor.LayoutApexProperties(ref properties);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if (properties != null && properties.Count > 0)
            {
                for (int i = 0; i < properties.Count; i++)
                {
                    ApexSerializedField apexSerializedField = properties[i];
                    if (apexSerializedField.IsVisible())
                    {
                        properties[i].DrawFieldLayout();
                    }
                }
            }
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();

            serializedObject.Update();
            EditorGUILayout.Space();
            onClick.DrawFieldLayout();
            serializedObject.ApplyModifiedProperties();
        }
    }
}