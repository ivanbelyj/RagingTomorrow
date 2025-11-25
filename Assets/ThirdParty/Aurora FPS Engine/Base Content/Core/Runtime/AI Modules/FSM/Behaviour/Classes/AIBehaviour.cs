/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.AIModules.Conditions;
using AuroraFPSRuntime.Attributes;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using Transition = AuroraFPSRuntime.AIModules.Transitions.Transition;

#region [Unity Editor Section]
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
#endregion

namespace AuroraFPSRuntime.AIModules.Behaviour
{
    [Serializable]
    public abstract class AIBehaviour : IBehaviourOwner, IBehaviourTransitions
    {
        [SerializeField]
        [Foldout("Event Callbacks")]
        [Order(997)]
        private UnityEvent onEnableEvent;

        [SerializeField]
        [Foldout("Event Callbacks")]
        [Order(998)]
        private UnityEvent onDisableEvent;

        [SerializeField]
        [Array(GetElementLabelCallback = "GetTransitionLabelCallback")]
        [Order(999)]
        private List<Transition> transitions;

        // Stored required properties.
        protected AIController owner;

        /// <summary>
        /// Register this behaviour to AIController owner.
        /// </summary>
        public void RegisterBehaviour(AIController owner)
        {
            this.owner = owner;
            InitiailzeTrasitions(ref transitions, owner);
            OnInitialize();
        }

        /// <summary>
        /// Initialize all transition conditions of this AIController.
        /// </summary>
        protected void InitiailzeTrasitions(ref List<Transition> transitions, AIController owner)
        {
            for (int i = 0; i < transitions.Count; i++)
            {
                transitions[i].Internal_Initialization(owner);
            }
        }

        /// <summary>
        /// Called when AIController owner instance is being loaded.
        /// </summary>
        protected virtual void OnInitialize()
        {

        }

        /// <summary>
        /// Called when this behaviour becomes enabled.
        /// </summary>
        protected virtual void OnEnable()
        {

        }

        /// <summary>
        /// Called every frame, while this behaviour is running.
        /// </summary>
        protected abstract void Update();

        /// <summary>
        /// Called when this behaviour becomes disabled.
        /// </summary>
        protected virtual void OnDisable()
        {

        }

        #region [IBehaviourOwner Implementation]
        public AIController GetOwner()
        {
            return owner;
        }
        #endregion

        #region [IBehaviourTransitions Implementation]
        public void AddTransition(Transition transition)
        {
            transitions.Add(transition);
        }

        public bool RemoveTransition(Transition transition)
        {
            return transitions.Remove(transition);
        }

        public void RemoveTransition(int index)
        {
            transitions.RemoveAt(index);
        }

        public void ClearTransitions()
        {
            transitions.Clear();
        }
        #endregion

        #region [Internal Callbacks]
        internal void Internal_EnableBehaviour()
        {
            for (int i = 0; i < transitions.Count; i++)
            {
                transitions[i].Internal_Enable();
            }
            onEnableEvent.Invoke();
            OnEnable();
        }

        internal void Internal_DisableBehaviour()
        {
            for (int i = 0; i < transitions.Count; i++)
            {
                transitions[i].Internal_Disable();
            }
            onDisableEvent.Invoke();
            OnDisable();
        }

        internal void Internal_UpdateBehaviour()
        {
            Update();
        }

        internal void Internal_CheckTrasition()
        {
            for (int i = 0; i < transitions.Count; i++)
            {
                Transition transition = transitions[i];
                if (transition.IsComplete())
                {
                    owner.SwitchBehaviour(transition.GetTargetBehaviour());
                }
            }
        }
        #endregion

        #region [Unity Editor Section]
#if UNITY_EDITOR
        private List<ReorderableList> reorderableLists;

