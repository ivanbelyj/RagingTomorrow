/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Mathematics;
using AuroraFPSRuntime.SystemModules.HealthModules;
using UnityEngine;
using UnityEngine.UI;

namespace AuroraFPSRuntime.UIModules.UIElements.HUD
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/HUD/Health/Health Points Receiver")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public sealed class HealthPointsReceiver : MonoBehaviour
    {
        public enum DisplayType
        {
            Points,
            Present
        }

        [SerializeField]
        private DisplayType display = DisplayType.Points;

        [SerializeField]
        [NotNull]
        [InlineButton("FindHealthComponent", Label = "@Search Icon", Style = "IconButton")]
        private ObjectHealth reference;

        // Stored required components.
        private float lastHealth;
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
            if(lastHealth != reference.GetHealth())
            {
                switch (display)
                {
                    case DisplayType.Points:
                        textComponent.text = reference.GetHealth().ToString();
                        break;
                    case DisplayType.Present:
                        textComponent.text = Math.GetPersent(reference.GetHealth(), reference.GetMaxHealth()).ToString();
                        break;
                }
                lastHealth = reference.GetHealth();
            }
        }

        private void FindHealthComponent()
        {
            if(reference == null)
            {
                reference = transform.GetComponentInParent<ObjectHealth>();
                if(reference == null)
                {
                    reference = transform.GetComponentInChildren<ObjectHealth>();
                }
            }
        }

        #region [Getter / Setter]
        public DisplayType GetDisplayType()
        {
            return display;
        }

        public void SetDisplayType(DisplayType value)
        {
            display = value;
        }

        public ObjectHealth GetHealthReference()
        {
            return reference;
        }

        public void SetHealthReference(ObjectHealth value)
        {
            reference = value;
        }

        public Text GetTextComponent()
        {
            return textComponent;
        }
        #endregion
    }
}