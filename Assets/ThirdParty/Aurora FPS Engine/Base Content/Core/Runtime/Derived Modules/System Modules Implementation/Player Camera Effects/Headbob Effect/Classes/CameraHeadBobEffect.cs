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
    [ReferenceContent("Head Bob", "Transform/HeadBob Effect")]
    public sealed class CameraHeadBobEffect : PlayerCameraHingeEffect
    {
        [SerializeField]
        [NotNull]
        private HeadBobSettings settings;

        // Stored required components.
        private PlayerController controller;

        // Stored required properties.
        private float xPositionScroll;
        private float yPositionScroll;
        private float xRotationScroll;
        private float yRotationScroll;

        public override void Initialization(PlayerController controller, PlayerCamera cameraControl)
        {
            this.controller = controller;
            this.controller.StartCoroutine(HeadBobProcessing());
        }

        private IEnumerator HeadBobProcessing()
        {
            while (true)
            {
                if (controller.IsMoving())
                {
                    ControllerState controllerState = (ControllerState)controller.GetState();
                    if(settings.TryGetMultiplier(controllerState, out HeadBobSettings.Multiplier multiplier))
                    {
                        Vector2 movementInput = controller.GetControlInput();
                        if (settings.PositionBobEnabled())
                        {
                            DoPositionBob(movementInput, multiplier);
                        }

                        if (settings.RotationBobEnabled())
                        {
                            DoRotationBob(movementInput, multiplier);
                        }
                    }
                }
                yield return null;
            }
        }

        public void DoPositionBob(Vector2 movementInput, HeadBobSettings.Multiplier multiplier)
        {
            float additionalMultiplier = movementInput.y == -1 ? controller.GetBackwardSpeedPercent() : 1f;
            additionalMultiplier = movementInput.x != 0 & movementInput.y == 0 ? controller.GetSideSpeedPercent() : additionalMultiplier;

            xPositionScroll += Time.deltaTime * settings.GetPositionFrequencyX() * multiplier.GetFrequency();
            yPositionScroll += Time.deltaTime * settings.GetPositionFrequencyY() * multiplier.GetFrequency();

            float xCurveEvaluate = settings.GetPositionCurveX().Evaluate(xPositionScroll);
            float yCurveEvaluate = settings.GetPositionCurveY().Evaluate(yPositionScroll);

            Vector3 targetPosition = hinge.localPosition;

            targetPosition.x = xCurveEvaluate * settings.GetPositionAmplitudeX() * multiplier.GetAmplitude() * additionalMultiplier;
            targetPosition.y = yCurveEvaluate * settings.GetPositionAmplitudeY() * multiplier.GetAmplitude() * additionalMultiplier;

            hinge.localPosition = Vector3.Lerp(hinge.localPosition, targetPosition, settings.GetSpeed() * Time.deltaTime);
        }

        public void DoRotationBob(Vector2 movementInput, HeadBobSettings.Multiplier multiplier)
        {
            float additionalMultiplier = movementInput.y == -1 ? controller.GetBackwardSpeedPercent() : 1f;
            additionalMultiplier = movementInput.x != 0 & movementInput.y == 0 ? controller.GetSideSpeedPercent() : additionalMultiplier;

            xRotationScroll += Time.deltaTime * settings.GetRotationFrequencyX() * multiplier.GetFrequency();
            yRotationScroll += Time.deltaTime * settings.GetRotationFrequencyY() * multiplier.GetFrequency();

            float xCurveEvaluate = settings.GetRotationCurveX().Evaluate(xRotationScroll);
            float yCurveEvaluate = settings.GetRotationCurveY().Evaluate(yRotationScroll);

            Vector3 targetRotation = hinge.localEulerAngles;

            targetRotation.x = xCurveEvaluate * settings.GetRotationAmplitudeX() * multiplier.GetAmplitude() * additionalMultiplier;
            targetRotation.y = yCurveEvaluate * settings.GetRotationAmplitudeY() * multiplier.GetAmplitude() * additionalMultiplier;

            hinge.localRotation = Quaternion.Slerp(hinge.localRotation, Quaternion.Euler(targetRotation), settings.GetSpeed() * Time.deltaTime);
        }

        #region [Getter / Setter]
        public HeadBobSettings GetSettings()
        {
            return settings;
        }

        public void SetSettings(HeadBobSettings value)
        {
            settings = value;
        }
        #endregion
    }
}