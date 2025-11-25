/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System;
using System.Collections;
using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Coroutines;
using UnityEngine;
using UnityEngine.Events;

namespace AuroraFPSRuntime.UIModules.UIElements.Animation
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Animation/Transition")]
    [DisallowMultipleComponent]
    public abstract class Transition : MonoBehaviour
    {
        public enum AwakeAction
        {
            Nothing,
            FadeIn,
            FadeOut
        }

        [SerializeField]
        [MinValue(0.01f)]
        [Order(501)]
        private float duration = 0.25f;

        [SerializeField]
        [Indent(1)]
        [Order(502)]
        private AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [SerializeField]
        [Foldout("Event Callbacks", Style = "Header")]
        [Order(601)]
        private UnityEvent onFadeInCompleteEvent;

        [SerializeField]
        [Foldout("Event Callbacks", Style = "Header")]
        [Order(602)]
        private UnityEvent onFadeOutCompleteEvent;

        [SerializeField]
        [Label("On Awake Action")]
        [Foldout("Advanced Settings", Style = "Header")]
        [Order(701)]
        private AwakeAction awakeAction = AwakeAction.Nothing;

        // Stored required properties.
        private CoroutineObject coroutineObject;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            coroutineObject = new CoroutineObject(this);
            switch (awakeAction)
            {
                case AwakeAction.FadeIn:
                    FadeIn();
                    break;
                case AwakeAction.FadeOut:
                    FadeOut();
                    break;
            }
        }

        /// <summary>
        /// Called on the frame when a script is enabled,
        /// just before any of the Update methods are called the first time.
        /// </summary>
        protected virtual void Start()
        {
            OnFadeInCompleteCallback += onFadeInCompleteEvent.Invoke;
            OnFadeOutCompleteCallback += onFadeOutCompleteEvent.Invoke;
        }

        #region [Abstract Methods]
        /// <summary>
        /// Implement this method to make fade in logic.
        /// </summary>
        /// <param name="smooth">Smooth interpolation value.</param>
        protected abstract void OnFadeIn(float smooth);

        /// <summary>
        /// Implement this method to make fade out logic.
        /// </summary>
        /// <param name="smooth">Smooth interpolation value.</param>
        protected abstract void OnFadeOut(float smooth);
        #endregion

        /// <summary>
        /// Make transition to fade in.
        /// </summary>
        public void FadeIn()
        {
            coroutineObject.Start(WaitForFadeIn, true);
        }

        /// <summary>
        /// Make transition to fade out.
        /// </summary>
        public void FadeOut()
        {
            coroutineObject.Start(WaitForFadeOut, true);
        }

        /// <summary>
        /// Transition processing.
        /// </summary>
        public IEnumerator WaitForFadeIn()
        {
            float speed = 1 / duration;
            float time = 0.0f;

            while (time < 1.0f)
            {
                time += Time.unscaledDeltaTime * speed;
                float smooth = curve.Evaluate(time);
                OnFadeIn(smooth);
                OnFadeInCallback?.Invoke(smooth);
                yield return null;
            }
            OnFadeInCompleteCallback?.Invoke();
        }

        /// <summary>
        /// Transition processing.
        /// </summary>
        public IEnumerator WaitForFadeOut()
        {
            float speed = 1 / duration;
            float time = 0.0f;

            while (time < 1.0f)
            {
                time += Time.unscaledDeltaTime * speed;
                float smooth = curve.Evaluate(time);
                OnFadeOut(smooth);
                OnFadeOutCallback?.Invoke(smooth);
                yield return null;
            }
            OnFadeOutCompleteCallback?.Invoke();
        }

        public bool IsFading()
        {
            return coroutineObject.IsProcessing();
        }

        #region [Event Callback Functions]
        /// <summary>
        /// Called when performing fade in transition.
        /// <br><i>Parameter type of (float):</i> Interpolation value evaluated by curve.</br>
        /// </summary>
        public event Action<float> OnFadeInCallback;

        /// <summary>
        /// Called when performing fade out transition.
        /// <br><i>Parameter type of (float):</i> Interpolation value evaluated by curve.</br>
        /// </summary>
        public event Action<float> OnFadeOutCallback;

        /// <summary>
        /// Called when fade in transition in complete.
        /// </summary>
        public event Action OnFadeInCompleteCallback;

        /// <summary>
        /// Called when fade out transition in complete.
        /// </summary>
        public event Action OnFadeOutCompleteCallback;
        #endregion

        #region [Getter / Setter]
        public float GetDuration()
        {
            return duration;
        }

        public void SetDuration(float value)
        {
            duration = value;
        }

        public AnimationCurve GetCurve()
        {
            return curve;
        }

        public void SetCurve(AnimationCurve value)
        {
            curve = value;
        }
        #endregion
    }
}
