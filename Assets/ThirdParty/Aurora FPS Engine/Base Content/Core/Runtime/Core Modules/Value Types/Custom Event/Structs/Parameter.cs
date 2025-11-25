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
using Object = UnityEngine.Object;

namespace AuroraFPSRuntime.CoreModules.ValueTypes
{
    public sealed partial class CustomEvent
    {
        [Serializable]
        public struct Parameter
        {
            public enum ParameterType
            {
                Float,
                Integer,
                Boolean,
                String,
                Vector3,
                Object
            }

            [SerializeField]
            [Label("Parameter Type")]
            private ParameterType parameterType;

            [SerializeField]
            [Label("Float")]
            [VisibleIf("parameterType", "Float")]
            private float floatParameter;

            [SerializeField]
            [Label("Integer")]
            [VisibleIf("parameterType", "Integer")]
            private int integerParameter;

            [SerializeField]
            [Label("Boolean")]
            [VisibleIf("parameterType", "Bool")]
            private bool booleanParameter;

            [SerializeField]
            [Label("String")]
            [VisibleIf("parameterType", "String")]
            private string stringParameter;

            [SerializeField]
            [Label("Vector3")]
            [VisibleIf("parameterType", "Vector3")]
            private Vector3 vector3Parameter;

            [SerializeField]
            [Label("Object")]
            [VisibleIf("parameterType", "Object")]
            private Object objectParameter;

            public static bool IsValidType(Type type)
            {
                return type != null &&
                    (type == typeof(float) ||
                    type == typeof(int) ||
                    type == typeof(bool) ||
                    type == typeof(string) ||
                    type == typeof(Vector3) ||
                    type.IsSubclassOf(typeof(Object)));
            }

            #region [Getter / Setter]
            public ParameterType GetParameterType()
            {
                return parameterType;
            }

            public void SetParameterType(ParameterType value)
            {
                parameterType = value;
            }

            public float GetFloatParameter()
            {
                return floatParameter;
            }

            public void SetFloatParameter(float value)
            {
                floatParameter = value;
            }

            public int GetIntegerParameter()
            {
                return integerParameter;
            }

            public void SetIntegerParameter(int value)
            {
                integerParameter = value;
            }

            public bool GetBoolParameter()
            {
                return booleanParameter;
            }

            public void SetBoolParameter(bool value)
            {
                booleanParameter = value;
            }

            public string GetStringParameter()
            {
                return stringParameter;
            }

            public void SetStringParameter(string value)
            {
                stringParameter = value;
            }

            public Vector3 GetVector3Parameter()
            {
                return vector3Parameter;
            }

            public void SetVector3Parameter(Vector3 value)
            {
                vector3Parameter = value;
            }

            public Object GetObjectParameter()
            {
                return objectParameter;
            }

            public void SetObjectParameter(Object value)
            {
                objectParameter = value;
            }
            #endregion
        }
    }
}
