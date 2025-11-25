/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov, Deryabin Vladimir
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using System;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.ControllerSystems
{
    [HideScriptField]
    [AddComponentMenu(null)]
    [DisallowMultipleComponent]
    public abstract class Controller : MonoBehaviour, IControllerVelocity, IControllerGrounded, IControllerBounds
    {
        [SerializeField]
        [Label("Limit")]
        [Slider(0.0f, 90.0f)]
        [Foldout("Slope Settings", Style = "Header")]
        [Order(950)]
        private float slopeLimit = 60.0f;

        [SerializeField]
        [Label("Radius")]
        [Foldout("Grounded Settings", Style = "Header")]
        [MinValue(0.01f)]
        [Order(991)]
        private float groundCheckRadius = 0.1f;

        [SerializeField]
        [Label("Range")]
        [Foldout("Grounded Settings", Style = "Header")]
        [MinValue(0.01f)]
        [Order(992)]
        private float groundCheckRange = 0.1f;

        [SerializeField]
        [Label("Culling Layer")]
        [Foldout("Grounded Settings", Style = "Header")]
        [Order(993)]
        private LayerMask groundCullingLayer = 1 << 0;

        // Stored required properties.
        private bool isGrounded;
        private bool onSlope;
        private bool previouslyGrounded = true;
        private RaycastHit groundHitInfo;

        /// <summary>
        /// Called every frame, while the controller is enabled.
        /// </summary>
        protected virtual void Update()
        {
            isGrounded = CalculateGrounded(out groundHitInfo);
            if (isGrounded)
            {
                onSlope = CalculateSlope(groundHitInfo, out float angle);
                if(angle > slopeLimit)
                {
                    isGrounded = false;
                }
            }
        }

        /// <summary>
        /// Called after all Update functions have been called, while the controller is enabled.
        /// </summary>
        protected virtual void LateUpdate()
        {
            GroundedCallbackHandler();
        }

        /// <summary>
        /// Whether the controller is currently on the ground.
        /// </summary>
        /// <param name="hitInfo">Ground hit info.</param>
        protected virtual bool CalculateGrounded(out RaycastHit hitInfo)
        {
            CopyBounds(out Vector3 center, out float height);

            float radius = groundCheckRadius * 0.9f;
            float distance = 10;
            float halfHeight = height / 2;

            Ray ray = new Ray(transform.position + new Vector3(0, halfHeight, 0), -transform.up);
            if (Physics.Raycast(ray, out hitInfo, halfHeight + groundCheckRange, GetGroundCullingLayer()))
            {
                distance = transform.position.y - hitInfo.point.y;
            }

            Vector3 origin = transform.position + transform.up * (groundCheckRadius);
            ray = new Ray(origin, -transform.up);
            if (Physics.SphereCast(ray, radius, out hitInfo, groundCheckRadius + 2f, GetGroundCullingLayer()))
            {
                if (distance > (hitInfo.distance - groundCheckRadius * 0.1f))
                {
                    distance = (hitInfo.distance - groundCheckRadius * 0.1f);
                }
            }

            return (float)Math.Round(distance, 2) <= 0.05f;
        }

        /// <summary>
        /// Check that controller is on slope.
        /// </summary>
        /// <param name="hitInfo">Raycast hit info of ground.</param>
        /// <param name="angle">Out reference value on slope angle.</param>
        /// <returns></returns>
        protected virtual bool CalculateSlope(RaycastHit hitInfo, out float angle)
        {
            angle = Vector3.Angle(transform.up, hitInfo.normal);
            return angle > 0;
        }

        /// <summary>
        /// Is the controller currently on the slope?
        /// </summary>
        public bool OnSlope()
        {
            return onSlope;
        }

        /// <summary>
        /// Projects a vector onto a ground defined by a hit normal orthogonal to the ground.
        /// </summary>
        /// <param name="vector">Desired direction vector.</param>
        /// <returns>The location of the vector on the plane.</returns>
        public Vector3 FlattenSlopes(Vector3 vector)
        {
            if (isGrounded)
            {
                return Vector3.ProjectOnPlane(vector, groundHitInfo.normal);
            }
            return vector;
        }

        /// <summary>
        /// OnGroundedCallback handler.
        /// </summary>
        protected void GroundedCallbackHandler()
        {
            if (!previouslyGrounded && isGrounded)
            {
                OnGroundedCallback?.Invoke();
            }
            else if (previouslyGrounded && !isGrounded)
            {
                OnBecomeAirCallback?.Invoke();
            }
            previouslyGrounded = isGrounded;
        }

        #region [IControllerVelocity Implementation]
        /// <summary>
        /// The velocity vector of the controller. 
        /// <br>It represents the rate of change of controller position.</br>
        /// </summary>
        public abstract Vector3 GetVelocity();
        #endregion

        #region [IControllerBounds Implementation]
        /// <summary>
        /// Copy controller collider bounds.
        /// </summary>
        /// <param name="center">Controller collider center.</param>
        /// <param name="height">Controller collider height.</param>
        public abstract void CopyBounds(out Vector3 center, out float height);

        /// <summary>
        /// Edit current controller collider bounds.
        /// </summary>
        /// <param name="center">Controller collider center.</param>
        /// <param name="height">Controller collider height.</param>
        public abstract void EditBounds(Vector3 center, float height);
        #endregion

        #region [IControllerGrounded Implementation]
        /// <summary>
        /// Controller is grounded at the moment.
        /// </summary>
        /// <returns>Grounded state.</returns>
        public bool IsGrounded()
        {
            return isGrounded;
        }
        #endregion

        #region [Event Callback Functions]
        /// <summary>
        /// Called when the controller lifts off the ground.
        /// </summary>
        public event Action OnBecomeAirCallback;

        /// <summary>
        /// Called when the controller lands on the ground.
        /// </summary>
        public event Action OnGroundedCallback;

        #endregion

        #region [Getter / Setter]
        public float GetGroundCheckRadius()
        {
            return groundCheckRadius;
        }

        public void SetGroundCheckRadius(float value)
        {
            groundCheckRadius = value;
        }

        public float GetGroundCheckRange()
        {
            return groundCheckRange;
        }

        public void SetGroundCheckRange(float value)
        {
            groundCheckRange = value;
        }

        public LayerMask GetGroundCullingLayer()
        {
            return groundCullingLayer;
        }

        public void SetGroundCullingLayer(LayerMask value)
        {
            groundCullingLayer = value;
        }

        public RaycastHit GetGroundHitInfo()
        {
            return groundHitInfo;
        }

        public bool IsPreviouslyGrounded()
        {
            return previouslyGrounded;
        }
        #endregion
    }
}

