/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.HealthModules
{
    public abstract class HealthComponent : MonoBehaviour, IHealth, IDamageable
    {
        #region [IHealth Implementation]
        /// <summary>
        /// Get current health point.
        /// </summary>
        public abstract float GetHealth();

        /// <summary>
        /// Take damage to the health.
        /// </summary>
        /// <param name="amount">Damage amount.</param>
        /// <param name="damageInfo">Additional information about damage.</param>
        public abstract void TakeDamage(float amount, DamageInfo damageInfo);

        /// <summary>
        /// Alive state of health.
        /// </summary>
        /// <returns>
        /// True if health > 0.
        /// Otherwise false.
        /// </returns>
        public abstract bool IsAlive();
        #endregion
    }
}