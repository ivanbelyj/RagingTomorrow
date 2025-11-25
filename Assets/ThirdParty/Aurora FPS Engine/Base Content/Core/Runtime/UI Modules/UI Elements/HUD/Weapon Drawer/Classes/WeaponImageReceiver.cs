/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.SystemModules.InventoryModules;
using UnityEngine;
using UnityEngine.UI;

namespace AuroraFPSRuntime.UIModules.UIElements.HUD
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/HUD/Weapon/Weapon Image Receiver")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public sealed class WeaponImageReceiver : MonoBehaviour
    {
        [SerializeField]
        [NotNull]
        private EquippableObjectIdentifier weaponIdentifier;

        // Stored required components.
        private Image imageComponent;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            imageComponent = GetComponent<Image>();
        }

        /// <summary>
        /// Called after all Update functions have been called.
        /// </summary>
        private void LateUpdate()
        {
            imageComponent.sprite = weaponIdentifier.GetItem().GetItemImage();
        }

        #region [Getter / Setter]
        public EquippableObjectIdentifier GetEquippableObjectIdentifier()
        {
            return weaponIdentifier;
        }

        public Image GetImageComponent()
        {
            return imageComponent;
        }
        #endregion
    }
}