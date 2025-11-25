/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.WeaponModules;
using UnityEngine;
using UnityEngine.UI;

namespace AuroraFPSRuntime.UIModules.UIElements.HUD
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/HUD/Weapon/Ammo Slider Drawer")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Slider))]
    public sealed class AmmoSliderDrawer : MonoBehaviour
    {
        [SerializeField]
        [NotNull]
        private AmmoSystem ammoSystem;

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
        /// Called after all Update functions have been called.
        /// </summary>
        private void LateUpdate()
        {
            sliderComponent.value = Mathf.InverseLerp(0, ammoSystem.GetMaxAmmoCount(), ammoSystem.GetAmmoCount());

        }

        #region [Getter / Setter]
        public AmmoSystem GetAmmoSystem()
        {
            return ammoSystem;
        }

        public void SetAmmoSystem(AmmoSystem value)
        {
            ammoSystem = value;
        }

        public Slider GetSliderComponent()
        {
            return sliderComponent;
        }
        #endregion
    }
}
