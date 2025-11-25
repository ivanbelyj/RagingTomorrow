/* ================================================================
  ----------------------------------------------------------------
  Project   :   Aurora FPS Engine
  Publisher :   Renowned Games
  Developer :   Tamerlan Shakirov
  ----------------------------------------------------------------
  Copyright 2022 Renowned Games All rights reserved.
  ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.InputSystem;
using AuroraFPSRuntime.SystemModules.CameraSystems;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AuroraFPSRuntime.WeaponModules.EffectSystem
{
    [System.Serializable]
    [ReferenceContent("Breathing", "Camera/Zoom/Breathing")]
    public sealed class BreathingEffect : Effect
    {
        private const string MapFormat = "<map>/<action>";
        private static Transform Hinge = null;

        [SerializeField]
        private float speed = 10.0f;

        // Rotation amplitude and frequency properties.
        [SerializeField]
        [TabGroup("Transform Layer", "Rotation")]
        [Label("Amplitude X")]
        private float xAmplitude = 0.05f;

        [SerializeField]
        [TabGroup("Transform Layer", "Rotation")]
        [Label("Amplitude Y")]
        private float yAmplitude = 0.1f;

        [SerializeField]
        [TabGroup("Transform Layer", "Rotation")]
        [Label("Frequency X")]
        private float xFrequency = 2.0f;

        [SerializeField]
        [TabGroup("Transform Layer", "Rotation")]
        [Label("Frequency Y")]
        private float yFrequency = 4.0f;

        [SerializeField]
        [TabGroup("Transform Layer", "Rotation")]
        [Label("Curve X")]
        private AnimationCurve xCurve = new AnimationCurve(
            new Keyframe(0.0f, 0.0f, 3.633036f, 3.633036f, 0.3333333f, 0.1579228f),
            new Keyframe(0.5f, 1.0f, 0.006286651f, 0.006286651f, 0.3333333f, 0.3333333f),
            new Keyframe(1.0f, 0.0f, -3.619263f, -3.619263f, 0.3333333f, 0.1500669f),
            new Keyframe(1.5f, -1.0f, 0.0f, 0.0f, 0.3333333f, 0.3333333f),
            new Keyframe(2.0f, 0.0f, 3.835659f, 3.835659f, 0.2075124f, 0.3333333f))
        {
            postWrapMode = WrapMode.Loop,
            preWrapMode = WrapMode.Loop
        };

        [SerializeField]
        [TabGroup("Transform Layer", "Rotation")]
        [Label("Curve Y")]
        private AnimationCurve yCurve = new AnimationCurve(
            new Keyframe(0.0f, 0.0f, 3.633036f, 3.633036f, 0.3333333f, 0.1579228f),
            new Keyframe(0.5f, 1.0f, 0.006286651f, 0.006286651f, 0.3333333f, 0.3333333f),
            new Keyframe(1.0f, 0.0f, -3.619263f, -3.619263f, 0.3333333f, 0.1500669f),
            new Keyframe(1.5f, -1.0f, 0.0f, 0.0f, 0.3333333f, 0.3333333f),
            new Keyframe(2.0f, 0.0f, 3.835659f, 3.835659f, 0.2075124f, 0.3333333f))
        {
            postWrapMode = WrapMode.Loop,
            preWrapMode = WrapMode.Loop
        };

        [SerializeField]
        [Foldout("Breath Holding")]
        private string inputPath = MapFormat;

        [SerializeField]
        [Foldout("Breath Holding")]
        [MinValue(0.1f)]
        private float holdTime;

        [SerializeField]
        [Foldout("Breath Holding")]
        [MinValue("amplitude")]
        private float amplitudeAmplifier;

        [SerializeField]
        [Foldout("Breath Holding")]
        [MinValue("speed")]
        private float frequencyAmplifier;

        [SerializeField]
        [MinValue(0.1f)]
        [Foldout("Advanced Settings")]
        public float returnSpeed = 10;

        // Stored required components.
        private PlayerCamera camera;

        // Stored required properties.
        private bool keepHold;
        private float modifier;
        private float pressTime;
        private float xRotationScroll;
        private float yRotationScroll;
        private float currentXAmplitude;
        private float currentYAmplitude;
        private float currentXFrequency;
        private float currentYFrequency;
        private InputAction inputAction;

        /// <summary>
        /// Called when the effect instance is being loaded.
        /// </summary>
        /// <param name="weapon">Weapon owner transform of this effect instance.</param>
        public override void Initialize(Transform weapon)
        {
            camera = weapon.GetComponentInParent<PlayerCamera>();
            if(Hinge == null)
            {
                Hinge = InstantiateHinge("Breathing Animation Hinge", camera.GetHinge());
            }

            if (!string.IsNullOrEmpty(inputPath))
            {
                inputAction = InputReceiver.Asset.FindAction(inputPath);
            }
        }

        public override void OnEnable()
        {
            if(inputAction != null)
            {
                inputAction.performed += OnHold;
                inputAction.canceled += OnHold;
            }
        }

        /// <summary>
        /// Called every frame, if the effect is enabled.
        /// </summary>
        public override void OnAnimationUpdate()
        {
            if (keepHold && pressTime > 0 && (Time.time - pressTime >= holdTime))
            {
                keepHold = false;
            }

            if (!keepHold && pressTime > 0)
            {
                if(modifier == 0)
                {
                    modifier = Mathf.InverseLerp(0, holdTime, Time.time - pressTime);

                    currentXAmplitude += amplitudeAmplifier * modifier;
                    currentYAmplitude += amplitudeAmplifier * modifier;
                    currentXFrequency += frequencyAmplifier * modifier;
                    currentYFrequency += frequencyAmplifier * modifier;
                }

                currentXAmplitude = Mathf.Max(xAmplitude, currentXAmplitude - Time.deltaTime);
                currentYAmplitude = Mathf.Max(yAmplitude, currentYAmplitude - Time.deltaTime);
                currentXFrequency = Mathf.Max(xFrequency, currentXFrequency - Time.deltaTime);
                currentYFrequency = Mathf.Max(yFrequency, currentYFrequency - Time.deltaTime);

                if(currentXAmplitude <= xAmplitude && 
                    currentYAmplitude <= yAmplitude &&
                    currentXFrequency <= xFrequency &&
                    currentYFrequency <= yFrequency)
                {
                    currentXAmplitude = xAmplitude;
                    currentYAmplitude = yAmplitude;
                    currentXFrequency = xFrequency;
                    currentYFrequency = yFrequency;
                    pressTime = 0;
                    modifier = 0;
                }
            }
            else
            {
                currentXAmplitude = Mathf.Min(xAmplitude, currentXAmplitude + Time.deltaTime);
                currentYAmplitude = Mathf.Min(yAmplitude, currentYAmplitude + Time.deltaTime);
                currentXFrequency = Mathf.Min(xFrequency, currentXFrequency + Time.deltaTime);
                currentYFrequency = Mathf.Min(yFrequency, currentYFrequency + Time.deltaTime);
            }
            

            if (camera.IsZooming() && !keepHold)
            {
                xRotationScroll += Time.deltaTime * currentXFrequency;
                yRotationScroll += Time.deltaTime * currentYFrequency;

                float xCurveEvaluate = xCurve.Evaluate(xRotationScroll);
                float yCurveEvaluate = yCurve.Evaluate(yRotationScroll);

                Vector3 targetRotation = Hinge.localEulerAngles;

                targetRotation.x = xCurveEvaluate * currentXAmplitude;
                targetRotation.y = yCurveEvaluate * currentYAmplitude;

                Hinge.localRotation = Quaternion.Slerp(Hinge.localRotation, Quaternion.Euler(targetRotation), speed * Time.deltaTime);
            }
            else
            {
                currentXAmplitude = 0;
                currentYAmplitude = 0;
                currentXFrequency = 0;
                currentYFrequency = 0;
                Hinge.localRotation = Quaternion.Slerp(Hinge.localRotation, Quaternion.identity, returnSpeed * Time.deltaTime);
            }
        }

        public override void OnDisable()
        {
            if(inputAction != null)
            {
                inputAction.performed -= OnHold;
                inputAction.canceled -= OnHold;
            }
        }

        #region [Input Action Wrapper]
        private void OnHold(InputAction.CallbackContext context)
        {
            if (context.performed && pressTime == 0)
            {
                pressTime = Time.time;
                keepHold = true;
            }
            else if (context.canceled)
            {
                keepHold = false;
            }
            
        }
        #endregion
    }
}