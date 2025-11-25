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
    public sealed class VelocityDamage
    {
        [SerializeField]
        private float damage;

        [SerializeField]
        [MinMaxSlider(0.1f, 500.0f)]
        private Vector2 velocity;

        public VelocityDamage(int damage, Vector2 velocity)
        {
            this.damage = damage;
            this.velocity = velocity;
        }

        #region [Getter / Setter]
        public float GetDamage()
        {
            return damage;
        }

        public void SetDamage(float value)
        {
            damage = value;
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