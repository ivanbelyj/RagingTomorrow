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
using UnityEngine;

namespace AuroraFPSRuntime.WeaponModules
{
    [System.Serializable]
    [WeaponEffectMenu("Weapon Sway", "Transformation/Weapon Sway")]
    public sealed class WeaponSwayEffect : WeaponEffect
    {
        [SerializeField]
        private Vector2 positionAmount = new Vector2(0.1f, 0.05f);

        [SerializeField]
        [Label("Smooth")]
        [Indent]
        private float positionSmooth = 3.0f;

        [SerializeField]
        private Vector2 rotationAmount = new Vector2(5.0f, 5.0f);

        [SerializeField]
        [Label("Smooth")]
        [Indent]
        private float rotationSmooth = 7.5f;

        [SerializeField]
        [CustomView(ViewGUI = "OnGroupBoolGUI")]
        private Vector2Int invert = new Vector2Int(-1, 1);

        // Stored required properties.
        private Vector3 targetPosition;
        private Vector3 targetEulerAngels;

        public override void OnUpdate()
        {
            // Read input.
            float vertical = InputReceiver.CameraVerticalAction.ReadValue<float>() * invert.y;
            float horizontal = InputReceiver.CameraHorizontalAction.ReadValue<float>() * invert.x;

            vertical = Mathf.Clamp(vertical, -1, 1);
            horizontal = Mathf.Clamp(horizontal, -1, 1);

            // Calculate target position.
            targetPosition.x = horizontal * positionAmount.x;
            targetPosition.y = vertical * positionAmount.y;

            // Calculate target euler angels.
            targetEulerAngels.x = vertical * rotationAmount.y;
            targetEulerAngels.y = horizontal * rotationAmount.x;

            // Convert euler angels to quaternion.
            Quaternion targetRotation = Quaternion.Euler(targetEulerAngels);

            // Apply position and rotation.
            hinge.localPosition = Vector3.Lerp(hinge.localPosition, targetPosition, positionSmooth * Time.deltaTime);
            hinge.localRotation = Quaternion.Slerp(hinge.localRotation, targetRotation, rotationSmooth * Time.deltaTime);
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
