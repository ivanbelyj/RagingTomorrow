/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.SystemModules;
using UnityEngine;

namespace AuroraFPSRuntime.WeaponModules
{
    [System.Serializable]
    public sealed class DistanceSettings
    {
        [SerializeField]
        [MinMaxSlider(0, 100)]
        private Vector2 distance = new Vector2(0, 10);

        [SerializeField]
        [MinValue(0)]
        private float damage = 15;

        [SerializeField]
        [MinValue(0.0f)]
        private float impulse = 10.0f;

        [SerializeField]
        [MinValue(0.0f)]
        private float upwardsModifier = 0.5f;

        [SerializeField]
        private PerlinShake.Settings shakeSettings = new PerlinShake.Settings(
            new Displacement(Vector3.zero, new Vector3(1, 1, 0.5f) * 0.08f),
            new PerlinShake.NoiseMode[2] { new PerlinShake.NoiseMode(6, 1), new PerlinShake.NoiseMode(20, 0.2f) },
            new Envelope.EnvelopeSettings(10.0f, 0.0f, 1.5f, Degree.Cubic),
            new Attenuator.StrengthAttenuationSettings());

        public DistanceSettings()
        {
            distance = new Vector2(0, 10);
            damage = 15;
            impulse = 10.0f;
            upwardsModifier = 0.5f;
            shakeSettings = new PerlinShake.Settings(
            new Displacement(Vector3.zero, new Vector3(1, 1, 0.5f) * 0.08f),
            new PerlinShake.NoiseMode[2] { new PerlinShake.NoiseMode(6, 1), new PerlinShake.NoiseMode(20, 0.2f) },
            new Envelope.EnvelopeSettings(10.0f, 0.0f, 1.5f, Degree.Cubic),
            new Attenuator.StrengthAttenuationSettings());
        }

        public DistanceSettings(Vector2 distance, int damage, float impulse, float upwardsModifier)
        {
            this.distance = distance;
            this.damage = damage;
            this.impulse = impulse;
            this.upwardsModifier = upwardsModifier;
        }

        public DistanceSettings(Vector2 distance, int damage, float impulse, float upwardsModifier, PerlinShake.Settings shakeSettings)
        {
            this.distance = distance;
            this.damage = damage;
            this.impulse = impulse;
            this.upwardsModifier = upwardsModifier;
            this.shakeSettings = shakeSettings;
        }

        #region [Getter / Setter]
        public Vector2 GetDistance()
        {
            return distance;
        }

        public void SetDistance(Vector2 value)
        {
            distance = value;
        }

        public float GetDamage()
        {
            return damage * WeaponUtilities.DamageMultiplier;
        }

        public void SetDamage(float value)
        {
            damage = value;
        }

        public float GetImpulse()
        {
            return impulse;
        }

        public void SetImpulse(float value)
        {
            impulse = value;
        }

        public float GetUpwardsModifier()
        {
            return upwardsModifier;
        }

        public void SetUpwardsModifier(float value)
        {
            upwardsModifier = value;
        }

        public PerlinShake.Settings GetShakeSettings()
        {
            return shakeSettings;
        }

        public void SetShakeSettings(PerlinShake.Settings value)
        {
            shakeSettings = value;
        }
        #endregion
    }
}