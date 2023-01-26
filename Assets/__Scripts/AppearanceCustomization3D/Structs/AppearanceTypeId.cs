using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AppearanceCustomization3D {
    [System.Serializable]
    public struct AppearanceTypeId : IEquatable<AppearanceTypeId>
    {
        [SerializeField]
        private string _value;
        public string Value => _value;

        public AppearanceTypeId(string value) {
            _value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is AppearanceTypeId other && Equals(other);
        }

        public bool Equals(AppearanceTypeId other)
        {
            return other._value == _value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_value);
        }

        public static bool operator ==(AppearanceTypeId id1, AppearanceTypeId id2)
            => id1._value == id2._value;
        public static bool operator !=(AppearanceTypeId id1, AppearanceTypeId id2)
            => id1._value != id2._value;

        public override string ToString()
        {
            return _value;
        }
    }
}
