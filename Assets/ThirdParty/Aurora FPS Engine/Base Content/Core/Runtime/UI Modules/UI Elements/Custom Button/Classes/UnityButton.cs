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
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AuroraFPSRuntime.UIModules.UIElements
{
    [HideScriptField]
    [AddComponentMenu(null)]
    [DisallowMultipleComponent]
    public abstract class UnityButton : Selectable, IPointerClickHandler, ISubmitHandler
    {
        [SerializeField]
        private UnityEvent onClick = new UnityEvent();

        // Stored required properties.
        private CoroutineObject coroutineObject;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            coroutineObject = new CoroutineObject(this);
        }

        /// <summary>
        /// Called before invoking OnClick events.
        /// </summary>
        protected virtual void OnBeforeClick() { }

        /// <summary>
        /// Called after invoking all OnClick unity events.
        /// </summary>
        protected virtual void OnAfterClick() { }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            Press();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            Press();

            if (!IsActive() || !IsInteractable())
                return;

            DoStateTransition(SelectionState.Pressed, false);
            coroutineObject.Start(OnFinishSubmit, true);
        }

        private IEnumerator OnFinishSubmit()
        {
            var fadeTime = colors.fadeDuration;
            var elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }

        private void Press()
        {
            if (!IsActive() || !IsInteractable())
                return;
            OnBeforeClick();
            onClick.Invoke();
            OnAfterClick();
        }
    }
}
