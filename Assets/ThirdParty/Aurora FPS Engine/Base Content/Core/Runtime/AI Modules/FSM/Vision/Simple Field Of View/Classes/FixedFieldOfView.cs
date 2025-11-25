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
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

#region [Unity Editor Section]
#if UNITY_EDITOR
#endif
#endregion

namespace AuroraFPSRuntime.AIModules.Vision
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/AI Modules/FSM/Vision/Fixed Field Of View")]
    public sealed class FixedFieldOfView : FieldOfView, IVisionCallback
    {
        public enum TargetInitialization
        {
            Manual,
            ByTag,
        }

        private List<Transform> VisibleTargets = new List<Transform>();

        [SerializeField]
        private TargetInitialization targetInitialization = TargetInitialization.Manual;

        [SerializeField]
        [ReorderableList()]
        [VisibleIf("targetInitialization", "Manual")]
        private Transform[] targets;

        [SerializeField]
        [TagPopup]
        [VisibleIf("targetInitialization", "ByTag,ByName")]
        private string targetTag = string.Empty;

        [SerializeField]
        [Foldout("Vision Settings", Style = "Header")]
        [Slider(0.0f, 360.0f)]
        private float viewAngle = 120.0f;

        [SerializeField]
        [Foldout("Vision Settings", Style = "Header")]
        private float viewRadius = 10.0f;

        [SerializeField]
        [Foldout("Vision Settings", Style = "Header")]
        private LayerMask obstacleLayer = 1 << 0;

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
            if (targetInitialization == TargetInitialization.ByTag)
            {
                GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(targetTag);
                targets = new Transform[gameObjects.Length];
                for (int i = 0; i < gameObjects.Length; i++)
                {
                    targets[i] = gameObjects[i].transform;
                }
            }

            searchCoroutine = new CoroutineObject(this);
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            searchCoroutine.Start(SearchProcessing);
        }

        /// <summary>
        /// Called when the behaviour becomes disabled.
        /// </summary>
        private void OnDisable()
        {
            searchCoroutine.Stop();
        }

        /// <summary>
        /// Search target coroutine processing.
        /// </summary>
        private IEnumerator SearchProcessing()
        {
            Transform target = null;
            bool hasAnyTarget = true;
            WaitForSeconds delay = new WaitForSeconds(searchRate);
            while (true)
            {
                VisibleTargets.Clear();
                Vector3 originPosition = GetOriginPosition();
                for (int i = 0; i < targets.Length; i++)
                {
                    target = targets[i];
                    Vector3 direction = (target.position - originPosition).normalized;
                    if (Vector3.Angle(transform.forward, direction) < (viewAngle / 2))
                    {
                        float distance = Vector3.Distance(originPosition, target.position);
                        if (!Physics.Raycast(originPosition, direction, distance, obstacleLayer))
                        {
                            VisibleTargets.Add(target);
                            hasAnyTarget = true;
                            OnTargetBecomeVisible?.Invoke(target);
                        }
                    }
                }

                if (hasAnyTarget && VisibleTargets.Count == 0)
                {
                    OnTargetsBecomeInvisible?.Invoke();
                    hasAnyTarget = false;
                }

                yield return delay;
            }
        }

        public override IReadOnlyList<Transform> GetVisibleTargets()
        {
            return VisibleTargets;
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
                Vector3 position = transform.position;
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
        public TargetInitialization GetTargetInitialization()
        {
            return targetInitialization;
        }

        public void SetTargetInitialization(TargetInitialization value)
        {
            targetInitialization = value;
        }

        public string GetSearchTargetFormat()
        {
            return targetTag;
        }

        public void SetSearchTargetFormat(string value)
        {
            targetTag = value;
        }

        public float GetViewAngle()
        {
            return viewAngle;
        }

        public void SetViewAngle(float value)
        {
            viewAngle = value;
        }

        public float GetViewRadius()
        {
            return viewRadius;
        }

        public void SetViewRadius(float value)
        {
            viewRadius = value;
        }

        public LayerMask GetObstacleLayer()
        {
            return obstacleLayer;
        }

        public void SetObstacleLayer(LayerMask value)
        {
            obstacleLayer = value;
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