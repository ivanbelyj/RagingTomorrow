/* ================================================================
   ---------------------------------------------------
   Project   :    Aurora FPS Engine
   Publisher :    Renowned Games
   Author    :    Tamerlan Shakirov, Alexandra Averyanova
   ---------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Coroutines;
using AuroraFPSRuntime.SystemModules.HealthModules;
using System.Collections;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Interactive/Explosion Object")]
    [DisallowMultipleComponent]
    public sealed class ExplosionObject : ObjectHealth
    {
        [SerializeField]
        [Foldout("Explosion Settings", Style = "Header")]
        [MinValue(0.01f)]
        private float explosionRadius;

        [SerializeField]
        [Foldout("Explosion Settings", Style = "Header")]
        [MinValue(0.01f)]
        private float explosionForce;

        [SerializeField]
        [Foldout("Explosion Settings", Style = "Header")]
        [MinValue(0.01f)]
        private float upwardsModifier;

        [SerializeField]
        [Foldout("Explosion Settings", Style = "Header")]
        [MinValue(0)]
        private float explosionDamage;

        [SerializeField]
        [Foldout("Explosion Settings", Style = "Header")]
        private LayerMask cullingLayer = 1 << 0;

        [SerializeField]
        [Foldout("Effect Settings", Style = "Header")]
        private ParticleSystem particleEffect;

        [SerializeField]
        [Foldout("Effect Settings", Style = "Header")]
        private AudioClip audioClip;

        [SerializeField]
        [Foldout("Effect Settings", Style = "Header")]
        private bool hideMesh;

        [SerializeField]
        [Foldout("Effect Settings", Style = "Header")]
        private bool destroyEmit;

        // Stored required components.
        private AudioSource audioSource;

        // Stored required properties.
        private CoroutineObject coroutine;

        /// <summary>
        /// Called when the script is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            coroutine = new CoroutineObject(this);
            if (audioClip != null)
            {
                if (!TryGetComponent<AudioSource>(out audioSource))
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
                audioSource.clip = audioClip;
            }
        }

        /// <summary>
        /// Called when object health become more then zero.
        /// </summary>
        protected override void OnDead()
        {
            coroutine.Start(Explode);
        }

        /// <summary>
        /// Adds explosion force to all objects within explosion radius.
        /// </summary>
        private IEnumerator Explode()
        {
            Collider[] affectedColliders = Physics.OverlapSphere(transform.position, explosionRadius, cullingLayer);
            for (int i = 0; i < affectedColliders.Length; i++)
            {
                Collider collider = affectedColliders[i];
                if (collider.TryGetComponent<IDamageable>(out IDamageable damageable))
                {
                    damageable.TakeDamage(explosionDamage, new DamageInfo(transform));
                }
            }

            yield return new WaitForEndOfFrame();

            for (int i = 0; i < affectedColliders.Length; i++)
            {
                Collider collider = affectedColliders[i];
                if (collider.TryGetComponent(out Rigidbody rigidbody))
                {
                    rigidbody.AddExplosionForce(explosionForce, transform.position,
                        explosionRadius, upwardsModifier, ForceMode.Impulse);
                }
            }

            if (hideMesh)
            {
                MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    meshRenderers[i].enabled = false;
                }
            }

            if(particleEffect != null)
            {
                particleEffect.Play(true);
            }

            if(audioSource != null)
            {
                audioSource.Play();
            }

            while ((particleEffect != null && particleEffect.isPlaying) ||
                (audioSource != null && audioSource.isPlaying))
            {
                yield return null;
            }

            if (destroyEmit)
            {
                Destroy(gameObject);
            }
        }
    }
}
