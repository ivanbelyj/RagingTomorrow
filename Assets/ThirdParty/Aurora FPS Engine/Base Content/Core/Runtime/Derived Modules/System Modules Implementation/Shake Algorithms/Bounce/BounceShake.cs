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
    public class BounceShake : ICameraShake
    {
        [System.Serializable]
        public class Settings
        {
            /// <summary>
            /// Strength of the shake for positional axes.
            /// </summary>
            [SerializeField]
            [Tooltip("Strength of the shake for positional axes.")]
            private float positionStrength = 0.05f;

            /// <summary>
            /// Strength of the shake for rotational axes.
            /// </summary>
            [SerializeField]
            [Tooltip("Strength of the shake for rotational axes.")]
            private float rotationStrength = 0.1f;

            /// <summary>
            /// Preferred direction of shaking.
            /// </summary>
            [SerializeField]
            [Tooltip("Preferred direction of shaking.")]
            private Displacement axesMultiplier = new Displacement(Vector2.one, Vector3.forward);

            /// <summary>
            /// Frequency of shaking.
            /// </summary>
            [SerializeField]
            [Tooltip("Frequency of shaking.")]
            private float frequency = 25;

            /// <summary>
            /// Number of vibrations before stop.
            /// </summary>
            [SerializeField]
            [Tooltip("Number of vibrations before stop.")]
            private int numBounces = 5;

            /// <summary>
            /// Randomness of motion.
            /// </summary>
            [SerializeField]
            [Range(0, 1)]
            [Tooltip("Randomness of motion.")]
            private float randomness = 0.5f;

            /// <summary>
            /// How strength falls with distance from the shake source.
            /// </summary>
            [SerializeField]
            [Tooltip("How strength falls with distance from the shake source.")]
            private Attenuator.StrengthAttenuationSettings attenuation;

            public Settings() { }

            public Settings(float positionStrength, float rotationStrength, float frequency, int numBounces)
            {
                this.positionStrength = positionStrength;
                this.rotationStrength = rotationStrength;
                this.frequency = frequency;
                this.numBounces = numBounces;
            }

            public Settings(float positionStrength, float rotationStrength, Displacement axesMultiplier, float frequency, int numBounces, float randomness, Attenuator.StrengthAttenuationSettings attenuation)
            {
                this.positionStrength = positionStrength;
                this.rotationStrength = rotationStrength;
                this.axesMultiplier = axesMultiplier;
                this.frequency = frequency;
                this.numBounces = numBounces;
                this.randomness = randomness;
                this.attenuation = attenuation;
            }

            

            #region [Getter / Setter]
            public float GetPositionStrength()
            {
                return positionStrength;
            }

            public void SetPositionStrength(float value)
            {
                positionStrength = value;
            }

            public float GetRotationStrength()
            {
                return rotationStrength;
            }

            public void SetRotationStrength(float value)
            {
                rotationStrength = value;
            }

            public Displacement GetAxesMultiplier()
            {
                return axesMultiplier;
            }

            public void SetAxesMultiplier(Displacement value)
            {
                axesMultiplier = value;
            }

            public float GetFrequency()
            {
                return frequency;
            }

            public void SetFrequency(float value)
            {
                frequency = value;
            }

            public int GetNumBounces()
            {
                return numBounces;
            }

            public void SetNumBounces(int value)
            {
                numBounces = value;
            }

            public float GetRandomness()
            {
                return randomness;
            }

            public void SetRandomness(float value)
            {
                randomness = value;
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
        private readonly AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        private readonly Vector3? sourcePosition = null;

        private float attenuation = 1;
        private Displacement direction;
        private Displacement previousWaypoint;
        private Displacement currentWaypoint;
        private Displacement currentDisplacement;
        private int bounceIndex;
        private float time;
        private bool isFinished;

        /// <summary>
        /// Creates an instance of BounceShake.
        /// </summary>
        /// <param name="settings">Parameters of the shake.</param>
        /// <param name="sourcePosition">World position of the source of the shake.</param>
        public BounceShake(Settings settings, Vector3? sourcePosition = null)
        {
            this.sourcePosition = sourcePosition;
            this.settings = settings;
            Displacement rnd = Displacement.InsideUnitSpheres();
            direction = Displacement.Scale(rnd, this.settings.GetAxesMultiplier()).Normalized;
        }

        /// <summary>
        /// Creates an instance of BounceShake.
        /// </summary>
        /// <param name="settings">Parameters of the shake.</param>
        /// <param name="initialDirection">Initial direction of the shake motion.</param>
        /// <param name="sourcePosition">World position of the source of the shake.</param>
        public BounceShake(Settings settings, Displacement initialDirection, Vector3? sourcePosition = null)
        {
            this.sourcePosition = sourcePosition;
            this.settings = settings;
            direction = Displacement.Scale(initialDirection, this.settings.GetAxesMultiplier()).Normalized;
        }

        #region [ICameraShake Implementation]
        public void Initialize(Vector3 cameraPosition, Quaternion cameraRotation)
        {
            attenuation = sourcePosition == null ?
                1 : Attenuator.Strength(settings.GetAttenuation(), sourcePosition.Value, cameraPosition);
            currentWaypoint = attenuation * direction.ScaledBy(settings.GetPositionStrength(), settings.GetRotationStrength());
        }

        public void Update(float deltaTime, Vector3 cameraPosition, Quaternion cameraRotation)
        {
            if (time < 1)
            {

                time += deltaTime * settings.GetFrequency();
                if (settings.GetFrequency() == 0) time = 1;

                currentDisplacement = Displacement.Lerp(previousWaypoint, currentWaypoint,
                    moveCurve.Evaluate(time));
            }
            else
            {
                time = 0;
                currentDisplacement = currentWaypoint;
                previousWaypoint = currentWaypoint;
                bounceIndex++;
                if (bounceIndex > settings.GetNumBounces())
                {
                    isFinished = true;
                    return;
                }

                Displacement rnd = Displacement.InsideUnitSpheres();
                direction = -direction
                    + settings.GetRandomness() * Displacement.Scale(rnd, settings.GetAxesMultiplier()).Normalized;
                direction = direction.Normalized;
                float decayValue = 1 - (float)bounceIndex / settings.GetNumBounces();
                currentWaypoint = decayValue * decayValue * attenuation
                    * direction.ScaledBy(settings.GetPositionStrength(), settings.GetRotationStrength());
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
    }
}
