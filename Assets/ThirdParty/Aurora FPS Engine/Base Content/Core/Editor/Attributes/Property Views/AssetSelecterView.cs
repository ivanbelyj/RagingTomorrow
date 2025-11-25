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
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AuroraFPSEditor.Attributes
{
    [ViewTarget(typeof(AssetSelecterAttribute))]
    public sealed class AssetSelecterView : PropertyView
    {
        private AssetSelecterAttribute attribute;
        private Texture icon;

        public override void OnInitialize(SerializedProperty property, ViewAttribute viewAttribute, GUIContent label)
        {
            attribute = viewAttribute as AssetSelecterAttribute;
            icon = EditorGUIUtility.IconContent("align_vertically_center").image;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float lastLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = lastLabelWidth + 15;

            EditorGUI.PropertyField(position, property, label);

            EditorGUI.BeginDisabledGroup(!Directory.Exists(attribute.Path));
            Rect iconPosition = new Rect(position.x + (EditorGUIUtility.labelWidth - 15), position.y + 1.5f, 20, 20);
            if (GUI.Button(iconPosition, icon, "IconButton"))
            {
                Type propertyType = attribute.AssetType == null ? ApexReflection.GetPropertyType(property) : attribute.AssetType;
                SearchableMenu searchableMenu = new SearchableMenu();
                string[] paths = Directory.GetFiles(attribute.Path, "*.*", attribute.Search);
                for (int i = 0; i < paths.Length; i++)
                {
                    string currentPath = paths[i];
                    if (Path.GetExtension(currentPath) != ".meta")
                    {
                        Object asset = AssetDatabase.LoadAssetAtPath(currentPath, propertyType);
                        if (asset != null)
                        {
                            searchableMenu.AddItem(new GUIContent(asset.name), true, () =>
                            {
                                property.objectReferenceValue = asset;
                                property.serializedObject.ApplyModifiedProperties();
                            });
                        }
                    }
                }

                Rect menuPosition = new Rect(iconPosition.x, iconPosition.y, position.width, position.height);
                searchableMenu.ShowAsDropdown(menuPosition, new Vector2(position.width - (EditorGUIUtility.labelWidth - 15), 250));
            }
            EditorGUI.EndDisabledGroup();
            EditorGUIUtility.labelWidth = lastLabelWidth;
        }
    }
}