/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.SystemModules.HealthModules;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

namespace AuroraFPSRuntime.UIModules.UIElements.HUD
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/HUD/Health/Damage Effect")]
    [DisallowMultipleComponent]
    public class DamageEffect : MonoBehaviour
    {
        [SerializeField]
        [NotNull]
        private CharacterHealth characterHealth;

        [SerializeField]
        private float startPoint = 50;

        [SerializeField]
        [Suffix("Optional", ItalicText = true)]
        private Image image;

        [SerializeField]
        [Slider(0, 1)]
        [VisibleIf("image", "NotNull")]
        [Indent(1)]
        private float maxAlpha;

        [SerializeField]
        [Suffix("Optional", ItalicText = true)]
        private PostProcessVolume postProcessVolume;

        [SerializeField]
        [Suffix("Optional", ItalicText = true)]
        private PostProcessProfile postProcessProfile;

        /// <summary>
        /// Called after all Update functions have been called, 
        /// while the MonoBehaviour is enabled.
        /// </summary>
        private void LateUpdate()
        {
            float linearValue = Mathf.InverseLerp(startPoint, characterHealth.GetMinHealth(), characterHealth.GetHealth());

            if (image != null)
                CalculateImage(image, linearValue);

            if (postProcessVolume != null)
                CalculateVolume(postProcessVolume, linearValue);

            if (postProcessProfile != null)
                CalculateProfile(postProcessProfile, linearValue);
        }

        /// <summary>
        /// Called after all Update functions have been called, 
        /// while the MonoBehaviour is enabled.
        /// </summary>
        /// <param name="linearValue">Linear value which clamped [0-1] of current character health state.</param>
        protected virtual void CalculateImage(Image image, float linearValue)
        {
            Color color = image.color;
            color.a = Mathf.Clamp(linearValue, 0, maxAlpha);
            image.color = color;
        }

        /// <summary>
        /// Called after all Update functions have been called, 
        /// while the MonoBehaviour is enabled and volume is assigned.
        /// </summary>
        /// <param name="layer">Reference of post process volume.</param>
        /// <param name="linearValue">Linear value which clamped [0-1] of current character health state.</param>
        protected virtual void CalculateVolume(PostProcessVolume volume, float linearValue)
        {
            volume.weight = linearValue;
        }

        /// <summary>
        /// Called after all Update functions have been called, 
        /// while the MonoBehaviour is enabled and profile is assigned.
        /// </summary>
        /// <param name="layer">Reference of post process profile.</param>
        /// <param name="linearValue">Linear value which clamped [0-1] of current character health state.</param>
        protected virtual void CalculateProfile(PostProcessProfile profile, float linearValue)
        {
            if (profile.TryGetSettings<ChromaticAberration>(out ChromaticAberration settings))
            {
                settings.intensity.value = Mathf.PingPong(Time.time * linearValue, linearValue) + linearValue;
            }
        }
    }
}
