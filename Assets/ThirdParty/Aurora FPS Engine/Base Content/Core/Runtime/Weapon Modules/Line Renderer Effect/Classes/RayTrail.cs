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
using AuroraFPSRuntime.SystemModules;
using System.Collections;
using UnityEngine;

namespace AuroraFPSRuntime.WeaponModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/Weapon Modules/Other/Ray Trail")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(LineRenderer))]
    public sealed class RayTrail : PoolObject
    {
        [SerializeField]
        [MinValue(0.0f)]
        private float duration = 3.0f;

        [SerializeField]
        [MinValue(0.0f)]
        private AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        // Stored required components.
        private LineRenderer lineRenderer;

        // Stored required properties.
        private Color startColor;
        private Color endColor;
        private CoroutineObject<float, AnimationCurve> visualizeCoroutine;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            lineRenderer = GetComponent<LineRenderer>();
            visualizeCoroutine = new CoroutineObject<float, AnimationCurve>(this);

            startColor = lineRenderer.startColor;
            endColor = lineRenderer.endColor;
        }

        public void Visualize(Vector3 start, Vector3 end)
        {
            lineRenderer.SetPositions(new Vector3[2] { start, end });
            lineRenderer.startColor = startColor;
            lineRenderer.endColor = endColor;
            visualizeCoroutine.Start(VisualizeProcessing, duration, curve, true);
        }

        public void Visualize(Vector3 origin, Vector3 direction, float range)
        {
            Visualize(origin, direction * range);
        }

        public void Visualize(Vector3 start, Vector3 end, float duration, AnimationCurve curve)
        {
            lineRenderer.SetPositions(new Vector3[2] { start, end});
            lineRenderer.startColor = startColor;
            lineRenderer.endColor = endColor;
            visualizeCoroutine.Start(VisualizeProcessing, duration, curve, true);
        }

        public void Visualize(Vector3 origin, Vector3 direction, float range, float duration, AnimationCurve curve)
        {
            Visualize(origin, direction * range, duration, curve);
        }

        private IEnumerator VisualizeProcessing(float duration, AnimationCurve curve)
        {
            float time = 0.0f;
            float speed = 1 / duration;

            while (time < 1.0f)
            {
                time += speed * Time.deltaTime;
                float smooth = curve.Evaluate(time);
                lineRenderer.startColor = Color.Lerp(lineRenderer.startColor, Color.clear, smooth);
                lineRenderer.endColor = Color.Lerp(lineRenderer.endColor, Color.clear, smooth);
                yield return null;
            }
            Push();
        }
    }
}