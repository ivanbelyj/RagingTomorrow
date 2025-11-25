/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov, Deryabin Vladimir
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.ControllerSystems
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Controller/Rigidbody Controller/First Person Controller")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerRigidbodyController : PlayerController
    {
        // Stored required components.
        private new Rigidbody rigidbody;
        private CapsuleCollider capsuleCollider;

        // Stored physics materials.
#if UNITY_6000_0_OR_NEWER
        private PhysicsMaterial idlePhysicMaterial;
        private PhysicsMaterial movePhysicMaterial;
        private PhysicsMaterial airPhysicMaterial;
#else
        private PhysicMaterial idlePhysicMaterial;
        private PhysicMaterial movePhysicMaterial;
        private PhysicMaterial airPhysicMaterial;
#endif

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            rigidbody = GetComponent<Rigidbody>();
            capsuleCollider = GetComponent<CapsuleCollider>();
            CopyIdlePhysicMaterial(out idlePhysicMaterial);
            CopyMovePhysicMaterial(out movePhysicMaterial);
            CopyAirPhysicMaterial(out airPhysicMaterial);
        }

        /// <summary>
        /// Called every fixed frame-rate frame.
        /// <br>0.02 seconds (50 calls per second) is the default time between calls.</br>
        /// </summary>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            UpdatePhysicMaterial();
        }

        /// <summary>
        /// Applying movement vector to controller.
        /// </summary>
        /// <param name="velocity">Movement velocity.</param>
        protected override void Move(Vector3 velocity)
        {
#if UNITY_6000_0_OR_NEWER
            rigidbody.linearVelocity = velocity;
#else
            rigidbody.velocity = velocity;
#endif
        }

        protected override void SnapToGround(ref Vector3 movementVector)
        {
            Vector2 input = GetControlInput();
            if (input != Vector2.zero)
            {
                RaycastHit groundHitInfo = GetGroundHitInfo();
                CopyBounds(out Vector3 center, out float height);
                Vector3 origin = transform.TransformPoint(center) + (transform.forward * capsuleCollider.radius);
                if (Physics.Raycast(origin, -transform.up, out RaycastHit hitInfo, height, GetGroundCullingLayer()))
                {
                    if (hitInfo.point.y <= groundHitInfo.point.y)
                    {
                        base.SnapToGround(ref movementVector);
                    }
                }
            }
        }

        /// <summary>
        /// The velocity vector of the controller. 
        /// <br>It represents the rate of change of controller position.</br>
        /// </summary>
        public override Vector3 GetVelocity()
        {
#if UNITY_6000_0_OR_NEWER
            return rigidbody.linearVelocity;
#else
            return rigidbody.velocity;
#endif
        }

        /// <summary>
        /// Copy controller collider bounds.
        /// </summary>
        /// <param name="center">Controller collider center.</param>
        /// <param name="height">Controller collider height.</param>
        public sealed override void CopyBounds(out Vector3 center, out float height)
        {
            center = capsuleCollider.center;
            height = capsuleCollider.height;
        }

        /// <summary>
        /// Edit current controller collider bounds.
        /// </summary>
        /// <param name="center">Controller collider center.</param>
        /// <param name="height">Controller collider height.</param>
        public sealed override void EditBounds(Vector3 center, float height)
        {
            capsuleCollider.center = center;
            capsuleCollider.height = height;
        }

        /// <summary>
        /// Controller movement update order.
        /// </summary>
        protected sealed override UpdateOrder GetUpdateOrder()
        {
            return UpdateOrder.FixedUpdate;
        }

        /// <summary>
        /// Called every fixed frame-rate frame, 
        /// for switching capsule collider physic material relative controller state.
        /// </summary>
        protected virtual void UpdatePhysicMaterial()
        {
            if (IsGrounded() && GetControlInput() == Vector2.zero)
                capsuleCollider.material = idlePhysicMaterial;
            else if (IsGrounded() && GetControlInput() != Vector2.zero)
                capsuleCollider.material = movePhysicMaterial;
            else
                capsuleCollider.material = airPhysicMaterial;
        }

        /// <summary>
        /// Used while controller is idle.
        /// </summary>
#if UNITY_6000_0_OR_NEWER
        protected virtual void CopyMovePhysicMaterial(out PhysicsMaterial physicMaterial)
        {
            physicMaterial = new PhysicsMaterial();
            physicMaterial.name = "Move Physic Material";
            physicMaterial.staticFriction = .25f;
            physicMaterial.dynamicFriction = .25f;
            physicMaterial.bounciness = 0.0f;
            physicMaterial.frictionCombine = PhysicsMaterialCombine.Multiply;
            physicMaterial.bounceCombine = PhysicsMaterialCombine.Minimum;
        }
#else
        protected virtual void CopyMovePhysicMaterial(out PhysicMaterial physicMaterial)
        {
            physicMaterial = new PhysicMaterial();
            physicMaterial.name = "Move Physic Material";
            physicMaterial.staticFriction = .25f;
            physicMaterial.dynamicFriction = .25f;
            physicMaterial.bounciness = 0.0f;
            physicMaterial.frictionCombine = PhysicMaterialCombine.Multiply;
            physicMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
        }
#endif

        /// <summary>
        /// Used while controller is moving.
        /// </summary>
#if UNITY_6000_0_OR_NEWER
        protected virtual void CopyIdlePhysicMaterial(out PhysicsMaterial physicMaterial)
        {
            physicMaterial = new PhysicsMaterial();
            physicMaterial.name = "Idle Physic Material";
            physicMaterial.staticFriction = 1f;
            physicMaterial.dynamicFriction = 1f;
            physicMaterial.bounciness = 0.0f;
            physicMaterial.frictionCombine = PhysicsMaterialCombine.Maximum;
            physicMaterial.bounceCombine = PhysicsMaterialCombine.Minimum;
        }
#else
        protected virtual void CopyIdlePhysicMaterial(out PhysicMaterial physicMaterial)
        {
            physicMaterial = new PhysicMaterial();
            physicMaterial.name = "Idle Physic Material";
            physicMaterial.staticFriction = 1f;
            physicMaterial.dynamicFriction = 1f;
            physicMaterial.bounciness = 0.0f;
            physicMaterial.frictionCombine = PhysicMaterialCombine.Maximum;
            physicMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
        }
#endif

        /// <summary>
        /// Used while controller is in air.
        /// </summary>
#if UNITY_6000_0_OR_NEWER
        protected virtual void CopyAirPhysicMaterial(out PhysicsMaterial physicMaterial)
        {
            physicMaterial = new PhysicsMaterial();
            physicMaterial.name = "Air Physic Material";
            physicMaterial.staticFriction = 0f;
            physicMaterial.dynamicFriction = 0f;
            physicMaterial.bounciness = 0.0f;
            physicMaterial.frictionCombine = PhysicsMaterialCombine.Minimum;
            physicMaterial.bounceCombine = PhysicsMaterialCombine.Minimum;
        }
#else
        protected virtual void CopyAirPhysicMaterial(out PhysicMaterial physicMaterial)
        {
            physicMaterial = new PhysicMaterial();
            physicMaterial.name = "Air Physic Material";
            physicMaterial.staticFriction = 0f;
            physicMaterial.dynamicFriction = 0f;
            physicMaterial.bounciness = 0.0f;
            physicMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
            physicMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
        }
#endif

        #region [Getter / Setter]
        public Rigidbody GetRigidbody()
        {
            return rigidbody;
        }

        public CapsuleCollider GetCapsuleCollider()
        {
            return capsuleCollider;
        }
        #endregion
    }
}
