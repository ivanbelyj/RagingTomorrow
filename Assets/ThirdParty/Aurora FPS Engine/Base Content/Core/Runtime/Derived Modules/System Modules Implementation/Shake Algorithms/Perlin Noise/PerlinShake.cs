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
    public class PerlinShake : ICameraShake
    {
        [System.Serializable]
        public struct NoiseMode
        {
            /// <summary>
            /// Frequency multiplier for the noise.
            /// </summary>
            [SerializeField]
            [Tooltip("Frequency multiplier for the noise.")]
            private float frequency;

            /// <summary>
            /// Amplitude of the mode.
            /// </summary>
            [SerializeField]
            [Tooltip("Amplitude of the mode.")]
            [Range(0, 1)]
            private float amplitude;

            public NoiseMode(float frequency, float amplitude)
            {
                this.frequency = frequency;
                this.amplitude = amplitude;
            }

            #region [Getter / Setter]
            public float GetFrequency()
            {
                return frequency;
            }

            public void SetFrequency(float value)
            {
                frequency = value;
            }

            public float GetAmplitude()
            {
                return amplitude;
            }

            public void SetAmplitude(float value)
            {
                amplitude = value;
            }
            #endregion
        }

        [System.Serializable]
        public class Settings
        {
            /// <summary>
            /// Strength of the shake for each axis.
            /// </summary>
            [SerializeField]
            [Tooltip("Strength of the shake for each axis.")]
            private Displacement strength = new Displacement(Vector3.zero, new Vector3(2, 2, 0.8f));

            /// <summary>
            /// Layers of perlin noise with different frequencies.
            /// </summary>
            [SerializeField]
            [Tooltip("Layers of perlin noise with different frequencies.")]
            private NoiseMode[] noiseModes = new NoiseMode[1] { new NoiseMode(12, 1) };

            /// <summary>
            /// Strength over time.
            /// </summary>
            [SerializeField]
            [Tooltip("Strength of the shake over time.")]
            private Envelope.EnvelopeSettings envelope;

            /// <summary>
            /// How strength falls with distance from the shake source.
            /// </summary>
            [SerializeField]
            [Tooltip("How strength falls with distance from the shake source.")]
            private Attenuator.StrengthAttenuationSettings attenuation;

            public Settings() { }

            public Settings(Displacement strength, NoiseMode[] noiseModes, Envelope.EnvelopeSettings envelope, Attenuator.StrengthAttenuationSettings attenuation)
            {
                this.strength = strength;
                this.noiseModes = noiseModes;
                this.envelope = envelope;
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

            public NoiseMode[] GetNoiseModes()
            {
                return noiseModes;
            }

            public NoiseMode GetNoiseMode(int index)
            {
                return noiseModes[index];
            }

            public void SetNoiseModes(NoiseMode[] value)
            {
                noiseModes = value;
            }

            public int GetNoiseModesCount()
            {
                return noiseModes.Length;
            }

            public Envelope.EnvelopeSettings GetEnvelope()
            {
                return envelope;
            }

            public void SetEnvelope(Envelope.EnvelopeSettings value)
            {
                envelope = value;
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
        private readonly Envelope envelope;

        private IAmplitudeController AmplitudeController;
        private Displacement currentDisplacement;
        private Vector2[] seeds;
        private float time;
        private Vector3? sourcePosition;
        private float norm;
        private bool isFinished;

        /// <summary>
        /// Creates an instance of PerlinShake.
        /// </summary>
        /// <param name="settings">Parameters of the shake.</param>
        /// <param name="maxAmplitude">Maximum amplitude of the shake.</param>
        /// <param name="sourcePosition">World position of the source of the shake.</param>
        /// <param name="manualStrengthControl">Pass true if you want to control amplitude manually.</param>
        public PerlinShake(Settings settings, float maxAmplitude = 1, Vector3? sourcePosition = null, bool manualStrengthControl = false)
        {
            this.settings = settings;
            envelope = new Envelope(this.settings.GetEnvelope(), maxAmplitude,
                manualStrengthControl ?
                    Envelope.EnvelopeControlMode.Manual : Envelope.EnvelopeControlMode.Auto);
            AmplitudeController = envelope;
            this.sourcePosition = sourcePosition;
        }

        #region [ICameraShake Implementation]
        public void Initialize(Vector3 cameraPosition, Quaternion cameraRotation)
        {
            seeds = new Vector2[settings.GetNoiseModesCount()];
            norm = 0;
            for (int i = 0; i < seeds.Length; i++)
            {
                seeds[i] = Random.insideUnitCircle * 20;
                norm += settings.GetNoiseMode(i).GetAmplitude();
            }
        }

        public void Update(float deltaTime, Vector3 cameraPosition, Quaternion cameraRotation)
        {
            if (envelope.IsFinished())
            {
                isFinished = true;
                return;
            }
            time += deltaTime;
            envelope.Update(deltaTime);

            Displacement disp = Displacement.zero;
            for (int i = 0; i < settings.GetNoiseModesCount(); i++)
            {
                disp += settings.GetNoiseMode(i).GetAmplitude() / norm *
                    SampleNoise(seeds[i], settings.GetNoiseMode(i).GetFrequency());
            }

            currentDisplacement = envelope.GetIntensity() * Displacement.Scale(disp, settings.GetStrength());
            if (sourcePosition != null)
                currentDisplacement *= Attenuator.Strength(settings.GetAttenuation(), sourcePosition.Value, cameraPosition);
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

        private Displacement SampleNoise(Vector2 seed, float freq)
        {
            Vector3 position = new Vector3(
                Mathf.PerlinNoise(seed.x + time * freq, seed.y),
                Mathf.PerlinNoise(seed.x, seed.y + time * freq),
                Mathf.PerlinNoise(seed.x + time * freq, seed.y + time * freq));
            position -= Vector3.one * 0.5f;

            Vector3 rotation = new Vector3(
                Mathf.PerlinNoise(-seed.x - time * freq, -seed.y),
                Mathf.PerlinNoise(-seed.x, -seed.y - time * freq),
                Mathf.PerlinNoise(-seed.x - time * freq, -seed.y - time * freq));
            rotation -= Vector3.one * 0.5f;

            return new Displacement(position, rotation);
        }
    }
}
