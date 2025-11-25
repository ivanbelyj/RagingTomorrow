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

namespace AuroraFPSRuntime.SystemModules.CameraSystems.Effects
{
    [System.Serializable]
    [ReferenceContent("Sprinting Field Of View", "Field Of View/Sprint Effect")]
    public sealed class SprintFOVEffect : PlayerCameraEffect
    {
        [SerializeField]
        [HideExpandButton]
        private FieldOfViewSettings sprintFOV = new FieldOfViewSettings(10, 0.5f, AnimationCurve.Linear(0, 0, 1, 1), true);

        // Stored required components.
        private PlayerController controller;
        private PlayerCamera cameraControl;

        // Stored required properties.
        private bool isSprintFOV;

        public override void Initialization(PlayerController controller, PlayerCamera cameraControl)
        {
            this.controller = controller;
            this.cameraControl = cameraControl;
            this.controller.OnMoveCallback += OnMoveCallback;
        }

        /// <summary>
        /// Called while controller is moving.
        /// </summary>
        /// <param name="velocity">Current controller movement velocity.</param>
        private void OnMoveCallback(Vector3 movementVector)
        {
            if (controller.HasState(ControllerSystems.ControllerState.Sprinting) & !isSprintFOV)
            {
                cameraControl.ChangeFieldOfView(sprintFOV, true);
                isSprintFOV = true;
            }
            else if(!controller.HasState(ControllerSystems.ControllerState.Sprinting) & isSprintFOV)
            {
                cameraControl.ChangeFieldOfView(cameraControl.GetDefaultFOVSettings(), true);
                isSprintFOV = false;
            }
        }
    }
}