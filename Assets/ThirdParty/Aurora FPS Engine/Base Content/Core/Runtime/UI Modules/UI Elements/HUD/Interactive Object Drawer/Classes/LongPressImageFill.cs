/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Coroutines;
using AuroraFPSRuntime.CoreModules.InputSystem;
using AuroraFPSRuntime.SystemModules;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;

namespace AuroraFPSRuntime.UIModules.UIElements.HUD
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/HUD/Interactive Object/Long Press Image Fill")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public sealed class LongPressImageFill : MonoBehaviour
    {
        [SerializeField]
        [NotNull]
        private InteractiveObject interactiveObject;

        // Stored required components.
        private Image imageComponent;
        private CoroutineObject<float> fillImageCoroutine;

        // Stored required properties.
        private float elapsedTime;
        private float storedTime;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            Debug.Assert(interactiveObject != null, $"<b><color=#FF0000>Long Press Image Fill component cannot be used without Interactive Object.\nAttach reference of the Interactive Object component to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Interactive Object<i>(field)</i>.</color></b>");
            imageComponent = GetComponent<Image>();
            fillImageCoroutine = new CoroutineObject<float>(this);
        }

        private void OnEnable()
        {
            ClearFillForce();
            storedTime = 0;
            elapsedTime = 0;
            interactiveObject.InputAction.started += OnLootHoldAction;
            interactiveObject.InputAction.canceled += OnLootHoldAction;
            interactiveObject.OnBecomeInactiveCallback += ClearFillForce;
        }

        private void OnDisable()
        {
            interactiveObject.InputAction.started -= OnLootHoldAction;
            interactiveObject.InputAction.canceled -= OnLootHoldAction;
            interactiveObject.OnBecomeInactiveCallback -= ClearFillForce;
        }

        private void OnLootHoldAction(InputAction.CallbackContext context)
        {
            if (context.started && context.interaction is HoldInteraction holdInteraction)
            {
                fillImageCoroutine.Start(FillImage, holdInteraction.duration, force: true);
            }
            else if (context.canceled)
            {
                fillImageCoroutine.Start(ClearFill, elapsedTime, force: true);
            }
        }

        private IEnumerator FillImage(float duration)
        {
            float time = storedTime;
            while (time < duration)
            {
                imageComponent.fillAmount = time/duration;
                time += Time.deltaTime;
                elapsedTime = time;
                yield return null;
            }
            storedTime = 0;
        }

        private IEnumerator ClearFill(float elapsedTime)
        {
            float time = 0.0f;
            float amount = imageComponent.fillAmount;
            while (time < elapsedTime)
            {
                imageComponent.fillAmount = Mathf.Lerp(amount, 0, time/elapsedTime);
                time += Time.deltaTime;
                storedTime = elapsedTime - time;
                yield return null;
            }
            ClearFillForce();
        }

        private void ClearFillForce()
        {
            imageComponent.fillAmount = 0.0f;
        }

        #region [Getter / Setter]
        public InteractiveObject GetInteractiveObject()
        {
            return interactiveObject;
        }

        public void SetInteractiveObject(InteractiveObject value)
        {
            interactiveObject = value;
        }

        public Image GetImageComponent()
        {
            return imageComponent;
        }
        #endregion
    }
}