/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.AIModules.BehaviourTree.Attributes;
using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules;
using AuroraFPSRuntime.CoreModules.Coroutines;
using AuroraFPSRuntime.CoreModules.ValueTypes;
using AuroraFPSRuntime.SystemModules.HealthModules;
using System.Collections;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Nodes
{
    [TreeNodeContent("Melee Attack", "Actions/Combat/Melee Attack")]
    [HideScriptField]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    public class MeleeAttackNode : ActionNode
    {
        [SerializeField]
        private string attackPointTransformName = "ATTACK POINT";

        [SerializeField]
        private float damage = 5f;

        [SerializeField]
        private float impactTime = 1f;

        [SerializeField]
        private AnimatorState attackState = "Fire";

        [SerializeField]
        private AudioClip attackSound = null;

        // Stored required components.
        private Animator animator;
        private AudioSource audioSource;
        private Transform attackPoint;

        // Stored required properties.
        private CoroutineObject attackProcessing;
        private bool attacking;
        private bool attacked;

        protected override void OnInitialize()
        {
            animator = owner.GetComponent<Animator>();
            audioSource = owner.GetComponent<AudioSource>();
            attackProcessing = new CoroutineObject(owner);

            attackPoint = FindInChildren(owner.transform, attackPointTransformName);
        }

        protected override State OnUpdate()
        {
            if (!attacking)
            {
                if (!attacked)
                {
                    attackProcessing.Start(Attacking);
                }
                else
                {
                    attacked = false;
                    return State.Success;
                }
            }

            return State.Running;
        }

        private IEnumerator Attacking()
        {
            attacking = true;
            attacked = false;

            audioSource.PlayOneShot(attackSound);
            animator.CrossFadeInFixedTime(attackState);

            yield return new WaitForSeconds(impactTime);

            Collider[] colliders = Physics.OverlapSphere(attackPoint.position, 0.1f);
            for (int i = 0; i < colliders.Length; i++)
            {
                IDamageable damageable = colliders[i].GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damage, new DamageInfo(owner.transform));
                }
            }

            yield return new WaitForSeconds(impactTime);
            attacked = true;
            attacking = false;
        }

        private Transform FindInChildren(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name) return child;
                Transform t = FindInChildren(child, name);
                if (t != null)
                {
                    return t;
                }
            }
            return null;
        }
    }
}