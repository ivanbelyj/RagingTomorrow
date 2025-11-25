/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;

namespace AuroraFPSRuntime.SystemModules
{
    public abstract class CharacterIKSystem : MonoBehaviour
    {
        // Base IK properties
        [Header("IK Properties")]
        [SerializeField] private bool ikIsActive = true;

        // Foot IK properties
        [Header("Foot IK")]
        [SerializeField] private Transform leftFoot;
        [SerializeField] private Transform rightFoot;
        [SerializeField] private LayerMask groundLayer = 1 << 0;
        [SerializeField] private float footOffset = 0.125f;
        [SerializeField] private float deltaAmplifier = 1.75f;
        [SerializeField] private float colliderSmooth = 17.5f;
        [SerializeField] private bool processFootRotation = false;

        // Upper body IK properties
        [Header("Upper Body IK")]
        [SerializeField] private Transform lookTarget;
        [SerializeField] private float weight = 1.0f;
        [SerializeField] private float bodyWeight = 1.0f;
        [SerializeField] private float headWeight = 1.0f;
        [SerializeField] private float eyesWeight = 1.0f;
        [SerializeField] private float clampWeight = 1.0f;

        // Hands IK properties
        [Header("Hands IK")]
        [SerializeField] private Transform leftHandTarget;
        [SerializeField] private Transform rightHandTarget;
        [SerializeField] private float handIKSmooth = 7.0f;

        // Stored required properites.
        private float leftFootY;
        private float rightFootY;
        private float storedColliderHeight;
        private float handWeight;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        protected virtual void Start()
        {
            if (GetCapsuleCollider() != null)
            {
                storedColliderHeight = GetCapsuleCollider().height;
            }
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        protected virtual void Update()
        {
            if (ikIsActive && GetAnimator() != null && GetCapsuleCollider() != null)
            {
                HandleColliderOffset();
            }
        }

        /// <summary>
        /// Callback for setting up animation IK (inverse kinematics).
        /// </summary>
        /// <param name="layerIndex">Index of the layer on which the IK solver is called.</param>
        protected virtual void OnAnimatorIK(int layerIndex)
        {
            if (ikIsActive && GetAnimator() != null)
            {
                FootIKProcessing();
                UpperBodyIKProcessing();
                HandsIKProcessing();
            }
        }

        /// <summary>
        /// Processing character fool IK system.
        /// </summary>
        protected virtual void FootIKProcessing()
        {
            RaycastHit floorHit;
            Vector3 targetPosition = Vector3.zero;
            Quaternion targetRotation = Quaternion.identity;
            float legDistance = GetLegDistance();

            if (leftFoot != null && Physics.Linecast(GetFootOrigin(leftFoot.position, legDistance), GetFootTarget(leftFoot.position, legDistance), out floorHit, groundLayer, QueryTriggerInteraction.Ignore))
            {
                targetPosition = GetFootPosition(floorHit.point);
                GetAnimator().SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1.0f);
                GetAnimator().SetIKPosition(AvatarIKGoal.LeftFoot, targetPosition);

                leftFootY = targetPosition.y;

                if (processFootRotation)
                {
                    targetRotation = GetFootRotation(leftFoot, floorHit.normal);
                    GetAnimator().SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1.0f);
                    GetAnimator().SetIKRotation(AvatarIKGoal.LeftFoot, targetRotation);
                }
            }

            if (rightFoot != null && Physics.Linecast(GetFootOrigin(rightFoot.position, legDistance), GetFootTarget(rightFoot.position, legDistance), out floorHit, groundLayer, QueryTriggerInteraction.Ignore))
            {
                targetPosition = GetFootPosition(floorHit.point);
                GetAnimator().SetIKPositionWeight(AvatarIKGoal.RightFoot, 1.0f);
                GetAnimator().SetIKPosition(AvatarIKGoal.RightFoot, targetPosition);

                rightFootY = targetPosition.y;

                if (processFootRotation)
                {
                    targetRotation = GetFootRotation(rightFoot, floorHit.normal);
                    GetAnimator().SetIKRotationWeight(AvatarIKGoal.RightFoot, 1.0f);
                    GetAnimator().SetIKRotation(AvatarIKGoal.RightFoot, targetRotation);
                }
            }
        }

        /// <summary>
        /// Upper body IK processing.
        /// </summary>
        protected virtual void UpperBodyIKProcessing()
        {
            if (lookTarget != null)
            {
                GetAnimator().SetLookAtPosition(lookTarget.position);
                GetAnimator().SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
            }
        }

        /// <summary>
        /// Hands IK processing.
        /// </summary>
        protected virtual void HandsIKProcessing()
        {
            bool handsConditions = leftHandTarget != null && rightHandTarget != null;
            handWeight = Mathf.SmoothStep(handWeight, handsConditions ? 1.0f : 0.0f, handIKSmooth * Time.deltaTime);

            if (leftHandTarget != null)
            {
                GetAnimator().SetIKPositionWeight(AvatarIKGoal.LeftHand, handWeight);
                GetAnimator().SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);

                GetAnimator().SetIKRotationWeight(AvatarIKGoal.LeftHand, handWeight);
                GetAnimator().SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
            }

