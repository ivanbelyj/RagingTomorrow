/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.AIModules.BehaviourTree.Attributes;
using AuroraFPSRuntime.AIModules.BehaviourTree.Variables;
using AuroraFPSRuntime.CoreModules;
using AuroraFPSRuntime.CoreModules.Coroutines;
using AuroraFPSRuntime.CoreModules.ValueTypes;
using AuroraFPSRuntime.WeaponModules;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Nodes
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    public abstract class ShootNode : ActionNode
    {
        [SerializeField]
        private string firePointName = "FIRE POINT";

        [SerializeField]
        protected float damage = 5f;

        [SerializeField]
        private AnimatorState attackState = "Fire";

        [SerializeField]
        private int rpm = 800;

        [SerializeField]
        private int clipCapacity = 30;

        [SerializeField]
        private AnimatorState reloadState = "Reload";

        [SerializeField]
        private float reloadTime = 3f;

        [SerializeField]
        private AudioClip fireSound = null;

        [SerializeField]
        private AudioClip reloadSound = null;

        [SerializeField]
        private string fireEffectName = null;

        [SerializeField]
        [TreeVariable(typeof(Transform))]
        private string targetVariable;

        #region [Variables Toggle]
#if UNITY_EDITOR
        //[SerializeField]
        //[HideInInspector]
        //private bool rpmToggle;
#endif
        #endregion

        // Stored required components.
        private Animator animator;
        private AudioSource audioSource;
        protected Transform firePoint;
        protected ParticleSystem fireEffect;

        // Stored required properties.
        private CoroutineObject reloadingProcessing;

        private Transform target = null;
        private float fireDelay;
        private float lastShootTime;

        private int bulletCount;

        protected override void OnInitialize()
        {
            animator = owner.GetComponent<Animator>();
            audioSource = owner.GetComponent<AudioSource>();

            reloadingProcessing = new CoroutineObject(owner);

            firePoint = owner.GetComponentsInChildren<Transform>().Where(t => t.name == firePointName).FirstOrDefault();

            Transform fireEffectTransform = owner.GetComponentsInChildren<Transform>().Where(t => t.name == fireEffectName).FirstOrDefault();
            if (fireEffectTransform != null)
            {
                fireEffect = fireEffectTransform.GetComponent<ParticleSystem>();
            }
            bulletCount = clipCapacity;
        }

        protected override void OnEntry()
        {
            if (tree.TryGetVariable<TransformVariable>(targetVariable, out TransformVariable targetVar))
            {
                target = targetVar;
            }

            fireDelay = WeaponShootingSystem.RPMToDelay(rpm);
        }

        protected override State OnUpdate()
        {
            if (target != null)
            {
                LookAtTarget();
                if (bulletCount > 0 && lastShootTime + fireDelay <= Time.time)
                {
                    MakeShoot();
                    bulletCount--;
                    if (fireEffect != null)
                    {
                        fireEffect.Play();
                    }
                    if (fireSound != null && audioSource != null)
                    {
                        audioSource.PlayOneShot(fireSound);
                    }

                    if(animator != null)
                    {
                        animator?.CrossFadeInFixedTime(attackState);
                    }

                    lastShootTime = Time.time;
                    return State.Success;
                }
                else if (bulletCount <= 0)
                {
                    reloadingProcessing.Start(ReloadingProcessing);
                }
            }

            return State.Failure;
        }

        private IEnumerator ReloadingProcessing()
        {
            if (reloadSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(reloadSound);
            }

            if (animator != null)
            {
                animator.CrossFadeInFixedTime(reloadState);
            }
            yield return new WaitForSeconds(reloadTime);
            bulletCount = clipCapacity;
        }

        protected void LookAtTarget()
        {
            Vector3 lookPos = target.position - owner.transform.position;
            lookPos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, rotation, Time.deltaTime * 5);
        }
        
        protected abstract void MakeShoot();

        protected Collider GetTargetCollider()
        {
            return target.GetComponent<Collider>();
        }
    }
}