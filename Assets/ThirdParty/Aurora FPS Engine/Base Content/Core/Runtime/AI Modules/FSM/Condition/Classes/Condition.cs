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

namespace AuroraFPSRuntime.AIModules.Conditions
{
    [System.Serializable]
    public abstract class Condition : ICondition, IConditionMute
    {
        [SerializeField] 
        [Order(999)]
        private bool mute;

        // Stored required components.
        protected AIController owner;

        /// <summary>
        /// Called once when condition being loaded.
        /// </summary>
        /// <param name="owner">AIController owner reference.</param>
        protected virtual void OnInitialize(AIController owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Called when the behaviour in which this condition is located, becomes enabled.
        /// </summary>
        protected virtual void OnEnable()
        {
            
        }

        /// <summary>
        /// Called when the behaviour in which this condition is located, become disable.
        /// </summary>
        protected virtual void OnDisable()
        {

        }

        #region [ICondition Implementation]
        /// <summary>
        /// Condition for translate to the next AI behaviour.
        /// </summary>
        public abstract bool IsExecuted();
        #endregion

        #region [IConditionMute Implementation]
        /// <summary>
        /// Condition mute state value.
        /// If true this condition will be ignored.
        /// </summary>
        public bool IsMuted()
        {
            return mute;
        }
        #endregion

        #region [Internal Callbacks]
        internal void Internal_Initialize(AIController owner)
        {
            OnInitialize(owner);
        }

        internal void Internal_Enable()
        {
            OnEnable();
        }

        internal void Internal_Disable()
        {
            OnDisable();
        }
        #endregion

        #region [Getter / Setter]
        public void Mute(bool value)
        {
            mute = value;
        }
        #endregion
    }
}
