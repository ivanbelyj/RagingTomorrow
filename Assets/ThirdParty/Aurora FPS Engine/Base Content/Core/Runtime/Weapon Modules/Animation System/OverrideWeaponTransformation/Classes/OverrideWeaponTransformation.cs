/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Serialization.Collections;
using AuroraFPSRuntime.SystemModules.ControllerSystems;
using UnityEngine;

namespace AuroraFPSRuntime.WeaponModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/Weapon Modules/Other/Override Weapon Transformation")]
    [DisallowMultipleComponent]
    public sealed class OverrideWeaponTransformation : MonoBehaviour
    {
        [System.Serializable]
        public sealed class Preset
        {
            [SerializeField]
            [Foldout("Position Settings", Style = "Indent")]
            private Vector3 position = Vector3.zero;

            [SerializeField]
            [Label("Smooth")]
            [VisualClamp(20, 0.01f)]
            [Foldout("Position Settings", Style = "Indent")]
            private float positionSmooth = 12.5f;

            [SerializeField]
            [Foldout("Rotation Settings", Style = "Indent")]
            private Vector3 eulerAngles = Vector3.zero;

            [SerializeField]
            [Label("Smooth")]
            [VisualClamp(20, 0.01f)]
            [Foldout("Rotation Settings", Style = "Indent")]
            private float rotationSmooth = 12.5f;

            #region [Getter / Setter]
            public Vector3 GetPosition()
            {
                return position;
            }

            public void SetPosition(Vector3 value)
            {
                position = value;
            }

            public float GetPositionSmooth()
            {
                return positionSmooth;
            }

            public void SetPositionSmooth(float value)
            {
                positionSmooth = value;
            }

            public Vector3 GetEulerAngles()
            {
                return eulerAngles;
            }

            public void SetEulerAngles(Vector3 value)
            {
                eulerAngles = value;
            }

            public float GetRotationSmooth()
            {
                return rotationSmooth;
            }

            public void SetRotationSmooth(float value)
            {
                rotationSmooth = value;
            }
            #endregion
        }

        [System.Serializable]
        public sealed class PresetDictionary : SerializableDictionary<ControllerState, Preset>
        {
            [SerializeField]
            private ControllerState[] keys;

            [SerializeField]
            private Preset[] values;

            protected override ControllerState[] GetKeys()
            {
                return keys;
            }

            protected override Preset[] GetValues()
            {
                return values;
            }

            protected override void SetKeys(ControllerState[] keys)
            {
                this.keys = keys;
            }

            protected override void SetValues(Preset[] values)
            {
                this.values = values;
            }
        }

        [SerializeField]
        [NotNull]
        private Transform hinge;

        [SerializeField]
        private PresetDictionary presets;

        [SerializeField]
        [Foldout("Advanced Settings")]
        [VisualClamp(20.0f, 0.01f)]
        private float positionSmooth;

        [SerializeField]
        [Foldout("Advanced Settings")]
        [VisualClamp(20.0f, 0.01f)]
        private float rotationSmooth;

        // Stored required components.
        private PlayerController controller;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            controller = transform.GetComponentInParent<PlayerController>();
        }

        /// <summary>
        /// Called after all Update functions have been called.
        /// </summary>
        private void LateUpdate()
        {
            if (presets.TryGetValue(controller.GetState(), out Preset preset))
            {
                hinge.localPosition = Vector3.Lerp(hinge.localPosition, preset.GetPosition(), preset.GetPositionSmooth() * Time.deltaTime);
                hinge.localRotation = Quaternion.Slerp(hinge.localRotation, Quaternion.Euler(preset.GetEulerAngles()), preset.GetRotationSmooth() * Time.deltaTime);
            }
            else
            {
                hinge.localPosition = Vector3.Lerp(hinge.localPosition, Vector3.zero, positionSmooth * Time.deltaTime);
                hinge.localRotation = Quaternion.Slerp(hinge.localRotation, Quaternion.identity, rotationSmooth * Time.deltaTime);
            }
        }
    }
}