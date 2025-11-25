/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;

namespace AuroraFPSRuntime.CoreModules.ValueTypes
{
    [System.Serializable]
    public struct RangedFloat
    {
        [SerializeField] private float min;
        [SerializeField] private float max;
        [SerializeField] private float minLimit;
        [SerializeField] private float maxLimit;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="min">Min value.</param>
        /// <param name="max">Max value.</param>
        public RangedFloat(float min, float max)
        {
            this.min = min;
            this.max = max;
            this.minLimit = min - 1;
            this.maxLimit = max + 1;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="min">Min value.</param>
        /// <param name="max">Max value.</param>
        public RangedFloat(float min, float max, float minLimit, float maxLimit)
        {
            this.min = min;
            this.max = max;
            this.minLimit = minLimit;
            this.maxLimit = maxLimit;
        }

        #region [Override Operators]
        public static bool operator ==(RangedFloat left, RangedFloat right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RangedFloat left, RangedFloat right)
        {
            return !Equals(left, right);
        }

        public override bool Equals(object obj)
        {
            return (obj is RangedFloat metrics) && Equals(metrics);
        }

        public bool Equals(RangedFloat other)
        {
            return (min, max) == (other.min, other.max);
        }

        public override int GetHashCode()
        {
            return (min, max).GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Min: {0}, Max: {1}, Min Limit: {2}, Max Limit: {3}", min, max, minLimit, maxLimit);
        }
        #endregion

        #region [Getter / Setter]
        public float GetMin()
        {
            return min;
        }

        public void SetMin(float value)
        {
            min = value > minLimit ? value : minLimit;
        }

        public float GetMax()
        {
            return max;
        }

        public void SetMax(float value)
        {
            max = value < maxLimit ? value : maxLimit;
        }

        public float GetMinLimit()
        {
            return minLimit;
        }

        public void SetMinLimit(float value)
        {
            minLimit = value;
        }

        public float GetMaxLimit()
        {
            return maxLimit;
        }

        public void SetMaxLimit(float value)
        {
            maxLimit = value;
        }
        #endregion
    }
}

