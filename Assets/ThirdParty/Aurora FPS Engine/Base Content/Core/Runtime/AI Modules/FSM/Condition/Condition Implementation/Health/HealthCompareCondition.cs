/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.SystemModules.HealthModules;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.Conditions
{
    [RequireComponent(typeof(AICharacterHealth))]
    [ConditionMenu("Compare", "Health/Compare", Description = "Compare AI health point.\nHealth {Comparison type} value.")]
    public class HealthCompareCondition : Condition
    {
        public enum Comparison
        {
            Equal,
            NotEqual,
            Greater,
            Less,
        }

        // Base health condition properties.
        [SerializeField] private Comparison comparison;
        [SerializeField] private int value;

        // Stored required components.
        private CharacterHealth health;


        /// <summary>
        /// Initializing required properties.
        /// </summary>
        /// <param name="core">Target AIController instance.</param>
        protected override void OnInitialize(AIController core)
        {
            health = core.GetComponent<CharacterHealth>();
        }

        /// <summary>
        /// Condition for translate to the next AI behaviour.
        /// </summary>
        public override bool IsExecuted()
        {
            switch (comparison)
            {
                case Comparison.Equal:
                    return health.GetHealth() == value;
                case Comparison.NotEqual:
                    return health.GetHealth() != value;
                case Comparison.Greater:
                    return health.GetHealth() > value;
                case Comparison.Less:
                    return health.GetHealth() < value;
                default:
                    return false;
            }
        }

        #region [Getter / Setter]
        public Comparison GetComparison()
        {
            return comparison;
        }

        public void SetComparison(Comparison value)
        {
            comparison = value;
        }

        public int GetValue()
        {
            return value;
        }

        public void SetValue(int value)
        {
            this.value = value;
        }
        #endregion
    }
}