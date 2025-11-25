/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov, Deryabin Vladimir
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.InputSystem;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.ControllerSystems
{
    [HideScriptField]
    [AddComponentMenu(null)]
    [DisallowMultipleComponent]
    public abstract class PawnController : Controller, IControlInput
    {
        // Stored required properties.
        private Vector2 input = Vector2.zero;

        /// <summary>
        /// Called every frame, while the controller is enabled.
        /// </summary>
        protected override void Update()
        {
            base.Update();
            ReadInput();
        }

        /// <summary>
        /// Read input and save in Vector2D representation.
        /// </summary>
        protected virtual void ReadInput()
        {
            input.x = InputReceiver.MovementHorizontalAction.ReadValue<float>();
            input.y = InputReceiver.MovementVerticalAction.ReadValue<float>();
        }

        #region [IControlInput Implementation]
        public Vector2 GetControlInput()
        {
            return input;
        }
        #endregion
    }
}
