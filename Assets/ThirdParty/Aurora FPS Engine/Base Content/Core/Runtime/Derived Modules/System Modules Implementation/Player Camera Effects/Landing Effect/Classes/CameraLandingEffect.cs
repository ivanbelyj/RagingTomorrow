/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Coroutines;
using AuroraFPSRuntime.CoreModules.Mathematics;
using AuroraFPSRuntime.SystemModules.ControllerSystems;
using System.Collections;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.CameraSystems.Effects
{
    [System.Serializable]
    [ReferenceContent("Landing Bob", "Transform/Landing Effect")]
    public sealed class CameraLandingEffect : PlayerCameraHingeEffect
    {
        // Serialized properties.
        [SerializeField]
        [MinMaxSlider(0.0f, 1.0f)]
        private Vector2 amountLimit = new Vector2(0.1f, 0.35f);

        [SerializeField] 
        [VisualClamp(0, 1)]
        [Suffix("%", true)]
        private float amountPersent = 0.5f;

        [SerializeField]
        [Slider(0, 90)]
        private float angle = 2.5f;

        [SerializeField]
        [MinValue(0.01f)]
        private float duration = 0.35f;

        [SerializeField] 
        private AnimationCurve curve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(0.2f, -1.0f), new Keyframe(1.0f, 0.0f));

        // Stored required components.
        private PlayerController controller;

        // Stored required properties.
        private CoroutineObject<float> coroutineObject;

        /// <summary>
        /// Implement this method to make some initialization 
        /// or get access to Controller and CameraControl references.
        /// </summary>
        /// <param name="controller">Player controller reference.</param>
        /// <param name="cameraControl">Player camera control reference.</param>
        public override void Initialization(PlayerController controller, PlayerCamera cameraControl)
        {
            this.controller = controller;

            coroutineObject = new CoroutineObject<float>(controller);
            controller.OnGroundedCallback += OnGroundCallback;
        }

        private void OnGroundCallback()
        {
            float verticalVelocity = controller.GetVelocity().y;
            if (verticalVelocity < 0)
            {
                coroutineObject.Start(EffectProcessing, Mathf.Abs(verticalVelocity), true);
            }
        }

        /// <summary>
        /// Play camera landing effect.
        /// </summary>
        private IEnumerator EffectProcessing(float amount)
        {
            float time = 0f;
            float speed = 1.0f / duration;

            Vector3 storedLocalPosition = Vector3.zero;
            Vector3 storedLocalEulerAngle = Vector3.zero;
            float storedCameraHeight = storedLocalPosition.y;
            float storedCameraAngle = storedLocalEulerAngle.x;
            float landAmount = amount * amountPersent;
            landAmount = Math.Clamp(landAmount, amountLimit);
            while (time < 1.0f)
            {
                time += Time.deltaTime * speed;
                float evaluate = curve.Evaluate(time);

                float positionOffset = evaluate * landAmount;
                storedLocalPosition.y = storedCameraHeight + positionOffset;
                hinge.localPosition = storedLocalPosition;

                float rotationOffset = evaluate * angle;
                storedLocalEulerAngle.x = storedCameraAngle - rotationOffset;
                hinge.localEulerAngles = storedLocalEulerAngle;

                yield return null;
            }
        }
    }
}