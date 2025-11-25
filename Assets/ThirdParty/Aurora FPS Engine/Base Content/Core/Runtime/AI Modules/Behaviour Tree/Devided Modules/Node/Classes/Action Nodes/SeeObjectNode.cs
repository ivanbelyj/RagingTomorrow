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
using System.Linq;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Nodes
{
    [TreeNodeContent("See Object", "Conditions/See Object")]
    [HideScriptField]
    public class SeeObjectNode : ActionNode
    {
        [SerializeField]
        [TagPopup]
        private string objectTag = "Player";


        [SerializeField]
        [TreeVariable(typeof(float))]
        private string fovVariable;

        [SerializeField]
        [TreeVariable(typeof(float))]
        [MinValue(0f)]
        [MaxValue(360f)]
        private float fov = 90f;

        [SerializeField]
        [TreeVariable(typeof(float))]
        private string heightOffsetVariable;

        [SerializeField]
        [TreeVariable(typeof(float))]
        [Min(0f)]
        private float heightOffset = 1.635f;


        [SerializeField]
        [TreeVariable(typeof(float))]
        private string maxDistanceVariable;

        [SerializeField]
        [TreeVariable(typeof(float))]
        [Min(0.01f)]
        private float maxDistance = 10f;


        [SerializeField]
        private LayerMask obstacleLayer = ~0;

        [SerializeField]
        [TreeVariable(typeof(Transform))]
        private string targetVariable;

        [SerializeField]
        [TreeVariable(typeof(Vector3))]
        private string lastPositionVariable;

        #region [Variables Toggle]
#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        private bool fovToggle;

        [SerializeField]
        [HideInInspector]
        private bool heightOffsetToggle;

        [SerializeField]
        [HideInInspector]
        private bool maxDistanceToggle;
#endif
        #endregion

        protected override State OnUpdate()
        {
            if (!string.IsNullOrEmpty(fovVariable) && tree.TryGetVariable<FloatVariable>(fovVariable, out FloatVariable floatVariable1))
            {
                fov = floatVariable1;
            }

            if (!string.IsNullOrEmpty(maxDistanceVariable) && tree.TryGetVariable<FloatVariable>(maxDistanceVariable, out FloatVariable floatVariable2))
            {
                maxDistance = floatVariable2;
            }

            List<Transform> targets = new List<Transform>();
            Vector3 eyePos = owner.transform.position + Vector3.up * heightOffset;

            Collider[] colliders = Physics.OverlapSphere(eyePos, maxDistance);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].tag == objectTag)
                {
                    Vector3 point = colliders[i].bounds.center;
                    if (!Physics.Linecast(eyePos, point, obstacleLayer))
                    {
                        float angle = Vector3.Angle(owner.transform.forward, (point - eyePos).normalized);
                        if (angle <= fov / 2)
                        {
                            targets.Add(colliders[i].transform);
                        }
                    }
                }
            }

            if (targets.Count > 0)
            {
                Transform target = targets.OrderBy(t => Vector3.Distance(t.position, eyePos)).First();

                if (tree.TryGetVariable<TransformVariable>(targetVariable, out TransformVariable variable))
                {
                    variable.SetValue(target);

                    if (tree.TryGetVariable<Vector3Variable>(lastPositionVariable, out Vector3Variable vector3Variable))
                    {
                        vector3Variable.SetValue(target.position);
                    }
                }

                return State.Success;
            }

            return State.Failure;
        }

        public override void OnDrawGizmos()
        {
            DrawFOV(owner.transform, fov, maxDistance);
        }

        private void DrawFOV(Transform owner, float fov, float distance)
        {
            Gizmos.color = Color.blue;
            fov = Mathf.Clamp(fov, 0, 360);
            int iterations = Mathf.Max((int)(fov / 10), 2);
            fov = fov * Mathf.Deg2Rad / 2;
            Vector3 eyePos = owner.position + Vector3.up * heightOffset;
            Vector3 prevPoint = eyePos + owner.forward * Mathf.Cos(-fov) * distance + owner.right * Mathf.Sin(-fov) * distance;
            Vector3 currPoint = Vector3.zero;
            Gizmos.DrawLine(eyePos, prevPoint);
            for (int i = 1; i <= iterations; i++)
            {
                float angle = Mathf.Lerp(-fov, fov, (float)i / iterations);
                float x = Mathf.Cos(angle) * distance;
                float y = Mathf.Sin(angle) * distance;
                currPoint = eyePos + owner.forward * x + owner.right * y;

                Gizmos.DrawLine(prevPoint, currPoint);
                prevPoint = currPoint;
            }
            Gizmos.DrawLine(eyePos, currPoint);
        }
    }
}