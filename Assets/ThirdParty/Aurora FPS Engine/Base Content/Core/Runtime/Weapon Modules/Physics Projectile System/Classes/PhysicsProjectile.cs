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
using AuroraFPSRuntime.CoreModules.Mathematics;
using AuroraFPSRuntime.SystemModules.ControllerModules;
using AuroraFPSRuntime.CoreModules.Coroutines;
using System.Collections;
using UnityEngine;
using AuroraFPSRuntime.SystemModules;
using AuroraFPSRuntime.SystemModules.ControllerSystems;

namespace AuroraFPSRuntime.WeaponModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/Weapon Modules/Physics Shell/Physics Projectile")]
    [RequireComponent(typeof(AudioSource))]
    public class PhysicsProjectile : PhysicsBullet
    {
        [SerializeField]
        private GameObject projectileMesh;

        [SerializeField]
        private ParticleSystem explosionEffect;

        [SerializeField]
        [Foldout("Explosion Settings", Style = "Header")]
        [MinValue(0.0f)]
        private float coverageRadius = 10.0f;

        [SerializeField]
        [Foldout("Explosion Settings", Style = "Header")]
        private LayerMask cullingLayer = Physics.AllLayers;

        [SerializeField]
        [Foldout("Explosion Settings", Style = "Header")]
        [ReorderableList(ElementLabel = "Settings {niceIndex}")]
        private DistanceSettings[] distanceSettings = new DistanceSettings[1] { new DistanceSettings() };

        [SerializeField]
        [Foldout("Other Settings", Style = "Header")]
        [ReorderableList(ElementLabel = null)]
        private ParticleSystem[] effects;

        // Stored required components.
        private AudioSource audioSource;

        // Stored required properties.
        private CollisionDetectionMode storedCollisionDetectionMode;
        private CoroutineObject explodeCoroutine;

        protected override void Awake()
        {
            base.Awake();
            audioSource = GetComponent<AudioSource>();
            explodeCoroutine = new CoroutineObject(this);
        }

        protected virtual void OnEnable()
        {
            PlayCustomEffects();
        }

        protected override void OnCollisionEnter(Collision other)
        {
            SleepProjectile();
            explodeCoroutine.Start(Explode);
        }

        protected virtual IEnumerator Explode()
        {
            if (projectileMesh != null)
            {
                projectileMesh.SetActive(false);
            }

            audioSource.Play();

            Collider[] colliders = Physics.OverlapSphere(transform.position, coverageRadius, cullingLayer, QueryTriggerInteraction.Ignore);

            for (int i = 0; i < colliders.Length; i++)
            {
                Collider collider = colliders[i];
                if (collider.gameObject == gameObject)
                {
                    continue;
                }

                float distance = Vector3.Distance(transform.position, collider.transform.position);
                for (int j = 0; j < distanceSettings.Length; j++)
                {
                    DistanceSettings distanceSetting = distanceSettings[j];
                    if (Math.InRange(distance, distanceSetting.GetDistance()))
                    {
                        if (collider.TryGetComponent<PlayerController>(out PlayerController controller))
                        {
                            controller.GetPlayerCamera().GetShaker().RegisterShake(new PerlinShake(distanceSetting.GetShakeSettings()));
                        }
                        SendDamage(collider.transform, distanceSetting.GetDamage());
                        SendExplosionImpulse(collider.transform, distanceSetting.GetImpulse(), distanceSetting.GetUpwardsModifier());
                    }
                }
            }

            if (explosionEffect != null)
            {
                explosionEffect.Play(true);
                while (explosionEffect.isPlaying || audioSource.isPlaying)
                {
                    yield return null;
                }
            }


            Push();
        }

        protected virtual void SendExplosionImpulse(Transform other, float impulse, float upwardsModifier)
        {
            Rigidbody rigidbody = other.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.AddExplosionForce(impulse, transform.position, coverageRadius, upwardsModifier, ForceMode.Impulse);
            }
        }

        /// <summary>
        /// Called before pushing object to pool.
        /// </summary>
        protected override void OnBeforePush()
        {
            base.OnBeforePush();
            AwakeProjectile();
            StopCustomEffects();
            explodeCoroutine.Stop();
            if (projectileMesh != null)
            {
                projectileMesh.SetActive(true);
            }
        }

        protected void PlayCustomEffects()
        {
            for (int i = 0; i < effects.Length; i++)
            {
                effects[i].Play();
            }
        }

        protected void StopCustomEffects()
        {
            for (int i = 0; i < effects.Length; i++)
            {
                effects[i].Stop();
            }
        }

        protected void AwakeProjectile()
        {
            GetRigidbody().isKinematic = false;
            GetRigidbody().collisionDetectionMode = storedCollisionDetectionMode;
            GetCollider().isTrigger = false;
        }

        protected void SleepProjectile()
        {
            storedCollisionDetectionMode = GetRigidbody().collisionDetectionMode;
            GetRigidbody().collisionDetectionMode = CollisionDetectionMode.Discrete;
            GetRigidbody().isKinematic = true;
            GetCollider().isTrigger = true;
        }
    }
}