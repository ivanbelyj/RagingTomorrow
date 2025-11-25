/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Coroutines;
using System.Collections;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.HealthModules
{
    [System.Serializable]
    [HealthFunctionMenu("Regeneration", "Regeneration")]
    public sealed class RegenerationEffect : HealthEffect
    {
        [SerializeField]
        [MinValue(1)]
        private int increasePoints = 1;

        [SerializeField]
        [Label("Rate")]
        [MinValue(0.01f)]
        private float rateTime = 1.0f;

        [SerializeField]
        [Label("Delay")]
        [MinValue(0.0f)]
        private float delayTime = 0.0f;

        // Stored required components.
        private CharacterHealth characterHealth;

        // Stored required properties.
        private CoroutineObject regenerationProcessCoroutine;
        private WaitForSeconds delay;
        private WaitForSeconds rate;

        public override void Initialization(CharacterHealth characterHealth)
        {
            this.characterHealth = characterHealth;
            regenerationProcessCoroutine = new CoroutineObject(characterHealth);
            rate = new WaitForSeconds(rateTime);
            delay = new WaitForSeconds(delayTime);
            characterHealth.OnTakeDamageCallback += OnTakeDamageAction;
        }

        public IEnumerator RegenerationProcess()
        {
            yield return delay;
            while(characterHealth.GetHealth() < characterHealth.GetMaxHealth())
            {
                if (!characterHealth.IsAlive())
                {
                    yield break;
                }

                characterHealth.ApplyHealth(increasePoints);
                yield return rate;
            }
        }

        #region [Event Callback Wrapper]
        private void OnTakeDamageAction(float amount, DamageInfo damageInfo)
        {
            if (characterHealth.IsAlive())
            {
                regenerationProcessCoroutine.Start(RegenerationProcess, true);
            }
        }
        #endregion
    }
}