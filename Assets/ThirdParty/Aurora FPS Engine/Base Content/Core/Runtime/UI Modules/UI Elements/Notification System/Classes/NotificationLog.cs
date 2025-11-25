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
using AuroraFPSRuntime.UIModules.UIElements.Animation;
using System.Collections;
using UnityEngine;

namespace AuroraFPSRuntime.UIModules.UIElements.NotificationSystem
{
    [HideScriptField]
    [AddComponentMenu(null)]
    [DisallowMultipleComponent]
    public abstract class NotificationLog : MonoBehaviour
    {
        [SerializeField]
        [MinValue(0.1f)]
        private float lifeTime = 5.0f;

        // Stored required component;
        private new RectTransform transform;
        private Transition transition;

        // Stored required properties.
        private Vector2 sizeDelta = Vector3.zero;
        private CoroutineObject coroutineObject;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            transform = GetComponent<RectTransform>();
            coroutineObject = new CoroutineObject(this);

            transition = GetComponent<Transition>();
            if (transition != null)
            {
                sizeDelta = transform.sizeDelta;
                transform.sizeDelta = Vector2.zero;
                transition.OnFadeInCallback += OnFadeInEvent;
                transition.OnFadeOutCallback += OnFadeOutEvent;
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
            coroutineObject.Start(LifeTimer, true);
        }

        private IEnumerator LifeTimer()
        {
            transition?.FadeIn();
            yield return new WaitForSeconds(lifeTime);
            if (transition != null)
            {
                yield return transition.WaitForFadeOut();
            }
            gameObject.SetActive(false);
        }

        #region [Event Callback Wrapper]
        private void OnFadeInEvent(float time)
        {
            transform.sizeDelta = Vector2.Lerp(transform.sizeDelta, sizeDelta, time);
        }

        private void OnFadeOutEvent(float time)
        {
            transform.sizeDelta = Vector2.Lerp(transform.sizeDelta, Vector3.zero, time);
        }
        #endregion

        #region [Getter / Setter]
        public float GetLifeTime()
        {
            return lifeTime;
        }

        public void SetLifeTime(float value)
        {
            lifeTime = value;
        }

        public Transition GetTransition()
        {
            return transition;
        }

        public void SetTransition(Transition value)
        {
            transition = value;
        }
        #endregion
    }
}