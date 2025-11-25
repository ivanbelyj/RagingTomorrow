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
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/HUD/Weapon/Weapon Type Receiver")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public sealed class WeaponTypeReceiver : MonoBehaviour
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
        /// Called after all Update functions have been called.
        /// </summary>
        private void LateUpdate()
        {
            textComponent.text = weaponIdentifier.GetItem().GetItemType();
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