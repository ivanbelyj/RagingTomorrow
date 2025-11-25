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

namespace AuroraFPSEditor.Attributes
{
    public sealed class PropertyCondition
    {
        private ConditionAttribute conditionData;

        public PropertyCondition(ConditionAttribute conditionData)
        {
            this.conditionData = conditionData;
        }

        public bool GetConditionResult(SerializedProperty property)
        {
            SerializedProperty parentProperty = ApexReflection.GetParentProperty(property);
            SerializedProperty serialized_FirstProperty = parentProperty.FindPropertyRelative(conditionData.FirstProperty) ?? property.serializedObject.FindProperty(conditionData.FirstProperty);
            if (serialized_FirstProperty != null && serialized_FirstProperty.propertyType == SerializedPropertyType.Boolean && string.IsNullOrEmpty(conditionData.SecondProperty) && string.IsNullOrEmpty(conditionData.NumericCondition))
            {
                return serialized_FirstProperty.boolValue == conditionData.BoolCondition;
            }
            else if (serialized_FirstProperty != null && serialized_FirstProperty.propertyType == SerializedPropertyType.ObjectReference && !string.IsNullOrEmpty(conditionData.SecondProperty))
            {
                if(conditionData.SecondProperty == "NotNull")
                {
                    return serialized_FirstProperty.objectReferenceValue != null;
                }
                else if(conditionData.SecondProperty == "Null")
                {
                    return serialized_FirstProperty.objectReferenceValue == null;
                }
                else
                {
                    return false;
                }
            }
            else if(serialized_FirstProperty != null && serialized_FirstProperty.propertyType == SerializedPropertyType.Enum && !string.IsNullOrEmpty(conditionData.SecondProperty))
            {
                string[] enumValues = conditionData.SecondProperty.Split(',');
                for (int i = 0; i < enumValues.Length; i++)
                {
                    if(serialized_FirstProperty.enumNames[serialized_FirstProperty.enumValueIndex] == enumValues[i])
                    {
                        return true;
                    }
                }
                return false;
            }
            else if(serialized_FirstProperty != null && serialized_FirstProperty.propertyType == SerializedPropertyType.String)
            {
                SerializedProperty serialized_SecondProperty = parentProperty.FindPropertyRelative(conditionData.SecondProperty) ?? property.serializedObject.FindProperty(conditionData.SecondProperty);
                if (serialized_SecondProperty != null && serialized_SecondProperty.propertyType == SerializedPropertyType.String)
                {
                    return serialized_FirstProperty.stringValue == serialized_SecondProperty.stringValue;
                }
                else if (!string.IsNullOrEmpty(conditionData.SecondProperty))
                {
                    return serialized_FirstProperty.stringValue == conditionData.SecondProperty;
                }
                return false;
            }
            else if (serialized_FirstProperty != null && serialized_FirstProperty.propertyType != SerializedPropertyType.Boolean)
            {

                SerializedProperty serialized_SecondProperty = parentProperty.FindPropertyRelative(conditionData.SecondProperty) ?? property.serializedObject.FindProperty(conditionData.SecondProperty);
                switch (conditionData.NumericCondition)
                {
                    case "==":
                        if (serialized_FirstProperty.propertyType == SerializedPropertyType.Float)
                        {
                            if (serialized_SecondProperty != null && serialized_SecondProperty.propertyType == SerializedPropertyType.Float)
                            {
                                return serialized_FirstProperty.floatValue == serialized_SecondProperty.floatValue;
                            }
                            else if (serialized_SecondProperty == null && float.TryParse(conditionData.SecondProperty, out float result))
                            {
                                return serialized_FirstProperty.floatValue == result;
                            }
                            return true;
                        }
                        else if (serialized_FirstProperty.propertyType == SerializedPropertyType.Integer)
                        {
                            if (serialized_SecondProperty != null && serialized_SecondProperty.propertyType == SerializedPropertyType.Integer)
                            {
                                return serialized_FirstProperty.intValue == serialized_SecondProperty.intValue;
                            }
                            else if (serialized_SecondProperty == null && int.TryParse(conditionData.SecondProperty, out int result))
                            {
                                return serialized_FirstProperty.intValue == result;
                            }
                            return true;
                        }
                        return true;
                    case "!=":
                        if (serialized_FirstProperty.propertyType == SerializedPropertyType.Float)
                        {
                            if (serialized_SecondProperty != null && serialized_SecondProperty.propertyType == SerializedPropertyType.Float)
                            {
                                return serialized_FirstProperty.floatValue != serialized_SecondProperty.floatValue;
                            }
                            else if (serialized_SecondProperty == null && float.TryParse(conditionData.SecondProperty, out float result))
                            {
                                return serialized_FirstProperty.floatValue != result;
                            }
                            return true;
                        }
                        else if (serialized_FirstProperty.propertyType == SerializedPropertyType.Integer)
                        {
                            if (serialized_SecondProperty != null && serialized_SecondProperty.propertyType == SerializedPropertyType.Integer)
                            {
                                return serialized_FirstProperty.intValue != serialized_SecondProperty.intValue;
                            }
                            else if (serialized_SecondProperty == null && int.TryParse(conditionData.SecondProperty, out int result))
                            {
                                return serialized_FirstProperty.intValue != result;
                            }
                            return true;
                        }
                        return true;
                    case "<=":
                        if (serialized_FirstProperty.propertyType == SerializedPropertyType.Float)
                        {
                            if (serialized_SecondProperty != null && serialized_SecondProperty.propertyType == SerializedPropertyType.Float)
                            {
                                return serialized_FirstProperty.floatValue <= serialized_SecondProperty.floatValue;
                            }
                            else if (serialized_SecondProperty == null && float.TryParse(conditionData.SecondProperty, out float result))
                            {
                                return serialized_FirstProperty.floatValue <= result;
                            }
                            return true;
                        }
                        else if (serialized_FirstProperty.propertyType == SerializedPropertyType.Integer)
                        {
                            if (serialized_SecondProperty != null && serialized_SecondProperty.propertyType == SerializedPropertyType.Integer)
                            {
                                return serialized_FirstProperty.intValue <= serialized_SecondProperty.intValue;
                            }
                            else if (serialized_SecondProperty == null && int.TryParse(conditionData.SecondProperty, out int result))
                            {
                                return serialized_FirstProperty.intValue <= result;
                            }
                            return true;
                        }
                        return true;
                    case ">=":
                        if (serialized_FirstProperty.propertyType == SerializedPropertyType.Float)
                        {
                            if (serialized_SecondProperty != null && serialized_SecondProperty.propertyType == SerializedPropertyType.Float)
                            {
                                return serialized_FirstProperty.floatValue >= serialized_SecondProperty.floatValue;
                            }
                            else if (serialized_SecondProperty == null && float.TryParse(conditionData.SecondProperty, out float result))
                            {
                                return serialized_FirstProperty.floatValue >= result;
                            }
                            return true;
                        }
                        else if (serialized_FirstProperty.propertyType == SerializedPropertyType.Integer)
                        {
                            if (serialized_SecondProperty != null && serialized_SecondProperty.propertyType == SerializedPropertyType.Integer)
                            {
                                return serialized_FirstProperty.intValue >= serialized_SecondProperty.intValue;
                            }
                            else if (serialized_SecondProperty == null && int.TryParse(conditionData.SecondProperty, out int result))
                            {
                                return serialized_FirstProperty.intValue >= result;
                            }
                            return true;
                        }
                        return true;
                    case "<":
                        if (serialized_FirstProperty.propertyType == SerializedPropertyType.Float)
                        {
                            if (serialized_SecondProperty != null && serialized_SecondProperty.propertyType == SerializedPropertyType.Float)
                            {
                                return serialized_FirstProperty.floatValue < serialized_SecondProperty.floatValue;
                            }
                            else if (serialized_SecondProperty == null && float.TryParse(conditionData.SecondProperty, out float result))
                            {
                                return serialized_FirstProperty.floatValue < result;
                            }
                            return true;
                        }
                        else if (serialized_FirstProperty.propertyType == SerializedPropertyType.Integer)
                        {
                            if (serialized_SecondProperty != null && serialized_SecondProperty.propertyType == SerializedPropertyType.Integer)
                            {
                                return serialized_FirstProperty.intValue < serialized_SecondProperty.intValue;
                            }
                            else if (serialized_SecondProperty == null && int.TryParse(conditionData.SecondProperty, out int result))
                            {
                                return serialized_FirstProperty.intValue < result;
                            }
                            return true;
                        }
                        return true;
                    case ">":
                        if (serialized_FirstProperty.propertyType == SerializedPropertyType.Float)
                        {
                            if (serialized_SecondProperty != null && serialized_SecondProperty.propertyType == SerializedPropertyType.Float)
                            {
                                return serialized_FirstProperty.floatValue > serialized_SecondProperty.floatValue;
                            }
                            else if (serialized_SecondProperty == null && float.TryParse(conditionData.SecondProperty, out float result))
                            {
                                return serialized_FirstProperty.floatValue > result;
                            }
                            return true;
                        }
                        else if (serialized_FirstProperty.propertyType == SerializedPropertyType.Integer)
                        {
                            if (serialized_SecondProperty != null && serialized_SecondProperty.propertyType == SerializedPropertyType.Integer)
                            {
                                return serialized_FirstProperty.intValue > serialized_SecondProperty.intValue;
                            }
                            else if (serialized_SecondProperty == null && int.TryParse(conditionData.SecondProperty, out int result))
                            {
                                return serialized_FirstProperty.intValue > result;
                            }
                            return true;
                        }
                        return true;
                }

            }
            return true;
        }
    }
}