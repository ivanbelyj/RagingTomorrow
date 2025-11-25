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
    public class KickShake : ICameraShake
    {
        [System.Serializable]
        public class Settings
        {
            /// <summary>
            /// Strength of the shake for each axis.
            /// </summary>
            [SerializeField]
            [Tooltip("Strength of the shake for each axis.")]
            private Displacement strength = new Displacement(Vector3.zero, Vector3.one);

            /// <summary>
            /// How long it takes to move forward.
            /// </summary>
            [SerializeField]
            [Tooltip("How long it takes to move forward.")]
            private float attackTime = 0.05f;

            [SerializeField]
            private AnimationCurve attackCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

            /// <summary>
            /// How long it takes to move back.
            /// </summary>
            [SerializeField]
            [Tooltip("How long it takes to move back.")]
            private float releaseTime = 0.2f;

            [SerializeField]
            private AnimationCurve releaseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

            /// <summary>
            /// How strength falls with distance from the shake source.
            /// </summary>
            [SerializeField]
            [Tooltip("How strength falls with distance from the shake source.")]
            private Attenuator.StrengthAttenuationSettings attenuation;

            public Settings() { }

            public Settings(Displacement strength, float attackTime, AnimationCurve attackCurve, float releaseTime, AnimationCurve releaseCurve, Attenuator.StrengthAttenuationSettings attenuation)
            {
                this.strength = strength;
                this.attackTime = attackTime;
                this.attackCurve = attackCurve;
                this.releaseTime = releaseTime;
                this.releaseCurve = releaseCurve;
                this.attenuation = attenuation;
            }



            #region [Getter / Setter]
            public Displacement GetStrength()
            {
                return strength;
            }

            public void SetStrength(Displacement value)
            {
                strength = value;
            }

            public float GetAttackTime()
            {
                return attackTime;
            }

            public void SetAttackTime(float value)
            {
                attackTime = value;
            }

            public AnimationCurve GetAttackCurve()
            {
                return attackCurve;
            }

            public void SetAttackCurve(AnimationCurve value)
            {
                attackCurve = value;
            }

            public float GetReleaseTime()
            {
                return releaseTime;
            }

            public void SetReleaseTime(float value)
            {
                releaseTime = value;
            }

            public AnimationCurve GetReleaseCurve()
            {
                return releaseCurve;
            }

            public void SetReleaseCurve(AnimationCurve value)
            {
                releaseCurve = value;
            }

            public Attenuator.StrengthAttenuationSettings GetAttenuation()
            {
                return attenuation;
            }

            public void SetAttenuation(Attenuator.StrengthAttenuationSettings value)
            {
                attenuation = value;
            }
            #endregion
        }

        private readonly Settings settings;
        private readonly Vector3? sourcePosition;
        private readonly bool attenuateStrength;

        private Displacement direction;
        private Displacement prevWaypoint;
        private Displacement currentWaypoint;
        private Displacement currentDisplacement;
        private bool release;
        private float time;
        private bool isFinished;

        /// <summary>
        /// Creates an instance of KickShake in the direction from the source to the camera.
        /// </summary>
        /// <param name="settings">Parameters of the shake.</param>
        /// <param name="sourcePosition">World position of the source of the shake.</param>
        /// <param name="attenuateStrength">Change strength depending on distance from the camera?</param>
        public KickShake(Settings settings, Vector3 sourcePosition, bool attenuateStrength)
        {
            this.settings = settings;
            this.sourcePosition = sourcePosition;
            this.attenuateStrength = attenuateStrength;
        }

        /// <summary>
        /// Creates an instance of KickShake. 
        /// </summary>
        /// <param name="settings">Parameters of the shake.</param>
        /// <param name="direction">Direction of the kick.</param>
        public KickShake(Settings settings, Displacement direction)
        {
            this.settings = settings;
            this.direction = direction.Normalized;
        }

        #region [ICameraShake Implementation]
        public void Initialize(Vector3 cameraPosition, Quaternion cameraRotation)
        {
            if (sourcePosition != null)
            {
                direction = Attenuator.Direction(sourcePosition.Value, cameraPosition, cameraRotation);
                if (attenuateStrength)
                    direction *= Attenuator.Strength(settings.GetAttenuation(), sourcePosition.Value, cameraPosition);
            }
            currentWaypoint = Displacement.Scale(direction, settings.GetStrength());
        }

        public void Update(float deltaTime, Vector3 cameraPosition, Quaternion cameraRotation)
        {
            if (time < 1)
            {
                Move(deltaTime,
                    release ? settings.GetReleaseTime() : settings.GetAttackTime(),
                    release ? settings.GetReleaseCurve() : settings.GetAttackCurve());
            }
            else
            {
                currentDisplacement = currentWaypoint;
                prevWaypoint = currentWaypoint;
                if (release)
                {
                    isFinished = true;
                    return;
                }
                else
                {
                    release = true;
                    time = 0;
                    currentWaypoint = Displacement.zero;
                }
            }
        }

        public bool IsFinished()
        {
            return isFinished;
        }

        public Displacement GetCurrentDisplacement()
        {
            return currentDisplacement;
        }
        #endregion

        private void Move(float deltaTime, float duration, AnimationCurve curve)
        {
            if (duration > 0)
                time += deltaTime / duration;
            else
                time = 1;
            currentDisplacement = Displacement.Lerp(prevWaypoint, currentWaypoint, curve.Evaluate(time));
        }
    }
}
