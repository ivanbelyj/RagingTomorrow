/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEngine;
using System;

#region [Unity Editor Section]
#if UNITY_EDITOR
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;
using Object = UnityEngine.Object;
#endif
#endregion

namespace AuroraFPSRuntime.WeaponModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/Weapon Modules/Other/Weapon Effects System (Deprecated)")]
    [DisallowMultipleComponent]
    [Obsolete("Weapon Effects System is deprecated.\nUse Procedural Animator instead.")]
    public sealed class WeaponEffectsSystem : MonoBehaviour
    {
        [SerializeReference]
        [ReorderableList(
            OnDropdownButtonCallback = "OnAddEffect", 
            GetElementLabelCallback = "GetEffectLabel", 
            OnRemoveCallbackCallback = "OnRemoveEffect",
            NoneElementLabel = "Add new effects...")]
        private WeaponEffect[] effects;

        private void Awake()
        {
            for (int i = 0; i < effects.Length; i++)
            {
                effects[i].Initialize(transform);
            }
        }

        private void OnEnable()
        {
            for (int i = 0; i < effects.Length; i++)
            {
                effects[i].OnEnable();
            }
        }

        private void Update()
        {
            for (int i = 0; i < effects.Length; i++)
            {
                effects[i].OnUpdate();
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < effects.Length; i++)
            {
                effects[i].OnDisable();
            }
        }

        #region [Unity Editor Section]
#if UNITY_EDITOR
        private void OnAddEffect(Rect position, SerializedProperty property)
        {
            GenericMenu genericMenu = new GenericMenu();
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            IEnumerable<Type> effectTypes = types.Where(t => t.IsSubclassOf(typeof(WeaponEffect)));
            foreach (Type type in effectTypes)
            {
                IEnumerable<WeaponEffectMenu> attributes = type.GetCustomAttributes<WeaponEffectMenu>();
                foreach (WeaponEffectMenu attribute in attributes)
                {
                    GUIContent content = new GUIContent(attribute.Path);
                    genericMenu.AddItem(content, false, () =>
                    {
                        int index = property.arraySize;
                        Transform hinge = CreateCameraEffectHinge(string.Format("{0} [Effect Hinge]", attribute.Name));
                        if(hinge != null)
                        {
                            property.arraySize++;
                            SerializedProperty element = property.GetArrayElementAtIndex(index);
                            element.managedReferenceValue = Activator.CreateInstance(type);
                            element.FindPropertyRelative("hinge").objectReferenceValue = hinge;
                            property.serializedObject.ApplyModifiedProperties();
                        }
                    });
                    break;
                }
            }
            genericMenu.DropDown(position);
        }

        private void OnRemoveEffect(SerializedProperty property, int index)
        {
            SerializedProperty element = property.GetArrayElementAtIndex(index);
            Transform hinge = element.FindPropertyRelative("hinge").objectReferenceValue as Transform;
            if(hinge != null)
            {
                RemoveCameraEffectHinge(hinge);
            }
            property.DeleteArrayElementAtIndex(index);
            property.serializedObject.ApplyModifiedProperties();
        }

        private string GetEffectLabel(SerializedProperty property, int index)
        {
            string[] baseTypeAndAssemblyName = property.managedReferenceFullTypename.Split(' ');
            Type type = Type.GetType(baseTypeAndAssemblyName[1]);
            WeaponEffectMenu attribute = type.GetCustomAttribute<WeaponEffectMenu>();
            if (attribute != null)
            {
                return attribute.Name;
            }
            else
            {
                return "Effect " + index;
            }
        }

        private Transform CreateCameraEffectHinge(string name)
        {
            GameObject ownerObject = gameObject;
            if (PrefabUtility.IsPartOfAnyPrefab(ownerObject))
            {
                PrefabStage stage = PrefabStageUtility.GetPrefabStage(ownerObject);
                if (stage != null)
                {
                    ownerObject = stage.prefabContentsRoot;
                }
                else
                {
                    if (EditorUtility.DisplayDialog("Aurora FPS Engine: Cannot restructure Prefab instance",
                        "This action need to restructure Prefab instance.\n\n" +
                        "You can open Prefab in Prefab Mode to restructure the Prefab Asset itself, or unpack the Prefab instance to remove its Prefab connection.",
                        "Open Prefab",
                        "Cancel"))
                    {
                        string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(ownerObject);
                        GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                        AssetDatabase.OpenAsset(asset);
                    }
                    return null;
                }
            }

            GameObject hingeObject = new GameObject(name);
            StageUtility.PlaceGameObjectInCurrentStage(hingeObject);

            Transform hinge = hingeObject.transform;
            hinge.localPosition = Vector3.zero;
            hinge.localRotation = Quaternion.identity;
            hinge.localScale = Vector3.one;
            hinge.SetParent(ownerObject.transform);
            hinge.SetAsFirstSibling();

            hingeObject.layer = ownerObject.layer;

            List<Transform> copyChildren = new List<Transform>(ownerObject.transform.childCount - 1);
            for (int i = 1; i < ownerObject.transform.childCount; i++)
            {
                copyChildren.Add(ownerObject.transform.GetChild(i));
            }

            for (int i = 0; i < copyChildren.Count; i++)
            {
                copyChildren[i].SetParent(hinge);
            }

            return hinge;
        }

        private static void RemoveCameraEffectHinge(Transform target)
        {
            GameObject rootObject = target.root.gameObject;
            if (PrefabUtility.IsPartOfAnyPrefab(rootObject))
            {
                PrefabStage stage = PrefabStageUtility.GetPrefabStage(rootObject);
                if (stage != null)
                {
                    rootObject = stage.prefabContentsRoot;
                }
                else
                {
                    if (EditorUtility.DisplayDialog("Aurora FPS Engine: Cannot restructure Prefab instance",
                        "This action need to restructure Prefab instance.\n\n" +
                        "You can open Prefab in Prefab Mode to restructure the Prefab Asset itself, or unpack the Prefab instance to remove its Prefab connection.",
                        "Open Prefab",
                        "Cancel"))
                    {
                        string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(rootObject);
                        GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                        AssetDatabase.OpenAsset(asset);
                    }
                    return;
                }
            }

            Transform parent = target.parent;
            List<Transform> copyChildren = new List<Transform>(target.childCount);
            for (int i = 0; i < target.childCount; i++)
            {
                Transform child = target.GetChild(i);
                copyChildren.Add(child);
            }

            for (int i = 0; i < copyChildren.Count; i++)
            {
                Transform child = copyChildren[i];
                child.SetParent(parent);
            }

            Object.DestroyImmediate(target.gameObject);
        }
#endif
        #endregion
    }
}