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
using AuroraFPSRuntime.CoreModules.ValueTypes;
using AuroraFPSRuntime.SystemModules.HealthModules;
using AuroraFPSRuntime.CoreModules;
using AuroraFPSRuntime.CoreModules.Mathematics;
using AuroraFPSRuntime.AIModules.Vision;
using System.Collections;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.Behaviour
{
    [AIBehaviourMenu("Melee Combat", "Combat/Melee Combat")]
    [RequireComponent(typeof(Animator))]
    public sealed class AIMeleeCombatBehaviour : AIBehaviour
    {
        public enum AttackState
        {
            Sequental,
            Random
        }

        [System.Serializable]
        internal struct AttackStateProperties
        {
            public AnimatorState state;

            [MinValue(0.01f)]
            public float time;

            [Slider(0.0f, "time")]
            public float damageTime;

            public int damageAmount;
        }

        [SerializeField]
        [Foldout("Target Settings")]
        private TargetSelection targetSelection = TargetSelection.Nearest;

        [SerializeField]
        [NotNull]
        private FieldOfView fieldOfView;

        [SerializeField]
        [Foldout("Combat Settings")]
        private AttackState attackState = AttackState.Random;

        [SerializeField]
        [Label("Distance Tolerance")]
        [Foldout("Combat Settings")]
        [MinValue(0.0f)]
        private float attackDistance = 1.25f;

        [SerializeField]
        [Foldout("Combat Settings")]
        [ReorderableList]
        private AttackStateProperties[] attackStateProperties;

        // Stored required components.
        private Animator animator;
        private Transform transform;
        private Transform target;

        // Stored required properties.
        private int currentAttackStateIndex;
        private float[] attackTimes;
        private CoroutineObject attackCoroutine;

        /// <summary>
        /// Called when AIController owner instance is being loaded.
        /// </summary>
        protected override void OnInitialize()
        {
            animator = owner.GetComponent<Animator>();
            transform = owner.transform;
            attackCoroutine = new CoroutineObject(owner);
        }

        /// <summary>
        /// Called when this behaviour becomes enabled.
        /// </summary>
        protected override void OnEnable()
        {
            attackCoroutine.Start(AttackProcessing);
        }

        /// <summary>
        /// Called every frame, while this behaviour is running.
        /// </summary>
        protected override void Update()
        {
            switch (targetSelection)
            {
                case TargetSelection.First:
                    target = fieldOfView.GetFirstTarget();
                    break;
                case TargetSelection.Nearest:
                    target = fieldOfView.GetNearestTarget();
                    break;
                case TargetSelection.Distant:
                    target = fieldOfView.GetDistantTarget();
                    break;
            }

            if(target != null)
            {
                LookAtTarget();
            }
        }

        /// <summary>
        /// Called when this behaviour becomes disabled.
        /// </summary>
        protected override void OnDisable()
        {
            attackCoroutine.Stop();
        }

        private void LookAtTarget()
        {
            Vector3 lookPos = target.position - transform.position;
            lookPos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5);
        }

        private IEnumerator AttackProcessing()
        {
            while (true)
            {
                if (target != null)
                {
                    if (owner.IsReachDestination() && Math.Distance2D(transform.position, target.position) <= attackDistance)
                    {
                        owner.IsMoving(false);
                        currentAttackStateIndex = FetchAttackStateIndex(currentAttackStateIndex);
                        AttackStateProperties attackStateProperty = attackStateProperties[currentAttackStateIndex];
                        animator.CrossFadeInFixedTime(attackStateProperty.state);
                        yield return new WaitForSeconds(attackStateProperty.damageTime);

                        if (Math.Distance2D(transform.position, target.position) <= attackDistance)
                        {
                            IDamageable damageable = target.GetComponent<IDamageable>();
                            if (damageable != null)
                            {
                                damageable.TakeDamage(attackStateProperty.damageAmount, new DamageInfo(transform));
                            }
                        }
                        yield return new WaitForSeconds(attackStateProperty.time - attackStateProperty.damageTime);
                        owner.IsMoving(true);
                    }
                }
                yield return null;
            }
        }

        private int FetchAttackStateIndex(int currentIndex)
        {
            switch (attackState)
            {
                case AttackState.Sequental:
                    currentIndex = Math.Loop(currentIndex + 1, 0, attackStateProperties.Length - 1);
                    break;
                case AttackState.Random:
                    currentIndex = Random.Range(0, attackStateProperties.Length);
                    break;
            }
            return currentIndex;
        }
    }
}