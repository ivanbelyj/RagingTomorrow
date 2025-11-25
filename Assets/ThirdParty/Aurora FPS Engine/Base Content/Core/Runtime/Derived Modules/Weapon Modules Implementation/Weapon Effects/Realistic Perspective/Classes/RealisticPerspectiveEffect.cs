/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.SystemModules.CameraSystems;
using UnityEngine;

namespace AuroraFPSRuntime.WeaponModules.EffectSystem
{
    [ReferenceContent("Realistic Perspective", "Weapon/Realistic Perspective")]
    public sealed class RealisticPerspectiveEffect : Effect
    {
        [SerializeField]
        private float amount = 0.0025f;

        [SerializeField]
        private bool interpolate = true;

        [SerializeField]
        [VisibleIf("interpolate", true)]
        [Indent(1)]
        private float smooth = 12.5f;

        [SerializeField]
        private float zoomSmooth = 12.5f;

        // Stored required properties.
        private Transform hinge;
        private PlayerCamera camera;

        /// <summary>
        /// Called when the effect instance is being loaded.
        /// </summary>
        /// <param name="weapon">Weapon owner transform of this effect instance.</param>
        public override void Initialize(Transform weapon)
        {
            camera = weapon.GetComponentInParent<PlayerCamera>();
            hinge = InstantiateHinge("Realistic Perspective Hinge", weapon);
        }

        /// <summary>
        /// Called every frame, if the effect is enabled.
        /// </summary>
        public override void OnAnimationUpdate()
        {
            if (!camera.IsZooming())
            {
                Vector3 position = hinge.localPosition;
                position.y = EulerToRotation(camera.GetHinge().localEulerAngles.x) * amount;
                if (interpolate)
                    hinge.localPosition = Vector3.Lerp(hinge.localPosition, position, smooth * Time.deltaTime);
                else
                    hinge.localPosition = position;
            }
            else
            {
                hinge.localPosition = Vector3.Lerp(hinge.localPosition, Vector3.zero, zoomSmooth * Time.deltaTime);
            }
        }

        private float EulerToRotation(float value)
        {
            if (value > 180)
            {
                return value - 360f;
            }
            else if (value < -180)
            {
                return value + 360f;
            }
            else
            {
                return value;
            }
        }
    }
}