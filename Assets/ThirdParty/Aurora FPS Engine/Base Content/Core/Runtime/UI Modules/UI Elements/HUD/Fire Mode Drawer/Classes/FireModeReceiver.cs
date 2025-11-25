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
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/HUD/Weapon/Fire Mode Receiver")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public sealed class FireModeReceiver : MonoBehaviour
    {
        [SerializeField]
        [NotEmpty]
        private string mute = "Mute";

        [SerializeField]
        [NotEmpty]
        private string single = "Single";

        [SerializeField]
        [NotEmpty]
        private string queue = "Queue";

        [SerializeField]
        [NotEmpty]
        private string free = "Free";

        [SerializeField]
        [NotNull]
        private WeaponShootingSystem shootingSystem;

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
            switch (shootingSystem.GetFireMode())
            {
                case FireMode.Mute:
                    textComponent.text = mute;
                    break;
                case FireMode.Single:
                    textComponent.text = single;
                    break;
                case FireMode.Queue:
                    textComponent.text = queue;
                    break;
                case FireMode.Free:
                    textComponent.text = free;
                    break;
            }
        }

        #region [Getter / Setter]
        public WeaponShootingSystem GetShootingSystem()
        {
            return shootingSystem;
        }

        public void SetShootingSystem(WeaponShootingSystem value)
        {
            shootingSystem = value;
        }

        public Text GetTextComponent()
        {
            return textComponent;
        }
        #endregion
    }
}