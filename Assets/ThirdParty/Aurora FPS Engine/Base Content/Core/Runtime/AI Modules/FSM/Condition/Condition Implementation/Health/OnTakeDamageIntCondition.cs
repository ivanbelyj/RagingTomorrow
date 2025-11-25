/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;

namespace AuroraFPSRuntime.AIModules.Conditions
{
    [RequireComponent(typeof(AICharacterHealth))]
    [ConditionMenu("On Take Damage(Int32 amount)", "Health/On Take Damage(Int32 amount)", Description = "Called every time when AI take specific(value) damage.")]
    public class OnTakeDamageIntCondition : OnTakeDamageCondition
    {
        [SerializeField] private int value;

        /// <summary>
        /// Register health take damage callback function.
        /// </summary>
        protected override void RegisterCallbackFunction()
        {
            health.OnTakeDamageCallback += (amount, info) =>
            {
                InvokeDefaultCallback();
            };
        }
    }
}