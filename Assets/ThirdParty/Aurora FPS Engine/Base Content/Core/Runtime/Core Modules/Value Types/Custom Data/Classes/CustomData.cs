/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.CoreModules.Serialization.Collections;
using UnityEngine;

namespace AuroraFPSRuntime.CoreModules.ValueTypes
{
    [System.Serializable]
    public class CustomValue
    {
        public static readonly float DefalutNumber = 0.0f;
        public static readonly string DefalutString = string.Empty;
        public static readonly Quaternion DefalutAxes = Quaternion.identity;
        public static readonly Object DefalutObject = null;

        public enum ValueType
        {
            Integer,
            Float,
            String,
            Bool,
            Vector2,
            Vector3,
            Quaternion,
            Object
        }

        [SerializeField]
        public ValueType valueType = ValueType.Integer;

        [SerializeField]
        public float numberValue = DefalutNumber;

        [SerializeField]
        public string stringValue = DefalutString;

        [SerializeField]
        public Quaternion axesValue = DefalutAxes;

        [SerializeField]
        public Object objectValue = DefalutObject;
    }

    [System.Serializable]
    public sealed class CustomDatas : SerializableDictionary<string, CustomValue>
    {
        [SerializeField]
        private string[] keys;

        [SerializeField]
        private CustomValue[] values;

        protected override string[] GetKeys()
        {
            return keys;
        }

        protected override CustomValue[] GetValues()
        {
            return values;
        }

        protected override void SetKeys(string[] keys)
        {
            this.keys = keys;
        }

        protected override void SetValues(CustomValue[] values)
        {
            this.values = values;
        }
    }
}
