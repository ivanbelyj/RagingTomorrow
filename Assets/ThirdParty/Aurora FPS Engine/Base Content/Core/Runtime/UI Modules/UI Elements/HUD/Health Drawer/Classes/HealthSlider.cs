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
using UnityEngine.UI;

namespace AuroraFPSRuntime.UIModules.UIElements.HUD
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/HUD/Health Drawer/Health Slider")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Slider))]
    public sealed class HealthSlider : MonoBehaviour
    {
        [SerializeField]
        [NotNull]
        [InlineButton("FindHealthComponent", Label = "@Search Icon", Style = "IconButton")]
        private ObjectHealth reference;

        // Stored required components.
        private Slider sliderComponent;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            sliderComponent = GetComponent<Slider>();
            sliderComponent.minValue = 0.0f;
            sliderComponent.maxValue = 1.0f;
        }

        /// <summary>
        /// LateUpdate is called after all Update functions have been called.
        /// </summary>
        private void LateUpdate()
        {
            sliderComponent.value = Mathf.InverseLerp(0, reference.GetMaxHealth(), reference.GetHealth());
        }

        private void FindHealthComponent()
        {
            if (reference == null)
            {
                reference = transform.GetComponentInParent<ObjectHealth>();
                if (reference == null)
                {
                    reference = transform.GetComponentInChildren<ObjectHealth>();
                }
            }
        }

        #region [Getter / Setter]
        public ObjectHealth GetHealthReference()
        {
            return reference;
        }

        public void SetHealthReference(ObjectHealth value)
        {
            reference = value;
        }

        public Slider GetSliderComponent()
        {
            return sliderComponent;
        }
        #endregion
    }
}