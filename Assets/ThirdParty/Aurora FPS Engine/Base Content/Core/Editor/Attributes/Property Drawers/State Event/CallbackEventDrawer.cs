/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.CoreModules.ValueTypes;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace AuroraFPSEditor.Attributes
{
    [DrawerTarget(typeof(CallbackEvent), SubClasses = true)]
    public sealed class CallbackEventDrawer : PropertyDrawer
    {
        private SerializedProperty serializedTarget;
        private SerializedProperty serializedEventName;

        public override void OnInitialize(SerializedProperty property, GUIContent label)
        {
            serializedTarget = property.FindPropertyRelative("target");
            serializedEventName = property.FindPropertyRelative("eventName");
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float standardVerticalSpacing = EditorGUIUtility.standardVerticalSpacing;

            Rect typePosition = EditorGUI.PrefixLabel(new Rect(position.x, position.y, position.width, singleLineHeight), new GUIContent("Component"));
            if (GUI.Button(typePosition, GetComponentLabel(), EditorStyles.popup))
            {
                SearchableMenu searchableMenu = GetComponentsMenu(property);
                searchableMenu.ShowAsDropdown(typePosition, new Vector2(typePosition.width, 163));
            }

            if (serializedTarget.objectReferenceValue != null)
            {
                Rect eventPosition = EditorGUI.PrefixLabel(new Rect(position.x, typePosition.yMax + standardVerticalSpacing, position.width, singleLineHeight), new GUIContent("Event"));
                if (GUI.Button(eventPosition, GetEventLabel(), EditorStyles.popup))
                {
                    SearchableMenu searchableMenu = GetEventsMenu(property);
                    searchableMenu.ShowAsDropdown(eventPosition, new Vector2(typePosition.width, 163));
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float standardVerticalSpacing = EditorGUIUtility.standardVerticalSpacing;

            float height = singleLineHeight + standardVerticalSpacing;
            if (serializedTarget.objectReferenceValue != null)
            {
                height += singleLineHeight + standardVerticalSpacing;
            }
            return height;
        }

        public string GetComponentLabel()
        {
            return serializedTarget.objectReferenceValue == null ? "Select component..." : serializedTarget.objectReferenceValue.GetType().Name;
        }

        public string GetEventLabel()
        {
            return string.IsNullOrEmpty(serializedEventName.stringValue) ? "Select event..." : serializedEventName.stringValue;
        }

        private SearchableMenu GetComponentsMenu(SerializedProperty property)
        {
            SearchableMenu searchableMenu = new SearchableMenu();
            List<Component> components = new List<Component>();
            Component thisComponent = property.serializedObject.targetObject as Component;
            components.AddRange(thisComponent.transform.root.GetComponents<Component>());
            components.AddRange(thisComponent.transform.root.GetComponentsInChildren<Component>());
            for (int i = 0; i < components.Count; i++)
            {
                Component component = components[i];
                if (component is Transform ||
                    component.GetType().GetEvents() == null ||
                    component.GetType().GetEvents().Length == 0)
                {
                    continue;
                }

                string shortObjectName = component.gameObject.name;
                if(shortObjectName.Length > 7)
                {
                    shortObjectName = shortObjectName.Substring(0, 7);
                    shortObjectName += "...";
                }
                GUIContent content = new GUIContent(string.Format("{0}: {1}", component.GetType().Name, shortObjectName));
                searchableMenu.AddItem(content, true, () =>
                {
                    serializedTarget.objectReferenceValue = component;
                    property.serializedObject.ApplyModifiedProperties();
                });
            }
            return searchableMenu;
        }

        private SearchableMenu GetEventsMenu(SerializedProperty property)
        {
            SearchableMenu searchableMenu = new SearchableMenu();
            EventInfo[] eventInfos = serializedTarget.objectReferenceValue.GetType().GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            for (int i = 0; i < eventInfos.Length; i++)
            {
                EventInfo eventInfo = eventInfos[i];
                MethodInfo methodInfo = eventInfo.EventHandlerType.GetMethods()[0];
                if (methodInfo.GetParameters().Length <= 3)
                {
                    GUIContent content = new GUIContent(eventInfo.Name);
                    searchableMenu.AddItem(content, true, () =>
                    {
                        serializedEventName.stringValue = eventInfo.Name;
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }
            }
            return searchableMenu;
        }
    }

    [DrawerTarget(typeof(CallbackEvent<>), SubClasses = true)]
    public sealed class CallbackEventArgDrawer : PropertyDrawer
    {
        private SerializedProperty serializedTarget;
        private SerializedProperty serializedEventName;
        private ApexProperty apexSerializedParameter;

        public override void OnInitialize(SerializedProperty property, GUIContent label)
        {
            serializedTarget = property.FindPropertyRelative("target");
            serializedEventName = property.FindPropertyRelative("eventName");
            SerializedProperty serializedParameter = property.FindPropertyRelative("arg");
            apexSerializedParameter = new ApexProperty(serializedParameter);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float standardVerticalSpacing = EditorGUIUtility.standardVerticalSpacing;

            Rect typePosition = EditorGUI.PrefixLabel(new Rect(position.x, position.y, position.width, singleLineHeight), new GUIContent("Component"));
            if (GUI.Button(typePosition, GetComponentLabel(), EditorStyles.popup))
            {
                SearchableMenu searchableMenu = GetComponentsMenu(property);
                searchableMenu.ShowAsDropdown(typePosition, new Vector2(typePosition.width, 163));
            }

            if (serializedTarget.objectReferenceValue != null)
            {
                Rect eventPosition = EditorGUI.PrefixLabel(new Rect(position.x, typePosition.yMax + standardVerticalSpacing, position.width, singleLineHeight), new GUIContent("Event"));
                if (GUI.Button(eventPosition, GetEventLabel(), EditorStyles.popup))
                {
                    SearchableMenu searchableMenu = GetEventsMenu(property);
                    searchableMenu.ShowAsDropdown(eventPosition, new Vector2(typePosition.width, 163));
                }

                if (!string.IsNullOrEmpty(serializedEventName.stringValue))
                {
                    Rect parameterPosition = new Rect(position.x, eventPosition.yMax + standardVerticalSpacing, position.width, apexSerializedParameter.GetFieldHeight());
                    apexSerializedParameter.DrawField(parameterPosition);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float standardVerticalSpacing = EditorGUIUtility.standardVerticalSpacing;

            float height = singleLineHeight + standardVerticalSpacing;
            if (serializedTarget.objectReferenceValue != null)
            {
                height += singleLineHeight + standardVerticalSpacing;
                if (!string.IsNullOrEmpty(serializedEventName.stringValue))
                {
                    height += apexSerializedParameter.GetFieldHeight() + standardVerticalSpacing;
                }
            }
            return height;
        }

        public string GetComponentLabel()
        {
            return serializedTarget.objectReferenceValue == null ? "Select component..." : serializedTarget.objectReferenceValue.GetType().Name;
        }

        public string GetEventLabel()
        {
            return string.IsNullOrEmpty(serializedEventName.stringValue) ? "Select event..." : serializedEventName.stringValue;
        }

        private SearchableMenu GetComponentsMenu(SerializedProperty property)
        {
            SearchableMenu searchableMenu = new SearchableMenu();
            List<Component> components = new List<Component>();
            Component thisComponent = property.serializedObject.targetObject as Component;
            components.AddRange(thisComponent.transform.root.GetComponents<Component>());
            components.AddRange(thisComponent.transform.root.GetComponentsInChildren<Component>());
            for (int i = 0; i < components.Count; i++)
            {
                Component component = components[i];
                if (component is Transform ||
                    component.GetType().GetEvents() == null ||
                    component.GetType().GetEvents().Length == 0)
                {
                    continue;
                }

                string shortObjectName = component.gameObject.name;
                if (shortObjectName.Length > 7)
                {
                    shortObjectName = shortObjectName.Substring(0, 7);
                    shortObjectName += "...";
                }
                GUIContent content = new GUIContent(string.Format("{0}: {1}", component.GetType().Name, shortObjectName));
                searchableMenu.AddItem(content, true, () =>
                {
                    serializedTarget.objectReferenceValue = component;
                    property.serializedObject.ApplyModifiedProperties();
                });
            }
            return searchableMenu;
        }

        private SearchableMenu GetEventsMenu(SerializedProperty property)
        {
            SearchableMenu searchableMenu = new SearchableMenu();
            EventInfo[] eventInfos = serializedTarget.objectReferenceValue.GetType().GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            for (int i = 0; i < eventInfos.Length; i++)
            {
                EventInfo eventInfo = eventInfos[i];
                MethodInfo methodInfo = eventInfo.EventHandlerType.GetMethods()[0];
                if (methodInfo.GetParameters().Length <= 3)
                {
                    GUIContent content = new GUIContent(eventInfo.Name);
                    searchableMenu.AddItem(content, true, () =>
                    {
                        serializedEventName.stringValue = eventInfo.Name;
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }
            }
            return searchableMenu;
        }
    }

    [DrawerTarget(typeof(CallbackEvent<,>), SubClasses = true)]
    public sealed class CallbackEventArgs2Drawer : PropertyDrawer
    {
        private SerializedProperty serializedTarget;
        private SerializedProperty serializedEventName;
        private ApexProperty apexSerializeArg1;
        private ApexProperty apexSerializeArg2;

        public override void OnInitialize(SerializedProperty property, GUIContent label)
        {
            serializedTarget = property.FindPropertyRelative("target");
            serializedEventName = property.FindPropertyRelative("eventName");
            SerializedProperty serializedArg1 = property.FindPropertyRelative("arg1");
            SerializedProperty serializedArg2 = property.FindPropertyRelative("arg2");
            apexSerializeArg1 = new ApexProperty(serializedArg1);
            apexSerializeArg2 = new ApexProperty(serializedArg2);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float standardVerticalSpacing = EditorGUIUtility.standardVerticalSpacing;

            Rect typePosition = EditorGUI.PrefixLabel(new Rect(position.x, position.y, position.width, singleLineHeight), new GUIContent("Component"));
            if (GUI.Button(typePosition, GetComponentLabel(), EditorStyles.popup))
            {
                SearchableMenu searchableMenu = GetComponentsMenu(property);
                searchableMenu.ShowAsDropdown(typePosition, new Vector2(typePosition.width, 163));
            }


            if (serializedTarget.objectReferenceValue != null)
            {
                Rect eventPosition = EditorGUI.PrefixLabel(new Rect(position.x, typePosition.yMax + standardVerticalSpacing, position.width, singleLineHeight), new GUIContent("Event"));
                if (GUI.Button(eventPosition, GetEventLabel(), EditorStyles.popup))
                {
                    SearchableMenu searchableMenu = GetEventsMenu(property);
                    searchableMenu.ShowAsDropdown(eventPosition, new Vector2(typePosition.width, 163));
                }

                if (!string.IsNullOrEmpty(serializedEventName.stringValue))
                {
                    Rect arg1Position = new Rect(position.x, eventPosition.yMax + standardVerticalSpacing, position.width, apexSerializeArg1.GetFieldHeight());
                    apexSerializeArg1.DrawField(arg1Position);

                    Rect arg2Position = new Rect(position.x, arg1Position.yMax + standardVerticalSpacing, position.width, apexSerializeArg2.GetFieldHeight());
                    apexSerializeArg2.DrawField(arg2Position);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float standardVerticalSpacing = EditorGUIUtility.standardVerticalSpacing;

            float height = singleLineHeight + standardVerticalSpacing;
            if (serializedTarget.objectReferenceValue != null)
            {
                height += singleLineHeight + standardVerticalSpacing;
                if (!string.IsNullOrEmpty(serializedEventName.stringValue))
                {
                    height += apexSerializeArg1.GetFieldHeight() + standardVerticalSpacing;
                    height += apexSerializeArg2.GetFieldHeight() + standardVerticalSpacing;
                }
            }
            return height;
        }

        public string GetComponentLabel()
        {
            return serializedTarget.objectReferenceValue == null ? "Select component..." : serializedTarget.objectReferenceValue.GetType().Name;
        }

        public string GetEventLabel()
        {
            return string.IsNullOrEmpty(serializedEventName.stringValue) ? "Select event..." : serializedEventName.stringValue;
        }

        private SearchableMenu GetComponentsMenu(SerializedProperty property)
        {
            SearchableMenu searchableMenu = new SearchableMenu();
            List<Component> components = new List<Component>();
            Component thisComponent = property.serializedObject.targetObject as Component;
            components.AddRange(thisComponent.transform.root.GetComponents<Component>());
            components.AddRange(thisComponent.transform.root.GetComponentsInChildren<Component>());
            for (int i = 0; i < components.Count; i++)
            {
                Component component = components[i];
                if (component is Transform ||
                    component.GetType().GetEvents() == null ||
                    component.GetType().GetEvents().Length == 0)
                {
                    continue;
                }

                string shortObjectName = component.gameObject.name;
                if (shortObjectName.Length > 7)
                {
                    shortObjectName = shortObjectName.Substring(0, 7);
                    shortObjectName += "...";
                }
                GUIContent content = new GUIContent(string.Format("{0}: {1}", component.GetType().Name, shortObjectName)); searchableMenu.AddItem(content, true, () =>
                {
                    serializedTarget.objectReferenceValue = component;
                    property.serializedObject.ApplyModifiedProperties();
                });
            }
            return searchableMenu;
        }

        private SearchableMenu GetEventsMenu(SerializedProperty property)
        {
            SearchableMenu searchableMenu = new SearchableMenu();
            EventInfo[] eventInfos = serializedTarget.objectReferenceValue.GetType().GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            for (int i = 0; i < eventInfos.Length; i++)
            {
                EventInfo eventInfo = eventInfos[i];
                MethodInfo methodInfo = eventInfo.EventHandlerType.GetMethods()[0];
                if (methodInfo.GetParameters().Length <= 3)
                {
                    GUIContent content = new GUIContent(eventInfo.Name);
                    searchableMenu.AddItem(content, true, () =>
                    {
                        serializedEventName.stringValue = eventInfo.Name;
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }
            }
            return searchableMenu;
        }
    }
}