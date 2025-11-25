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
using System.Collections;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.CameraSystems.Effects
{
    [System.Serializable]
    [ReferenceContent("Side Sway", "Transform/Side Sway Effect")]
    public sealed class CameraSideSwayEffect : PlayerCameraHingeEffect
    {
        // Base camera side sway properties.
        [SerializeField] 
        [Range(0, 90)]
        private float maxAngle = 2;

        [SerializeField]
        [VisualClamp(0, 20)]
        private float swaySpeed = 3;

        [SerializeField]
        [VisualClamp(0, 20)]
        private float resetSpeed = 5;

        // Stored required components.
        private PlayerController controller;

        public override void Initialization(PlayerController controller, PlayerCamera cameraControl)
        {
            this.controller = controller;
            this.controller.OnMoveCallback += OnMove;
        }

        private void OnMove(Vector3 velocity)
        {
            Vector2 input = controller.GetControlInput();
            if (input.x != 0)
                DoSway(input.x);
            else
                ResetSway();
        }

        /// <summary>
        /// Calculate camera side sway.
        /// </summary>
        private void DoSway(float horizontalInput)
        {
            Vector3 targetRotation = hinge.localEulerAngles;
            targetRotation.z = -Mathf.Sign(horizontalInput) * maxAngle;
            hinge.localRotation = Quaternion.Slerp(hinge.localRotation, Quaternion.Euler(targetRotation), swaySpeed * Time.deltaTime);
        }

        private void ResetSway()
        {
            hinge.localRotation = Quaternion.Slerp(hinge.localRotation, Quaternion.identity, resetSpeed * Time.deltaTime);
        }

        #region [Getter / Setter]
        public float GetSwayAmount()
        {
            return maxAngle;
        }

        public void SetSwayAmount(float value)
        {
            maxAngle = value;
        }

        public float GetSwaySpeed()
        {
            return swaySpeed;
        }

        public void SetSwaySpeed(float value)
        {
            swaySpeed = value;
        }

        public float GetReturnSpeed()
        {
            return resetSpeed;
        }

        public void SetReturnSpeed(float value)
        {
            resetSpeed = value;
        }
        #endregion
    }
}