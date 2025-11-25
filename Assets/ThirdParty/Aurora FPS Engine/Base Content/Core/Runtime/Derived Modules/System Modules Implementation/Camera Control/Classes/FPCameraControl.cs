/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.CoreModules.InputSystem;
using AuroraFPSRuntime.CoreModules.Coroutines;
using AuroraFPSRuntime.SystemModules;
using AuroraFPSRuntime.SystemModules.ControllerModules;
using AuroraFPSRuntime.Attributes;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Math = AuroraFPSRuntime.CoreModules.Mathematics.Math;

#region [Unity Editor Section]
#if UNITY_EDITOR
#endif
#endregion


namespace AuroraFPSRuntime
{
    [Serializable]
    [Obsolete("Use Player Camera implementation instead.")]
    public class FPCameraControl : CameraControl
    {
        // Default first person camera control properties.
        [SerializeField]
        [Foldout("Control Settings", Style = "Header")]
        [NotNull]
        private Camera camera;

        [SerializeField]
        [Foldout("Control Settings", Style = "Header")]
        [NotNull]
        private Transform hinge;

        [SerializeField]
        [Foldout("Control Settings", Style = "Header")]
        private Vector2 sensitivity = new Vector2(175, 175);

        [SerializeField]
        [Foldout("Control Settings", Style = "Header")]
        private Vector2 rotationSmooth = new Vector2(20, 20);

        [SerializeField]
        [Foldout("Control Settings", Style = "Header")]
        [MinMaxSlider(-180, 180)]
        private Vector2 verticalRotationLimits = new Vector2(-90, 90);

        [SerializeField]
        [Foldout("Control Settings", Style = "Header")]
        private bool clampVerticalRotation = true;

        [SerializeField]
        [CustomView(ViewGUI = "OnGroupBoolGUI")]
        [Foldout("Control Settings", Style = "Header")]
        private Vector2Int invertRotation = Vector2Int.zero;

        // Zoom properties.
        [SerializeField]
        [Foldout("Zoom Settings", Style = "Header")]
        private InputHandleType zoomHandleType = InputHandleType.Hold;

        [SerializeField]
        [Foldout("Zoom Settings", Style = "Header")]
        [HideExpandButton]
        private FieldOfViewSettings zoomSettings = new FieldOfViewSettings(-10, 0.25f, AnimationCurve.Linear(0, 0, 1, 1), true);

        [SerializeField]
        [Foldout("Other Settings", Style = "Header")]
        [Label("Default Field Of View Settings")]
        [Indent(1)]
        private FieldOfViewSettings defaultFOVSettings = new FieldOfViewSettings(85.0f, 0.25f, AnimationCurve.Linear(0, 0, 1, 1), false);

        // Stored required properties.
        private Vector2 input;
        private Vector2 desiredVector;
        private float xSmoothAngle;
        private Quaternion yDesiredRotation;
        private Quaternion ySmoothRotation;
        private Vector2 addCustomRotationCache;
        private bool isZooming;
        private CoroutineObject<FieldOfViewSettings> changeFOVCoroutine;

        /// <summary>
        /// Initialize CameraControl instance.
        /// </summary>
        public override void Initialize(Controller controller)
        {
            base.Initialize(controller);

            changeFOVCoroutine = new CoroutineObject<FieldOfViewSettings>(controller);
        }

