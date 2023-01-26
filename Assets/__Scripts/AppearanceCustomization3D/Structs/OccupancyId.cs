using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AppearanceCustomization3D {
    [System.Serializable]
    public struct OccupancyId : IEquatable<OccupancyId>
    {
        [SerializeField]
        private uint _value;
        public uint Value => _value;

        public OccupancyId(uint value) {
            _value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is OccupancyId other && Equals(other);
        }

        public bool Equals(OccupancyId other)
        {
            return other._value == _value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_value);
        }

        public static bool operator ==(OccupancyId id1, OccupancyId id2) => id1._value == id2._value;
        public static bool operator !=(OccupancyId id1, OccupancyId id2) => id1._value != id2._value;

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
