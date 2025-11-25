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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.Vision
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/AI Modules/FSM/Vision/Procedural Field Of View")]
    public sealed class ProceduralFieldOfView : FieldOfView, IVisionCallback
    {
        internal readonly List<Transform> VisibleTargets = new List<Transform>();

        [SerializeField]
        private LayerMask cullingLayer = 1 << 0;

        [SerializeField]
        private LayerMask obstacleLayer = ~0;

        [SerializeField]
        [MinValue(0.01f)]
        private float viewRadius = 10.0f;

        [SerializeField]
        [Slider(1.0f, 360.0f)]
        private float viewAngle = 140.0f;

        [SerializeField]
        [Foldout("Advanced Settings", Style = "Header")]
        [MinValue(0.0f)]
        private float heightOffset = 0.0f;

        [SerializeField]
        [Foldout("Advanced Settings", Style = "Header")]
        [MinValue(0.01f)]
        private float searchRate = 0.25f;

        // Stored required properties.
        private CoroutineObject searchCoroutine;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            searchCoroutine = new CoroutineObject(this);
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            searchCoroutine.Start(SearchTargets);
        }

        /// <summary>
        /// Called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable()
        {
            searchCoroutine.Stop();
        }

        private IEnumerator SearchTargets()
        {
            bool hasAnyTarget = false;
            WaitForSeconds searchDelay = new WaitForSeconds(searchRate);
            while (true)
            {
                VisibleTargets.Clear();

                Vector3 originPosition = GetOriginPosition();
                Collider[] overlapColliders = Physics.OverlapSphere(originPosition, viewRadius, cullingLayer);

                for (int j = 0, length = overlapColliders.Length; j < length; j++)
                {
                    Collider overlapCollider = overlapColliders[j];

                    Vector3 overlapColliderOrigin = overlapCollider.bounds.center;
                    Vector3 direction = (overlapColliderOrigin - originPosition).normalized;
                    if (Vector3.Angle(transform.forward, direction) < (viewAngle / 2))
                    {
                        float distance = Vector3.Distance(originPosition, overlapColliderOrigin);
                        if (!Physics.Raycast(originPosition, direction, distance, obstacleLayer))
                        {
                            VisibleTargets.Add(overlapCollider.transform);
                            hasAnyTarget = true;
                            OnTargetBecomeVisible?.Invoke(overlapCollider.transform);
                        }
                    }
                }

                if(hasAnyTarget && VisibleTargets.Count == 0)
                {
                    OnTargetsBecomeInvisible?.Invoke();
                    hasAnyTarget = false;
                }

                yield return searchDelay;
            }
        }

        /// <summary>
        /// Get vision origin position considering the height offset.
        /// </summary>
        private Vector3 GetOriginPosition()
        {
            if (heightOffset > 0)
            {
                return transform.position + (transform.up * heightOffset);
            }
            return transform.position;
        }


        /// <summary>
        /// Get targets list of specific preset.
        /// </summary>
        /// <param name="name">Name of preset.</param>
        /// <returns>Readonly list of targets.</returns>
        public override IReadOnlyList<Transform> GetVisibleTargets()
        {
            return VisibleTargets;
        }

        #region [Aurora Engine Debug Directive]
#if AURORA_ENGINE_DEBUG && UNITY_EDITOR
        [SerializeField]
        [Foldout("Debug Settings", Style = "Header")]
        private bool Visualize = false;

        [SerializeField]
        [Foldout("Debug Settings", Style = "Header")]
        private Color ColorFOV = Color.white;

        private void OnDrawGizmos()
        {
            if (Visualize)
            {
                Gizmos.color = ColorFOV;

                float steps = 35.0f;
                float halfFOV = viewAngle / 2.0f;
                Vector3 position = GetOriginPosition();
                Vector3 direction = transform.forward;
                Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
                Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
                Vector3 leftRayDirection = leftRayRotation * direction * viewRadius;
                Vector3 rightRayDirection = rightRayRotation * direction * viewRadius;

                Gizmos.DrawRay(position, leftRayDirection);
                Gizmos.DrawRay(position, rightRayDirection);

                Vector3 forwardLimitPos = position + direction;
                float srcAngles = Mathf.Rad2Deg * Mathf.Atan2(forwardLimitPos.z - position.z, forwardLimitPos.x - position.x);
                Vector3 initialPos = position;
                Vector3 posA = initialPos;
                float stepAngles = viewAngle / steps;
                float _angle = srcAngles - halfFOV;
                for (int j = 0; j <= steps; j++)
                {
                    float rad = Mathf.Deg2Rad * _angle;
                    Vector3 posB = initialPos;
                    posB += new Vector3(viewRadius * Mathf.Cos(rad), 0, viewRadius * Mathf.Sin(rad));

                    Gizmos.DrawLine(posA, posB);

                    _angle += stepAngles;
                    posA = posB;
                }
                Gizmos.DrawLine(posA, initialPos);
            }

        }
#endif
        #endregion

        #region [Event Callback Function]
        /// <summary>
        /// Called when target become visible in field of view.
        /// </summary>
        public override event Action<Transform> OnTargetBecomeVisible;

        /// <summary>
        /// Called when target become invisible in field of view.
        /// </summary>
        public override event Action OnTargetsBecomeInvisible;
        #endregion

        #region [Getter / Setter]
        public LayerMask GetCullingLayer()
        {
            return cullingLayer;
        }

        public void SetCullingLayer(LayerMask value)
        {
            cullingLayer = value;
        }

        public LayerMask GetObstacleLayer()
        {
            return obstacleLayer;
        }

        public void SetObstacleLayer(LayerMask value)
        {
            obstacleLayer = value;
        }

        public float GetViewRadius()
        {
            return viewRadius;
        }

        public void SetViewRadius(float value)
        {
            viewRadius = value;
        }

        public float GetViewAngle()
        {
            return viewAngle;
        }

        public void SetViewAngle(float value)
        {
            viewAngle = value;
        }

        public float GetHeightOffset()
        {
            return heightOffset;
        }

        public void SetHeightOffset(float value)
        {
            heightOffset = value;
        }

        public float GetSearchRate()
        {
            return searchRate;
        }

        public void SetSearchRate(float value)
        {
            searchRate = value;
        }
        #endregion
    }
}