/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.AIModules.BehaviourTree.Attributes;
using AuroraFPSRuntime.AIModules.BehaviourTree.Variables;
using AuroraFPSRuntime.Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Nodes
{
    [TreeNodeContent("Cover Position", "Actions/Position/Cover Position")]
    [HideScriptField]
    public class CoverPositionNode : ActionNode
    {
        [SerializeField]
        [TreeVariable(typeof(Transform))]
        private string hideFromVariable;


        [SerializeField]
        [TreeVariable(typeof(int))]
        private string pointDensityVariable;

        [SerializeField]
        [TreeVariable(typeof(int))]
        private int pointDensity = 4;


        [SerializeField]
        [TreeVariable(typeof(Vector2))]
        private string minMaxDistanceVariable;

        [SerializeField]
        [TreeVariable(typeof(Vector2))]
        private Vector2 minMaxDistance = new Vector2(5, 10);


        [SerializeField]
        [TreeVariable(typeof(float))]
        private string shelterOffsetVariable;

        [SerializeField]
        [TreeVariable(typeof(float))]
        private float shelterOffset = 0.5f;


        [SerializeField]
        private LayerMask coverSurfaceLayer;

        [SerializeField]
        [TreeVariable(typeof(Vector3))]
        private string storageVariable;

        #region [Variables Toggle]
#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        private bool pointDensityToggle;

        [SerializeField]
        [HideInInspector]
        private bool minMaxDistanceToggle;

        [SerializeField]
        [HideInInspector]
        private bool shelterOffsetToggle;
#endif
        #endregion

        // Stored required properties.
        private Transform hideFrom;
        private List<Vector3> hidePoints;
        private List<Vector3> availablePoints;

        protected override State OnUpdate()
        {
            if (!tree.TryGetVariable<TransformVariable>(hideFromVariable, out TransformVariable transformVariable) || transformVariable.GetValue() == null)
            {
                return State.Failure;
            }

            hideFrom = transformVariable;

            if (!string.IsNullOrEmpty(pointDensityVariable) && tree.TryGetVariable<IntVariable>(pointDensityVariable, out IntVariable intVariable))
            {
                pointDensity = intVariable;
            }

            if (!string.IsNullOrEmpty(minMaxDistanceVariable) && tree.TryGetVariable<Vector2Variable>(minMaxDistanceVariable, out Vector2Variable vector2Variable))
            {
                minMaxDistance = vector2Variable;
            }

            if (!string.IsNullOrEmpty(shelterOffsetVariable) && tree.TryGetVariable<FloatVariable>(shelterOffsetVariable, out FloatVariable floatVariable))
            {
                shelterOffset = floatVariable;
            }

            hidePoints = new List<Vector3>();
            Collider[] colliders = Physics.OverlapSphere(owner.transform.position, minMaxDistance.y, coverSurfaceLayer);
            for (int i = 0; i < colliders.Length; i++)
            {
                Bounds bounds = colliders[i].bounds;

                Vector3 bottomLeft = new Vector3(bounds.min.x - 0.1f, bounds.min.y + 0.1f, bounds.min.z - 0.1f);
                Vector3 topLeft = new Vector3(bounds.min.x - 0.1f, bounds.min.y + 0.1f, bounds.max.z + 0.1f);
                Vector3 topRight = new Vector3(bounds.max.x + 0.1f, bounds.min.y + 0.1f, bounds.max.z + 0.1f);
                Vector3 bottomRight = new Vector3(bounds.max.x + 0.1f, bounds.min.y + 0.1f, bounds.min.z - 0.1f);

                for (int j = 0; j < pointDensity; j++)
                {
                    float percent = (float)j / pointDensity;

                    Ray[] rays = new Ray[]
                    {
                        new Ray(Vector3.Lerp(bottomLeft, topLeft, percent), Vector3.right),
                        new Ray(Vector3.Lerp(topLeft, topRight, percent), Vector3.back),
                        new Ray(Vector3.Lerp(topRight, bottomRight, percent), Vector3.left),
                        new Ray(Vector3.Lerp(bottomRight, bottomLeft, percent), Vector3.forward)
                    };

                    for (int k = 0; k < 4; k++)
                    {
                        if (colliders[i].Raycast(rays[k], out RaycastHit hitInfo, float.MaxValue))
                        {
                            Vector3 point = hitInfo.point + hitInfo.normal * shelterOffset;
                            hidePoints.Add(point);
                        }
                    }
                }
            }

            availablePoints = new List<Vector3>();
            for (int i = 0; i < hidePoints.Count; i++)
            {
                float sqrtDst = (hidePoints[i] - owner.transform.position).sqrMagnitude;

                if (minMaxDistance.x * minMaxDistance.x < sqrtDst && sqrtDst < minMaxDistance.y * minMaxDistance.y && Physics.Linecast(hideFrom.position, hidePoints[i], coverSurfaceLayer))
                {
                    availablePoints.Add(hidePoints[i]);
                }
            }

            if (!tree.TryGetVariable<Vector3Variable>(storageVariable, out Vector3Variable vector3Variable))
            {
                return State.Failure;
            }

            vector3Variable.SetValue(GetDistantPoint(availablePoints));
            return State.Success;
        }

        private Vector3 GetDistantPoint(List<Vector3> points)
        {
            float maxSqrtDst = float.MinValue;
            Vector3 point = Vector3.zero;
            for (int i = 0; i < points.Count; i++)
            {
                float sqrtDst = (hideFrom.position - points[i]).sqrMagnitude;
                if (maxSqrtDst < sqrtDst)
                {
                    maxSqrtDst = sqrtDst;
                    point = points[i];
                }
            }

            return point;
        }

        #region [Gizmos]
        public override void OnDrawGizmos()
        {
            //Draw overlap circle
            Gizmos.color = Color.yellow;
            DrawCircle(minMaxDistance.x);
            DrawCircle(minMaxDistance.y);

            //Draw points
            Gizmos.color = Color.red;
            if (availablePoints != null)
            {
                for (int i = 0; i < availablePoints.Count; i++)
                {
                    Gizmos.DrawSphere(availablePoints[i], 0.1f);
                }
            }
        }

        private void DrawCircle(float radius)
        {
            Vector3? lastPoint = null;
            int iterations = 16;
            for (int i = 0; i <= iterations; i++)
            {
                float angle = ((float)i / iterations) * Mathf.PI * 2;
                float x = Mathf.Cos(angle);
                float y = Mathf.Sin(angle);

                Vector3 point = owner.transform.position + (Vector3.forward * y + Vector3.right * x) * radius;
                if (lastPoint != null)
                {
                    Gizmos.DrawLine(lastPoint.Value, point);
                }
                lastPoint = point;
            }
        }
        #endregion
    }
}