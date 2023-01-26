using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AppearanceCustomization3D {
    [System.Serializable]
    public struct AppearanceElementLocalId : IEquatable<AppearanceElementLocalId>
    {
        [SerializeField]
        private uint _value;

        public AppearanceElementLocalId(uint value) {
            _value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is AppearanceElementLocalId other && Equals(other);
        }

        public bool Equals(AppearanceElementLocalId other)
        {
            return other._value == _value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_value);
        }

        public static bool operator ==(AppearanceElementLocalId id1, AppearanceElementLocalId id2)
            => id1._value == id2._value;
        public static bool operator !=(AppearanceElementLocalId id1, AppearanceElementLocalId id2)
            => id1._value != id2._value;
    }
}
