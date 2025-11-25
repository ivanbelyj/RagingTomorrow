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
    [ConditionMenu("Delay", "Time/Delay", Description = "Return true after specific delay time.")]
    public class DelayCondition : Condition
    {
        // Base dealy condition properties.
        [SerializeField] private float delay;

        // Stored required properties.
        private float startTime;

        /// <summary>
        /// Called when the behaviour in which this condition is located, becomes enabled.
        /// </summary>
        protected override void OnEnable()
        {
            startTime = Time.time;
        }

        /// <summary>
        /// Condition for translate to the next AI behaviour.
        /// </summary>
        public override bool IsExecuted()
        {
            return Time.time - startTime >= delay;
        }

        /// <summary>
        /// Called when the behaviour in which this condition is located, switch to another.
        /// </summary>
        protected override void OnDisable()
        {
            startTime = 0;
        }

        #region [Getter / Setter]
        public float GetDealy()
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