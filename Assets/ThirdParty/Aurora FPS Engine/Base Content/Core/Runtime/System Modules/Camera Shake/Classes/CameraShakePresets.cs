/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.ControllerModules
{
    /// <summary>
    /// Contains shorthands for creating common shake types.
    /// </summary>
    public class CameraShakePresets
    {
        private readonly CameraShaker shaker;

        public CameraShakePresets(CameraShaker shaker)
        {
            this.shaker = shaker;
        }

        /// <summary>
        /// Suitable for short and snappy shakes in 2D. Moves camera in X and Y axes and rotates it in Z axis. 
        /// </summary>
        /// <param name="positionStrength">Strength of motion in X and Y axes.</param>
        /// <param name="rotationStrength">Strength of rotation in Z axis.</param>
        /// <param name="frequency">Frequency of shaking.</param>
        /// <param name="numBounces">Number of vibrations before stop.</param>
        public void ShortShake2D(float positionStrength = 0.08f, float rotationStrength = 0.1f, float frequency = 25, int numBounces = 5)
        {
            BounceShake.Settings settings = new BounceShake.Settings(positionStrength, rotationStrength, frequency, numBounces);
            shaker.RegisterShake(new BounceShake(settings));
        }

        /// <summary>
        /// Suitable for longer and stronger shakes in 3D. Rotates camera in all three axes.
        /// </summary>
        /// <param name="strength">Strength of the shake.</param>
        /// <param name="frequency">Frequency of shaking.</param>
        /// <param name="numBounces">Number of vibrations before stop.</param>
        public void ShortShake3D(float strength = 0.3f, float frequency = 25, int numBounces = 5)
        {
            BounceShake.Settings settings = new BounceShake.Settings();
            settings.SetAxesMultiplier(new Displacement(Vector3.zero, new Vector3(1, 1, 0.4f)));
            settings.SetRotationStrength(strength);
            settings.SetFrequency(frequency);
            settings.SetNumBounces(numBounces);
            shaker.RegisterShake(new BounceShake(settings));
        }

        /// <summary>
        /// Suitable for longer and stronger shakes in 2D. Moves camera in X and Y axes and rotates it in Z axis.
        /// </summary>
        /// <param name="positionStrength">Strength of motion in X and Y axes.</param>
        /// <param name="rotationStrength">Strength of rotation in Z axis.</param>
        /// <param name="duration">Duration of the shake.</param>
        public void Explosion2D(float positionStrength = 1f, float rotationStrength = 3, float duration = 0.5f)
        {
            PerlinShake.NoiseMode[] noiseModes = new PerlinShake.NoiseMode[2] { new PerlinShake.NoiseMode(8, 1), new PerlinShake.NoiseMode(20, 0.3f) };
            Envelope.EnvelopeSettings envelopeSettings = new Envelope.EnvelopeSettings();
            envelopeSettings.SetDecay(duration <= 0 ? 1 : 1 / duration);
            PerlinShake.Settings settings = new PerlinShake.Settings();
            settings.SetStrength(new Displacement(new Vector3(1, 1) * positionStrength, Vector3.forward * rotationStrength));
            settings.SetNoiseModes(noiseModes);
            settings.SetEnvelope(envelopeSettings);
            shaker.RegisterShake(new PerlinShake(settings));
        }

        /// <summary>
        /// Suitable for longer and stronger shakes in 3D. Rotates camera in all three axes. 
        /// </summary>
        /// <param name="strength">Strength of the shake.</param>
        /// <param name="duration">Duration of the shake.</param>
        public void Explosion3D(float strength = 8f, float duration = 0.7f)
        {
            PerlinShake.NoiseMode[] noiseModes = new PerlinShake.NoiseMode[2] { new PerlinShake.NoiseMode(6, 1), new PerlinShake.NoiseMode(20, 0.2f) };
            Envelope.EnvelopeSettings envelopeSettings = new Envelope.EnvelopeSettings();
            envelopeSettings.SetDecay(duration <= 0 ? 1 : 1 / duration);
            PerlinShake.Settings settings = new PerlinShake.Settings();
            settings.SetStrength(new Displacement(Vector3.zero, new Vector3(1, 1, 0.5f) * strength));
            settings.SetNoiseModes(noiseModes);
            settings.SetEnvelope(envelopeSettings);
            shaker.RegisterShake(new PerlinShake(settings));
        }
    }
}
