/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AuroraFPSRuntime.UIModules.UIElements
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/SFX/Button SFX")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Selectable))]
    public sealed class ButtonSFX : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
    {
        [SerializeField]
        private AudioClip onEnterSound;

        [SerializeField]
        private AudioClip onClickSound;

        [SerializeField]
        [NotNull]
        private AudioSource audioSource;

        // Stored required components.
        private Selectable selectable;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            selectable = GetComponent<Selectable>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (selectable.interactable)
                audioSource.PlayOneShot(onEnterSound);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (selectable.interactable)
                audioSource.PlayOneShot(onClickSound);
        }
    }
}