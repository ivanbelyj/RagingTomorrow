/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.AIModules.Vision;
using UnityEngine;

#region [Unity Editor Section]
#if UNITY_EDITOR
#endif
#endregion

namespace AuroraFPSRuntime.AIModules.Conditions
{
    [ConditionMenu("Distance (Vision Target)", "Vector3/Distance(Vision Target)")]
    [RequireComponent(typeof(IVisionCallback))]
    public sealed class DistanceTargetCondition : Condition
    {
        public enum Comparison
        {
            Equal,
            NotEqual,
            Greater,
            Less,
            GreaterOrEqual,
            LessOrEqual,
        }

        [SerializeField]
        private Comparison comparison;

        [SerializeField]
        [MinValue(0.01f)]
        private float distance;

        // Stored required components.
        private Transform transform;
        private Transform targetReference;
        private IVisionCallback vision;

        protected override void OnInitialize(AIController owner)
        {
            transform = owner.transform;
            vision = owner.GetComponent<IVisionCallback>();
        }

        protected override void OnEnable()
        {
            vision.OnTargetBecomeVisible += OnFindTargetCallback;
        }

        protected override void OnDisable()
        {
            vision.OnTargetBecomeVisible -= OnFindTargetCallback;
        }

        public override bool IsExecuted()
        {
            if (targetReference == null)
            {
                return false;
            }

            switch (comparison)
            {
                case Comparison.Equal:
                    return Vector3.Distance(transform.position, targetReference.position) == distance;
                case Comparison.NotEqual:
                    return Vector3.Distance(transform.position, targetReference.position) != distance;
                case Comparison.Greater:
                    return Vector3.Distance(transform.position, targetReference.position) > distance;
                case Comparison.Less:
                    return Vector3.Distance(transform.position, targetReference.position) < distance;
                case Comparison.GreaterOrEqual:
                    return Vector3.Distance(transform.position, targetReference.position) >= distance;
                case Comparison.LessOrEqual:
                    return Vector3.Distance(transform.position, targetReference.position) <= distance;
                default:
                    return false;
            }
        }


        private void OnFindTargetCallback(Transform target)
        {
            if (targetReference == null)
            {
                targetReference = target;
            }
            else
            {
                float currentDistance = Vector3.Distance(transform.position, targetReference.position);
                float targetDistance = Vector3.Distance(transform.position, target.position);
                if (targetDistance < currentDistance)
                {
                    targetReference = target;
                }
            }
        }
    }
}