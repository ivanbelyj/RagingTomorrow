/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System;
using System.Reflection;
using UnityEngine;

namespace AuroraFPSRuntime.CoreModules.ValueTypes
{
    [Serializable]
    public class CallbackEvent
    {
        [SerializeField]
        private Component target;

        [SerializeField]
        private string eventName;

        public void RegisterCallback(Action callback)
        {
            if (target != null)
            {
                EventInfo[] eventInfos = target.GetType().GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                for (int i = 0; i < eventInfos.Length; i++)
                {
                    EventInfo eventInfo = eventInfos[i];
                    if (eventInfo.Name == eventName)
                    {
                        MethodInfo methodInfo = eventInfo.EventHandlerType.GetMethods()[0];
                        if (methodInfo.GetParameters().Length == 0)
                        {
                            Action internalCallback = () => callback?.Invoke();
                            Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, internalCallback.Target, internalCallback.Method);
                            eventInfo.AddEventHandler(target, handler);
                        }
                        else if (methodInfo.GetParameters().Length == 1)
                        {
                            Action<object> internalCallback = (_arg) => callback?.Invoke();
                            Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, internalCallback.Target, internalCallback.Method);
                            eventInfo.AddEventHandler(target, handler);
                        }
                        else if (methodInfo.GetParameters().Length == 2)
                        {
                            Action<object, object> internalCallback = (_arg1, _arg2) => callback?.Invoke();
                            Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, internalCallback.Target, internalCallback.Method);
                            eventInfo.AddEventHandler(target, handler);
                        }
                        else if (methodInfo.GetParameters().Length == 3)
                        {
                            Action<object, object, object> internalCallback = (_arg1, _arg2, _arg3) => callback?.Invoke();
                            Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, internalCallback.Target, internalCallback.Method);
                            eventInfo.AddEventHandler(target, handler);
                        }
                    }
                }
            }
        }

        #region [Getter / Setter]

        public Component GetTarget()
        {
            return target;
        }

        public void SetTarget(Component value)
        {
            target = value;
        }

        public string GetEventName()
        {
            return eventName;
        }

        public void SetEventName(string value)
        {
            eventName = value;
        }
        #endregion
    }

    [Serializable]
    public class CallbackEvent<TArg>
    {
        [SerializeField]
        private Component target;

        [SerializeField]
        private string eventName;

        [SerializeField]
        private TArg arg;

        public void RegisterCallback(Action<TArg> callback)
        {
            if(target != null)
            {
                EventInfo[] eventInfos = target.GetType().GetEvents(BindingFlags.Public | BindingFlags.Instance);
                for (int i = 0; i < eventInfos.Length; i++)
                {
                    EventInfo eventInfo = eventInfos[i];
                    if (eventInfo.Name == eventName)
                    {
                        MethodInfo methodInfo = eventInfo.EventHandlerType.GetMethods()[0];
                        if (methodInfo.GetParameters().Length == 0)
                        {
                            Action internalCallback = () => callback?.Invoke(arg);
                            Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, internalCallback.Target, internalCallback.Method);
                            eventInfo.AddEventHandler(target, handler);
                        }
                        else if (methodInfo.GetParameters().Length == 1)
                        {
                            Action<object> internalCallback = (_arg) => callback?.Invoke(arg);
                            Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, internalCallback.Target, internalCallback.Method);
                            eventInfo.AddEventHandler(target, handler);
                        }
                        else if (methodInfo.GetParameters().Length == 2)
                        {
                            Action<object, object> internalCallback = (_arg1, _arg2) => callback?.Invoke(arg);
                            Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, internalCallback.Target, internalCallback.Method);
                            eventInfo.AddEventHandler(target, handler);
                        }
                        else if (methodInfo.GetParameters().Length == 3)
                        {
                            Action<object, object, object> internalCallback = (_arg1, _arg2, _arg3) => callback?.Invoke(arg);
                            Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, internalCallback.Target, internalCallback.Method);
                            eventInfo.AddEventHandler(target, handler);
                        }
                    }
                }
            }
        }

        #region [Getter / Setter]
        public TArg GetArg()
        {
            return arg;
        }

        public void SetArg(TArg value)
        {
            this.arg = value;
        }

        public Component GetTarget()
        {
            return target;
        }

        public void SetTarget(Component value)
        {
            target = value;
        }

        public string GetEventName()
        {
            return eventName;
        }

        public void SetEventName(string value)
        {
            eventName = value;
        }
        #endregion
    }

    [Serializable]
    public class CallbackEvent<TArg1, TArg2>
    {
        [SerializeField]
        private Component target;

        [SerializeField]
        private string eventName;

        [SerializeField]
        private TArg1 arg1;

        [SerializeField]
        private TArg2 arg2;

        public void RegisterCallback(Action<TArg1, TArg2> callback)
        {
            if (target != null)
            {
                EventInfo[] eventInfos = target.GetType().GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                for (int i = 0; i < eventInfos.Length; i++)
                {
                    EventInfo eventInfo = eventInfos[i];
                    if (eventInfo.Name == eventName)
                    {
                        MethodInfo methodInfo = eventInfo.EventHandlerType.GetMethods()[0];
                        if (methodInfo.GetParameters().Length == 0)
                        {
                            Action internalCallback = () => callback?.Invoke(arg1,arg2);
                            Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, internalCallback.Target, internalCallback.Method);
                            eventInfo.AddEventHandler(target, handler);
                        }
                        else if (methodInfo.GetParameters().Length == 1)
                        {
                            Action<object> internalCallback = (arg) => callback?.Invoke(arg1, arg2);
                            Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, internalCallback.Target, internalCallback.Method);
                            eventInfo.AddEventHandler(target, handler);
                        }
                        else if (methodInfo.GetParameters().Length == 2)
                        {
                            Action<object, object> internalCallback = (_arg1, _arg2) => callback?.Invoke(arg1, arg2);
                            Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, internalCallback.Target, internalCallback.Method);
                            eventInfo.AddEventHandler(target, handler);
                        }
                        else if (methodInfo.GetParameters().Length == 3)
                        {
                            Action<object, object, object> internalCallback = (_arg1, _arg2, _arg3) => callback?.Invoke(arg1, arg2);
                            Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, internalCallback.Target, internalCallback.Method);
                            eventInfo.AddEventHandler(target, handler);
                        }
                    }
                }
            }
        }
        #region [Getter / Setter]
        public TArg1 GetArg1()
        {
            return arg1;
        }

        public void SetArg1(TArg1 value)
        {
            arg1 = value;
        }

        public TArg2 GetArg2()
        {
            return arg2;
        }

        public void SetArg2(TArg2 value)
        {
            arg2 = value;
        }

        public Component GetTarget()
        {
            return target;
        }

        public void SetTarget(Component value)
        {
            target = value;
        }

        public string GetEventName()
        {
            return eventName;
        }

        public void SetEventName(string value)
        {
            eventName = value;
        }
        #endregion
    }
}