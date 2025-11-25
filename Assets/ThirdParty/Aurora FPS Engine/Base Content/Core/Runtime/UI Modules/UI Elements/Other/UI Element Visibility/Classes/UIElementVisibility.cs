/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.ValueTypes;
using UnityEngine;

namespace AuroraFPSRuntime.UIModules.UIElements.HUD
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Other/UI Element Visibility")]
    [DisallowMultipleComponent]
    public sealed class UIElementVisibility : MonoBehaviour
    {
        [System.Serializable]
        public class VisibilityCallback : CallbackEvent { }

        [SerializeField]
        [Foldout("Enable Callback", Style = "Header")]
        private VisibilityCallback enableCallback;

        [SerializeField]
        [Foldout("Disable Callback", Style = "Header")]
        private VisibilityCallback disableCallback;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            enableCallback.RegisterCallback(() => gameObject.SetActive(true));
            disableCallback.RegisterCallback(() => gameObject.SetActive(false));
        }
    }
}