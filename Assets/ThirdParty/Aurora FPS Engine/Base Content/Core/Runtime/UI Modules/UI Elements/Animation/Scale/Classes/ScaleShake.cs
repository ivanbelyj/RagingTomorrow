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
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace AuroraFPSRuntime.UIModules.UIElements
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Animation/Scale/Scale Shake")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public sealed class ScaleShake : MonoBehaviour
    {
        [SerializeField]
        [MinValue(0.01f)]
        private float duration;

        [SerializeField]
        private AnimationCurve curve;

        // Stored required components.
        private new RectTransform transform;
        private Text text;

        // Stored required properties.
        private string previousText;
        private Vector2 originalScale;
        private CoroutineObject shakeCoroutine;

        /// <summary>
        /// Сalled when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            transform = GetComponent<RectTransform>();
            text = GetComponent<Text>();
            originalScale = transform.localScale;
            shakeCoroutine = new CoroutineObject(this);
        }

        /// <summary>
        /// Called after all Updates, while the MonoBehaviour is enabled.
        /// </summary>
        private void LateUpdate()
        {
            if(previousText != text.text)
            {
                shakeCoroutine.Start(Shake, true);
                previousText = text.text;
            }
        }

        private IEnumerator Shake()
        {
            float time = 0.0f;
            float speed = 1 / duration;

            while (time < 1.0f)
            {
                time += Time.unscaledDeltaTime * speed;
                float smooth = curve.Evaluate(time);
                transform.localScale = Vector2.Lerp(transform.localScale, originalScale * smooth, smooth);
                yield return null;
            }
        }
    }
}
