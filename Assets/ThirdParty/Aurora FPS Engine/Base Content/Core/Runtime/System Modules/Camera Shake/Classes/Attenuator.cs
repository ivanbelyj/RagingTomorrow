/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;

namespace AuroraFPSRuntime.SystemModules
{
    /// <summary>
    /// Contains methods for changing strength and direction of shakes depending on their position.
    /// </summary>
    public static class Attenuator
    {
        [System.Serializable]
        public class StrengthAttenuationSettings
        {
            /// <summary>
            /// Radius in which shake doesn't lose strength.
            /// </summary>
            [SerializeField]
            [Tooltip("Radius in which shake doesn't lose strength.")]
            private float clippingDistance = 10.0f;

            /// <summary>
            /// Defines how fast strength falls with distance.
            /// </summary>
            [SerializeField]
            [Tooltip("How fast strength falls with distance.")]
            private float falloffScale = 50.0f;

            /// <summary>
            /// Power of the falloff function.
            /// </summary>
            [SerializeField]
            [Tooltip("Power of the falloff function.")]
            private Degree falloffDegree = Degree.Quadratic;

            /// <summary>
            /// Contribution of each axis to distance. E. g. (1, 1, 0) for a 2D game in XY plane.
            /// </summary>
            [SerializeField]
            [Tooltip("Contribution of each axis to distance. E. g. (1, 1, 0) for a 2D game in XY plane.")]
            private Vector3 axesMultiplier = Vector3.one;

            public StrengthAttenuationSettings() { }

            public StrengthAttenuationSettings(float clippingDistance, float falloffScale, Degree falloffDegree, Vector3 axesMultiplier)
            {
                this.clippingDistance = clippingDistance;
                this.falloffScale = falloffScale;
                this.falloffDegree = falloffDegree;
                this.axesMultiplier = axesMultiplier;
            }

            #region [Getter / Setter]
            public float GetClippingDistance()
            {
                return clippingDistance;
            }

            public void SetClippingDistance(float value)
            {
                clippingDistance = value;
            }

            public float GetFalloffScale()
            {
                return falloffScale;
            }

            public void SetFalloffScale(float value)
            {
                falloffScale = value;
            }

            public Degree GetFalloffDegree()
            {
                return falloffDegree;
            }

            public void SetFalloffDegree(Degree value)
            {
                falloffDegree = value;
            }

            public Vector3 GetAxesMultiplier()
            {
                return axesMultiplier;
            }

            public void SetAxesMultiplier(Vector3 value)
            {
                axesMultiplier = value;
            }
            #endregion
        }

        /// <summary>
        /// Returns multiplier for the strength of a shake, based on source and camera positions.
        /// </summary>
        public static float Strength(StrengthAttenuationSettings settings, Vector3 sourcePosition, Vector3 cameraPosition)
        {
            Vector3 vec = cameraPosition - sourcePosition;
            float distance = Vector3.Scale(settings.GetAxesMultiplier(), vec).magnitude;
            float strength = Mathf.Clamp01(1 - (distance - settings.GetClippingDistance()) / settings.GetFalloffScale());
            return Power.Evaluate(strength, settings.GetFalloffDegree());
        }

        /// <summary>
        /// Returns displacement, opposite to the direction to the source in camera's local space.
        /// </summary>
        public static Displacement Direction(Vector3 sourcePosition, Vector3 cameraPosition, Quaternion cameraRotation)
        {
            Displacement direction = Displacement.zero;
            direction.SetPosition((cameraPosition - sourcePosition).normalized);
            direction.SetPosition(Quaternion.Inverse(cameraRotation) * direction.GetPosition());

            direction.SetEulerAngles(direction.GetPosition().z, direction.GetPosition().x, direction.GetPosition().x);

            return direction;
        }
    }
}
