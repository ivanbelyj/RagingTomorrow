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

namespace AuroraFPSRuntime.UIModules.UIElements.HUD
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Other/RectTransform Placer")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public sealed class RectTransformPlacer : MonoBehaviour
    {
        public enum Side
        {
            Top,
            Bottom,
            Left,
            Right
        }

        [SerializeField]
        private Side side = Side.Top;

        [SerializeField]
        private Transform relativeObject;

        [SerializeField]
        [Foldout("Advanced Settings", Style = "Header")]
        [VisualClamp(20, 0.01f)]
        private float smooth = 7.5f;

        [SerializeField]
        [Foldout("Advanced Settings", Style = "Header")]
        [Slider(0.0f, 1.0f)]
        private float tolerance = 0.2f;

        [SerializeField]
        [Foldout("Advanced Settings", Style = "Header")]
        private float offset = 0.1f;

        // Stored required components.
        private new RectTransform transform;
        private Vector2 originalAnchoredPosition;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            transform = GetComponent<RectTransform>();
            originalAnchoredPosition = transform.anchoredPosition;

            if(relativeObject == null)
            {
                relativeObject = transform.root;
            }
        }

        /// <summary>
        /// Called after all Update functions have been called.
        /// </summary>
        private void LateUpdate()
        {
            Vector2 targetPosition = originalAnchoredPosition;
            switch (side)
            {
                case Side.Top:
                    if (Vector3.Dot(relativeObject.up, Vector3.down) > tolerance)
                        targetPosition += Vector2.down * (transform.sizeDelta.y + offset);
                    else
                        targetPosition += Vector2.up * (transform.sizeDelta.y + offset);
                    break;
                case Side.Bottom:
                    if (Vector3.Dot(-relativeObject.up, Vector3.up) > tolerance)
                        targetPosition += Vector2.up * (transform.sizeDelta.y + offset);
                    else
                        targetPosition += Vector2.down * (transform.sizeDelta.y + offset);
                    break;
                case Side.Left:
                    if (Vector3.Dot(-relativeObject.right, Vector3.right) > tolerance)
                        targetPosition += Vector2.left * (transform.sizeDelta.x + offset);
                    else
                        targetPosition += Vector2.right * (transform.sizeDelta.x + offset);
                    break;
                case Side.Right:
                    if (Vector3.Dot(relativeObject.right, Vector3.left) > tolerance)
                        targetPosition += Vector2.right * (transform.sizeDelta.x + offset);
                    else
                        targetPosition += Vector2.left * (transform.sizeDelta.x + offset);
                    break;
            }
            transform.anchoredPosition = Vector2.Lerp(transform.anchoredPosition, targetPosition, smooth * Time.deltaTime);
        }
    }
}