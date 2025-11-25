/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.SystemModules.HealthModules;
using System.Collections;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.Conditions
{
    [RequireComponent(typeof(AICharacterHealth))]
    [ConditionMenu("On Take Damage", "Health/On Take Damage", Description = "Called every time when AI take damage.")]
    public class OnTakeDamageCondition : Condition
    {
        protected bool isExecuted;
        protected CharacterHealth health;
        protected WaitForEndOfFrame waitForEndOfFrame;

        /// <summary>
        /// Called once when condition being loaded.
        /// </summary>
        /// <param name="owner">AIController owner reference.</param>
        protected override void OnInitialize(AIController core)
        {
            health = core.GetComponent<CharacterHealth>();
            waitForEndOfFrame = new WaitForEndOfFrame();
            RegisterCallbackFunction();
        }

        /// <summary>
        /// Condition for translate to the next AI behaviour.
        /// </summary>
        public override bool IsExecuted()
        {
            return isExecuted;
        }

        /// <summary>
        /// Register health take damage callback function.
        /// </summary>
        protected virtual void RegisterCallbackFunction()
        {
            health.OnTakeDamageCallback += (amount, info) =>
            {
                InvokeDefaultCallback();
            };
        }
        
        /// <summary>
        /// Represents default actions to invoke on callback function.
        /// </summary>
        protected virtual void InvokeDefaultCallback()
        {
            isExecuted = true;
            owner.StartCoroutine(ResetExecutedValue());
        }

        /// <summary>
        /// Called every time when AI take damage.
        /// Reset executed value on false after end of frame.
        /// </summary>
        protected virtual IEnumerator ResetExecutedValue()
        {
            yield return waitForEndOfFrame;
            isExecuted = false;
        }

        #region [Getter / Setter]
        public CharacterHealth GetHealth()
        {
            return health;
        }

        protected void SetHealth(CharacterHealth value)
        {
            health = value;
        }

        public WaitForEndOfFrame GetWaitForEndOfFrame()
        {
            return waitForEndOfFrame;
        }

        protected void SetWaitForEndOfFrame(WaitForEndOfFrame value)
        {
            waitForEndOfFrame = value;
        }
        #endregion
    }
}