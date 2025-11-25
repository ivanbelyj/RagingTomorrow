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

namespace AuroraFPSRuntime.SystemModules
{
    public partial class FootstepSoundSystem
    {
        [System.Serializable]
        public struct Interval
        {
            [SerializeField]
            [MinValue(0.0f)]
            private float rate;

            [SerializeField]
            [MinMaxSlider(0.1f, 300.0f)]
            private Vector2 velocity;

            public Interval(float rate, Vector2 velocity)
            {
                this.rate = rate;
                this.velocity = velocity;
            }

            #region [Static Readonly]
            public static readonly Interval none = new Interval(float.NegativeInfinity, Vector2.negativeInfinity);
            #endregion

            #region [Getter / Setter]
            public float GetRate()
            {
                return rate;
            }

            public void SetRate(float value)
            {
                rate = value;
            }

            public Vector2 GetVelocity()
            {
                return velocity;
            }

            public void SetVelocity(Vector2 value)
            {
                velocity = value;
            }
            #endregion
        }
    }
}