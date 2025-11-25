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

namespace AuroraFPSRuntime.WeaponModules.EffectSystem
{
    [System.Serializable]
    [ReferenceContent("Sway", "Weapon/Sway")]
    public sealed class SwayEffect : Effect
    {
        [SerializeField]
        [Label("Amount X")]
        [Slider(-0.0007f, 0.0007f)]
        [Foldout("Position Settings")]
        private float positionAmountX = 0.0001f;

        [SerializeField]
        [Label("Amount Y")]
        [Slider(-0.0007f, 0.0007f)]
        [Foldout("Position Settings")]
        private float positionAmountY = 0.0001f;

        [SerializeField]
        [Label("Snappiness")]
        [Foldout("Position Settings")]
        private float positionSnappiness = 2.5f;

        [SerializeField]
        [Label("Snappiness Back")]
        [Foldout("Position Settings")]
        private float positionSnappinessBack = 5.0f;

        [SerializeField]
        [Label("Amount X")]
        [Slider(-0.01f, 0.01f)]
        [Foldout("Rotation Settings")]
        private float rotationAmountX = 0.0025f;

        [SerializeField]
        [Label("Amount Y")]
        [Slider(-0.01f, 0.01f)]
        [Foldout("Rotation Settings")]
        private float rotationAmountY = 0.0025f;

        [SerializeField]
        [Label("Amount Z")]
        [Slider(-0.01f, 0.01f)]
        [Foldout("Rotation Settings")]
        private float rotationAmountZ = -0.0025f;

        [SerializeField]
        [Label("Snappiness")]
        [Foldout("Rotation Settings")]
        private float rotationSnappiness = 2.5f;

        [SerializeField]
        [Label("Snappiness Back")]
        [Foldout("Rotation Settings")]
        private float rotationSnappinessBack = 5.0f;

        [SerializeField]
        [Slider(0, 0.9f)]
        private float zoomModifier = 0.25f;

        [SerializeField]
        [CustomView(ViewGUI = "OnGroupBoolGUI")]
        private Vector2Int invert = new Vector2Int(-1, 1);

        // Stored required components.
        private Transform hinge;
        private PlayerCamera playerCamera;

        // Stored required properties.
        private Vector3 targetPosition;
        private Vector3 targetRotation;

        /// <summary>
        /// Called when the effect instance is being loaded.
        /// </summary>
        /// <param name="weapon">Weapon owner transform of this effect instance.</param>
        public override void Initialize(Transform weapon)
        {
            playerCamera = weapon.GetComponentInParent<PlayerCamera>();
            hinge = InstantiateHinge("Sway Animation Hinge", weapon);
        }

        /// <summary>
        /// Called every frame, if the effect is enabled.
        /// </summary>
        public override void OnAnimationUpdate()
        {
            float horizontal = InputReceiver.CameraHorizontalAction.ReadValue<float>() * invert.x;
            float vertical = InputReceiver.CameraVerticalAction.ReadValue<float>() * invert.y;

            if (playerCamera != null && playerCamera.IsZooming())
            {
                horizontal *= zoomModifier;
                vertical *= zoomModifier;
            }

            targetRotation.x += vertical * rotationAmountX;
            targetRotation.y += horizontal * rotationAmountY;
            targetRotation.z -= horizontal * rotationAmountZ;
            float time = targetRotation == Vector3.zero ? rotationSnappinessBack : rotationSnappiness;
            targetRotation = Vector3.Slerp(targetRotation, Vector3.zero, time * Time.deltaTime);
            hinge.localRotation = Quaternion.Euler(targetRotation);

            targetPosition.x += horizontal * positionAmountX;
            targetPosition.y += vertical * positionAmountY;
            time = targetPosition == Vector3.zero ? positionSnappinessBack : positionSnappiness;
            targetPosition = Vector3.Lerp(targetPosition, Vector3.zero, time * Time.deltaTime);
            hinge.localPosition = targetPosition;
        }


        #region [Editor Section]
#if UNITY_EDITOR
        private void OnGroupBoolGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            Vector2Int invert = property.vector2IntValue;

            if (invert.x == 0)
                invert.x = 1;
            if (invert.y == 0)
                invert.y = 1;

            bool invertX = invert.x > 0;
            bool invertY = invert.y > 0;

            float y = position.y;
            position = UnityEditor.EditorGUI.PrefixLabel(position, label);
            position.y = y;
            Rect invertXLabelPosition = new Rect(position.x, position.y, 15, position.height);
            GUI.Label(invertXLabelPosition, "X");

            Rect invertXFieldPosition = new Rect(invertXLabelPosition.xMax, position.y, 20, position.height);
            invertX = UnityEditor.EditorGUI.Toggle(invertXFieldPosition, invertX);

            Rect invertYLabelPosition = new Rect(invertXFieldPosition.xMax, position.y, 15, position.height);
            GUI.Label(invertYLabelPosition, "Y");

            Rect invertYFieldPosition = new Rect(invertYLabelPosition.xMax, position.y, 20, position.height);
            invertY = UnityEditor.EditorGUI.Toggle(invertYFieldPosition, invertY);

            invert.x = invertX ? 1 : -1;
            invert.y = invertY ? 1 : -1;

            property.vector2IntValue = invert;
        }
#endif
        #endregion
    }
}
