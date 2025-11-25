/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.SystemModules.ControllerSystems;
using UnityEngine;

namespace AuroraFPSRuntime.UIModules.UIElements.Crosshair
{
    [System.Serializable]
    public abstract class CrosshairPreset : ICrosshairPreset
    {
        [SerializeField]
        [Order(200)]
        private ControllerState hideState = ControllerState.Disabled;

        // Stored required properties.
        private PlayerController controller;

        /// <summary>
        /// Initialize crosshair preset.
        /// </summary>
        /// <param name="controller">Controller reference.</param>
        public virtual void Initialize(PlayerController controller)
        {
            this.controller = controller;
        }

        public virtual void DrawElements(float spread)
        {
            if(controller.HasState(hideState))
            {
                SetVisibility(false);
                return;
            }
            SetVisibility(true);
            OnElementsUI(spread);
        }

        /// <summary>
        /// Crosshair elements GUI layout.
        /// </summary>
        /// <param name="spread">Specific spread value calculated by controller state.</param>
        protected abstract void OnElementsUI(float spread);

        public abstract void SetVisibility(bool value);

        #region [Getter / Setter]
        public ControllerState GetHideState()
        {
            return hideState;
        }

        public void SetHideState(ControllerState value)
        {
            hideState = value;
        }

        public PlayerController GetController()
        {
            return controller;
        }
        #endregion
    }
}