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

namespace AuroraFPSRuntime.AIModules.Conditions
{
    [ConditionMenu("Distance (Transform)", "Vector3/Distance(Transform)", Description = "Compare distance between target and AI.")]
    public class DistanceTransformCondition : Condition
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

        public enum Target
        {
            Automatically,
            Manual
        }

        [SerializeField]
        private Target target;

        [SerializeField]
        [TagPopup]
        [VisibleIf("target", "Automatically")]
        private string targetTag;

        [SerializeField]
        [VisibleIf("target", "Manual")]
        private Transform targetReference;

        [SerializeField] 
        private Comparison comparison;

        [SerializeField] 
        [MinValue(0.01f)]
        private float distance;

        // Stored required components.
        private Transform transform;

        // Stored required properties.
        private CoroutineObject searchCoroutine;

        protected override void OnInitialize(AIController owner)
        {
            transform = owner.transform;
            searchCoroutine = new CoroutineObject(owner);
        }

        /// <summary>
        /// Condition for translate to the next AI behaviour.
        /// </summary>
        public override bool IsExecuted()
        {
            if(targetReference == null)
            {
                if (!searchCoroutine.IsProcessing())
                {
                    searchCoroutine.Start(SearchTarget, true);
                }
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

        private IEnumerator SearchTarget()
        {
            WaitForSeconds delay = new WaitForSeconds(1.0f);
            GameObject targetObject = null;
            while (targetObject == null)
            {
                targetObject = GameObject.FindGameObjectWithTag(targetTag);
                yield return delay;
            }
            targetReference = targetObject.transform;
        }

        #region [Getter / Setter]
        public Target GetTarget()
        {
            return target;
        }

        public void SetTarget(Target value)
        {
            target = value;
        }

        public string GetTargetTag()
        {
            return targetTag;
        }

        public void SetTargetTag(string value)
        {
            targetTag = value;
        }

        public Transform GetTargetReference()
        {
            return targetReference;
        }

        public void SetTargetReference(Transform value)
        {
            targetReference = value;
        }

        public Comparison GetComparison()
        {
            return comparison;
        }

        public void SetComparison(Comparison value)
        {
            comparison = value;
        }

        public float GetDistance()
        {
            return distance;
        }

        public void SetDistance(float value)
        {
            distance = value;
        }
        #endregion
    }
}
