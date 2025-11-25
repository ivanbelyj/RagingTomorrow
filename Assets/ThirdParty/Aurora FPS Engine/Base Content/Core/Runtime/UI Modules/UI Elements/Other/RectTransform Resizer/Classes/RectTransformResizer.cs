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
using AuroraFPSRuntime.CoreModules.Coroutines;
using System.Collections;
using UnityEngine;

namespace AuroraFPSRuntime.UIModules.UIElements.HUD
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Other/RectTransform Resizer")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class RectTransformResizer : MonoBehaviour
    {
        [System.Serializable]
        public class ResizeCallback : CallbackEvent<float, AnimationCurve> { }

        [SerializeField]
        private Vector2 size = Vector2.one;

        [SerializeField]
        [Foldout("Show Event", Style = "Header")]
        private ResizeCallback showCallback;

        [SerializeField]
        [Foldout("Hide Event", Style = "Header")]
        private ResizeCallback hideCallback;

        // Stored required components.
        private new RectTransform transform;

        // Stored required properties.
        private CoroutineObject<Vector2, float, AnimationCurve> resizeCoroutine;

        private void Awake()
        {
            transform = GetComponent<RectTransform>();
            resizeCoroutine = new CoroutineObject<Vector2, float, AnimationCurve>(this);

            showCallback.RegisterCallback(ToOriginalSize);
            hideCallback.RegisterCallback(ToZeroSize);
        }

        private void ToOriginalSize(float duration, AnimationCurve curve)
        {
            resizeCoroutine.Start(Resize, size, duration, curve, true);
        }

        private void ToZeroSize(float duration, AnimationCurve curve)
        {
            resizeCoroutine.Start(Resize, Vector2.zero, duration, curve, true);
        }

        private IEnumerator Resize(Vector2 size, float duration, AnimationCurve curve)
        {
            float time = 0.0f;
            float speed = 1.0f / duration;
            while (time < 1.0f)
            {
                time += speed * Time.deltaTime;
                float smooth = curve.Evaluate(time);
                transform.sizeDelta = Vector2.Lerp(transform.sizeDelta, size, smooth);
                yield return null;
            }
        }
    }
}