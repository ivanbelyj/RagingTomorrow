/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Variables
{
    [System.Serializable]
    public abstract class TreeVariable
    {
        public abstract object GetValueObject();
        public abstract void SetValueObject(object value);
        public abstract Type GetVariableType();
    }

    [System.Serializable]
    public abstract class TreeVariable<T> : TreeVariable
    {
        [SerializeField]
        private T value;

        #region [Getter / Setter]
        public override object GetValueObject()
        {
            return value;
        }

        public override void SetValueObject(object value)
        {
            this.value = (T)value;
        }

        public override Type GetVariableType()
        {
            return typeof(T);
        }

        public T GetValue()
        {
            return value;
        }

        public void SetValue(T value)
        {
            this.value = value;
        }
        #endregion
    }
}