/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using System;
using UnityEngine;
using UnityEngine.Events;


namespace AuroraFPSRuntime.SystemModules.HealthModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Health/Object Health")]
    [DisallowMultipleComponent]
    public class ObjectHealth : HealthComponent
    {
        [Serializable]
        public class OnTakeDamageEvent : UnityEvent<float, DamageInfo> { }

        // Base object health properties.
        [SerializeField]
        [Slider("minHealth", "maxHealth")]
        private float health = 100;

        [SerializeField]
        [MinValue(0)]
        private float minHealth = 0;

        [SerializeField]
        private float maxHealth = 100;

        [SerializeField]
        [Foldout("Event Callbacks", Style = "Header")]
        [Order(500)]
        protected OnTakeDamageEvent onTakeDamageEvent;

        [SerializeField]
        [Foldout("Event Callbacks", Style = "Header")]
        [Order(501)]
        protected UnityEvent onDeadEvent;

        [SerializeField]
        [Foldout("Event Callbacks", Style = "Header")]
        [Order(502)]
        protected UnityEvent onReviveEvent;

        /// <summary>
        /// Called when the script instance is being loaded
        /// </summary>
        protected virtual void Awake()
        {
            OnTakeDamageCallback += onTakeDamageEvent.Invoke;
            OnDeadCallback += (other) => onDeadEvent.Invoke();
            OnReviveCallback += onReviveEvent.Invoke;
        }

        /// <summary>
        /// Called once when object health become zero.
        /// Implement this method to make custom death logic.
        /// </summary>
        protected virtual void OnDead()
        {

        }

        /// <summary>
        /// Called when object health become more then zero.
        /// Implement this method to make revive logic.
        /// </summary>
        protected virtual void OnRevive()
        {

        }

        /// <summary>
        /// Apply new health points.
        /// </summary>
        /// <param name="amount">Health amount.</param>
        public virtual void ApplyHealth(float amount)
        {
            amount = Mathf.Abs(amount);

            float previousHealth = health;
            SetHealth(health + amount);
            if(previousHealth == 0 && health > 0)
            {
                OnRevive();
                OnReviveCallback?.Invoke();
            }
        }

        #region [IHealth Implemetation]
        /// <summary>
        /// Get current health point.
        /// </summary>
        public override float GetHealth()
        {
            return health;
        }

        /// <summary>
        /// Alive state of health object.
        /// </summary>
        /// <returns>
        /// True if health > 0.
        /// Otherwise false.
        /// </returns>
        public override bool IsAlive()
        {
            return health > 0;
        }
        #endregion

        #region [IDamageable Implementation]
        /// <summary>
        /// Take damage to the health.
        /// </summary>
        /// <param name="amount">Damage amount.</param>
        /// <param name="damageInfo">Additional damage info.</param>
        public override void TakeDamage(float amount, DamageInfo damageInfo)
        {
            amount = Mathf.Abs(amount);

            float previousHealth = health;
            SetHealth(health - amount);
            OnTakeDamageCallback?.Invoke(amount, damageInfo);

            if (previousHealth > 0 && health == 0) 
            {
                OnDead();
                OnDeadCallback?.Invoke(damageInfo.sender);
            }
        }
        #endregion

        #region [Event Callback Functions]
        /// <summary>
        /// Called when an object takes damage.
        /// <i><br>Parameter (amount): Damage amount.</br></i>
        /// <i><br>Parameter (damageInfo): Additional damage info.</br></i>
        /// </summary>
        public event Action<float, DamageInfo> OnTakeDamageCallback;

        /// <summary>
        /// Called when object is die.
        /// <i><br>Parameter (other): Cause of death.</br></i>
        /// </summary>
        public event Action<Transform> OnDeadCallback;

        /// <summary>
        /// Called when object is revive.
        /// </summary>
        public event Action OnReviveCallback;
        #endregion

        #region [Getter / Setter]
        protected void SetHealth(float value)
        {
            health = Mathf.Clamp(value, minHealth, maxHealth);
        }

        public float GetMaxHealth()
        {
            return maxHealth;
        }

        public void SetMaxHealth(float value)
        {
            maxHealth = Mathf.Clamp(value, minHealth, float.MaxValue);
        }

        public float GetMinHealth()
        {
            return minHealth;
        }

        public void SetMinHealth(float value)
        {
            minHealth = Mathf.Clamp(value, 0, maxHealth);
        }
        #endregion
    }
}