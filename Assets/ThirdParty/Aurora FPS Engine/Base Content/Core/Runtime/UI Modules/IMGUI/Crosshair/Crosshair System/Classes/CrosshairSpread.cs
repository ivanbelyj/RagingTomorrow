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

namespace AuroraFPSRuntime.UIModules.IMGUI.Crosshair
{
    [System.Serializable]
    public struct CrosshairSpread
    {
        [SerializeField] 
        [Label("Spread")]
        [MinValue(0.0f)]
        private float value;

        [SerializeField] 
        [MinValue(0.0f)]
        private float speed;

        /// <summary>
        /// CrosshairSpread constructor.
        /// </summary>
        /// <param name="value">Spread value.</param>
        /// <param name="speed">Speed of step to calculate spread value.</param>
        public CrosshairSpread(float value, float speed)
        {
            this.value = value;
            this.speed = speed;
        }

        #region [Default Values]
        public readonly static CrosshairSpread zero = new CrosshairSpread(0.0f, 0.0f);
        #endregion

        #region [Getter / Setter]
        public float GetValue()
        {
            return value;
        }

        public void SetValue(float value)
        {
            this.value = value;
        }

        public float GetSpeed()
        {
            return speed;
        }

        public void SetSpeed(float value)
        {
            speed = value;
        }
        #endregion
    }
}