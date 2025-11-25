/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using Vexe.Runtime.Extensions;
using System.Reflection;
using UnityEngine;

namespace AuroraFPSRuntime.CoreModules.ValueTypes
{
    [System.Serializable]
    public sealed partial class CustomEvent
    {
        public static readonly Parameter[] EmptyParameters = new Parameter[0];
        private static readonly object[] EmptyObjectParameters = new object[0];

        [SerializeField]
        [Label("Component")]
        [NotEmpty]
        private string type;

        [SerializeField]
        [NotEmpty]
        private string function;

        [SerializeField]
        [ReorderableList]
        private Parameter[] parameters;

        private object eventTarget;
        private MethodCaller<object, object> callbackEvent;
        private object[] eventParameters;

        public void Initialize(Component owner)
        {
            InitializeTarget(owner);
            InitializeCallback();            
        }

        private void InitializeTarget(Component owner)
        {
            eventTarget = owner.GetComponent(type);
        }

        private void InitializeCallback()
        {
            if (eventTarget != null)
            {
                System.Type type = eventTarget.GetType();
                eventParameters = GetObjectParametersRepresentation(parameters);
                do
                {
                    MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                    if (methodInfos != null)
                    {
                        for (int i = 0; i < methodInfos.Length; i++)
                        {
                            MethodInfo methodInfo = methodInfos[i];
                            if (methodInfo.Name == function)
                            {
                                ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                                if ((eventParameters != null && parameterInfos != null) && (eventParameters.Length == parameterInfos.Length))
                                {
                                    bool validParameters = true;
                                    for (int j = 0; j < eventParameters.Length; j++)
                                    {
                                        ParameterInfo parameterInfo = parameterInfos[j];
                                        object objectParameter = eventParameters[j];
                                        if (parameterInfo.ParameterType != objectParameter.GetType())
                                        {
                                            validParameters = false;
                                            break;
                                        }
                                    }

                                    if (validParameters)
                                    {
                                        callbackEvent = methodInfo.DelegateForCall();
                                    }
                                }
                                else if (eventParameters == null && parameterInfos == null)
                                {
                                    callbackEvent = methodInfo.DelegateForCall();
                                }
                            }
                        }
                    }
                    type = type.BaseType;
                } while (type != null);
            }
            else
            {
                parameters = EmptyParameters;
                eventParameters = EmptyObjectParameters;
            }
        }

        public void Invoke()
        {
            if(eventTarget != null && callbackEvent != null)
            {
                callbackEvent.Invoke(eventTarget, eventParameters);
            }
        }

        public static bool IsValidMethod(MethodInfo method)
        {
            return method.ReturnType == typeof(void);
        }

        private object[] GetObjectParametersRepresentation(Parameter[] parameters)
        {
            if (parameters != null)
            {
                object[] objectParameters = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    Parameter parameter = parameters[i];
                    switch (parameter.GetParameterType())
                    {
                        case Parameter.ParameterType.Float:
                            objectParameters[i] = parameter.GetFloatParameter();
                            break;
                        case Parameter.ParameterType.Integer:
                            objectParameters[i] = parameter.GetIntegerParameter();
                            break;
                        case Parameter.ParameterType.Boolean:
                            objectParameters[i] = parameter.GetBoolParameter();
                            break;
                        case Parameter.ParameterType.String:
                            objectParameters[i] = parameter.GetStringParameter();
                            break;
                        case Parameter.ParameterType.Vector3:
                            objectParameters[i] = parameter.GetVector3Parameter();
                            break;
                        case Parameter.ParameterType.Object:
                            objectParameters[i] = parameter.GetObjectParameter();
                            break;
                    }
                }
                return objectParameters;
            }
            return null;
        }
    }
}
