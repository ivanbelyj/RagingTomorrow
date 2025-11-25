/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Mathematics;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.CoverSystem
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/AI Modules/FSM/Cover/Cover Map")]
    [DisallowMultipleComponent]
    public sealed class CoverMap : MonoBehaviour
    {
        [SerializeField]
        [ReorderableList(ElementLabel = "Point {niceIndex}")]
        private CoverPoint[] coverPoints;

        public bool TryGetNearestPoint(Transform target, Transform relative, out CoverPoint coverPoint)
        {
            int bestIndex = -1;
            float bestDistance = Mathf.Infinity;
            for (int i = 0; i < coverPoints.Length; i++)
            {
                coverPoint = coverPoints[i];
                if (!coverPoint.IsOccupied() && coverPoint.IsCover(relative))
                {
                    float distance = Math.Distance2D(target.position, coverPoint.GetPoint().position);
                    if (distance < bestDistance)
                    {
                        bestIndex = i;
                        bestDistance = distance;
                    }
                }
            }

            if (bestIndex > -1)
            {
                coverPoint = coverPoints[bestIndex];
                return true;
            }
            coverPoint = null;
            return false;
        }

        public bool TryGetDistantPoint(Transform target, Transform relative, out CoverPoint coverPoint)
        {
            int bestIndex = -1;
            float bestDistance = Mathf.NegativeInfinity;
            for (int i = 0; i < coverPoints.Length; i++)
            {
                coverPoint = coverPoints[i];
                if (!coverPoint.IsOccupied() && coverPoint.IsCover(relative))
                {
                    float distance = Math.Distance2D(target.position, coverPoint.GetPoint().position);
                    if (distance > bestDistance)
                    {
                        bestIndex = i;
                        bestDistance = distance;
                    }
                }
            }

            if (bestIndex > -1)
            {
                coverPoint = coverPoints[bestIndex];
                return true;
            }
            coverPoint = null;
            return false;
        }

        public bool TryGetFirstAvailablePoint(Transform target, Transform relative, out CoverPoint coverPoint)
        {
            for (int i = 0; i < coverPoints.Length; i++)
            {
                coverPoint = coverPoints[i];
                if (!coverPoint.IsOccupied() && coverPoint.IsCover(relative))
                {
                    return true;
                }
            }
            coverPoint = null;
            return false;
        }

        public bool ReleasePoint(Transform point)
        {
            for (int i = 0; i < coverPoints.Length; i++)
            {
                CoverPoint coverPoint = coverPoints[i];
                if (coverPoint.IsOccupied())
                {
                    if (coverPoint.GetPoint() == point)
                    {
                        coverPoints[i].IsOccupied(false);
                        return true;
                    }
                }
            }
            return false;
        }

        #region [Aurora Engine Debug Directive]
#if AURORA_ENGINE_DEBUG && UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if(coverPoints != null)
            {
                for (int i = 0; i < coverPoints.Length; i++)
                {
                    CoverPoint coverPoint = coverPoints[i];
                    if (coverPoint.Visualize)
                    {
                        Gizmos.color = Color.blue;

                        float rayRange = 3.5f;
                        float radius = 2.0f;
                        float steps = 35.0f;
                        float halfFOV = coverPoint.GetForwardTolerance() / 2.0f;
                        Vector3 position = coverPoint.GetPoint().position;
                        Vector3 direction = coverPoint.GetPoint().forward;
                        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
                        Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
                        Vector3 leftRayDirection = leftRayRotation * direction * rayRange;
                        Vector3 rightRayDirection = rightRayRotation * direction * rayRange;

                        Gizmos.DrawRay(position, direction * rayRange);
                        Gizmos.DrawRay(position, leftRayDirection);
                        Gizmos.DrawRay(position, rightRayDirection);

                        Vector3 forwardLimitPos = position + direction;
                        float srcAngles = Mathf.Rad2Deg * Mathf.Atan2(forwardLimitPos.z - position.z, forwardLimitPos.x - position.x);
                        Vector3 initialPos = position;
                        Vector3 posA = initialPos;
                        float stepAngles = coverPoint.GetForwardTolerance() / steps;
                        float angle = srcAngles - coverPoint.GetForwardTolerance() / 2;
                        for (int j = 0; j <= steps; j++)
                        {
                            float rad = Mathf.Deg2Rad * angle;
                            Vector3 posB = initialPos;
                            posB += new Vector3(radius * Mathf.Cos(rad), 0, radius * Mathf.Sin(rad));

                            Gizmos.DrawLine(posA, posB);

                            angle += stepAngles;
                            posA = posB;
                        }
                        Gizmos.DrawLine(posA, initialPos);
                    }
                }
            }
        }
#endif
        #endregion
    }
}