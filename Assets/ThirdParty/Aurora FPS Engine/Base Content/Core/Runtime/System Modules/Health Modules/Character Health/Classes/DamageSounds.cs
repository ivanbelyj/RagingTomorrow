/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.HealthModules
{
    [System.Serializable]
    public sealed class DamageSounds
    {
        [SerializeField]
        private AudioClip takeDamageSound;

        [SerializeField]
        private AudioClip velocityDamageSound;

        [SerializeField]
        private AudioClip deathSound;

        public DamageSounds(AudioClip takeDamageSound, AudioClip velocityDamageSound, AudioClip deathSound)
        {
            this.takeDamageSound = takeDamageSound;
            this.velocityDamageSound = velocityDamageSound;
            this.deathSound = deathSound;
        }

        #region [Getter / Setter]
        public AudioClip GetTakeDamageSound()
        {
            return takeDamageSound;
        }

        public void SetTakeDamageSound(AudioClip value)
        {
            takeDamageSound = value;
        }

        public AudioClip GetVelocityDamageSound()
        {
            return velocityDamageSound;
        }

        public void SetVelocityDamageSound(AudioClip value)
        {
            velocityDamageSound = value;
        }

        public AudioClip GetDeathSound()
        {
            return deathSound;
        }

        public void SetDeathSound(AudioClip value)
        {
            deathSound = value;
        }
        #endregion
    }
}