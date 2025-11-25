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
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/HUD/Weapon/Bullet Count Receiver")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public sealed class BulletCountReceiver : MonoBehaviour
    {
        [SerializeField]
        [NotNull]
        private AmmoSystem ammoSystem;

        // Stored required components.
        private Text textComponent;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            textComponent = GetComponent<Text>();
        }

        /// <summary>
        /// Called after all Update functions have been called.
        /// </summary>
        private void LateUpdate()
        {
            textComponent.text = ammoSystem.GetAmmoCount().ToString();
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

        public Text GetTextComponent()
        {
            return textComponent;
        }
        #endregion
    }
}