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

namespace AuroraFPSRuntime.SystemModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/IK/Body IK")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public sealed class BodyIK : InverseKinematic
    {
        [SerializeField]
        private Transform target;

        [SerializeField]
        [Slider(0, 1)]
        private float weight = 1.0f;

        [SerializeField]
        [Slider(0, 1)]
        private float bodyWeight = 1.0f;

        [SerializeField]
        [Slider(0, 1)]
        private float headWeight = 1.0f;

        [SerializeField]
        [Slider(0, 1)]
        private float eyesWeight = 1.0f;

        [SerializeField]
        [Slider(0, 1)]
        private float clampWeight = 1.0f;

        [SerializeField]
        [MinValue(0.01f)]
        private float smoothTime = 15.0f;

        // Stored required properties.
        private Vector3 ikPosition;

        /// <summary>
        /// Callback for calculation animation IK.
        /// </summary>
        /// <param name="layerIndex">Index of the layer on which the IK solver is called.</param>
        protected override void OnCalculateIK(int layerIndex)
        {
            ikPosition = Vector3.Lerp(ikPosition, target.position, smoothTime * Time.deltaTime);
            animator.SetLookAtPosition(ikPosition);
            animator.SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
        }

        /// <summary>
        /// Return true if IK is processing, otherwise false.
        /// </summary>
        public override bool IsActive()
        {
            return base.IsActive() && target != null;
        }

        #region [Getter / Setter]
        public Transform GetTarget()
        {
            return target;
        }

        public void SetTarget(Transform value)
        {
            target = value;
        }

        public float GetWeight()
        {
            return weight;
        }

        public void SetWeight(float value)
        {
            weight = value;
        }

        public float GetBodyWeight()
        {
            return bodyWeight;
        }

        public void SetBodyWeight(float value)
        {
            bodyWeight = value;
        }

        public float GetHeadWeight()
        {
            return headWeight;
        }

        public void SetHeadWeight(float value)
        {
            headWeight = value;
        }

        public float GetEyesWeight()
        {
            return eyesWeight;
        }

        public void SetEyesWeight(float value)
        {
            eyesWeight = value;
        }

        public float GetClampWeight()
        {
            return clampWeight;
        }

        public void SetClampWeight(float value)
        {
            clampWeight = value;
        }
        #endregion
    }
}