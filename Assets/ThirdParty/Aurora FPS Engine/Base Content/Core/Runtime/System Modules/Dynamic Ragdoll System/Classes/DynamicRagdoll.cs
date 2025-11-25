/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules;
using AuroraFPSRuntime.CoreModules.ValueTypes;
using System;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules
{
    [HideScriptField]
    [AddComponentMenu(null)]
    [DisallowMultipleComponent]
    public abstract partial class DynamicRagdoll : MonoBehaviour, IDynamicRagdoll
    {
        [SerializeField]
        private bool simulateOnStart = true;

        [SerializeField]
        [Foldout("Animation Settings", Style = "Header")]
        [Tooltip("Get up from belly animation state name from animator controller.")]
        [Order(898)]
        private AnimatorState getUpFromBelly = "FromBelly";

        [SerializeField]
        [Foldout("Animation Settings", Style = "Header")]
        [Tooltip("Get up from back animation state name from animator controller.")]
        [Order(899)]
        private AnimatorState getUpFromBack = "FromBack";

        [SerializeField]
        [Foldout("Advanced Settings", Style = "Header")]
        [Slider(0, 1)]
        [Label("Blend Duration")]
        [Tooltip("Duration time to blending physics ragdoll to animator animation.")]
        [Order(999)]
        private float ragdollToAnimBlendTime = 0.25f;

        // Stored required components.
        private Transform ragdollTransfrom;
        private Animator animator;
        private new Collider collider;
        private Rigidbody[] rigidbodies;
        private Transform hipsTransform;
        private Rigidbody hipsRigidbody;

        // Stored required properties.
        private RagdollState state;
        private BoneTransform[] bonesTransform;
        private Vector3 storedHipsPosition;
        private Vector3 storedHipsPositionPrivAnim;
        private Vector3 storedHipsPositionPrivBlend;
        private float ragdollTime;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            CopyAnimator(out animator);
            CopyRagdollTransform(out ragdollTransfrom);
            CopyRootCollider(out collider);
            CopyBonesTransform(out bonesTransform);
            CopyRigidbodies(out rigidbodies);
            GetHipsRigidbody();
            BonesKinematic(true);
        }

        protected virtual void Start()
        {
            if (simulateOnStart)
                SimulateRagdoll();
        }

        /// <summary>
        /// LateUpdate is called every frame, after all Update functions have been called, 
        /// if the Behaviour is enabled.
        /// </summary>
        protected virtual void LateUpdate()
        {
            if (StateIs(RagdollState.BlendingToAnimation))
            {
                BlendingToAnimationProcessing();
            }
        }

        /// <summary>
        /// Simulate character ragdoll .
        /// </summary>
        public void SimulateRagdoll()
        {
            if (StateIs(RagdollState.Animated))
            {
                OnSimulateRagdollCallback?.Invoke();
                DependentComponents(false);
                Vector3 storedVelocity = animator.velocity;
                BonesKinematic(false);
                AddVelocity(storedVelocity);
                state = RagdollState.Radolled;
                OnBlendCompleteCallback?.Invoke();
            }
        }

        /// <summary>
        /// Stop simulating character ragdoll and start blending to animator component.
        /// </summary>
        public void PlayAnimator()
        {
            ragdollTime = Time.time;
            OnPlayAnimatorCallback?.Invoke();
            DependentComponents(true);
            state = RagdollState.BlendingToAnimation;

            storedHipsPositionPrivAnim = Vector3.zero;
            storedHipsPositionPrivBlend = Vector3.zero;

            storedHipsPosition = hipsTransform.position;

            Vector3 shiftPos = hipsTransform.position - ragdollTransfrom.position;
            shiftPos.y = GetDistanceToFloor(shiftPos.y);

            MoveNodeWithoutChildren(shiftPos);

            for (int i = 0; i < bonesTransform.Length; i++)
            {
                BoneTransform tramsformComponent = bonesTransform[i];
                tramsformComponent.SetStoredRotation(tramsformComponent.GetTransform().localRotation);
                tramsformComponent.SetRotation(tramsformComponent.GetTransform().localRotation);

                tramsformComponent.SetStoredPosition(tramsformComponent.GetTransform().localPosition);
                tramsformComponent.SetPosition(tramsformComponent.GetTransform().localPosition);
                bonesTransform[i] = tramsformComponent;
            }

            if(!string.IsNullOrEmpty(getUpFromBelly) && !string.IsNullOrEmpty(getUpFromBack))
            {
                switch (GetCharacterLieSide())
                {
                    case LieSide.Front:
                        animator.CrossFadeInFixedTime(getUpFromBelly);
                        break;
                    case LieSide.Back:
                        animator.CrossFadeInFixedTime(getUpFromBack);
                        break;
                }
            }
            BonesKinematic(true);
        }

        /// <summary>
        /// Stop simulating character ragdoll and start playing animator component without blending.
        /// </summary>
        public void StartAnimatorForce()
        {
            PlayAnimator();
            ragdollTime = 0;
            animator.PlayInFixedTime(string.Empty, 0);
        }

        /// <summary>
        /// Calculate body direction, when character get up.
        /// </summary>
        /// <returns>Body direction when character get up.</returns>
        protected virtual Vector3 CalculateBodyDirection()
        {
            Vector3 ragdolledFeetPosition = animator.GetBoneTransform(HumanBodyBones.Hips).position;
            Vector3 ragdolledHeadPosition = animator.GetBoneTransform(HumanBodyBones.Head).position;
            Vector3 ragdollDirection = ragdolledFeetPosition - ragdolledHeadPosition;
            ragdollDirection.y = 0;
            ragdollDirection = ragdollDirection.normalized;

            if (GetCharacterLieSide() == LieSide.Front)
                return -ragdollDirection;

            return ragdollDirection;
        }

        /// <summary>
        /// Calculate distance between character and floor.
        /// </summary>
        protected float GetDistanceToFloor(float currentY)
        {
            RaycastHit[] hits = Physics.RaycastAll(new Ray(hipsTransform.position, Vector3.down));
            float distFromFloor = float.MinValue;

            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                if (!hit.transform.IsChildOf(ragdollTransfrom))
                {
                    distFromFloor = Mathf.Max(distFromFloor, hit.point.y);
                }
            }

            if (Mathf.Abs(distFromFloor - float.MinValue) > Mathf.Epsilon)
                currentY = distFromFloor - ragdollTransfrom.position.y;

            return currentY;
        }

        /// <summary>
        /// The side on which the character lies (while ragdolled).
        /// </summary>
        protected LieSide GetCharacterLieSide()
        {
            return Vector3.Dot(hipsTransform.up, Vector3.up) < 0 ? LieSide.Front : LieSide.Back;
        }

        /// <summary>
        /// Compare current state.
        /// </summary>
        protected bool StateIs(RagdollState state)
        {
            return this.state == state;
        }

        /// <summary>
        /// Return true when ragdoll bones stabilized (not moving).
        /// </summary>
        /// <returns></returns>
        protected bool RagdollStabilized()
        {
#if UNITY_6000_0_OR_NEWER
            return hipsRigidbody.linearVelocity.magnitude <= 0.1f;
#else
            return hipsRigidbody.velocity.magnitude <= 0.1f;
#endif
        }

        /// <summary>
        /// Override this method to return animator component of the ragdoll character.
        /// Use GetComponent<Animator>() method.
        /// </summary>
        /// <param name="animator">Animator component of the ragdoll character.</param>
        protected abstract void CopyAnimator(out Animator animator);

        /// <summary>
        /// Override this method to initialize transform component of the ragdoll character.
        /// </summary>
        /// <param name="transform">Transform component of the ragdoll character.</param>
        protected virtual void CopyRagdollTransform(out Transform ragdollTransform)
        {
            ragdollTransform = transform;
        }

        /// <summary>
        /// Override this method to initialize root collider component of the ragdoll character.
        /// </summary>
        /// <param name="collider">Collider component of the ragdoll character.</param>
        protected virtual void CopyRootCollider(out Collider collider)
        {
            collider = GetComponent<Collider>();
        }

        /// <summary>
        /// Copy all rigidbodies of character.
        /// </summary>
        /// <param name="rigidbodies">Rigidbody of the character, in array representation.</param>
        protected void CopyRigidbodies(out Rigidbody[] rigidbodies)
        {
            rigidbodies = ragdollTransfrom.GetComponentsInChildren<Rigidbody>();
        }

        /// <summary>
        /// Get BoneTranforms of character.
        /// </summary>
        /// <param name="bonesTransform">Bones transform of the character, in array representation.</param>
        protected void CopyBonesTransform(out BoneTransform[] bonesTransform)
        {
            Transform[] transforms = ragdollTransfrom.GetComponentsInChildren<Transform>();
            bonesTransform = new BoneTransform[transforms.Length];
            for (int i = 0; i < transforms.Length; i++)
            {
                bonesTransform[i] = new BoneTransform(transforms[i]);
            }
        }

        /// <summary>
        /// Get hips of body.
        /// </summary>
        private void GetHipsRigidbody()
        {
            hipsTransform = animator.GetBoneTransform(HumanBodyBones.Hips);
            hipsRigidbody = hipsTransform.GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Blending processing from ragdoll to animation.
        /// </summary>
        private void BlendingToAnimationProcessing()
        {
            float ragdollBlendAmount = 1f - Mathf.InverseLerp(ragdollTime, ragdollTime + ragdollToAnimBlendTime, Time.time);

            if (storedHipsPositionPrivBlend != hipsTransform.position)
            {
                storedHipsPositionPrivAnim = hipsTransform.position;
            }
            storedHipsPositionPrivBlend = Vector3.Lerp(storedHipsPositionPrivAnim, storedHipsPosition, ragdollBlendAmount);
            hipsTransform.position = storedHipsPositionPrivBlend;

            for (int i = 0; i < bonesTransform.Length; i++)
            {
                BoneTransform transformComponent = bonesTransform[i];
                if (transformComponent.GetPosition() != transformComponent.GetTransform().localPosition)
                {
                    transformComponent.SetPosition(Vector3.Lerp(transformComponent.GetTransform().localPosition, transformComponent.GetStoredPosition(), ragdollBlendAmount));
                    transformComponent.GetTransform().localPosition = transformComponent.GetPosition();
                }

                if (transformComponent.GetRotation() != transformComponent.GetTransform().localRotation)
                {
                    transformComponent.SetRotation(Quaternion.Slerp(transformComponent.GetTransform().localRotation, transformComponent.GetStoredRotation(), ragdollBlendAmount));
                    transformComponent.GetTransform().localRotation = transformComponent.GetRotation();
                }
            }

            if (Mathf.Abs(ragdollBlendAmount) < Mathf.Epsilon)
            {
                state = RagdollState.Animated;
                OnBlendCompleteCallback?.Invoke();
            }
        }

        /// <summary>
        /// Switch enabled state of dependent components.
        /// </summary>
        /// <param name="enabled">Enabled state of components.</param>
        protected virtual void DependentComponents(bool enabled)
        {
            animator.enabled = enabled;
            if (collider != null)
                collider.enabled = enabled;
        }

        /// <summary>
        /// Add velocty to each rigidbody component of the character.
        /// </summary>
        protected void AddVelocity(Vector3 velocity)
        {
            for (int i = 0; i < rigidbodies.Length; i++)
            {
#if UNITY_6000_0_OR_NEWER
                rigidbodies[i].linearVelocity = velocity;
#else
                rigidbodies[i].velocity = velocity;
#endif
            }
        }

        /// <summary>
        /// Switch kinematic enabled state of the each rigidbody component of the character.
        /// </summary>
        /// <param name="enabled">Enabled state of parts.</param>
        protected virtual void BonesKinematic(bool enabled)
        {
            Rigidbody[] rigidbodies = ragdollTransfrom.GetComponentsInChildren<Rigidbody>();
            int i = 0;
            for (i = 0;  i < rigidbodies.Length; i++)
            {
                rigidbodies[i].isKinematic = enabled;
            }
        }

        /// <summary>
        /// Move node without children.
        /// </summary>
        protected void MoveNodeWithoutChildren(Vector3 shiftPos)
        {
            Vector3 ragdollDirection = CalculateBodyDirection();

            hipsTransform.position -= shiftPos;
            ragdollTransfrom.position += shiftPos;

            Vector3 forward = ragdollTransfrom.forward;
            ragdollTransfrom.rotation = Quaternion.FromToRotation(forward, ragdollDirection) * ragdollTransfrom.rotation;
            hipsTransform.rotation = Quaternion.FromToRotation(ragdollDirection, forward) * hipsTransform.rotation;
        }

        #region [Event Callback Function]
        /// <summary>
        /// Called when simulating character ragdoll.
        /// </summary>
        public event Action OnSimulateRagdollCallback;

        /// <summary>
        /// Called when stating play animator.
        /// </summary>
        public event Action OnPlayAnimatorCallback;

        /// <summary>
        /// Called when complete blending.
        /// </summary>
        public event Action OnBlendCompleteCallback;
        #endregion

        #region [Getter / Setter]
        public float GetRagdollToAnimBlendTime()
        {
            return ragdollToAnimBlendTime;
        }

        public void SetRagdollToAnimBlendTime(float value)
        {
            ragdollToAnimBlendTime = value;
        }

        public AnimatorState GetGetUpFromBelly()
        {
            return getUpFromBelly;
        }

        public void SetGetUpFromBelly(AnimatorState value)
        {
            getUpFromBelly = value;
        }

        public AnimatorState GetGetUpFromBack()
        {
            return getUpFromBack;
        }

        public void SetGetUpFromBack(AnimatorState value)
        {
            getUpFromBack = value;
        }

        public Animator GetAnimator()
        {
            return animator;
        }
        #endregion
    }
}