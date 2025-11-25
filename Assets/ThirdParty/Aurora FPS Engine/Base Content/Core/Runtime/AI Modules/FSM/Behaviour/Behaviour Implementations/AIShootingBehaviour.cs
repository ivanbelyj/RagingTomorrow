/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.AIModules.CoverSystem;
using AuroraFPSRuntime.CoreModules;
using AuroraFPSRuntime.CoreModules.Coroutines;
using AuroraFPSRuntime.CoreModules.ValueTypes;
using AuroraFPSRuntime.AIModules.Vision;
using AuroraFPSRuntime.WeaponModules;
using System.Collections;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.Behaviour
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(FieldOfView))]
    public abstract class AIShootingBehaviour : AIBehaviour
    {
        public enum CoverMapInitialization
        {
            ByName,
            Manual
        }

        public enum CoverBehaviour
        {
            AnyAvailable,
            Nearest,
            Distant
        }

        [SerializeField]
        [Foldout("Vision Settings")]
        [Order(1)]
        private TargetSelection targetSelection = TargetSelection.First;

        [SerializeField]
        [Foldout("Vision Settings")]
        [Order(2)]
        private FieldOfView fieldOfView;

        // Shooting properties.
        [SerializeField]
        [Foldout("Shoot Settings")]
        [MinValue(0)]
        [Order(100)]
        private int rpm = 850;

        [SerializeField]
        [Foldout("Shoot Settings")]
        [MinValue(0)]
        [Order(100)]
        private int queue = 7;

        [SerializeField]
        [Label("Randomize")]
        [Foldout("Shoot Settings")]
        [VisibleIf("queue", ">", "0")]
        [MinValue(0.0f)]
        [Indent(2)]
        [Order(100)]
        private bool randomizeQueueDelay = false;

        [SerializeField]
        [Label("Delay")]
        [Foldout("Shoot Settings")]
        [VisibleIf("queue", ">", "0")]
        [VisibleIf("randomizeQueueDelay", false)]
        [Suffix("sec", true)]
        [MinValue(0.0f)]
        [Indent(2)]
        [Order(100)]
        private float queueDelay = 1.25f;

        [SerializeField]
        [Label("Delay")]
        [MinMaxSlider(0.0f, 10.0f)]
        [Foldout("Shoot Settings")]
        [VisibleIf("queue", ">", "0")]
        [VisibleIf("randomizeQueueDelay", true)]
        [Suffix("sec", true)]
        [MinValue(0.0f)]
        [Indent(2)]
        [Order(100)]
        private Vector2 queueDelayRange = new Vector2(0, 3.5f);

        [SerializeField]
        [Foldout("Shoot Settings")]
        [Order(101)]
        private AudioClip fireSound;

        [SerializeField]
        [Foldout("Shoot Settings")]
        [Order(102)]
        private ParticleSystem fireEffect;

        // Reload properties.
        [SerializeField]
        [Foldout("Reload Settings")]
        [Order(150)]
        private int bulletCount = 31;

        [SerializeField]
        [Foldout("Reload Settings")]
        [Order(151)]
        private float reloadTime = 2.0f;

        [SerializeField]
        [Foldout("Reload Settings")]
        [Order(152)]
        private AudioClip reloadSound;

        // Animator properties.
        [SerializeField]
        [Foldout("Animation Settings")]
        [Order(200)]
        private AnimatorState fireState = "Fire";

        [SerializeField]
        [Foldout("Animation Settings")]
        [Order(201)]
        private AnimatorState reloadState = "Reload";

        // Searching properties.
        [SerializeField]
        [Foldout("Search Settings")]
        [MinValue(0.0f)]
        [Order(250)]
        private float delayTime = 5.0f;

        [SerializeField]
        [Foldout("Search Settings")]
        [MinValue(0)]
        [Order(251)]
        private int searchSteps = 2;

        [SerializeField]
        [Foldout("Search Settings")]
        [MinValue(0.1f)]
        [Order(252)]
        private float searchStepRadius = 5.5f;

        // Cover properties.
        [SerializeField]
        [Foldout("Cover Settings")]
        [Order(300)]
        private CoverBehaviour coverBehaviour = CoverBehaviour.AnyAvailable;

        [SerializeField]
        [Foldout("Cover Settings")]
        [Order(301)]
        private CoverMapInitialization coverMapInitialization = CoverMapInitialization.Manual;

        [SerializeField]
        [Foldout("Cover Settings")]
        [VisibleIf("coverMapInitialization", "ByName")]
        [Order(302)]
        private string coverMapName = AIIdleBehaviour.IDLE_BEHAVIOUR;

        [SerializeField]
        [Foldout("Cover Settings")]
        [VisibleIf("coverMapInitialization", "Manual")]
        [NotNull]
        [Order(303)]
        private CoverMap coverMap;

        [SerializeField]
        [Foldout("Audio Settings")]
        [NotNull]
        [Order(350)]
        private AudioSource audioSource;

        [SerializeField]
        [CustomView(ViewInitialization = "OnTargetBehaviourInitialization", ViewGUI = "OnTargetBehaviourGUI")]
        [Foldout("Default Transition")]
        [Message("Execute transition to target behaviour, when AI lost target and all search steps is over.\nOtherwise loop searching processing and wait for other custom transition.", MessageStyle.Info)]
        [Order(500)]
        private string targetBehaviour;

        // Stored required components.
        private Animator animator;

        // Stored required properties.
        private int currentBulletCount;
        private Vector3 lastTargetPosition;
        private Transform target;
        private Transform previousTarget;
        private Collider targetCollider;
        private CoverPoint coverPoint;
        private CoroutineObject shootingProcessing;
        private CoroutineObject coverProcessing;
        private CoroutineObject searchProcessing;

        /// <summary>
        /// Called when AIController owner instance is being loaded.
        /// </summary>
        protected override void OnInitialize()
        {
            animator = owner.GetComponent<Animator>();
            shootingProcessing = new CoroutineObject(owner);
            coverProcessing = new CoroutineObject(owner);
            searchProcessing = new CoroutineObject(owner);
            currentBulletCount = bulletCount;
        }

        /// <summary>
        /// Called when this behaviour becomes enabled.
        /// </summary>
        protected override void OnEnable()
        {
            if(coverMap == null && coverMapInitialization == CoverMapInitialization.ByName)
            {
                GameObject coverMapObject = GameObject.Find(coverMapName);
                if(coverMapName != null)
                {
                    coverMap = coverMapObject.GetComponent<CoverMap>();
                }
            }

            shootingProcessing.Start(ShootingProcessing);
            coverProcessing.Start(CoverProcessing);
        }

        protected override void Update()
        {
            UpdateTarget();
            if(target == null)
            {
                searchProcessing.Start(SearchProcessing);
            }
            else
            {
                LookAtTarget();
                SaveTargetPosition();
            }
        }

        /// <summary>
        /// Called when this behaviour becomes disabled.
        /// </summary>
        protected override void OnDisable()
        {
            shootingProcessing.Stop();
            coverProcessing.Stop();
            if(audioSource.clip == reloadSound)
            {
                audioSource.Stop();
            }
            owner.UpdateOrientation(true);
        }

        private IEnumerator ShootingProcessing()
        {
            WaitForSeconds fireDelay = WeaponShootingSystem.RPMToInstruction(rpm);
            WaitForSeconds reloadDelay = new WaitForSeconds(reloadTime);
            WaitForSeconds queueDelay = new WaitForSeconds(this.queueDelay);
            int storedQueue = 0;
            while (true)
            {
                if (currentBulletCount == 0)
                {
                    animator.CrossFadeInFixedTime(reloadState);
                    audioSource.clip = reloadSound;
                    audioSource.Play();
                    yield return reloadDelay;
                    currentBulletCount = bulletCount;
                }

                if (target != null)
                {
                    MakeShoot();
                    animator.CrossFadeInFixedTime(fireState);
                    audioSource.clip = fireSound;
                    audioSource.Play();
                    fireEffect.Play();
                    currentBulletCount--;

                    if(queue > 0 && ++storedQueue >= queue)
                    {
                        storedQueue = 0;
                        if (randomizeQueueDelay)
                        {
                            queueDelay = new WaitForSeconds(Random.Range(queueDelayRange.x, queueDelayRange.y));
                        }
                        yield return queueDelay;
                    }
                    else
                    {
                        yield return fireDelay;
                    }
                }
                else
                {
                    yield return null;
                }
            }
        }

        private IEnumerator CoverProcessing()
        {
            WaitForSeconds delay = new WaitForSeconds(0.25f);
            while (true)
            {
                if (target != null)
                {
                    owner.UpdateOrientation(false);
                    if (coverMap != null)
                    {
                        if (!coverPoint?.IsCover(target) ?? true)
                        {
                            switch (coverBehaviour)
                            {
                                case CoverBehaviour.AnyAvailable:
                                    {
                                        if (coverMap.TryGetFirstAvailablePoint(owner.transform, target, out CoverPoint coverPoint))
                                        {
                                            if (this.coverPoint != null && this.coverPoint != coverPoint)
                                            {
                                                this.coverPoint.IsOccupied(false);
                                            }

                                            this.coverPoint = coverPoint;
                                            this.coverPoint.IsOccupied(true);
                                        }
                                    }
                                    break;
                                case CoverBehaviour.Nearest:
                                    {
                                        if (coverMap.TryGetNearestPoint(owner.transform, target, out CoverPoint coverPoint))
                                        {
                                            if (this.coverPoint != null && this.coverPoint != coverPoint)
                                            {
                                                this.coverPoint.IsOccupied(false);
                                            }

                                            this.coverPoint = coverPoint;
                                            this.coverPoint.IsOccupied(true);
                                        }
                                    }
                                    break;
                                case CoverBehaviour.Distant:
                                    {
                                        if (coverMap.TryGetDistantPoint(owner.transform, target, out CoverPoint coverPoint))
                                        {
                                            if (this.coverPoint != null && this.coverPoint != coverPoint)
                                            {
                                                this.coverPoint.IsOccupied(false);
                                            }

                                            this.coverPoint = coverPoint;
                                            this.coverPoint.IsOccupied(true);
                                        }
                                    }
                                    break;
                            }
                        }
                        if (coverPoint != null)
                        {
                            owner.SetDestination(coverPoint.GetPoint().position);
                        }
                    }
                    else
                    {
                        owner.SetDestination(target.forward * 2);
                    }
                }
                yield return delay;
            }
        }

        private IEnumerator SearchProcessing()
        {
            yield return new WaitForSeconds(delayTime);

            if (target == null)
            {
                shootingProcessing.Stop();
                coverProcessing.Stop();
                coverPoint = null;

                owner.SetDestination(lastTargetPosition);

                int stepCount = searchSteps;
                while (stepCount > 0)
                {
                    if (target != null)
                    {
                        shootingProcessing.Start(ShootingProcessing);
                        coverProcessing.Start(CoverProcessing);
                        yield break;
                    }

                    if (owner.IsReachDestination())
                    {
                        Vector2 randomPosition = Random.insideUnitCircle * searchStepRadius;
                        lastTargetPosition += new Vector3(randomPosition.x, 0.0f, randomPosition.y);
                        stepCount--;
                    }

                    owner.SetDestination(lastTargetPosition);
                    yield return null;
                }
            }

            if (target == null)
            {
                if (!string.IsNullOrEmpty(targetBehaviour))
                {
                    owner.SwitchBehaviour(targetBehaviour);
                }
            }
        }

        protected void UpdateTarget()
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

            if(target != previousTarget)
            {
                if(target != null)
                {
                    targetCollider = target.GetComponent<Collider>();
                }
                else
                {
                    targetCollider = null;
                }
                previousTarget = target;
            }
        }

        protected void SaveTargetPosition()
        {
            lastTargetPosition = target.position;
        }

        protected void LookAtTarget()
        {
            Vector3 lookPos = target.position - owner.transform.position;
            lookPos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, rotation, Time.deltaTime * 5);
        }

        /// <summary>
        /// Implement this method to make weapon shooting.
        /// </summary>
        protected abstract void MakeShoot();

        /// <summary>
        /// Get current target enemy.
        /// </summary>
        /// <returns>Target transform reference of enemy.</returns>
        protected Transform GetTarget()
        {
            return target;
        }

        /// <summary>
        /// Get current target collider.
        /// </summary>
        /// <returns>Collider component reference of target.</returns>
        protected Collider GetTargetCollider()
        {
            return targetCollider;
        }
    }
}