            if (rightHandTarget != null)
            {
                GetAnimator().SetIKPositionWeight(AvatarIKGoal.RightHand, handWeight);
                GetAnimator().SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);

                GetAnimator().SetIKRotationWeight(AvatarIKGoal.RightHand, handWeight);
                GetAnimator().SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
            }
        }

        /// <summary>
        /// Controller velocity.
        /// </summary>
        public abstract Vector3 GetVelocity();

        /// <summary>
        /// Controller animator component.
        /// </summary>
        public abstract Animator GetAnimator();

        /// <summary>
        /// Controller capsule collider component.
        /// </summary>
        public abstract CapsuleCollider GetCapsuleCollider();

        /// <summary>
        /// Handle capsule collider offset.
        /// </summary>
        protected virtual void HandleColliderOffset()
        {
            if (GetHorizontalVelocity() < 0.1f)
            {
                float delta = GetFootDelta();
                float targetHeight = storedColliderHeight - (delta * deltaAmplifier);
                GetCapsuleCollider().height = targetHeight;
            }
            else
            {
                GetCapsuleCollider().height = storedColliderHeight;
            }
        }

        /// <summary>
        /// Get state based leg distance.
        /// </summary>
        public float GetLegDistance()
        {
            return 1 / (GetHorizontalVelocity() + 0.8f);
        }

        /// <summary>
        /// Get velocity only on horizontal axes (x, z) ignoring vertical axis (y).
        /// </summary>
        public float GetHorizontalVelocity()
        {
            Vector3 velocity = GetVelocity();
            velocity.y = 0;
            return velocity.magnitude;
        }

        /// <summary>
        /// Get foot rotation relative surface normal.
        /// </summary>
        public Quaternion GetFootRotation(Transform foot, Vector3 normal)
        {
            Quaternion footRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, normal), normal);
            footRotation.eulerAngles = new Vector3(footRotation.eulerAngles.x, foot.rotation.eulerAngles.y, footRotation.eulerAngles.z);
            return footRotation;
        }

        /// <summary>
        /// Get foot position by hit point considering foot offset.
        /// </summary>
        public Vector3 GetFootPosition(Vector3 point)
        {
            point.y += footOffset;
            return point;
        }

        /// <summary>
        /// Get foot origin considering state based leg distance.
        /// </summary>
        public Vector3 GetFootOrigin(Vector3 footPosition, float legDistance)
        {
            Vector3 origin = footPosition + ((legDistance + 0.25f) * Vector3.up);
            return origin;
        }

        /// <summary>
        /// Get foot target considering state based leg distance.
        /// </summary>
        public Vector3 GetFootTarget(Vector3 footPosition, float legDistance)
        {
            Vector3 target = footPosition - ((legDistance / 2f) * Vector3.up);
            return target;
        }

        /// <summary>
        /// Get foot delta.
        /// </summary>
        /// <returns></returns>
        public float GetFootDelta()
        {
            return Mathf.Abs(leftFootY - rightFootY);
        }

        #region [Getter / Setter]
        public Transform GetLeftFoot()
        {
            return leftFoot;
        }

        public void SetLeftFoot(Transform value)
        {
            leftFoot = value;
        }

        public Transform GetRightFoot()
        {
            return rightFoot;
        }

        public void SetRightFoot(Transform value)
        {
            rightFoot = value;
        }

        public LayerMask GetGroundLayer()
        {
            return groundLayer;
        }

        public void SetGroundLayer(LayerMask value)
        {
            groundLayer = value;
        }

        public float GetFootOffset()
        {
            return footOffset;
        }

        public void SetFootOffset(float value)
        {
            footOffset = value;
        }

        public float GetDeltaAmplifier()
        {
            return deltaAmplifier;
        }

        public void SetDeltaAmplifier(float value)
        {
            deltaAmplifier = value;
        }

        public float GetColliderSmooth()
        {
            return colliderSmooth;
        }

        public void SetColliderSmooth(float value)
        {
            colliderSmooth = value;
        }

        public bool ProcessFootRotation()
        {
            return processFootRotation;
        }

        public void ProcessFootRotation(bool value)
        {
            processFootRotation = value;
        }

        public Transform GetLookTarget()
        {
            return lookTarget;
        }

        public void SetLookTarget(Transform value)
        {
            lookTarget = value;
        }

        public float GetLookWeight()
        {
            return weight;
        }

        public void SetLookWeight(float value)
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

        public float GetHandIKSmooth()
        {
            return handIKSmooth;
        }

        public void SetHandIKSmooth(float value)
        {
            handIKSmooth = value;
        }

        public bool IKIsActive()
        {
            return ikIsActive;
        }

        public void IKIsActive(bool value)
        {
            ikIsActive = value;
        }
        #endregion
    }
}