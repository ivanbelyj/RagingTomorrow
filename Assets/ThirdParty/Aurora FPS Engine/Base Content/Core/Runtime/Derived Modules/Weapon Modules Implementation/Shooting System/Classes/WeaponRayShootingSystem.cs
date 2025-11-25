/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.SystemModules.HealthModules;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AuroraFPSRuntime.WeaponModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/Weapon Modules/Shooting/Raycast Shooting System")]
    [DisallowMultipleComponent]
    public class WeaponRayShootingSystem : WeaponShootingSystem
    {
        /// <summary>
        /// Store of instance id's which already killed by player and OnKillCallback has been called for them.
        /// </summary>
        protected static readonly HashSet<int> KilledInstanceIDs = new HashSet<int>();

        [Serializable]
        private class RaycastHitUnityEvent : UnityEvent<RaycastHit> { }

        [Serializable]
        private class TransformUnityEvent : UnityEvent<Transform> { }

        [SerializeField]
        [NotNull]
        private BulletItem bulletItem;

        [SerializeField] 
        [MinValue(0.0f)]
        private float fireRange = 500.0f;

        [SerializeField] 
        private LayerMask cullingLayer = Physics.AllLayers;

        [SerializeField]
        [Foldout("Events", Style = "Header")]
        [Order(320)]
        private RaycastHitUnityEvent onFireRayEvent;

        [SerializeField]
        [Foldout("Events", Style = "Header")]
        [Order(321)]
        private TransformUnityEvent onDamageEvent;

        [SerializeField]
        [Foldout("Events", Style = "Header")]
        [Order(322)]
        private TransformUnityEvent onKillEvent;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(bulletItem != null, $"<b><color=#FF0000>.Bullet item reference is required!\nAttach reference of bullet item to {gameObject.name}<i>(gameobject)</i> -> Ray Shotgun Shooting System <i>(component)</i> -> Bullet Item<i>(field)</i>.</color></b>");

            OnFireRayCallback += onFireRayEvent.Invoke;
            OnDamageCallback += onDamageEvent.Invoke;
            OnKillCallback += onKillEvent.Invoke;
        }

        /// <summary>
        /// Implement this method to make logic of shooting. 
        /// </summary>
        /// <param name="origin">Origin vection of shoot.</param>
        /// <param name="direction">Direction vector of shoot.</param>
        protected override void MakeShoot(Vector3 origin, Vector3 direction)
        {
            if (Physics.Raycast(origin, direction, out RaycastHit hitInfo, fireRange, cullingLayer, QueryTriggerInteraction.Ignore))
            {
                Decal.Spawn(bulletItem.GetDecalMapping(), hitInfo);
                SendDamage(hitInfo);
                AddImpulseForce(hitInfo.transform, direction);
                OnFireRayCallback?.Invoke(hitInfo);
            }
        }

        /// <summary>
        /// Maximum range of calculation shoot direction.
        /// </summary>
        public override float GetDirectionRange()
        {
            return fireRange;
        }

        /// <summary>
        /// Culling layer of calculation shoot direction.
        /// </summary>
        public override LayerMask GetDirectionLayer()
        {
            return cullingLayer;
        }

        /// <summary>
        /// Send damage to transform containing health component.
        /// </summary>
        private void SendDamage(RaycastHit hitInfo)
        {
            Transform other = hitInfo.transform;
            if (other.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                if (other.TryGetComponent<IHealth>(out IHealth health))
                {
                    int instanceID = other.root.GetInstanceID();
                    if (health.IsAlive())
                    {
                        KilledInstanceIDs.Remove(instanceID);
                    }
                }

                damageable.TakeDamage(bulletItem.GetDamageDropsoff(hitInfo.distance), new DamageInfo(transform.root, hitInfo.point, hitInfo.normal));

                if (health != null)
                {
                    int instanceID = other.root.GetInstanceID();
                    if (health.IsAlive())
                    {
                        OnDamageCallback?.Invoke(hitInfo.transform);
                    }
                    else if (KilledInstanceIDs.Add(instanceID))
                    {
                        OnKillCallback?.Invoke(hitInfo.transform);
                    }
                }
            }
        }

        /// <summary>
        /// Add impulse force to rigidbody transform.
        /// </summary>
        private void AddImpulseForce(Transform other, Vector3 direction)
        {
            if (other.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
            {
                rigidbody.AddForce(direction * bulletItem.GetImpactImpulse(), ForceMode.Impulse);
            }
        }

        #region [Event Callback Functions]
        /// <summary>
        /// Called when ray receive hit info result.
        /// </summary>
        /// <param name="RaycastHit">Fire raycast hit info.</param>
        public event Action<RaycastHit> OnFireRayCallback;

        /// <summary>
        /// Called when ray being damage any alive object with health component.
        /// </summary>
        /// <param name="Transform">The Transform data associated with health.</param>
        public event Action<Transform> OnDamageCallback;

        /// <summary>
        /// Called when ray being kill any object with health component.
        /// </summary>
        /// <param name="Transform">The Transform data associated with health.</param>
        public event Action<Transform> OnKillCallback;
        #endregion

        #region [Getter / Setter]
        public BulletItem GetBulletItem()
        {
            return bulletItem;
        }

        public void SetBulletItem(BulletItem value)
        {
            bulletItem = value;
        }

        public float GetFireRange()
        {
            return fireRange;
        }

        public void SetFireRange(float value)
        {
            fireRange = value;
        }

        public LayerMask GetCullingLayer()
        {
            return cullingLayer;
        }

        public void SetCullingLayer(LayerMask value)
        {
            cullingLayer = value;
        }
        #endregion
    }
}