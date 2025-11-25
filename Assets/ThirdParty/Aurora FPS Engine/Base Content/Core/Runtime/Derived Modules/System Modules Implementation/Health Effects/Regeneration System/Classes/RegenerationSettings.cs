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

namespace AuroraFPSRuntime.SystemModules.HealthModules
{
    [System.Serializable]
    public sealed class RegenerationSettings
    {
        [SerializeField]
        [MinValue(0.01f)]
        private float rate;

        [SerializeField]
        [MinValue(0)]
        private int value;

        [SerializeField]
        [MinValue(0.01f)]
        private float delay;

        /// <summary>
        /// Regeneration properties constructor.
        /// </summary>
        /// <param name="rate">Rate (in seconds) of adding health points, (V/R - Value per rate).</param>
        /// <param name="value">Health point value.</param>
        /// <param name="delay">Delay before start adding health.</param>
        public RegenerationSettings(float rate, int value, float delay)
        {
            this.rate = rate;
            this.value = value;
            this.delay = delay;
        }

        #region [Getter / Setter]
        /// <summary>
        /// Return rate of adding health points, (V/R - Value per rate).
        /// </summary>
        /// <returns></returns>
        public float GetRate()
        {
            return rate;
        }

        /// <summary>
        /// Set rate of adding health points, (V/R - Value per rate).
        /// </summary>
        /// <param name="value"></param>
        public void SetRate(float value)
        {
            rate = value;
        }

        /// <summary>
        /// Return health point value.
        /// </summary>
        /// <returns></returns>
        public int GetValue()
        {
            return value;
        }

        /// <summary>
        /// Set health point value.
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(int value)
        {
            this.value = value;
        }

        /// <summary>
        /// Return delay before start adding health.
        /// </summary>
        /// <returns></returns>
        public float GetDelay()
        {
            return delay;
        }

        /// <summary>
        /// Set delay before start adding health.
        /// </summary>
        /// <param name="value"></param>
        public void SetDelay(float value)
        {
            delay = value;
        }
        #endregion
    }
}