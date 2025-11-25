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
using AuroraFPSRuntime.SystemModules;
using AuroraFPSRuntime.SystemModules.HealthModules;
using AuroraFPSRuntime.SystemModules.ControllerModules;
using System.Collections;
using UnityEngine;
using AuroraFPSRuntime.CoreModules.Mathematics;
using AuroraFPSRuntime.CoreModules;
using AuroraFPSRuntime.SystemModules.ControllerSystems;

namespace AuroraFPSRuntime.WeaponModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/Weapon Modules/Physics Shell/Physics Grenade")]
    [DisallowMultipleComponent]
    public partial class PhysicsGrenade : PoolObject
    {
        [SerializeField]
        private ParticleSystem explosionEffect;

        [SerializeField]
        private AudioClip explosionSound;

        [SerializeField]
        [ReorderableList(ElementLabel = null, DisplayHeader = false, NoneElementLabel = "Add grenade meshes...")]
        [Foldout("Meshes", Style = "Header")]
        private Renderer[] grenadeMeshes;

        [SerializeField]
        [Foldout("Explosion Settings", Style = "Header")]
        [MinValue(0.0f)]
        private float timer = 5.0f;

        [SerializeField]
        [Foldout("Explosion Settings", Style = "Header")]
        [MinValue(0.0f)]
        private float coverageRadius = 10.0f;

        [SerializeField]
        [Foldout("Explosion Settings", Style = "Header")]
        private LayerMask cullingLayer = Physics.AllLayers;

        [SerializeField]
        [Foldout("Explosion Settings", Style = "Header")]
        private LayerMask obstacleLayer = Physics.AllLayers;

        [SerializeField]
        [Foldout("Explosion Settings", Style = "Header")]
        [ReorderableList(ElementLabel = "Settings {niceIndex}")]
        private DistanceSettings[] distanceSettings;

        // Stored required components.
        private Rigidbody grenadeRigidbody;

        // Stored required properties.
        private CoroutineObject timerCoroutine;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            grenadeRigidbody = GetComponent<Rigidbody>();
            timerCoroutine = new CoroutineObject(this);
        }

        /// <summary>
        /// Pull pin of grenade.
        /// </summary>
        /// <returns>True, if the pin is pulled out successfully. False if pin already pulled.</returns>
        public virtual bool PullPin()
        {
            return timerCoroutine.Start(StartTimer);
        }

        /// <summary>
        /// Grenade timer.
        /// </summary>
        public virtual IEnumerator StartTimer()
        {
            yield return new WaitForSeconds(timer);

#if UNITY_6000_0_OR_NEWER
            grenadeRigidbody.linearVelocity = Vector3.zero;
#else
            grenadeRigidbody.velocity = Vector3.zero;
#endif
            grenadeRigidbody.rotation = Quaternion.identity;

            if (grenadeMeshes != null && grenadeMeshes.Length > 0)
            {
                for (int i = 0; i < grenadeMeshes.Length; i++)
                {
                    grenadeMeshes[i].enabled = false;
                }
            }

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

                        if (!Physics.Linecast(transform.position, collider.bounds.center, out RaycastHit hitInfo, obstacleLayer))
                        {
                            IDamageable damageable = collider.GetComponent<IDamageable>();
                            if (damageable != null)
                            {
                                damageable.TakeDamage(distanceSetting.GetDamage(), new DamageInfo(transform, hitInfo.point, hitInfo.normal));
                            }

                            Rigidbody rigidbody = collider.GetComponent<Rigidbody>();
                            if (rigidbody != null)
                            {
                                rigidbody.AddExplosionForce(distanceSetting.GetImpulse(), transform.position, coverageRadius, distanceSetting.GetUpwardsModifier(), ForceMode.Impulse);
                            }
                        }
                        break;
                    }
                }
            }

            if (explosionSound != null)
            {
                AudioSource.PlayClipAtPoint(explosionSound, transform.position);
            }

            if (explosionEffect != null)
            {
                explosionEffect.Play(true);
                while (explosionEffect.isPlaying)
                {
                    yield return null;
                }
            }

            Push();
        }

        /// <summary>
        /// Called before pushing object to pool.
        /// </summary>
        protected override void OnBeforePush()
        {
            base.OnBeforePush();
            timerCoroutine.Stop();
            if (grenadeMeshes != null && grenadeMeshes.Length > 0)
            {
                for (int i = 0; i < grenadeMeshes.Length; i++)
                {
                    grenadeMeshes[i].enabled = true;
                }
            }
        }

        #region [Getter / Setter]
        public float GetTimer()
        {
            return timer;
        }

        public void SetTimer(float value)
        {
            timer = value;
        }

        public float GetCoverageRadius()
        {
            return coverageRadius;
        }

        public void SetCoverageRadius(float value)
        {
            coverageRadius = value;
        }

        public LayerMask GetCullingLayer()
        {
            return cullingLayer;
        }

        public void SetCullingLayer(LayerMask value)
        {
            cullingLayer = value;
        }

        public DistanceSettings[] GetDistanceSettings()
        {
            return distanceSettings;
        }

        public void SetDistanceSettings(DistanceSettings[] value)
        {
            distanceSettings = value;
        }

        public DistanceSettings GetDistanceSetting(int index)
        {
            return distanceSettings[index];
        }

        public void SetDistanceSetting(int index, DistanceSettings value)
        {
            distanceSettings[index] = value;
        }

        public int GetDistanceSettingsCount()
        {
            return distanceSettings.Length;
        }

        public Renderer[] GetMeshes()
        {
            return grenadeMeshes;
        }

        public ParticleSystem GetExplosionEffect()
        {
            return explosionEffect;
        }

        public void SetExplosionEffect(ParticleSystem value)
        {
            explosionEffect = value;
        }

        public Rigidbody GetRigidbody()
        {
            return grenadeRigidbody;
        }
        #endregion
    }
}