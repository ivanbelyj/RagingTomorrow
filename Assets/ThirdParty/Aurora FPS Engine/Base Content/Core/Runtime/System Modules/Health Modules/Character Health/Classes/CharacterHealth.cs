/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Coroutines;
using UnityEngine;

#region [Unity Editor Section]
#if UNITY_EDITOR
using UnityEditor;
using System;
using System.Reflection;
using System.Linq;
#endif
#endregion

namespace AuroraFPSRuntime.SystemModules.HealthModules
{
    [HideScriptField]
    [AddComponentMenu(null)]
    [DisallowMultipleComponent]
    public abstract class CharacterHealth : ObjectHealth
    {
        [SerializeReference]
        [Foldout("Custom Effects", Style = "Header")]
        [ReorderableList(DisplayHeader = false, 
            GetElementLabelCallback = "GetHealthEffectsLabelCallback", 
            OnDropdownButtonCallback = "OnAddHealthEffectsCallback",
            NoneElementLabel = "Add new effects...")]
        [Order(100)]
        private HealthEffect[] healthEffect;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// Called on the frame when a script is enabled,
        /// just before any of the Update methods are called the first time.
        /// </summary>
        protected virtual void Start()
        {
            InitializeHealthEffects();
        }

        protected virtual void InitializeHealthEffects()
        {
            for (int i = 0; i < healthEffect.Length; i++)
            {
                healthEffect[i].Initialization(this);
            }
        }

        #region [Unity Editor Section]
#if UNITY_EDITOR
        private string GetHealthEffectsLabelCallback(SerializedProperty property, int index)
        {
            string[] baseTypeAndAssemblyName = property.managedReferenceFullTypename.Split(' ');
            Type type = Type.GetType(baseTypeAndAssemblyName[1]);
            HealthFunctionMenu menuAttribute = type.GetCustomAttribute<HealthFunctionMenu>();
            if (menuAttribute != null)
            {
                return menuAttribute.name;
            }
            else
            {
                return "Function " + index;
            }
        }

        private void OnAddHealthEffectsCallback(Rect position, SerializedProperty property)
        {
            Assembly assembly = typeof(HealthEffect).Assembly;
            Type[] assemblyTypes = assembly.GetTypes();
            Type[] functionTypes = assemblyTypes.Where(t => t.IsSubclassOf(typeof(HealthEffect))).ToArray();

            GenericMenu genericMenu = new GenericMenu();
            for (int i = 0; i < functionTypes.Length; i++)
            {
                Type functionType = functionTypes[i];
                HealthFunctionMenu menuAttribute = functionType.GetCustomAttribute<HealthFunctionMenu>();
                if (menuAttribute != null)
                {
                    genericMenu.AddItem(new GUIContent(menuAttribute.path), false, () =>
                    {
                        int index = property.arraySize;
                        property.arraySize++;
                        property.GetArrayElementAtIndex(index).managedReferenceValue = Activator.CreateInstance(functionType);
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }
            }
            genericMenu.DropDown(position);
        }

#endif
        #endregion

        #region [Getter / Setter]
        public HealthEffect[] GetHealthEffects()
        {
            return healthEffect;
        }

        public void SetHealthEffects(HealthEffect[] value)
        {
            healthEffect = value;
        }

        public void ClearHealthEffects()
        {
            healthEffect = null;
        }
        #endregion
    }
}