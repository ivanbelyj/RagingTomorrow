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
    /// Controls strength of the shake over time.
    /// </summary>
    [System.Serializable]
    public class Envelope : IAmplitudeController
    {
        [System.Serializable]
        public class EnvelopeSettings
        {
            /// <summary>
            /// How fast the amplitude rises.
            /// </summary>
            [SerializeField]
            [Tooltip("How fast the amplitude increases.")]
            private float attack = 10.0f;

            /// <summary>
            /// How long in seconds the amplitude holds a maximum value.
            /// </summary>
            [SerializeField]
            [Tooltip("How long in seconds the amplitude holds maximum value.")]
            private float sustain = 0.0f;

            /// <summary>
            /// How fast the amplitude falls.
            /// </summary>
            [SerializeField]
            [Tooltip("How fast the amplitude decreases.")]
            private float decay = 1.0f;

            /// <summary>
            /// Power in which the amplitude is raised to get intensity.
            /// </summary>
            [SerializeField]
            [Tooltip("Power in which the amplitude is raised to get intensity.")]
            private Degree degree = Degree.Cubic;

            public EnvelopeSettings()
            {

            }

            public EnvelopeSettings(float attack, float sustain, float decay, Degree degree)
            {
                this.attack = attack;
                this.sustain = sustain;
                this.decay = decay;
                this.degree = degree;
            }

            

            #region [Getter / Setter]
            public float GetAttack()
            {
                return attack;
            }

            public void SetAttack(float value)
            {
                attack = value;
            }

            public float GetSustain()
            {
                return sustain;
            }

            public void SetSustain(float value)
            {
                sustain = value;
            }

            public float GetDecay()
            {
                return decay;
            }

            public void SetDecay(float value)
            {
                decay = value;
            }

            public Degree GetDegree()
            {
                return degree;
            }

            public void SetDegree(Degree value)
            {
                degree = value;
            }
            #endregion
        }

        public enum EnvelopeControlMode
        {
            Auto,
            Manual
        }

        public enum EnvelopeState
        {
            Sustain,
            Increase,
            Decrease
        }

        private readonly EnvelopeSettings settings;
        private readonly EnvelopeControlMode controlMode;

        private float amplitude;
        private float intensity;
        private float targetAmplitude;
        private float sustainEndTime;
        private bool finishWhenAmplitudeZero;
        private bool finishImmediately;
        private EnvelopeState state;


        /// <summary>
        /// Creates an Envelope instance.
        /// </summary>
        /// <param name="settings">Envelope parameters.</param>
        /// <param name="controlMode">Pass Auto for a single shake, or Manual for controlling strength manually.</param>
        public Envelope(EnvelopeSettings settings, float initialTargetAmplitude, EnvelopeControlMode controlMode)
        {
            this.settings = settings;
            this.controlMode = controlMode;
            SetTarget(initialTargetAmplitude);
        }

        /// <summary>
        /// Update is called every frame by the shake.
        /// </summary>
        public void Update(float deltaTime)
        {
            if (IsFinished()) return;

            if (state == EnvelopeState.Increase)
            {
                if (settings.GetAttack() > 0)
                    amplitude += deltaTime * settings.GetAttack();
                if (amplitude > targetAmplitude || settings.GetAttack() <= 0)
                {
                    amplitude = targetAmplitude;
                    state = EnvelopeState.Sustain;
                    if (controlMode == EnvelopeControlMode.Auto)
                        sustainEndTime = Time.time + settings.GetSustain();
                }
            }
            else
            {
                if (state == EnvelopeState.Decrease)
                {

                    if (settings.GetDecay() > 0)
                        amplitude -= deltaTime * settings.GetDecay();
                    if (amplitude < targetAmplitude || settings.GetDecay() <= 0)
                    {
                        amplitude = targetAmplitude;
                        state = EnvelopeState.Sustain;
                    }
                }
                else
                {
                    if (controlMode == EnvelopeControlMode.Auto && Time.time > sustainEndTime)
                    {
                        SetTarget(0);
                    }
                }
            }

            amplitude = Mathf.Clamp01(amplitude);
            SetIntensity(Power.Evaluate(amplitude, settings.GetDegree()));
        }

        public bool IsFinished()
        {
            if (finishImmediately) return true;
            return (finishWhenAmplitudeZero || controlMode == EnvelopeControlMode.Auto)
                && amplitude <= 0 && targetAmplitude <= 0;
        }

        private void SetTarget(float value)
        {
            targetAmplitude = Mathf.Clamp01(value);
            state = targetAmplitude > amplitude ? EnvelopeState.Increase : EnvelopeState.Decrease;
        }

        #region [IAmplitudeController Implementation]
        public void SetTargetAmplitude(float value)
        {
            if (controlMode == EnvelopeControlMode.Manual && !finishWhenAmplitudeZero)
            {
                SetTarget(value);
            }
        }

        public void Finish()
        {
            finishWhenAmplitudeZero = true;
            SetTarget(0);
        }

        public void FinishImmediately()
        {
            finishImmediately = true;
        }

        #endregion
        #region [Getter / Setter]

        #endregion

        #region [Getter / Setter]
        public float GetIntensity()
        {
            return intensity;
        }

        public void SetIntensity(float value)
        {
            intensity = value;
        }
        #endregion
    }
}
