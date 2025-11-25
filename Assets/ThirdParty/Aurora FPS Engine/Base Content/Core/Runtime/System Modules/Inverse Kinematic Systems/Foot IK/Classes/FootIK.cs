/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.ValueTypes;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/IK/Foot IK")]
    [DisallowMultipleComponent]
    public class FootIK : InverseKinematic
    {
        #region [Const Values]
        protected const float DefaultLegDistance = 1.0f;
        #endregion

        [SerializeField]
        [Tooltip("Left foot weight animator controller parameter.")]
        [Order(981)]
        private AnimatorParameter leftFootParameter = "Left Foot IK Weight";

        [SerializeField]
        [Tooltip("Right foot weight animator controller parameter.")]
        [Order(982)]
        private AnimatorParameter rightFootParameter = "Right Foot IK Weight";

        [SerializeField]
        [Foldout("Advanced Settings", Style = "Header")]
        [Tooltip("Vertical foot offset distance.")]
        [Order(991)]
        private float offset = 0.125f;

        [SerializeField]
        [Foldout("Advanced Settings", Style = "Header")]
        [Tooltip("Vertical check ground range distance.")]
        [Order(994)]
        private float range = 1.0f;

        [SerializeField]
        [Foldout("Advanced Settings", Style = "Header")]
        [Tooltip("Ground culling layer.")]
        [Order(995)]
        private LayerMask cullingLayer = 1 << 0;

        // Stored required properties.
        private Transform leftFoot;
        private Transform rightFoot;
        private float leftFootY;
        private float rightFootY;
        private bool hasParameters;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        protected virtual void Start()
        {
            leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);
            hasParameters = HasRequiredParameters();
        }

        /// <summary>
        /// Callback for calculation animation IK.
        /// </summary>
        /// <param name="layerIndex">Index of the layer on which the IK solver is called.</param>
        protected override void OnCalculateIK(int layerIndex)
        {
            RaycastHit hitInfo;
            Vector3 targetPosition = Vector3.zero;
            Quaternion targetRotation = Quaternion.identity;
            float legDistance = GetLegDistance();

            float width = animator.GetFloat(leftFootParameter);
            if (leftFoot != null && Physics.Linecast(GetFootOrigin(leftFoot.position, legDistance), GetFootTarget(leftFoot.position, legDistance), out hitInfo, cullingLayer, QueryTriggerInteraction.Ignore))
            {
                targetPosition = GetFootPosition(hitInfo.point);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, width);
                animator.SetIKPosition(AvatarIKGoal.LeftFoot, targetPosition);

                leftFootY = targetPosition.y;

                targetRotation = GetFootRotation(leftFoot, hitInfo.normal);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, width);
                animator.SetIKRotation(AvatarIKGoal.LeftFoot, targetRotation);
            }

            width = animator.GetFloat(rightFootParameter);
            if (rightFoot != null && Physics.Linecast(GetFootOrigin(rightFoot.position, legDistance), GetFootTarget(rightFoot.position, legDistance), out hitInfo, cullingLayer, QueryTriggerInteraction.Ignore))
            {
                targetPosition = GetFootPosition(hitInfo.point);
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, width);
                animator.SetIKPosition(AvatarIKGoal.RightFoot, targetPosition);

                rightFootY = targetPosition.y;

                targetRotation = GetFootRotation(rightFoot, hitInfo.normal);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, width);
                animator.SetIKRotation(AvatarIKGoal.RightFoot, targetRotation);
            }
        }

        /// <summary>
        /// Checking that animator controller has all required parameters for IK work.
        /// </summary>
        /// <returns></returns>
        protected bool HasRequiredParameters()
        {
            bool hasLeftFootParameter = false;
            bool hasRightFootParameter = false;
            AnimatorControllerParameter[] parameters = animator.parameters;
            for (int i = 0; i < parameters.Length; i++)
            {
                AnimatorControllerParameter parameter = parameters[i];
                if(parameter.name == leftFootParameter)
                {
                    hasLeftFootParameter = true;
                }
                else if(parameter.name == rightFootParameter)
                {
                    hasRightFootParameter = true;
                }
            }
            return hasLeftFootParameter && hasRightFootParameter;
        }

        /// <summary>
        /// Return true if IK is processing, otherwise false.
        /// </summary>
        public override bool IsActive()
        {
            return hasParameters && base.IsActive();
        }

        /// <summary>
        /// Get state based leg distance.
        /// </summary>
        protected virtual float GetLegDistance()
        {
            return DefaultLegDistance;
        }

        /// <summary>
        /// Get foot rotation relative surface normal.
        /// </summary>
        protected Quaternion GetFootRotation(Transform foot, Vector3 normal)
        {
            return Quaternion.LookRotation(Vector3.ProjectOnPlane(foot.forward, normal), normal);
        }

        /// <summary>
        /// Get foot position by hit point considering foot offset.
        /// </summary>
        protected Vector3 GetFootPosition(Vector3 point)
        {
            point.y += offset;
            return point;
        }

        /// <summary>
        /// Get foot origin considering state based leg distance.
        /// </summary>
        protected Vector3 GetFootOrigin(Vector3 footPosition, float legDistance)
        {
            footPosition.y += legDistance;
            return footPosition;
        }

        /// <summary>
        /// Get foot target considering state based leg distance.
        /// </summary>
        protected Vector3 GetFootTarget(Vector3 footPosition, float legDistance)
        {
            footPosition.y -= legDistance;
            return footPosition;
        }

        /// <summary>
        /// Get foot delta.
        /// </summary>
        /// <returns></returns>
        protected float GetFootDelta()
        {
            return Mathf.Abs(leftFootY - rightFootY);
        }

        #region [Getter / Setter]
        public AnimatorParameter GetLeftFootParameter()
        {
            return leftFootParameter;
        }

        public void SetLeftFootParameter(AnimatorParameter value)
        {
            leftFootParameter = value;
        }

        public AnimatorParameter GetRightFootParameter()
        {
            return rightFootParameter;
        }

        public void SetRightFootParameter(AnimatorParameter value)
        {
            rightFootParameter = value;
        }

        public float GetOffset()
        {
            return offset;
        }

        public void SetOffset(float value)
        {
            offset = value;
        }

        public float GetRange()
        {
            return range;
        }

        public void SetRange(float value)
        {
            range = value;
        }

        public LayerMask GetCullingLayer()
        {
            return cullingLayer;
        }

        public void SetCullingLayer(LayerMask value)
        {
            cullingLayer = value;
        }
        #endregion
    }
}
