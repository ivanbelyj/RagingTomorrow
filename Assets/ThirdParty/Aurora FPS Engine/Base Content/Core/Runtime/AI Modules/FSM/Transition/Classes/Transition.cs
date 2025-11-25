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
using System;
using System.Collections.Generic;
using UnityEngine;

#region [Unity Editor Section]
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using System.Linq;
using System.Reflection;
#endif
#endregion

namespace AuroraFPSRuntime.AIModules.Transitions
{
    [Serializable]
    public sealed class Transition : ITransition, ITransitionConditions
    {
        [SerializeField]
        [CustomView(ViewInitialization = "OnTargetBehaviourInitialization", ViewGUI = "OnTargetBehaviourGUI")]
        [NotEmpty]
        private string targetBehaviour;

        [SerializeReference]
        [ReorderableList(OnDropdownButtonCallback = "OnDropDownConditionsCallback", GetElementLabelCallback = "GetConditionLabelCallback")]
        private List<Condition> conditions;

        [SerializeField]
        private bool mute;

        #region [ITransition Implementation]
        public bool IsComplete()
        {
            if(mute)
            {
                return false;
            }

            for (int i = 0; i < conditions.Count; i++)
            {
                Condition condition = conditions[i];

                if (condition.IsMuted())
                {
                    continue;
                }

                if (!conditions[i].IsExecuted())
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region [ITransitionConditions Implementation]
        public void AddCondition(Condition condition)
        {
            conditions.Add(condition);
        }

        public bool RemoveCondition(Condition condition)
        {
            return conditions.Remove(condition);
        }

        public void RemoveCondition(int index)
        {
            conditions.RemoveAt(index);
        }

        public void ClearConditions()
        {
            conditions.Clear();
        }

        public T FindCondition<T>() where T : Condition
        {
            for (int i = 0; i < conditions.Count; i++)
            {
                T condition = conditions[i] as T;
                if (condition != null)
                {
                    return condition;
                }
            }
            return null;
        }

        public List<T> FindConditions<T>() where T : Condition
        {
            List<T> temp = null;
            for (int i = 0; i < conditions.Count; i++)
            {
                T condition = conditions[i] as T;
                if (condition != null)
                {
                    if (temp == null)
                    {
                        temp = new List<T>(1);
                    }
                    temp.Add(condition);
                }
            }
            return temp;
        }
        #endregion

        #region [Internal Callbacks]
        internal void Internal_Initialization(AIController owner)
        {
            for (int i = 0; i < conditions.Count; i++)
            {
                conditions[i].Internal_Initialize(owner);
            }
        }

        internal void Internal_Enable()
        {
            for (int i = 0; i < conditions.Count; i++)
            {
                conditions[i].Internal_Enable();
            }
        }

        internal void Internal_Disable()
        {
            for (int i = 0; i < conditions.Count; i++)
            {
                conditions[i].Internal_Disable();
            }
        }
        #endregion

        #region [Unity Editor Section]
#if UNITY_EDITOR
        private AIController owner;

        private void OnTargetBehaviourInitialization(SerializedProperty property, GUIContent label)
        {
            owner = property.serializedObject.targetObject as AIController;
        }

        private void OnTargetBehaviourGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);
            if (GUI.Button(position, string.IsNullOrEmpty(property.stringValue) ? "None" : property.stringValue, EditorStyles.popup))
            {
                GenericMenu genericMenu = new GenericMenu();
                foreach (string name in owner.GetBehaviourNames())
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

        private void OnDropDownConditionsCallback(Rect position, SerializedProperty property)
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
                    SerializedObject serializedObject = property.serializedObject;
                    AIController controller = serializedObject.targetObject as AIController;

                    if (IsValidCore(controller, type))
                    {
                        genericMenu.AddItem(new GUIContent(attribute.Path, attribute.Description), false, () =>
                        {
                            int index = property.arraySize;
                            property.arraySize++;
                            property.GetArrayElementAtIndex(index).managedReferenceValue = Activator.CreateInstance(type);
                            serializedObject.ApplyModifiedProperties();
                            AddRequiredComponents(controller, type);
                        });
                    }
                }
            }
            genericMenu.DropDown(position);
        }

        private string GetConditionLabelCallback(SerializedProperty property, int index)
        {
            string[] baseTypeAndAssemblyName = property.managedReferenceFullTypename.Split(' ');
            Type type = Type.GetType(baseTypeAndAssemblyName[1]);
            ConditionMenuAttribute attribute = type.GetCustomAttribute<ConditionMenuAttribute>();
            if (attribute != null)
            {
                return attribute.Name;
            }
            else
            {
                return "Condition " + index;
            }
        }

        private bool IsValidCore(AIController controller, Type type)
        {
            IEnumerable<AICoreSupportAttribute> supportAttributes = type.GetCustomAttributes<AICoreSupportAttribute>();
            if (supportAttributes.Count() > 0)
            {
                foreach (AICoreSupportAttribute supportAttribute in supportAttributes)
                {
                    if (controller.GetType() == supportAttribute.target.GetType())
                    {
                        return true;
                    }
                }
            }
            else
            {
                return true;
            }
            return false;
        }

        private void AddRequiredComponents(AIController core, Type type)
        {
            IEnumerable<RequireComponent> requireComponents = type.GetCustomAttributes<RequireComponent>();
            foreach (RequireComponent requireComponent in requireComponents)
            {
                if(requireComponent.m_Type0 != null && core.GetComponent(requireComponent.m_Type0) == null)
                {
                    core.gameObject.AddComponent(requireComponent.m_Type0);
                }

                if (requireComponent.m_Type1 != null && core.GetComponent(requireComponent.m_Type1) == null)
                {
                    core.gameObject.AddComponent(requireComponent.m_Type1);
                }

                if (requireComponent.m_Type2 != null && core.GetComponent(requireComponent.m_Type2) == null)
                {
                    core.gameObject.AddComponent(requireComponent.m_Type2);
                }
            }
        }
#endif
        #endregion

        #region [Getter / Setter]
        public string GetTargetBehaviour()
        {
            return targetBehaviour;
        }

        public void SetTargetBehaviour(string value)
        {
            targetBehaviour = value;
        }

        public bool IsMuted()
        {
            return mute;
        }

        public void SetMute(bool value)
        {
            mute = value;
        }

        public List<Condition> GetConditions()
        {
            return conditions;
        }

        public void SetConditions(List<Condition> value)
        {
            conditions = value;
        }

        public Condition GetCondition(int index)
        {
            return conditions[index];
        }

        public void SetCondition(int index, Condition value)
        {
            conditions[index] = value;
        }

        public int GetConditionCount()
        {
            return conditions.Count;
        }
        #endregion
    }
}