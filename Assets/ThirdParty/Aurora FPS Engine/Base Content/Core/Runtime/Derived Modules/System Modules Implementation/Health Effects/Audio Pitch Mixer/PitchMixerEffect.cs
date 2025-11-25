/* ================================================================
   ----------------------------------------------------------------
   Project   :   Apex Inspector
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Coroutines;
using AuroraFPSRuntime.SystemModules.CameraSystems;
using AuroraFPSRuntime.SystemModules.ControllerSystems;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace AuroraFPSRuntime.SystemModules.HealthModules
{
    [HealthFunctionMenu("Pitch Mixer", "Audio/Audio Mixer/Pitch Mixer")]
    public sealed class PitchMixerEffect : HealthEffect
    {
        [SerializeField]
        private float health = 50;

        [SerializeField]
        [NotNull]
        private AudioMixer mixer;

        [SerializeField]
        [NotEmpty]
        [VisibleIf("mixer", "NotNull")]
        [Indent(1)]
        private string parameter = string.Empty;

        [SerializeField]
        [Slider(0.001f, 0.9f)]
        [VisibleIf("mixer", "NotNull")]
        [Indent(1)]
        private float minPitch = 0.35f;

        [SerializeField]
        [VisibleIf("mixer", "NotNull")]
        [Indent(1)]
        private float fadeInSpeed = 5.0f;

        [SerializeField]
        [VisibleIf("mixer", "NotNull")]
        [Indent(1)]
        private float fadeOutSpeed = 0.5f;

        // Stored required properties.
        private CharacterHealth characterHealth;
        private CoroutineObject coroutineObject;

        /// <summary>
        /// Implement this method to make some initialization 
        /// and get access to CharacterHealth references.
        /// </summary>
        /// <param name="healthComponent">Player health component reference.</param>
        public override void Initialization(CharacterHealth characterHealth)
        {
            this.characterHealth = characterHealth;

            Debug.Assert(mixer != null, $"<color=red><b>Mixer reference is empty!</b>\nError path: {characterHealth.name}->{characterHealth.GetType().Name}->Custom Effects->Pitch Mixer</color>");
            if(mixer != null)
            {
                Debug.Assert(mixer.GetFloat(parameter, out float pitch), $"<color=red><b>Mixer parameter <i>{parameter}</i> is not found!</b>\nError path: {characterHealth.name}->{characterHealth.GetType().Name}->Custom Effects->Pitch Mixer</color>");
                coroutineObject = new CoroutineObject(characterHealth);
                coroutineObject.Start(PitchProcessor, true);
            }
        }

        private IEnumerator PitchProcessor()
        {
            if(mixer.GetFloat(parameter, out float pitch))
            {
                while (true)
                {
                    float targetPitch = 1.0f;
                    float speed = fadeOutSpeed;
                    if(characterHealth.IsAlive())
                    {
                        targetPitch = Mathf.InverseLerp(characterHealth.GetMinHealth(), health, characterHealth.GetHealth());
                        targetPitch = Mathf.Max(minPitch, targetPitch);
                        speed = fadeInSpeed;
                    }
                    
                    pitch = Mathf.Lerp(pitch, targetPitch, speed * Time.unscaledDeltaTime);
                    mixer.SetFloat(parameter, pitch);
                    yield return null;
                }
            }
        }
    }
}