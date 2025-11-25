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

namespace AuroraFPSRuntime.CoreModules.ValueTypes
{
    [Serializable]
    public struct AnimatorParameter : IEquatable<AnimatorParameter>
    {
        // Base animator value properties.
        [SerializeField] 
        [NotEmpty]
        private string name;

        [SerializeField]
        [HideInInspector] 
        private int nameHash;

        /// <summary>
        /// Animator value constructor.
        /// </summary>
        public AnimatorParameter(string value)
        {
            this.name = value;
            this.nameHash = Animator.StringToHash(name);
        }

        public readonly static AnimatorParameter none = new AnimatorParameter(string.Empty);

        #region [IEquatable Implementation]
        public override bool Equals(object obj)
        {
            return (obj is AnimatorParameter metrics) && Equals(metrics);
        }

        public bool Equals(string other)
        {
            return nameHash == Animator.StringToHash(other);
        }

        public bool Equals(AnimatorParameter other)
        {
            return nameHash == other.nameHash;
        }

        public override int GetHashCode()
        {
            return (name, nameHash).GetHashCode();
        }
        #endregion

        #region [Operator Overloading]
        public static implicit operator AnimatorParameter(string name)
        {
            return new AnimatorParameter(name);
        }

        public static implicit operator int(AnimatorParameter animatorValue)
        {
            return animatorValue.GetNameHash();
        }

        public static bool operator ==(AnimatorParameter left, AnimatorParameter right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AnimatorParameter left, AnimatorParameter right)
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
            name = value;
            nameHash = Animator.StringToHash(this.name);
        }

        public int GetNameHash()
        {
            if(nameHash == 0)
            {
                RegerenateHash();
            }
            return nameHash;
        }

        public void RegerenateHash()
        {
            nameHash = Animator.StringToHash(this.name);
        }
        #endregion
    }
}