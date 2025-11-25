/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;
using System;

namespace AuroraFPSRuntime.CoreModules.ValueTypes
{
    [Serializable]
    public struct AnimatorState : IEquatable<AnimatorState>
    {
        // Base animator value properties.
        [SerializeField] private string name;
        [SerializeField] private int layer;
        [SerializeField] private float fixedTime;
        [SerializeField, HideInInspector] private int nameHash;

        /// <summary>
        /// Animator value constructor.
        /// </summary>
        public AnimatorState(string value, int layer, float fixedTime)
        {
            this.name = value;
            this.layer = layer;
            this.fixedTime = fixedTime;
            this.nameHash = Animator.StringToHash(name);
        }

        public readonly static AnimatorState none = new AnimatorState(string.Empty, 0, 0.0f);

        #region [IEquatable Implementation]
        public override bool Equals(object obj)
        {
            return (obj is AnimatorState metrics) && Equals(metrics);
        }

        public bool Equals(string other)
        {
            return nameHash == Animator.StringToHash(other);
        }

        public bool Equals(AnimatorState other)
        {
            return nameHash == other.nameHash;
        }

        public override int GetHashCode()
        {
            return (name, nameHash).GetHashCode();
        }
        #endregion

        #region [Operator Overloading]
        public static implicit operator AnimatorState(string name)
        {
            return new AnimatorState(name, 0, 0.1f);
        }

        public static implicit operator int(AnimatorState animatorValue)
        {
            return animatorValue.GetNameHash();
        }

        public static implicit operator string(AnimatorState animatorValue)
        {
            return animatorValue.GetName();
        }

        public static bool operator ==(AnimatorState left, AnimatorState right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AnimatorState left, AnimatorState right)
        {
            return !Equals(left, right);
        }
        #endregion

        #region [Getter / Setter]
        public string GetName()
        {
            return name;
        }

        public void SetName(string value)
        {
            this.name = value;
            nameHash = Animator.StringToHash(this.name);
        }

        public int GetLayer()
        {
            return layer;
        }

        public void SetLayer(int value)
        {
            layer = value;
        }

        public float GetFixedTime()
        {
            return fixedTime;
        }

        public void SetFixedTime(float value)
        {
            fixedTime = value;
        }

        public int GetNameHash()
        {
            return nameHash;
        }

        private void SetNameHash(int value)
        {
            nameHash = value;
        }
        #endregion
    }
}