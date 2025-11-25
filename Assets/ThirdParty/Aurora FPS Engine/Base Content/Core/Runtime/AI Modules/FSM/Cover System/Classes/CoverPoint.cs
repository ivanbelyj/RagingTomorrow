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

namespace AuroraFPSRuntime.AIModules.CoverSystem
{
    [System.Serializable]
    public sealed class CoverPoint
    {
        [SerializeField]
        [NotNull]
        private Transform point;

        [SerializeField]
        [Slider(0, 360)]
        private float angle = 90;

        [SerializeField]
        private bool isOccupied;

        #region [Aurora Engine Debug Directive]
#if AURORA_ENGINE_DEBUG && UNITY_EDITOR
        [SerializeField]
        internal bool Visualize;
#endif
        #endregion
        public bool IsCover(Transform relative)
        {
            Vector3 difference = relative.position - point.position;
            return Vector3.Angle(point.forward, difference) <= (angle / 2);
        }

        #region [Getter / Setter]
        public Transform GetPoint()
        {
            return point;
        }

        public void SetPoint(Transform value)
        {
            point = value;
        }

        public float GetForwardTolerance()
        {
            return angle;
        }

        public void SetForwardTolerance(float value)
        {
            angle = value;
        }

        public bool IsOccupied()
        {
            return isOccupied;
        }

        public void IsOccupied(bool value)
        {
            isOccupied = value;
        }
        #endregion
    }
}