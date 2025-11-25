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
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AuroraFPSEditor.Attributes
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Object), true)]
    public class ApexEditor : Editor
    {
        public const string ScriptReferencePropertyName = "m_Script";

        private List<ApexSerializedField> apexSerializedFields;
        private List<ApexField> buttons;
        private ApexSettings apexSettings;
        private bool hideScriptReference;
        private bool enabled;
        private bool alwaysEnabled;

        /// <summary>
        /// Initializing Apex editor properties.
        /// </summary>
        public virtual void OnEnable()
        {
            if(target != null)
            {
                InitializeApexSerializedFields();
                CreateApexButtons(target, out buttons);
                LayoutApexButtons(ref buttons);
            }
        }

        /// <summary>
        /// This function is called when the Apex editor becomes enabled and active.
        /// </summary>
        public void InitializeApexSerializedFields()
        {
            FindFirstLevelProperties(serializedObject, out List<SerializedProperty> properties);

            apexSettings = ApexSettings.Current;

            hideScriptReference = HideScriptReference();

            if (hideScriptReference)
            {
                RemoveScriptReference(ref properties);
            }

            enabled = !apexSettings.GetExceptScripts()?.Any(e => e == target.GetType().Name) ?? true;

            if (enabled)
            {
                CreateApexSerializedField(properties, out apexSerializedFields);
                OrderSerializedProperties(ref apexSerializedFields);
                LayoutApexProperties(ref apexSerializedFields);
            }
        }

        /// <summary>
        /// Custom inspector GUI.
        /// </summary>
        public override void OnInspectorGUI()
        {
            if (enabled && (alwaysEnabled || apexSettings.ApexEnabled()))
            {
                serializedObject.Update();
                DrawAttributeProperties();
                DrawAttributeButtons();
                serializedObject.ApplyModifiedProperties();
            }
            else
            {
                if (hideScriptReference)
                {
                    DrawPropertiesExcluding(serializedObject, ScriptReferencePropertyName);
                }
                else
                {
                    DrawDefaultInspector();
                }
            }
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            apexSettings = ApexSettings.Current;
            ComponentIcon componentIcon = System.Attribute.GetCustomAttribute(target.GetType(), typeof(ComponentIcon)) as ComponentIcon;
            if(componentIcon != null)
            {
                string relativePath = "Base Content/Core/Editor/Editor Resources/Icons";
                Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(System.IO.Path.Combine(ApexSettings.Current.GetRootPath(), relativePath,string.Format("{0}.png", componentIcon.name)));
                if(icon != null)
                {
                    icon.alphaIsTransparency = true;
                    Texture2D cache = new Texture2D(width, height);
                    cache.alphaIsTransparency = true;
                    EditorUtility.CopySerialized(icon, cache);
                    return cache;
                }
            }
            return base.RenderStaticPreview(assetPath, subAssets, width, height);
        }

        /// <summary>
        /// Draw Aurora FPS editor.
        /// </summary>
        protected virtual void DrawAttributeProperties()
        {
            if (apexSerializedFields != null && apexSerializedFields.Count > 0)
            {
                for (int i = 0; i < apexSerializedFields.Count; i++)
                {
                    ApexSerializedField apexSerializedField = apexSerializedFields[i];
                    if (apexSerializedField.IsVisible())
                    {
                        apexSerializedFields[i].DrawFieldLayout();
                    }
                }
            }
        }

        /// <summary>
        /// Draw default build-in Aurora FPS editor.
        /// </summary>
        public void DrawDefaultAttributeProperties()
        {
            if (apexSerializedFields != null && apexSerializedFields.Count > 0)
            {
                for (int i = 0; i < apexSerializedFields.Count; i++)
                {
                    apexSerializedFields[i].DrawFieldLayout();
                }
            }
        }

        /// <summary>
        /// Find properties of SerializedObject only in first level.
        /// </summary>
        public static void FindFirstLevelProperties(SerializedObject serializedObject, out List<SerializedProperty> properties)
        {
            properties = new List<SerializedProperty>();

            using (SerializedProperty iterator = serializedObject.GetIterator())
            {
                if (iterator.NextVisible(true))
                {
                    do
                    {
                        properties.Add(iterator.Copy());
                    }
                    while (iterator.NextVisible(false));
                }
            }
        }

        /// <summary>
        /// Remove script property reference if it contains in properties list.
        /// </summary>
        public static void RemoveScriptReference(ref List<SerializedProperty> properties)
        {
            int index = -1;
            for (int i = 0; i < properties.Count; i++)
            {
                if (properties[i].name == ScriptReferencePropertyName)
                {
                    index = i;
                    break;
                }
            }

            if (index > -1)
            {
                properties.RemoveAt(index);
            }
        }

        /// <summary>
        /// Create Apex serialized fields by Unity serialized properties.
        /// </summary>
        public static void CreateApexSerializedField(List<SerializedProperty> serializedProperty, out List<ApexSerializedField> apexSerializedFields)
        {
            apexSerializedFields = new List<ApexSerializedField>(serializedProperty.Count);
            int count = 0;
            foreach (SerializedProperty property in serializedProperty)
            {
                apexSerializedFields.Add(new ApexProperty(property, count++));
            }
        }

        public static void OrderSerializedProperties(ref List<ApexSerializedField> properties)
        {
            properties.Sort(OrderComparisonCallback);
        }

        public static int OrderComparisonCallback(ApexSerializedField x, ApexSerializedField y)
        {
            OrderAttribute xOrderAttribute = ApexReflection.GetAttribute<OrderAttribute>(x.TargetSerializedProperty);
            OrderAttribute yOrderAttribute = ApexReflection.GetAttribute<OrderAttribute>(y.TargetSerializedProperty);
            float xPriority = xOrderAttribute?.priority ?? x.Order;
            float yPriority = yOrderAttribute?.priority ?? y.Order;
            if (xPriority > yPriority)
                return 1;
            else if (xPriority < yPriority)
                return -1;
            else
                return 0;
        }

        /// <summary>
        /// Layout properties by layout attributes.
        /// </summary>
        /// <param name="properties"></param>
        public static void LayoutApexProperties(ref List<ApexSerializedField> properties)
        {
            List<ApexSerializedField> layoutedProperties = new List<ApexSerializedField>();
            for (int i = 0; i < properties.Count; i++)
            {
                ApexSerializedField serializedField = properties[i];
                SerializedProperty serializedProperty = serializedField.TargetSerializedProperty;
                if (ApexReflection.GetAttribute<LayoutAttribute>(serializedProperty) == null)
                {
                    layoutedProperties.Add(serializedField);
                    continue;
                }

                GroupAttribute groupAttribute = ApexReflection.GetAttribute<GroupAttribute>(serializedProperty);
                TabGroupAttribute tabAttribute = ApexReflection.GetAttribute<TabGroupAttribute>(serializedProperty);
                FoldoutAttribute foldoutAttribute = ApexReflection.GetAttribute<FoldoutAttribute>(serializedProperty);
                if (groupAttribute != null)
                {
                    bool alreadyHasGroup = false;
                    for (int j = 0; j < layoutedProperties.Count; j++)
                    {
                        if (layoutedProperties[j] is ApexGroup group && groupAttribute.name == group.GetGroupName())
                        {
                            alreadyHasGroup = true;
                            if (tabAttribute != null)
                            {
                                bool alreadyHasTab = false;
                                for (int k = 0; k < group.Count; k++)
                                {
                                    if (group.GetChild(k) is ApexTab tab && tabAttribute.name == tab.GetName())
                                    {
                                        alreadyHasTab = true;
                                        if (foldoutAttribute != null)
                                        {
                                            bool alreadyHasFoldout = false;
                                            Dictionary<string, List<ApexSerializedField>> sectionsEditable = tab.sections;
                                            foreach (var section in tab.sections)
                                            {
                                                if (section.Key == tabAttribute.title)
                                                {
                                                    for (int l = 0; l < section.Value.Count; l++)
                                                    {
                                                        if (section.Value[l] is ApexFoldout foldout && foldoutAttribute.name == foldout.GetName())
                                                        {
                                                            alreadyHasFoldout = true;
                                                            foldout.Add(serializedField);
                                                            sectionsEditable[section.Key][l] = foldout;
                                                        }
                                                    }
                                                }
                                            }

                                            if (!alreadyHasFoldout)
                                            {
                                                ApexFoldout foldout = new ApexFoldout(serializedProperty, foldoutAttribute.name, foldoutAttribute.Style, serializedField);
                                                if (sectionsEditable.TryGetValue(tabAttribute.title, out List<ApexSerializedField> sectionChildren))
                                                {
                                                    sectionChildren.Add(foldout);
                                                    sectionsEditable[tabAttribute.title] = sectionChildren;
                                                }
                                                else
                                                {
                                                    sectionChildren = new List<ApexSerializedField>();
                                                    sectionChildren.Add(foldout);
                                                    sectionsEditable.Add(tabAttribute.title, sectionChildren);
                                                }
                                            }
                                            tab.sections = sectionsEditable;
                                        }
                                        else
                                        {
                                            if (tab.sections.TryGetValue(tabAttribute.title, out List<ApexSerializedField> sectionChildren))
                                            {
                                                sectionChildren.Add(serializedField);
                                                tab.sections[tabAttribute.title] = sectionChildren;
                                            }
                                            else
                                            {
                                                sectionChildren = new List<ApexSerializedField>();
                                                sectionChildren.Add(serializedField);
                                                tab.sections.Add(tabAttribute.title, sectionChildren);
                                            }
                                        }
                                        group.SetChild(k, tab);
                                    }
                                }

                                if (!alreadyHasTab)
                                {
                                    ApexTab tab = new ApexTab(serializedProperty, tabAttribute.name);
                                    if (foldoutAttribute != null)
                                    {
                                        ApexFoldout foldout = new ApexFoldout(serializedProperty, foldoutAttribute.name, foldoutAttribute.Style, serializedField);
                                        tab.sections.Add(tabAttribute.title, new List<ApexSerializedField>() { foldout });
                                    }
                                    else
                                    {
                                        tab.sections.Add(tabAttribute.title, new List<ApexSerializedField>() { serializedField });
                                    }
                                    group.Add(tab);
                                }
                            }
                            else if (foldoutAttribute != null)
                            {
                                bool alreadyHasFoldout = false;
                                for (int k = 0; k < group.Count; k++)
                                {
                                    if (group.GetChild(k) is ApexFoldout foldout && foldoutAttribute.name == foldout.GetName())
                                    {
                                        alreadyHasFoldout = true;
                                        foldout.Add(serializedField);
                                        group.SetChild(k, foldout);
                                    }
                                }

                                if (!alreadyHasFoldout)
                                {
                                    ApexFoldout foldout = new ApexFoldout(serializedProperty, foldoutAttribute.name, foldoutAttribute.Style, serializedField);
                                    group.Add(foldout);
                                }
                            }
                            else
                            {
                                group.Add(serializedField);
                            }
                            layoutedProperties[j] = group;
                        }
                    }

                    if (!alreadyHasGroup)
                    {
                        ApexGroup group = new ApexGroup(serializedProperty, groupAttribute.name, new List<ApexSerializedField>());
                        if (tabAttribute != null)
                        {
                            ApexTab tab = new ApexTab(serializedProperty, tabAttribute.name);
                            if (foldoutAttribute != null)
                            {
                                ApexFoldout foldout = new ApexFoldout(serializedProperty, foldoutAttribute.name, foldoutAttribute.Style, serializedField);
                                tab.sections.Add(tabAttribute.title, new List<ApexSerializedField>() { foldout });
                            }
                            else
                            {
                                tab.sections.Add(tabAttribute.title, new List<ApexSerializedField>() { serializedField });
                            }
                            group.Add(tab);
                        }
                        else if (foldoutAttribute != null)
                        {
                            ApexFoldout foldout = new ApexFoldout(serializedProperty, foldoutAttribute.name, foldoutAttribute.Style, serializedField);
                            group.Add(foldout);
                        }
                        else
                        {
                            group.Add(serializedField);
                        }
                        layoutedProperties.Add(group);
                    }
                }
                else if (tabAttribute != null)
                {
                    bool alreadyHasTab = false;
                    for (int j = 0; j < layoutedProperties.Count; j++)
                    {
                        if (layoutedProperties[j] is ApexTab tab && tabAttribute.name == tab.GetName())
                        {
                            alreadyHasTab = true;
                            if (foldoutAttribute != null)
                            {
                                bool alreadyHasFoldout = false;
                                Dictionary<string, List<ApexSerializedField>> sectionsEditable = tab.sections;
                                foreach (var section in tab.sections)
                                {
                                    if (section.Key == tabAttribute.title)
                                    {
                                        for (int l = 0; l < section.Value.Count; l++)
                                        {
                                            if (section.Value[l] is ApexFoldout foldout && foldoutAttribute.name == foldout.GetName())
                                            {
                                                alreadyHasFoldout = true;
                                                foldout.Add(serializedField);
                                                sectionsEditable[section.Key][l] = foldout;
                                            }
                                        }
                                    }
                                }

                                if (!alreadyHasFoldout)
                                {
                                    ApexFoldout foldout = new ApexFoldout(serializedProperty, foldoutAttribute.name, foldoutAttribute.Style, serializedField);
                                    if (sectionsEditable.TryGetValue(tabAttribute.title, out List<ApexSerializedField> sectionChildren))
                                    {
                                        sectionChildren.Add(foldout);
                                        sectionsEditable[tabAttribute.title] = sectionChildren;
                                    }
                                    else
                                    {
                                        sectionChildren = new List<ApexSerializedField>();
                                        sectionChildren.Add(foldout);
                                        sectionsEditable.Add(tabAttribute.title, sectionChildren);
                                    }
                                }
                                tab.sections = sectionsEditable;
                            }
                            else
                            {
                                if (tab.sections.TryGetValue(tabAttribute.title, out List<ApexSerializedField> sectionChildren))
                                {
                                    sectionChildren.Add(serializedField);
                                    tab.sections[tabAttribute.title] = sectionChildren;
                                }
                                else
                                {
                                    sectionChildren = new List<ApexSerializedField>();
                                    sectionChildren.Add(serializedField);
                                    tab.sections.Add(tabAttribute.title, sectionChildren);
                                }
                            }
                            layoutedProperties[j] = tab;
                        }
                    }

                    if (!alreadyHasTab)
                    {
                        ApexTab tab = new ApexTab(serializedProperty, tabAttribute.name);
                        if (foldoutAttribute != null)
                        {
                            ApexFoldout foldout = new ApexFoldout(serializedProperty, foldoutAttribute.name, foldoutAttribute.Style, serializedField);
                            tab.sections.Add(tabAttribute.title, new List<ApexSerializedField>() { foldout });
                        }
                        else
                        {
                            tab.sections.Add(tabAttribute.title, new List<ApexSerializedField>() { serializedField });
                        }
                        layoutedProperties.Add(tab);
                    }
                }
                else if (foldoutAttribute != null)
                {
                    bool alreadyHasFoldout = false;
                    for (int j = 0; j < layoutedProperties.Count; j++)
                    {
                        if (layoutedProperties[j] is ApexFoldout foldout && foldoutAttribute.name == foldout.GetName())
                        {
                            alreadyHasFoldout = true;
                            foldout.Add(serializedField);
                            layoutedProperties[j] = foldout;
                        }
                    }

                    if (!alreadyHasFoldout)
                    {
                        ApexFoldout foldout = new ApexFoldout(serializedProperty, foldoutAttribute.name, foldoutAttribute.Style, serializedField);
                        layoutedProperties.Add(foldout);
                    }
                }
                else
                {
                    layoutedProperties.Add(serializedField);
                }
            }
            properties = layoutedProperties;
        }

        public static void CreateApexButtons(Object target, out List<ApexField> buttons)
        {
            buttons = new List<ApexField>();

            IEnumerable<MethodInfo> methods = target.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.GetParameters().Length == 0);

            foreach (MethodInfo method in methods)
            {
                object[] buttonAttributes = method.GetCustomAttributes(typeof(ButtonAttribute), true);
                if (buttonAttributes != null && buttonAttributes.Length > 0)
                {
                    for (int i = 0; i < buttonAttributes.Length; i++)
                    {
                        ButtonAttribute buttonAttribute = buttonAttributes[i] as ButtonAttribute;
                        if (buttonAttribute != null)
                        {
                            string methodLabel = buttonAttribute.lable;
                            GUIContent buttonContent = new GUIContent(methodLabel != "" ? methodLabel : method.Name);
                            if (!string.IsNullOrEmpty(methodLabel) && methodLabel.Length > 1 && methodLabel[0] == '@')
                            {
                                string iconName = methodLabel.Remove(0, 1);
                                buttonContent = EditorGUIUtility.IconContent(iconName);
                            }
                            buttons.Add(new ApexButton(method, target, buttonContent, buttonAttribute.style, buttonAttribute.height));
                            break;
                        }
                    }
                }
            }
        }

        public static void LayoutApexButtons(ref List<ApexField> buttons)
        {
            List<ApexField> tempList = new List<ApexField>();
            for (int i = 0; i < buttons.Count; i++)
            {
                ApexButton apexButton = buttons[i] as ApexButton;
                if (apexButton != null)
                {
                    MethodInfo action = apexButton.GetAction();
                    if (action != null)
                    {
                        object[] buttonAttributes = action.GetCustomAttributes(typeof(ButtonAttribute), true);
                        if (buttonAttributes != null && buttonAttributes.Length > 0)
                        {
                            bool hasAttribute = false;
                            for (int j = 0; j < buttonAttributes.Length; j++)
                            {
                                object[] buttonGroupAttributes = action.GetCustomAttributes(typeof(ButtonHorizontalGroupAttribute), true);
                                if(buttonGroupAttributes != null && buttonGroupAttributes.Length > 0)
                                {
                                    for (int k = 0; k < buttonGroupAttributes.Length; k++)
                                    {
                                        ButtonHorizontalGroupAttribute buttonGroupAttribute = buttonGroupAttributes[k] as ButtonHorizontalGroupAttribute;
                                        if (buttonGroupAttribute != null)
                                        {
                                            bool alreadyHasGroup = false;
                                            for (int l = 0; l < tempList.Count; l++)
                                            {
                                                ApexButtonGroup apexButtonGroup = tempList[l] as ApexButtonGroup;
                                                if (apexButtonGroup != null && buttonGroupAttribute.name == apexButtonGroup.name)
                                                {
                                                    apexButtonGroup.Add(apexButton);
                                                    tempList[l] = apexButtonGroup;
                                                    alreadyHasGroup = true;
                                                }
                                            }

                                            if (!alreadyHasGroup)
                                            {
                                                tempList.Add(new ApexButtonGroup(buttonGroupAttribute.name, new List<ApexField>() { apexButton }));
                                            }
                                            hasAttribute = true;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (!hasAttribute)
                            {
                                tempList.Add(apexButton);
                            }
                        }
                    }
                }
            }
            buttons = tempList;
        }

        protected virtual void DrawAttributeButtons()
        {
            if (buttons != null && buttons.Count > 0)
            {
                for (int i = 0; i < buttons.Count; i++)
                {
                    buttons[i].DrawFieldLayout();
                }
            }
        }

        /// <summary>
        /// Find all editor, which use Aurora FPS editor and repaint it all.
        /// </summary>
        public static void RepaintAllInstances()
        {
            ApexEditor[] editors = Resources.FindObjectsOfTypeAll<ApexEditor>();
            for (int i = 0; i < editors.Length; i++)
            {
                ApexEditor editor = editors[i];
                editor.InitializeApexSerializedFields();
                editor.Repaint();
            }
        }

        public static void RepaintInstance(Object target)
        {
            ApexEditor[] editors = Resources.FindObjectsOfTypeAll<ApexEditor>();
            for (int i = 0; i < editors.Length; i++)
            {
                ApexEditor editor = editors[i];
                if(editor.target == target)
                {
                    editor.InitializeApexSerializedFields();
                    editor.Repaint();
                    break;
                }
            }
        }

        /// <summary>
        /// Find all editor, which use Aurora FPS editor and rebuild it all.
        /// </summary>
        public static void RebuildAllInstances(params string[] except)
        {
            ApexEditor[] editors = Resources.FindObjectsOfTypeAll<ApexEditor>();
            for (int i = 0; i < editors.Length; i++)
            {
                ApexEditor editor = editors[i];

                if (except.Any(e => e == editor.target.GetType().Name))
                    continue;

                editor.InitializeApexSerializedFields();
                editor.Repaint();
            }
        }

        /// <summary>
        /// Find all editor, which use Aurora FPS editor and repaint it all.
        /// </summary>
        public static void RepaintAllInstances(params string[] except)
        {
            ApexEditor[] editors = Resources.FindObjectsOfTypeAll<ApexEditor>();
            for (int i = 0; i < editors.Length; i++)
            {
                ApexEditor editor = editors[i];

                if (except.Any(e => e == editor.target.GetType().Name))
                    continue;

                editor.Repaint();
            }
        }

        /// <summary>
        /// Check that this script contains HideScriptReference attribute.
        /// </summary>
        public bool HideScriptReference()
        {
            return System.Attribute.GetCustomAttribute(target.GetType(), typeof(HideScriptFieldAttribute)) != null;
        }

        /// <summary>
        /// Set true to enable Apex editor regardless of Apex settings.
        /// See also: Project Settings/Aurora FPS/Apex Enabled(bool).
        /// </summary>
        public void EnabledRegardlessOfSettings(bool value)
        {
            alwaysEnabled = value;
        }
    }
}