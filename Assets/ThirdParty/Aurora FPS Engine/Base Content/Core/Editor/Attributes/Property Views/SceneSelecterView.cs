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
using System.Collections.Generic;

namespace AuroraFPSEditor.Attributes
{
    [ViewTarget(typeof(SceneSelecterAttribute))]
    public sealed class SceneSelecterView : PropertyView
    {
        internal struct SceneInfo
        {
            public int id;
            public string name;

            public SceneInfo(int id, string name)
            {
                this.id = id;
                this.name = name;
            }
        }

        private List<SceneInfo> scenes;
        private string currentSceneName;

        public override void OnInitialize(SerializedProperty property, ViewAttribute viewAttribute, GUIContent label)
        {
            currentSceneName = "None";
            if (EditorBuildSettings.scenes.Length > 0)
            {
                scenes = new List<SceneInfo>(EditorBuildSettings.scenes.Length);
                for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
                {
                    EditorBuildSettingsScene buildSettingsScene = EditorBuildSettings.scenes[i];
                    SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(buildSettingsScene.path);
                    if(sceneAsset != null)
                    {
                        SceneInfo sceneInfo = new SceneInfo(i, sceneAsset.name);
                        scenes.Add(sceneInfo);
                        switch (property.propertyType)
                        {
                            case SerializedPropertyType.Integer:
                                if (property.intValue == sceneInfo.id)
                                {
                                    currentSceneName = sceneInfo.name;
                                }
                                break;
                            case SerializedPropertyType.String:
                                if (property.stringValue == sceneInfo.name)
                                {
                                    currentSceneName = sceneInfo.name;
                                }
                                break;
                        }
                    }
                }
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);

            if (GUI.Button(position, currentSceneName, EditorStyles.popup))
            {
                if (scenes != null && scenes.Count > 0)
                {
                    SearchableMenu searchableMenu = new SearchableMenu();
                    searchableMenu.AddItem(new GUIContent(string.Format("{0} ({1})", "None", -1)), true, () =>
                    {
                        currentSceneName = "None";
                        switch (property.propertyType)
                        {
                            case SerializedPropertyType.Integer:
                                    property.intValue = -1;
                                break;
                            case SerializedPropertyType.String:
                                    property.stringValue = string.Empty;
                                break;
                        }
                        property.serializedObject.ApplyModifiedProperties();
                    });
                    for (int i = 0; i < scenes.Count; i++)
                    {
                        SceneInfo scene = scenes[i];
                        switch (property.propertyType)
                        {
                            case SerializedPropertyType.Integer:
                                searchableMenu.AddItem(new GUIContent(string.Format("{0} ({1})", scene.name, scene.id)), true, () =>
                                {
                                    currentSceneName = scene.name;
                                    property.intValue = scene.id;
                                    property.serializedObject.ApplyModifiedProperties();
                                });

                                break;
                            case SerializedPropertyType.String:
                                searchableMenu.AddItem(new GUIContent(string.Format("{0} ({1})", scene.name, scene.id)), true, () =>
                                {
                                    currentSceneName = scene.name;
                                    property.stringValue = scene.name;
                                    property.serializedObject.ApplyModifiedProperties();
                                });
                                break;
                        }
                    }
                    searchableMenu.ShowAsDropdown(position, new Vector2(position.width, 250));
                }
            }
        }
    }
}