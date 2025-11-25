/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using System.Collections.Generic;
using UnityEngine;

#region [Unity Editor Section]
#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
#endif
#endregion

namespace AuroraFPSRuntime.WeaponModules.EffectSystem
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/Weapon Modules/Other/Weapon Effect System")]
    [DisallowMultipleComponent]
    public sealed class WeaponEffectSystem : MonoBehaviour
    {
        [SerializeReference]
        [ReorderableList(
            OnDropdownButtonCallback = "OnAddEffect",
            GetElementLabelCallback = "GetEffectLabel",
            NoneElementLabel = "Add new effect...")]
        private List<Effect> effects = new List<Effect>();

        /// <summary>
        /// Called when the animator instance is being loaded.
        /// </summary>
        private void Awake()
        {
            for (int i = 0; i < effects.Count; i++)
            {
                effects[i].Initialize(transform);
            }
        }

        /// <summary>
        /// Called when the animator becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            for (int i = 0; i < effects.Count; i++)
            {
                effects[i].OnEnable();
            }
        }

        /// <summary>
        /// Called on the frame when a Behaviour is enabled,
        /// just before any of the Update methods are called the first time.
        /// </summary>
        private void Start()
        {
            for (int i = 0; i < effects.Count; i++)
            {
                effects[i].Start();
            }
        }

        /// <summary>
        /// Called every frame, if the animator is enabled.
        /// </summary>
        private void Update()
        {
            for (int i = 0; i < effects.Count; i++)
            {
                effects[i].OnAnimationUpdate();
            }
        }

        /// <summary>
        /// Called when the behaviour becomes disabled.
        /// </summary>
        private void OnDisable()
        {
            for (int i = 0; i < effects.Count; i++)
            {
                effects[i].OnAnimationUpdate();
            }
        }

        /// <summary>
        /// Destroying the attached Behaviour will result in the game or Scene receiving OnDestroy.
        /// </summary>
        private void OnDestroy()
        {
            for (int i = 0; i < effects.Count; i++)
            {
                effects[i].OnDestroy();
            }
        }

        #region [Unity Editor Section]
#if UNITY_EDITOR
        private void OnAddEffect(Rect position, SerializedProperty property)
        {
            GenericMenu genericMenu = new GenericMenu();
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            IEnumerable<Type> effectTypes = types.Where(t => t.IsSubclassOf(typeof(Effect)));
            foreach (Type type in effectTypes)
            {
                IEnumerable<ReferenceContent> attributes = type.GetCustomAttributes<ReferenceContent>();
                foreach (ReferenceContent attribute in attributes)
                {
                    GUIContent content = new GUIContent(attribute.path);
                    genericMenu.AddItem(content, false, () =>
                    {
                        int index = property.arraySize;
                        property.arraySize++;
                        SerializedProperty element = property.GetArrayElementAtIndex(index);
                        element.managedReferenceValue = Activator.CreateInstance(type);
                        property.serializedObject.ApplyModifiedProperties();
                    });
                    break;
                }
            }
            genericMenu.DropDown(position);
        }

        private string GetEffectLabel(SerializedProperty property, int index)
        {
            string[] baseTypeAndAssemblyName = property.managedReferenceFullTypename.Split(' ');
            Type type = Type.GetType(baseTypeAndAssemblyName[1]);
            ReferenceContent attribute = type.GetCustomAttribute<ReferenceContent>();
            if (attribute != null)
            {
                return attribute.name;
            }
            else
            {
                return "Animation " + index;
            }
        }
#endif
        #endregion

        #region [Getter / Setter]
        /// <summary>
        /// Add new procedural effect.
        /// </summary>
        /// <param name="effect"></param>
        public void AddEffect(Effect effect)
        {
            effect.Initialize(transform);
            effect.OnEnable();
            effect.Start();
            effects.Add(effect);
        }

        /// <summary>
        /// Remove effect by specified reference.
        /// </summary>
        /// <param name="effect">Reference of effect.</param>
        public void RemoveEffect(Effect effect)
        {
            if (effects.Contains(effect))
            {
                effect.OnRemove();
                effects.Remove(effect);
            }
        }

        /// <summary>
        /// Remove effect by specified index.
        /// </summary>
        /// <param name="index">Index of effect.</param>
        public void RemoveEffect(int index)
        {
            if (index >= 0 && effects.Count < index)
            {
                Effect effect = effects[index];
                effect.OnRemove();
                effects.RemoveAt(index);
            }
        }
        #endregion
    }
}