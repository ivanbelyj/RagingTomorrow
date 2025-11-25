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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AuroraFPSEditor.Attributes
{
    public class ApexProperty : ApexSerializedField
    {
        public static bool Changed = false;

        private readonly static Padding FoldoutHeaderPadding = new Padding(0.0f, 0.0f, 0.0f, 4.0f);
        private readonly static Padding FoldoutHeaderChildrenPadding = new Padding(0.0f, 0.0f, 0.0f, -4.0f);

        // Base properties.
        private List<ApexSerializedField> children;

        // Stored apex attributes
        private PropertyDrawer propertyDrawer;
        private PropertyView propertyView;
        private PropertyPainter[] propertyPainters;
        private PropertyValidator[] propertyValidators;
        private PropertyCondition[] visibleConditions;
        private PropertyCondition[] activeConditions;
        private ReadOnlyAttribute readOnlyAttribute;
        private List<ApexField> buttons;
        private List<MethodInfo> onChangeCallbacks;

        // Stored custom display label
        private GUIContent displayLabel;

        // Stored Apex settings.
        private ApexSettings apexSettings;

        // Stored required properties.
        private object target;
        private Rect[] validatorsRects;
        private Rect[] painterRects;
        private float currentYPosition;
        private bool hasReceiver;
        private int previousArraySize;
        private bool ignoreType;
        private bool defaultArrayHasChanged;

        public ApexProperty(SerializedProperty serializedProperty) : base(serializedProperty)
        {
            InitializeDisplayLabelContent(serializedProperty, new GUIContent(serializedProperty.displayName));
            InititalizePropertyDrawer(serializedProperty, displayLabel);
            if (propertyDrawer == null)
            {
                InititalizePropertyView(serializedProperty, displayLabel);
            }
            InitializePropertyDecorators(serializedProperty, displayLabel);
            InitializeValidatorAttributes(serializedProperty, displayLabel);
            InitializeVisibleAttribute(serializedProperty);
            InitializeActiveAttribute(serializedProperty);
            InitializeReadOnlyAttribute(serializedProperty);
            InitializeBottomButtonAttributes(serializedProperty);
            InitializeOnChangeCallback(serializedProperty);

            apexSettings = ApexSettings.Current;
            ignoreType = IgnoreType();

            if (hasReceiver || propertyDrawer == null && propertyView == null)
            {
                ApplyChildren();
            }
            if (serializedProperty.name == ApexEditor.ScriptReferencePropertyName)
            {
                readOnlyAttribute = new ReadOnlyAttribute();
            }
        }

        public ApexProperty(SerializedProperty serializedProperty, int order) : base(serializedProperty, order)
        {
            InitializeDisplayLabelContent(serializedProperty, new GUIContent(serializedProperty.displayName));
            InititalizePropertyDrawer(serializedProperty, displayLabel);
            if (propertyDrawer == null)
            {
                InititalizePropertyView(serializedProperty, displayLabel);
            }
            InitializePropertyDecorators(serializedProperty, displayLabel);
            InitializeValidatorAttributes(serializedProperty, displayLabel);
            InitializeVisibleAttribute(serializedProperty);
            InitializeActiveAttribute(serializedProperty);
            InitializeReadOnlyAttribute(serializedProperty);
            InitializeBottomButtonAttributes(serializedProperty);
            InitializeOnChangeCallback(serializedProperty);

            apexSettings = ApexSettings.Current;
            ignoreType = IgnoreType();

            if (hasReceiver || propertyDrawer == null && propertyView == null)
            {
                ApplyChildren();
            }
            if (serializedProperty.name == ApexEditor.ScriptReferencePropertyName)
            {
                readOnlyAttribute = new ReadOnlyAttribute();
            }
        }

        private bool IgnoreType()
        {
            return apexSettings.GetDefaultTypes().Any(t => t == TargetSerializedProperty.type);
        }

        #region [Initialization Attributes]
        private void InitializeDisplayLabelContent(SerializedProperty property, GUIContent label)
        {
            displayLabel = label;
            if (ApexReflection.TryGetDisplayContent(property, out GUIContent content))
            {
                displayLabel = content;
            }
            else if (ApexReflection.GetAttribute<HideLabelAttribute>(property) != null)
            {
                displayLabel = GUIContent.none;
            }
        }

        private void InititalizePropertyDrawer(SerializedProperty property, GUIContent label)
        {
            Type propertyType = ApexReflection.GetPropertyType(property);
            if (propertyType == null)
            {
                object propertyObject = ApexReflection.GetObjectOfProperty(property);
                if (propertyObject != null)
                {
                    propertyType = propertyObject.GetType();
                }
            }

            if (propertyType != null && ApexReflection.Drawers.TryGetValue(propertyType, out PropertyDrawer propertyDrawer))
            {
                this.propertyDrawer = Activator.CreateInstance(propertyDrawer.GetType()) as PropertyDrawer;
                this.propertyDrawer.OnInitialize(property, label);
                if (this.propertyDrawer is IPropertyReceiver receiver)
                {
                    receiver.OnReceiveProperty(this);
                    hasReceiver = true;
                }
            }
        }

        private void InititalizePropertyView(SerializedProperty property, GUIContent label)
        {
            ViewAttribute viewAttribute = ApexReflection.GetAttribute<ViewAttribute>(property);
            if (viewAttribute != null && ApexReflection.Views.TryGetValue(viewAttribute.GetType(), out PropertyView propertyView))
            {
                this.propertyView = Activator.CreateInstance(propertyView.GetType()) as PropertyView;
                if (this.propertyView is IPropertyValidatorReceiver verification && !verification.IsValidProperty(property, label))
                {
                    this.propertyView = null;
                    return;
                }
                this.propertyView.OnInitialize(property, viewAttribute, label);
                if (this.propertyView is IPropertyReceiver receiver)
                {
                    receiver.OnReceiveProperty(this);
                    hasReceiver = true;
                }
            }
        }

        private void InitializePropertyDecorators(SerializedProperty property, GUIContent label)
        {
            PainterAttribute[] decoratorAttributes = ApexReflection.GetAttributes<PainterAttribute>(property);
            if (decoratorAttributes != null && decoratorAttributes.Length > 0)
            {
                List<PropertyPainter> decoratorList = new List<PropertyPainter>();
                for (int i = 0; i < decoratorAttributes.Length; i++)
                {
                    PainterAttribute decoratorAttribute = decoratorAttributes[i];
                    if (ApexReflection.Decorators.TryGetValue(decoratorAttribute.GetType(), out PropertyPainter propertyDecorator))
                    {
                        PropertyPainter decorator = Activator.CreateInstance(propertyDecorator.GetType()) as PropertyPainter;
                        if (decorator is IPropertyValidatorReceiver verification && !verification.IsValidProperty(property, label))
                        {
                            continue;
                        }
                        decorator.OnInitialize(property, decoratorAttribute, label);
                        decoratorList.Add(decorator);
                    }
                }
                propertyPainters = decoratorList.ToArray();
                painterRects = new Rect[propertyPainters.Length];
            }
        }

        private void InitializeValidatorAttributes(SerializedProperty property, GUIContent label)
        {
            ValidatorAttribute[] validatorAttributes = ApexReflection.GetAttributes<ValidatorAttribute>(property);
            if (validatorAttributes != null && validatorAttributes.Length > 0)
            {
                List<PropertyValidator> validatorList = new List<PropertyValidator>();
                for (int i = 0; i < validatorAttributes.Length; i++)
                {
                    ValidatorAttribute validatorAttribute = validatorAttributes[i];
                    if (ApexReflection.Validators.TryGetValue(validatorAttribute.GetType(), out PropertyValidator propertyValidator))
                    {
                        PropertyValidator validator = Activator.CreateInstance(propertyValidator.GetType()) as PropertyValidator;
                        if (validator is IPropertyValidatorReceiver verification && !verification.IsValidProperty(property, label))
                        {
                            continue;
                        }
                        validator.OnInitialize(property, validatorAttribute, label);
                        validatorList.Add(validator);
                    }
                }
                propertyValidators = validatorList.ToArray();
                validatorsRects = new Rect[propertyValidators.Length];
            }
        }

        private void InitializeVisibleAttribute(SerializedProperty property)
        {
            VisibleIfAttribute[] visibleIfAttributes = ApexReflection.GetAttributes<VisibleIfAttribute>(property);
            if (visibleIfAttributes != null)
            {
                visibleConditions = new PropertyCondition[visibleIfAttributes.Length];
                for (int i = 0; i < visibleIfAttributes.Length; i++)
                {
                    visibleConditions[i] = new PropertyCondition(visibleIfAttributes[i]);
                }
            }
        }

        private void InitializeActiveAttribute(SerializedProperty property)
        {
            ActiveIfAttribute[] activeIfAttributes = ApexReflection.GetAttributes<ActiveIfAttribute>(property);
            if (activeIfAttributes != null)
            {
                activeConditions = new PropertyCondition[activeIfAttributes.Length];
                for (int i = 0; i < activeConditions.Length; i++)
                {
                    activeConditions[i] = new PropertyCondition(activeIfAttributes[i]);
                }
            }
        }

        private void InitializeReadOnlyAttribute(SerializedProperty property)
        {
            readOnlyAttribute = ApexReflection.GetAttribute<ReadOnlyAttribute>(property);
        }

        private void InitializeBottomButtonAttributes(SerializedProperty property)
        {
            BottomButtonAttribute[] bottomButtonAttributes = ApexReflection.GetAttributes<BottomButtonAttribute>(property);
            if (bottomButtonAttributes != null && bottomButtonAttributes.Length > 0)
            {
                buttons = new List<ApexField>();
                for (int i = 0; i < bottomButtonAttributes.Length; i++)
                {
                    BottomButtonAttribute bottomButtonAttribute = bottomButtonAttributes[i];
                    MethodInfo action = ApexReflection.GetAllMethods(property.serializedObject.targetObject, m => m.Name == bottomButtonAttribute.name && m.GetParameters().Length == 0).FirstOrDefault();
                    if (action != null)
                    {
                        GUIContent buttonContent = GUIContent.none;
                        string methodLabel = bottomButtonAttribute.Label;
                        if (!string.IsNullOrEmpty(methodLabel) && methodLabel.Length > 1 && methodLabel[0] == '@')
                        {
                            string iconName = methodLabel.Remove(0, 1);
                            buttonContent = EditorGUIUtility.IconContent(iconName);
                        }
                        else
                        {
                            buttonContent = new GUIContent(methodLabel);
                        }
                        ApexButton apexButton = new ApexButton(action, property.serializedObject.targetObject, buttonContent, bottomButtonAttribute.Style, bottomButtonAttribute.Height);
                        if (!string.IsNullOrEmpty(bottomButtonAttribute.Group))
                        {
                            bool alreadyHasGroup = false;
                            for (int j = 0; j < buttons.Count; j++)
                            {
                                ApexButtonGroup apexButtonGroup = buttons[j] as ApexButtonGroup;
                                if (apexButtonGroup != null && bottomButtonAttribute.Group == apexButtonGroup.name)
                                {
                                    apexButtonGroup.Add(apexButton);
                                    buttons[j] = apexButtonGroup;
                                    alreadyHasGroup = true;
                                }
                            }

                            if (!alreadyHasGroup)
                            {
                                buttons.Add(new ApexButtonGroup(bottomButtonAttribute.Group, new List<ApexField>() { apexButton }));
                            }
                        }
                        else
                        {
                            buttons.Add(apexButton);
                        }
                    }
                }
            }
        }

        private void InitializeOnChangeCallback(SerializedProperty property)
        {
            target = ApexReflection.GetDeclaringObjectOfProperty(property);
            if (target != null)
            {
                onChangeCallbacks = new List<MethodInfo>();
                OnChangeCallbackAttribute[] onChangeCallbackAttributes = ApexReflection.GetAttributes<OnChangeCallbackAttribute>(property);
                for (int i = 0; i < onChangeCallbackAttributes.Length; i++)
                {
                    OnChangeCallbackAttribute onChangeCallbackAttribute = onChangeCallbackAttributes[i];
                    if (ApexReflection.TryDeepFindMethods(target.GetType(), onChangeCallbackAttribute.callback, out MethodInfo[] methods))
                    {
                        for (int j = 0; j < methods.Length; j++)
                        {
                            MethodInfo method = methods[i];
                            if(method.GetParameters().Length == 0)
                            {
                                onChangeCallbacks.Add(method);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region [ApexSerializedField Implementation]
        public override void DrawFieldLayout()
        {
            Rect position = GUILayoutUtility.GetRect(0, GetFieldHeight());
            DrawField(position);
            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
        }

        public void DrawField(Rect position, GUIContent label)
        {
            Rect storedPropertyPosition = position;
            storedPropertyPosition.height = GetPropertyHeight();
            position.height = storedPropertyPosition.height;

            ValidateProperty();

            Validators_BeforePropertyGUICallbacks(ref position);
            Painters_BeforePropertyGUICallbacks(ref position);

            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginDisabledGroup(readOnlyAttribute != null || !IsActive());
            if (propertyDrawer != null)
            {
                propertyDrawer.OnGUI(position, TargetSerializedProperty, label);
            }
            else if (propertyView != null)
            {
                propertyView.OnGUI(position, TargetSerializedProperty, label);
            }
            else
            {
                DefaultPropertyGUI(position, label);
            }
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                CallOnChangeCallbacks();
            }

            currentYPosition = position.y + storedPropertyPosition.height;
            Validators_AfterPropertyGUICallback(storedPropertyPosition);
            Painters_AfterPropertyGUICallbacks(storedPropertyPosition);

            DrawBottomButtons(storedPropertyPosition);
        }

        public override void DrawField(Rect position)
        {
            DrawField(position, displayLabel);
        }

        /// <summary>
        /// Get total height with validators, painters and etc.
        /// </summary>
        /// <returns></returns>
        public override float GetFieldHeight()
        {
            return GetPropertyHeight() + GetValidatorsHeight() + GetPaintersHeight() + GetBottomButtonHeight();
        }

        public override bool IsVisible()
        {
            if (visibleConditions != null)
            {
                for (int i = 0; i < visibleConditions.Length; i++)
                {
                    if (!visibleConditions[i].GetConditionResult(TargetSerializedProperty))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public virtual bool IsActive()
        {
            if (activeConditions != null)
            {
                for (int i = 0; i < activeConditions.Length; i++)
                {
                    if (!activeConditions[i].GetConditionResult(TargetSerializedProperty))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion

        #region [Validator Callbacks]
        protected void ValidateProperty()
        {
            if (propertyValidators != null && propertyValidators.Length > 0)
            {
                for (int i = 0; i < propertyValidators.Length; i++)
                {
                    propertyValidators[i].Validate(TargetSerializedProperty);
                }
            }
        }

        protected void Validators_BeforePropertyGUICallbacks(ref Rect position)
        {
            if (propertyValidators != null)
            {
                for (int i = 0; i < propertyValidators.Length; i++)
                {
                    PropertyValidator propertyValidator = propertyValidators[i];

                    // Receiver callback.
                    IPropertyPositionModifyReceiver receiver = propertyValidator as IPropertyPositionModifyReceiver;
                    if (receiver != null)
                    {
                        receiver.ModifyPropertyPosition(ref position);
                        validatorsRects[i] = position;
                    }

                    // Before GUI callback.
                    propertyValidator.BeforePropertyGUI();
                }
            }
        }

        protected void Validators_AfterPropertyGUICallback(Rect propertyPosition)
        {
            if (propertyValidators != null)
            {
                for (int i = 0; i < propertyValidators.Length; i++)
                {
                    PropertyValidator propertyValidator = propertyValidators[i];

                    // After GUI callback.
                    propertyValidator.AfterPropertyGUI();

                    Rect position = validatorsRects[i];
                    position.y = currentYPosition;
                    position.height = propertyValidator.GetValidatorHeight(TargetSerializedProperty, displayLabel);
                    propertyValidator.OnValidatorGUI(propertyPosition, position, TargetSerializedProperty, displayLabel);
                    currentYPosition += position.height;


                }
            }
        }

        public float GetValidatorsHeight()
        {
            if (propertyValidators != null)
            {
                float height = 0;
                for (int i = 0; i < propertyValidators.Length; i++)
                {
                    height += propertyValidators[i].GetValidatorHeight(TargetSerializedProperty, displayLabel);
                }
                return height;
            }
            return 0;
        }
        #endregion

        #region [Painter Callbacks]
        protected void Painters_BeforePropertyGUICallbacks(ref Rect position)
        {
            if (propertyPainters != null)
            {
                for (int i = 0; i < propertyPainters.Length; i++)
                {
                    PropertyPainter propertyPainter = propertyPainters[i];

                    // Receiver callback.
                    IPropertyPositionModifyReceiver receiver = propertyPainter as IPropertyPositionModifyReceiver;
                    if (receiver != null)
                    {
                        receiver.ModifyPropertyPosition(ref position);
                        painterRects[i] = position;
                    }

                    // Before GUI callback.
                    propertyPainter.BeforePropertyGUI();
                }
            }
        }

        protected void Painters_AfterPropertyGUICallbacks(Rect propertyPosition)
        {
            if (propertyPainters != null)
            {
                for (int i = 0; i < propertyPainters.Length; i++)
                {
                    PropertyPainter propertyPainter = propertyPainters[i];

                    // After GUI callback.
                    propertyPainter.AfterPropertyGUI();

                    // Draw painter.
                    Rect position = painterRects[i];
                    position.y = currentYPosition;
                    position.height = propertyPainter.GetPainterHeight(TargetSerializedProperty, displayLabel);
                    propertyPainter.SetPropertyPosition(propertyPosition);
                    propertyPainter.OnPainterGUI(position, TargetSerializedProperty, displayLabel);
                    currentYPosition += position.height + EditorGUIUtility.standardVerticalSpacing;
                }
            }
        }


        protected float GetPaintersHeight()
        {
            if (propertyPainters != null)
            {
                float height = 0;
                for (int i = 0; i < propertyPainters.Length; i++)
                {
                    height += propertyPainters[i].GetPainterHeight(TargetSerializedProperty, displayLabel);
                }
                return height;
            }
            return 0;
        }
        #endregion

        #region [Button Callbacks]
        private void DrawBottomButtons(Rect position)
        {
            if (buttons != null && buttons.Count > 0)
            {
                currentYPosition += 2;
                for (int i = 0; i < buttons.Count; i++)
                {
                    ApexField button = buttons[i];
                    Rect buttonPosition = new Rect(position.x, currentYPosition, position.width, button.GetFieldHeight());
                    button.DrawField(buttonPosition);
                    currentYPosition += button.GetFieldHeight() + EditorGUIUtility.standardVerticalSpacing;
                }
            }
        }

        private float GetBottomButtonHeight()
        {
            float height = 0;
            if (buttons != null && buttons.Count > 0)
            {
                for (int i = 0; i < buttons.Count; i++)
                {
                    height += buttons[i].GetFieldHeight() + EditorGUIUtility.standardVerticalSpacing;
                }
            }
            return height;
        }
        #endregion

        #region [OnChangeCallback]
        protected void CallOnChangeCallbacks()
        {
            if (onChangeCallbacks != null && onChangeCallbacks.Count != 0)
            {
                for (int i = 0; i < onChangeCallbacks.Count; i++)
                {
                    onChangeCallbacks[i].Invoke(target, null);
                }
            }
        }
        #endregion

        /// <summary>
        /// Get only property height without validators, painters and etc.
        /// </summary>
        /// <returns></returns>
        public virtual float GetPropertyHeight()
        {
            if (!IsVisible())
                return 0;

            float height = 0;
            if (propertyDrawer != null)
            {
                height = propertyDrawer.GetPropertyHeight(TargetSerializedProperty, displayLabel);
            }
            else if (propertyView != null)
            {
                height = propertyView.GetPropertyHeight(TargetSerializedProperty, displayLabel);
            }
            else if (!ignoreType && children != null && children.Count > 0)
            {
                height += EditorGUIUtility.singleLineHeight;
                if (TargetSerializedProperty.isExpanded)
                {
                    for (int i = 0; i < children.Count; i++)
                    {
                        ApexField apexField = children[i];
                        if (apexField.IsVisible())
                        {
                            height += apexField.GetFieldHeight() + EditorGUIUtility.standardVerticalSpacing;
                        }
                    }
                }

            }
            else
            {
                height = EditorGUI.GetPropertyHeight(TargetSerializedProperty, displayLabel, true);
            }

            return height;
        }

        public virtual float GetChildrenHeight()
        {
            float height = 0;
            if (children != null)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    ApexField apexField = children[i];
                    if (apexField.IsVisible())
                    {
                        height += apexField.GetFieldHeight() + EditorGUIUtility.standardVerticalSpacing;
                    }
                }
            }
            return height;
        }

        /// <summary>
        /// Default property GUI if property doesn't have PropertyView attribute.
        /// </summary>
        /// <param name="position">Draw property position in Rectangle representation.</param>
        protected virtual void DefaultPropertyGUI(Rect position, GUIContent label)
        {
            if (!ignoreType && TargetSerializedProperty.isArray && TargetSerializedProperty.propertyType != SerializedPropertyType.String)
            {
                position.height = EditorGUIUtility.singleLineHeight;
                TargetSerializedProperty.isExpanded = ApexEditorUtilities.IndentFoldoutGUI(position, TargetSerializedProperty.isExpanded, label, true);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                if (TargetSerializedProperty.isExpanded)
                {
                    position.x += 14;
                    position.width -= 14;
                    SerializedProperty arraySize = children[0].TargetSerializedProperty.Copy();
                    int lastArraySize = arraySize.intValue;
                    arraySize.intValue = EditorGUI.DelayedIntField(position, "Size", arraySize.intValue);
                    if (lastArraySize != arraySize.intValue)
                    {
                        ApplyChildren();
                    }
                    position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    for (int i = 1; i < children.Count; i++)
                    {
                        ApexSerializedField child = children[i];
                        if (child.IsVisible())
                        {
                            child.DrawField(position);
                            position.y += child.GetFieldHeight() + EditorGUIUtility.standardVerticalSpacing;
                        }
                    }
                }
            }
            else if (!ignoreType && children != null)
            {
                position.height = EditorGUIUtility.singleLineHeight;
                TargetSerializedProperty.isExpanded = ApexEditorUtilities.IndentFoldoutGUI(position, TargetSerializedProperty.isExpanded, label, true);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                if (TargetSerializedProperty.isExpanded)
                {
                    position.x += 14;
                    position.width -= 14;
                    DrawChildren(position);
                }
            }
            else
            {
                EditorGUI.PropertyField(position, TargetSerializedProperty, label, true);
            }
        }

        /// <summary>
        /// Draw property children if has.
        /// </summary>
        /// <param name="position">Position to draw children.</param>
        public void DrawChildren(Rect position)
        {
            if (children != null && children.Count > 0)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    ApexField child = children[i];
                    if (child.IsVisible())
                    {
                        child.DrawField(position);
                        position.y += child.GetFieldHeight() + EditorGUIUtility.standardVerticalSpacing;
                    }
                }
            }
        }

        /// <summary>
        /// Initialize all property children.
        /// </summary>
        public void ApplyChildren()
        {
            List<SerializedProperty> copyChildren = TargetSerializedProperty.CopyVisibleChildren();
            if (TargetSerializedProperty.hasVisibleChildren &&
                TargetSerializedProperty.propertyType != SerializedPropertyType.Vector3 &&
                TargetSerializedProperty.propertyType != SerializedPropertyType.Vector2 &&
                TargetSerializedProperty.propertyType != SerializedPropertyType.Vector3Int &&
                TargetSerializedProperty.propertyType != SerializedPropertyType.Vector2Int &&
                copyChildren[0].name != "m_PersistentCalls")
            {
                children = new List<ApexSerializedField>();
                foreach (SerializedProperty child in ApexReflection.GetVisibleChildren(TargetSerializedProperty))
                {
                    ApexProperty property = new ApexProperty(child.Copy());
                    children.Add(property);
                }
                children.Sort(ApexEditor.OrderComparisonCallback);
                LayoutToFoldout(ref children);
            }
        }

        protected void LayoutToFoldout(ref List<ApexSerializedField> properties)
        {
            List<ApexSerializedField> temp = new List<ApexSerializedField>();
            for (int i = 0; i < properties.Count; i++)
            {
                ApexSerializedField property = properties[i];
                FoldoutAttribute foldoutAttribute = ApexReflection.GetAttribute<FoldoutAttribute>(property.TargetSerializedProperty);
                if(foldoutAttribute != null)
                {
                    bool hasFoldout = false;
                    for (int j = 0; j < temp.Count; j++)
                    {
                        ApexFoldout foldout = temp[j] as ApexFoldout;
                        if(foldout != null)
                        {
                            if (foldoutAttribute.name == foldout.GetName() && foldout.GetStyle() != "Header")
                            {
                                foldout.Add(property);
                                temp[j] = foldout;
                                hasFoldout = true;
                                break;
                            }
                        }
                    }

                    if (!hasFoldout)
                    {
                        temp.Add(new ApexFoldout(property.TargetSerializedProperty, foldoutAttribute.name, foldoutAttribute.Style, property));
                    }
                }
                else
                {
                    temp.Add(property);
                }
            }
            properties = temp;
        }

        /// <summary>
        /// Check that property has visible children.
        /// </summary>
        public bool HasChildren()
        {
            return children != null && TargetSerializedProperty.hasVisibleChildren;
        }

        public bool HasCustomDrawer()
        {
            return propertyDrawer != null;
        }

        public void Diactivate()
        {
            SetReadOnlyAttribute(new ReadOnlyAttribute());
        }

        public void Activate()
        {
            SetReadOnlyAttribute(null);
        }



        #region [Getter / Setter]
        public List<ApexSerializedField> GetChildern()
        {
            return children;
        }

        private void SetChildern(List<ApexSerializedField> value)
        {
            children = value;
        }

        public ApexSerializedField GetChild(int index)
        {
            return children[index];
        }

        private void SetChild(int index, ApexProperty value)
        {
            children[index] = value;
        }

        public int GetChildCount()
        {
            return children?.Count ?? 0;
        }

        public PropertyView GetPropertyView()
        {
            return propertyView;
        }

        protected void SetPropertyView(PropertyView value)
        {
            propertyView = value;
        }

        public PropertyValidator[] GetPropertyValidators()
        {
            return propertyValidators;
        }

        protected void SetPropertyValidators(PropertyValidator[] value)
        {
            propertyValidators = value;
        }

        public int GetValidatorAttributesCount()
        {
            return propertyValidators?.Length ?? 0;
        }

        public PropertyCondition[] GetVisibleCondition()
        {
            return visibleConditions;
        }

        protected void SetVisibleCondition(PropertyCondition[] value)
        {
            visibleConditions = value;
        }

        public PropertyCondition[] GetActiveCondition()
        {
            return activeConditions;
        }

        protected void SetActiveCondition(PropertyCondition[] value)
        {
            activeConditions = value;
        }

        public ReadOnlyAttribute GetReadOnlyAttribute()
        {
            return readOnlyAttribute;
        }

        protected void SetReadOnlyAttribute(ReadOnlyAttribute value)
        {
            readOnlyAttribute = value;
        }

        public GUIContent GetDisplayLabel()
        {
            return displayLabel;
        }

        public void SetDisplayLabel(GUIContent value)
        {
            displayLabel = value;
        }

        public bool HasPropertyView()
        {
            return propertyView != null;
        }

        public bool HasValidators()
        {
            return propertyValidators?.Length > 0;
        }

        public bool IsReadOnly()
        {
            return readOnlyAttribute != null;
        }

        public bool HasVisibleCondition()
        {
            return visibleConditions != null;
        }

        public bool HasActiveCondition()
        {
            return activeConditions != null;
        }
        #endregion
    }
}