        protected virtual void OnEnable()
        {
            RegisterInputActions();
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        protected override void Update()
        {
            ReadInput();
            CalculateRotation();
            ClampRotation();
            CalculateQuaternion();
            SmoothingRotation();
            ApplyRotation();
        }

        /// <summary>
        /// Read required input values before calculation rotation.
        /// </summary>
        protected virtual void ReadInput()
        {
            input.x = InputReceiver.CameraHorizontalAction.ReadValue<float>();
            input.y = InputReceiver.CameraVerticalAction.ReadValue<float>();
        }

        /// <summary>
        /// Calculate camera rotation.
        /// </summary>
        protected virtual void CalculateRotation()
        {
            desiredVector.x = input.x * sensitivity.x * invertRotation.x * Time.deltaTime;
            desiredVector.y += input.y * sensitivity.y * invertRotation.y * Time.deltaTime;
            desiredVector += addCustomRotationCache;
            addCustomRotationCache = Vector2.zero;
        }

        protected virtual void CalculateQuaternion()
        {
            yDesiredRotation = Quaternion.AngleAxis(desiredVector.y, - Vector3.right);
        }

        /// <summary>
        /// Clamp vertical rotation by specific limits.
        /// </summary>
        protected virtual void ClampRotation()
        {
            if (clampVerticalRotation)
                desiredVector.y = Math.Clamp(desiredVector.y, verticalRotationLimits);
        }

        /// <summary>
        /// Smoothing calculated rotation.
        /// </summary>
        protected virtual void SmoothingRotation()
        {
            xSmoothAngle = Mathf.Lerp(xSmoothAngle, desiredVector.x, rotationSmooth.x * Time.deltaTime);
            ySmoothRotation = Quaternion.Slerp(ySmoothRotation, yDesiredRotation, rotationSmooth.y * Time.deltaTime);
        }

        /// <summary>
        /// Applying calculated and smoothed rotation to the camera transform.
        /// </summary>
        protected virtual void ApplyRotation()
        {
            GetController().transform.Rotate(Vector3.up, xSmoothAngle, Space.Self);
            hinge.localRotation = ySmoothRotation;
        }

        /// <summary>
        /// Apply zoom field of view settings to the camera component.
        /// </summary>
        public override void ZoomIn()
        {
            if (!isZooming)
            {
                OnStartZoomCallback?.Invoke();
            }
            isZooming = true;
            ChangeFieldOfView(zoomSettings, true);
        }

        /// <summary>
        /// Apply default field of view settings to the camera component.
        /// </summary>
        public override void ZoomOut()
        {
            if (isZooming)
            {
                OnStopZoomCallback?.Invoke();
            }
            isZooming = false;
            ChangeFieldOfView(defaultFOVSettings, true);
        }

        /// <summary>
        /// Add custom rotation to camera.
        /// </summary>
        public virtual void AddRotation(Vector2 rotation)
        {
            addCustomRotationCache += rotation;
        }

        /// <summary>
        /// Change camera field of view value.
        /// </summary>
        /// <param name="settings">Settings to change current field of view.</param>
        /// <param name="force">
        /// True: If at the moment change field of view processed, terminate it, and apply new field of view settings.
        /// False: Terminate applying new field of view settings, if the other field of view settings are currently being applied.
        /// </param>
        public void ChangeFieldOfView(FieldOfViewSettings settings, bool force = false)
        {
            if(settings != null)
            {
                changeFOVCoroutine.Start(ChangeFOV, settings, force);
            }
        }

        /// <summary>
        /// Increase camera field of view to target value.
        /// </summary>
        protected IEnumerator ChangeFOV(FieldOfViewSettings settings)
        {
            float time = 0.0f;
            float speed = 1.0f / settings.GetDuration();

            float currentFOV = camera.fieldOfView;
            float targetFOV = settings.CalculateFieldOfView(camera.fieldOfView);

            while (time < 1.0f)
            {
                time += Time.deltaTime * speed;
                float smoothTime = settings.EvaluateCurve(time);
                camera.fieldOfView = Mathf.Lerp(currentFOV, targetFOV, smoothTime);
                OnFOVProgressCallback?.Invoke(smoothTime);
                yield return null;
            }
        }

        protected virtual void OnDisable()
        {
            RemoveInputActions();
        }

        internal void OnEnable_Internal()
        {
            OnEnable();
        }

        internal void OnDisable_Internal()
        {
            OnDisable();
        }

        protected virtual void RegisterInputActions()
        {
            InputReceiver.ZoomAction.performed += ZoomAction;
            InputReceiver.ZoomAction.canceled += ZoomAction;
        }

        protected virtual void RemoveInputActions()
        {
            InputReceiver.ZoomAction.performed -= ZoomAction;
            InputReceiver.ZoomAction.canceled -= ZoomAction;
        }

        private void ZoomAction(InputAction.CallbackContext context)
        {
            if (!isZooming && context.performed && (ZoomConditionCallback?.Invoke() ?? true))
            {
                ZoomIn();
            }
            else if (isZooming &&
                (context.performed && zoomHandleType == InputHandleType.Trigger) ||
                (context.canceled && zoomHandleType == InputHandleType.Hold))
            {
                ZoomOut();
            }
        }

        #region [Event Callback Function]
        /// <summary>
        /// Called when field of view changed.
        /// </summary>
        public event Action<float> OnFOVProgressCallback;

        /// <summary>
        /// Called when camera start zooming.
        /// </summary>
        public override event Action OnStartZoomCallback;

        /// <summary>
        /// Called when camera stop zooming.
        /// </summary>
        public override event Action OnStopZoomCallback;

        /// <summary>
        /// Called every time before start start zooming to check, can be camera start zoom.
        /// </summary>
        public override event Func<bool> ZoomConditionCallback;
        #endregion

        #region [Getter / Setter]
        public Transform GetHinge()
        {
            return hinge;
        }

        public void SetHinge(Transform value)
        {
            hinge = value;
        }

        public Vector2 GetSensitivity()
        {
            return sensitivity;
        }

        public void SetSensitivity(Vector2 value)
        {
            sensitivity = value;
        }

        public Vector2 GetRotationSmooth()
        {
            return rotationSmooth;
        }

        public void SetRotationSmooth(Vector2 value)
        {
            rotationSmooth = value;
        }

        public Vector2 GetVerticalRotationLimits()
        {
            return verticalRotationLimits;
        }

        public void SetVerticalRotationLimits(Vector2 value)
        {
            verticalRotationLimits = value;
        }

        public bool ClampVerticalRotation()
        {
            return clampVerticalRotation;
        }

        public void ClampVerticalRotation(bool value)
        {
            clampVerticalRotation = value;
        }

        public void InvertRotation(bool x, bool y)
        {
            invertRotation.x = x ? -1 : 1;
            invertRotation.y = y ? -1 : 1;
        }

        public bool IsHorizontalRotationInverted()
        {
            return invertRotation.x < 0;
        }

        public bool IsVerticalRotationInverted()
        {
            return invertRotation.y < 0;
        }

        public InputHandleType GetZoomHandleType()
        {
            return zoomHandleType;
        }

        public void SetZoomHandleType(InputHandleType value)
        {
            zoomHandleType = value;
        }

        public FieldOfViewSettings GetZoomSettings()
        {
            return zoomSettings;
        }

        public void SetZoomSettings(FieldOfViewSettings value)
        {
            zoomSettings = value;
        }

        public FieldOfViewSettings GetDefaultFOVSettings()
        {
            return defaultFOVSettings;
        }

        public void SetDefaultFOVSettings(FieldOfViewSettings value)
        {
            defaultFOVSettings = value;
        }

        public Camera GetCamera()
        {
            return camera;
        }

        public void SetCamera(Camera value)
        {
            camera = value;
        }

        public Vector2 GetInput()
        {
            return input;
        }

        public override bool IsZooming()
        {
            return isZooming;
        }
        #endregion
    }
}