        private void OnTransitionInitialization(SerializedProperty property, GUIContent label)
        {
            reorderableLists = new List<ReorderableList>(property.arraySize);
            for (int i = 0; i < property.arraySize; i++)
            {
                SerializedProperty serializedTransition = property.GetArrayElementAtIndex(i);
                SerializedProperty serializedTargetBehaviour = serializedTransition.FindPropertyRelative("targetBehaviour");
                SerializedProperty serializedConditions = serializedTransition.FindPropertyRelative("conditions");
                ReorderableList reorderableList = new ReorderableList(property.serializedObject, serializedConditions, true, true, true, true);

                reorderableList.drawHeaderCallback = (rect) =>
                {
                    rect = EditorGUI.PrefixLabel(rect, new GUIContent("Target Behaviour"));
                    rect.height -= 2;
                    rect.y += 1;
                    rect.width += 3;
                    serializedTargetBehaviour.stringValue = EditorGUI.TextField(rect, serializedTargetBehaviour.stringValue);
                };

                reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    SerializedProperty serializedCondition = serializedConditions.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(rect, serializedCondition, true);
                };

                reorderableList.onAddDropdownCallback = (rect, list) =>
                {
                    GenericMenu genericMenu = new GenericMenu();
                    Assembly assembly = typeof(Condition).Assembly;
                    Type[] types = assembly.GetTypes();
                    IEnumerable<Type> subclasses = types.Where(t => t.IsSubclassOf(typeof(Condition)));
                    foreach (var type in types)
                    {
                        ConditionMenuAttribute attribute = type.GetCustomAttribute<ConditionMenuAttribute>();
                        if (attribute != null)
                        {
                            genericMenu.AddItem(new GUIContent(attribute.Path), false, () =>
                            {
                                SerializedObject serializedObject = serializedTransition.serializedObject;
                                int index = serializedConditions.arraySize;
                                serializedConditions.arraySize++;
                                serializedConditions.GetArrayElementAtIndex(index).managedReferenceValue = Activator.CreateInstance(type);
                                serializedObject.ApplyModifiedProperties();
                                OnTransitionInitialization(property, label);
                            });
                        }
                    }
                    genericMenu.DropDown(rect);
                };
                reorderableLists.Add(reorderableList);
            }
        }

        private void OnTransitionGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect listPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            for (int i = 0; i < reorderableLists.Count; i++)
            {
                reorderableLists[i].DoList(listPosition);
                listPosition.y += reorderableLists[i].GetHeight();
            }

            Rect buttonPosition = new Rect(listPosition.x, listPosition.yMax, 50, 20);
            if(GUI.Button(buttonPosition, "Add"))
            {
                property.arraySize++;
                property.serializedObject.ApplyModifiedProperties();
                OnTransitionInitialization(property, label);
            }
        }

        private float GetTransitionHeight(SerializedProperty property, GUIContent label)
        {
            float height = 0;
            for (int i = 0; i < reorderableLists.Count; i++)
            {
                height += reorderableLists[i].GetHeight();
            }
            return height;
        }

        private string GetTransitionLabelCallback(SerializedProperty property, int index)
        {
            SerializedProperty transition = property.GetArrayElementAtIndex(index);
            string targetBehaviour = transition.FindPropertyRelative("targetBehaviour").stringValue;
            bool muted = transition.FindPropertyRelative("mute").boolValue;
            if (string.IsNullOrEmpty(targetBehaviour))
            {
                return "Empty Transition";
            }
            else if(muted)
            {
                return "Transition to " + targetBehaviour + " [Muted]";
            }
            else
            {
                return "Transition to " + targetBehaviour;
            }
        }

        private AIController _owner;

        private void OnTargetBehaviourInitialization(SerializedProperty property, GUIContent label)
        {
            _owner = property.serializedObject.targetObject as AIController;
        }

        private void OnTargetBehaviourGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);
            if (GUI.Button(position, string.IsNullOrEmpty(property.stringValue) ? "None" : property.stringValue, EditorStyles.popup))
            {
                GenericMenu genericMenu = new GenericMenu();
                foreach (string name in _owner.GetBehaviourNames())
                {
                    genericMenu.AddItem(new GUIContent(name), false, () =>
                    {
                        property.stringValue = name;
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }
                genericMenu.DropDown(position);
            }
        }
#endif
        #endregion

        #region [Getter / Setter]
        public List<Transition> GetTransitions()
        {
            return transitions;
        }

        public void SetTransitions(List<Transition> value)
        {
            transitions = value;
        }

        public Transition GetTransition(int index)
        {
            return transitions[index];
        }

        public void SetTransition(int index, Transition value)
        {
            transitions[index] = value;
        }

        public int GetTransitionCount()
        {
            return transitions.Count;
        }
        #endregion
    }
}