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
using AuroraFPSRuntime.CoreModules;
using UnityEngine;

#region [Unity Editor Namespaces]
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

namespace AuroraFPSRuntime.SystemModules.CameraSystems
{
    [HideScriptField]
    [AddComponentMenu(null)]
    [DisallowMultipleComponent]
    public abstract class PawnCamera : EntityCamera, IControlInput
    {
        [SerializeField]
        [Foldout("Input Settings", Style = "Header")]
        [Order(-899)]
        private Vector2 sensitivity = new Vector2(1, 1);

        [SerializeField]
        [CustomView(ViewGUI = "OnGroupBoolGUI")]
        [Foldout("Input Settings", Style = "Header")]
        [Order(-898)]
        private Vector2Int invertRotation = Vector2Int.zero;

        // Stored required properties.
        private Vector2 inputVector = Vector2.zero;

        /// <summary>
        /// Called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        protected override void Update()
        {
            ReadInput();
            base.Update();
        }

        /// <summary>
        /// Read required input values before calculation rotation.
        /// </summary>
        protected virtual void ReadInput()
        {
            Vector2 sensitivity = CalculateSensitivity();
            inputVector.x = InputReceiver.CameraHorizontalAction.ReadValue<float>() * sensitivity.x * invertRotation.x;
            inputVector.y = InputReceiver.CameraVerticalAction.ReadValue<float>() * sensitivity.y * invertRotation.y;
        }

        /// <summary>
        /// Called when reading input value to software sensitivity.
        /// </summary>
        /// <returns>Software sensitivity.</returns>
        protected virtual Vector2 CalculateSensitivity()
        {
            return sensitivity;
        }

        public Vector2 GetControlInput()
        {
            return inputVector;
        }

        #region [Unity Editor]
#if UNITY_EDITOR
        private void OnGroupBoolGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Vector2Int invert = property.vector2IntValue;

            if (invert.x == 0)
                invert.x = 1;
            if (invert.y == 0)
                invert.y = 1;

            bool invertX = invert.x < 0;
            bool invertY = invert.y < 0;

            float y = position.y;
            position = EditorGUI.PrefixLabel(position, label);
            position.y = y;
            Rect invertXLabelPosition = new Rect(position.x, position.y, 15, position.height);
            GUI.Label(invertXLabelPosition, "X");

            Rect invertXFieldPosition = new Rect(invertXLabelPosition.xMax, position.y, 20, position.height);
            invertX = EditorGUI.Toggle(invertXFieldPosition, invertX);

            Rect invertYLabelPosition = new Rect(invertXFieldPosition.xMax, position.y, 15, position.height);
            GUI.Label(invertYLabelPosition, "Y");

            Rect invertYFieldPosition = new Rect(invertYLabelPosition.xMax, position.y, 20, position.height);
            invertY = EditorGUI.Toggle(invertYFieldPosition, invertY);

            invert.x = invertX ? -1 : 1;
            invert.y = invertY ? -1 : 1;

            property.vector2IntValue = invert;
        }
#endif
#endregion

        #region [Getter / Setter]
        public Vector2 GetSensitivity()
        {
            return sensitivity;
        }

        public void SetSensitivity(Vector2 value)
        {
            sensitivity = value;
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
        #endregion
    }
}