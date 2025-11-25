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
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/HUD/Weapon/Weapon Name Receiver")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public sealed class WeaponNameReceiver : MonoBehaviour
    {
        [SerializeField]
        [NotNull]
        private EquippableObjectIdentifier weaponIdentifier;

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
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            textComponent.text = weaponIdentifier.GetItem().GetItemName();
        }

        #region [Getter / Setter]
        public EquippableObjectIdentifier GetEquippableObjectIdentifier()
        {
            return weaponIdentifier;
        }

        public Text GetTextComponent()
        {
            return textComponent;
        }
        #endregion
    }
}