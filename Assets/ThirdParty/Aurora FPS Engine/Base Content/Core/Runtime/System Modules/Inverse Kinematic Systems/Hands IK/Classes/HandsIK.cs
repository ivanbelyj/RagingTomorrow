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
    [AddComponentMenu("Aurora FPS Engine/System Modules/IK/Hands IK")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class HandsIK : InverseKinematic
    {
        [SerializeField]
        [Label("Target")]
        [TabGroup("Hand IK", "Left Hand IK")]
        [Tooltip("IK follow target for left hand.")]
        private Transform leftHandTarget;

        [SerializeField]
        [Label("Weight")]
        [Slider(0, 1)]
        [TabGroup("Hand IK", "Left Hand IK")]
        [Tooltip("IK weight of left hand.")]
        private float leftHandWeight = 1.0f;

        [SerializeField]
        [Label("Smooth")]
        [VisualClamp(50.0f, 0.01f)]
        [TabGroup("Hand IK", "Left Hand IK")]
        [Tooltip("IK follow smooth of left hand.")]
        private float leftHandSmooth = 10.0f;

        [SerializeField]
        [Label("Target")]
        [TabGroup("Hand IK", "Right Hand IK")]
        [Tooltip("IK follow target for right hand.")]
        private Transform rightHandTarget;

        [SerializeField]
        [Label("Weight")]
        [Slider(0, 1)]
        [TabGroup("Hand IK", "Right Hand IK")]
        [Tooltip("IK weight of right hand.")]
        private float rightHandWeight = 1.0f;

        [SerializeField]
        [Label("Smooth")]
        [VisualClamp(50.0f, 0.01f)]
        [TabGroup("Hand IK", "Right Hand IK")]
        [Tooltip("IK follow smooth of right hand.")]
        private float rightHandSmooth = 10.0f;

        // Stored left hand IK properties.
        private Vector3 leftHandIKPosition;
        private Quaternion leftHandIKRotation;
        private bool updateLeftIKPosAndRot;

        // Stored right hand IK properties.
        private Vector3 rightHandIKPosition;
        private Quaternion rightHandIKRotation;
        private bool updateRightIKPosAndRot;

        /// <summary>
        /// Callback for calculation animation IK.
        /// </summary>
        /// <param name="layerIndex">Index of the layer on which the IK solver is called.</param>
        protected override void OnCalculateIK(int layerIndex)
        {
            CalculateLeftHandIK();
            CalculateRightHandIK();
        }

        /// <summary>
        /// Calculation left hand IK.
        /// </summary>
        /// <param name="layerIndex">Index of the layer on which the IK solver is called.</param>
        protected void CalculateLeftHandIK()
        {
            if (leftHandTarget != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandWeight);

                if (leftHandWeight > 0)
                {
                    if (updateLeftIKPosAndRot)
                    {
                        leftHandIKPosition = animator.GetIKPosition(AvatarIKGoal.LeftHand);
                        leftHandIKRotation = animator.GetIKRotation(AvatarIKGoal.LeftHand);
                        updateLeftIKPosAndRot = false;
                    }

                    Vector3 smoothPosition = Vector3.Lerp(leftHandIKPosition, leftHandTarget.position, leftHandSmooth * Time.deltaTime);
                    Quaternion smoothRotation = Quaternion.Slerp(leftHandIKRotation, leftHandTarget.rotation, leftHandSmooth * Time.deltaTime);

                    animator.SetIKPosition(AvatarIKGoal.LeftHand, smoothPosition);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, smoothRotation);

                    leftHandIKPosition = animator.GetIKPosition(AvatarIKGoal.LeftHand);
                    leftHandIKRotation = animator.GetIKRotation(AvatarIKGoal.LeftHand);
                }
            }
            else if (!updateLeftIKPosAndRot)
            {
                updateLeftIKPosAndRot = true;
            }
        }

        /// <summary>
        /// Calculation right hand IK.
        /// </summary>
        /// <param name="layerIndex">Index of the layer on which the IK solver is called.</param>
        protected void CalculateRightHandIK()
        {
            if (rightHandTarget != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandWeight);

                if (rightHandWeight > 0)
                {
                    if (updateRightIKPosAndRot)
                    {
                        rightHandIKPosition = animator.GetIKPosition(AvatarIKGoal.RightHand);
                        rightHandIKRotation = animator.GetIKRotation(AvatarIKGoal.RightHand);
                        updateRightIKPosAndRot = false;
                    }

                    Vector3 smoothPosition = Vector3.Lerp(rightHandIKPosition, rightHandTarget.position, rightHandSmooth * Time.deltaTime);
                    Quaternion smoothRotation = Quaternion.Slerp(rightHandIKRotation, rightHandTarget.rotation, rightHandSmooth * Time.deltaTime);

                    animator.SetIKPosition(AvatarIKGoal.RightHand, smoothPosition);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, smoothRotation);

                    rightHandIKPosition = animator.GetIKPosition(AvatarIKGoal.RightHand);
                    rightHandIKRotation = animator.GetIKRotation(AvatarIKGoal.RightHand);
                }
            }
            else if (!updateRightIKPosAndRot)
            {
                updateRightIKPosAndRot = true;
            }
        }

        #region [Getter / Setter]
        public Transform GetLeftHandTarget()
        {
            return leftHandTarget;
        }

        public void SetLeftHandTarget(Transform value)
        {
            leftHandTarget = value;
        }

        public Transform GetRightHandTarget()
        {
            return rightHandTarget;
        }

        public void SetRightHandTarget(Transform value)
        {
            rightHandTarget = value;
        }

        public float GetLeftHandWeight()
        {
            return leftHandWeight;
        }

        public void SetLeftHandWeight(float value)
        {
            leftHandWeight = value;
        }

        public float GetRightHandWeight()
        {
            return rightHandWeight;
        }

        public void SetRightHandWeight(float value)
        {
            rightHandWeight = value;
        }
        #endregion
    }
}