/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using System.Collections;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.HealthModules
{
    public abstract class CharacterRespawnEvent
    {
        [SerializeField]
        [MinValue(1)]
        private float health = 100;

        [SerializeField]
        [MinValue(0.0f)]
        private float delay = 3.0f;

        [SerializeField]
        private bool overrideTransformation = true;

        [SerializeField]
        [Foldout("Advanced Settings", Style = "Indent")]
        [VisibleIf("overrideTransformation", true)]
        private Vector3 position = Vector3.zero;

        [SerializeField]
        [Foldout("Advanced Settings", Style = "Indent")]
        [VisibleIf("overrideTransformation", true)]
        private Vector3 rotation = Vector3.zero;

        // Stored required properties.
        private CharacterHealth characterHealth;

        /// <summary>
        /// Called when the character re-spawn event instance is being loaded.
        /// </summary>
        /// <param name="characterHealth">CharacterHealth owner reference.</param>
        public virtual void Initialize(CharacterHealth characterHealth) 
        {
            this.characterHealth = characterHealth;
        }

        /// <summary>
        /// Called after delay is out and character health owner receive health points.
        /// </summary>
        public abstract void OnRevive();

        /// <summary>
        /// Called every time while counting delay.
        /// </summary>
        /// <param name="remaningTime">Delay remaning time.</param>
        public virtual void OnDelayCounter(float remaningTime) { }

        #region [Internal Callback]
        /// <summary>
        /// Internal callback for start delay coroutine.
        /// </summary>
        internal IEnumerator DelayCoroutine()
        {
            float time = delay;
            while (time > 0)
            {
                OnDelayCounter(time);
                time -= Time.deltaTime;
                yield return null;
            }

            if (overrideTransformation)
            {
                characterHealth.transform.position = position;
                characterHealth.transform.rotation = Quaternion.Euler(rotation);
            }

            characterHealth.ApplyHealth(health);
        }
        #endregion

        #region [Getter / Setter]
        public CharacterHealth GetCharacterHealth()
        {
            return characterHealth;
        }

        public float GetHealth()
        {
            return health;
        }

        public void SetHealth(float value)
        {
            health = value;
        }

        public float GetDelay()
        {
            return delay;
        }

        public void SetDelay(float value)
        {
            delay = value;
        }
        #endregion
    }
}