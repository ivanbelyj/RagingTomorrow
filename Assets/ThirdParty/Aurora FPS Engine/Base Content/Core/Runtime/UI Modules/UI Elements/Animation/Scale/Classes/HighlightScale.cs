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
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AuroraFPSRuntime.UIModules.UIElements.Animation
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Animation/Scale/Highlight Scale")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Selectable))]
    public sealed class HighlightScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        [MinValue(0)]
        private float ratio;

        [SerializeField]
        [MinValue(0.01f)]
        private float duration;

        [SerializeField]
        private AnimationCurve curve;

        [SerializeField]
        [Foldout("Advanced Settings", Style = "Header")]
        private bool autoBringToFront = false;

        [SerializeField]
        [Foldout("Advanced Settings", Style = "Header")]
        [VisibleIf("autoBringToFront", true)]
        [Indent(1)]
        [MinValue(-1)]
        private int maxLayerPosition = -1;

        // Stored required components.
        private new RectTransform transform;
        private Selectable selectable;

        // Stored required properties.
        private int originalSiblingIndex;
        private Vector2 originalScale;
        private CoroutineObject<float> resizeCoroutine;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            transform = GetComponent<RectTransform>();
            selectable = GetComponent<Selectable>();
            resizeCoroutine = new CoroutineObject<float>(this);
            originalScale = transform.localScale;
            originalSiblingIndex = transform.GetSiblingIndex();
        }

        private IEnumerator ResizeHandler(float ratio)
        {
            float time = 0.0f;
            float speed = 1 / duration;

            Vector2 targetScale = originalScale * ratio;
            while (time < 1.0f)
            {
                time += Time.unscaledDeltaTime * speed;
                float smooth = curve.Evaluate(time);
                transform.localScale = Vector2.Lerp(transform.localScale, targetScale, smooth);
                yield return null;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (selectable.interactable)
            {
                if (autoBringToFront)
                {
                    transform.SetAsLastSibling();
                    if(maxLayerPosition != -1)
                    {
                        int lastSibling = transform.GetSiblingIndex();
                        if(lastSibling > maxLayerPosition)
                        {
                            transform.SetSiblingIndex(maxLayerPosition);
                        }
                    }
                }
                resizeCoroutine.Start(ResizeHandler, ratio, true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (selectable.interactable)
            {
                if (autoBringToFront)
                {
                    transform.SetSiblingIndex(originalSiblingIndex);
                }
                resizeCoroutine.Start(ResizeHandler, 1.0f, true);
            }
        }
    }
}