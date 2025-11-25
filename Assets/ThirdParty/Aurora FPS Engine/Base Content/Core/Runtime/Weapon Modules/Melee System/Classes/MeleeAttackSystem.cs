/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.InputSystem;
using AuroraFPSRuntime.CoreModules.Coroutines;
using AuroraFPSRuntime.SystemModules.HealthModules;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using AuroraFPSRuntime.SystemModules.InventoryModules;

namespace AuroraFPSRuntime.WeaponModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/Weapon Modules/Melee/Melee Attack System", 0)]
    [DisallowMultipleComponent]
    public sealed class MeleeAttackSystem : MonoBehaviour
    {
        [SerializeField]
        private MeleeWeapon meleeWeapon;

        [SerializeField]
        private float animationTime = 1.0f;

        [SerializeField]
        private bool inactiveAccess = false;

        [SerializeField]
        [NotEmpty]
        [VisibleIf("inactiveAccess", true)]
        [Indent(1)]
        private string inputActionName;

        // Stored required components.
        private WeaponInventorySystem inventorySystem;

        // Stored required properties.
        private CoroutineObject coroutineObject;
        private EquippableItem weaponItem;
        private EquippableItem lastWeaponItem;
        private float lastAttackTime;
        private bool blockAttack;
        private bool inactiveRequest;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            coroutineObject = new CoroutineObject(this);
            inventorySystem = transform.GetComponentInParent<WeaponInventorySystem>();

            EquippableObjectIdentifier weaponIdentifier = GetComponent<EquippableObjectIdentifier>();
            weaponItem = weaponIdentifier.GetItem();

            if (inactiveAccess)
            {
                InputAction inputAction = InputReceiver.Asset.FindAction(inputActionName);
                inputAction.performed += OnInactiveAction;
            }

            meleeWeapon.OnTriggerEnterCallback += TriggerDetection;
            meleeWeapon.OnCollisionEnterCallback += CollisionDetection;

            lastAttackTime = -1;
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            InputReceiver.AttackAction.performed += OnAttackCallbackAction;

            if (inactiveRequest) 
            {
                coroutineObject.Start(QuickAttack);
            }
        }

        /// <summary>
        /// Called when the behaviour becomes disabled.
        /// </summary>
        private void OnDisable()
        {
            InputReceiver.AttackAction.performed -= OnAttackCallbackAction;

            if (inactiveRequest)
            {
                inactiveRequest = false;
                blockAttack = false;
            }
        }

        /// <summary>
        /// On trigger enter callback wrapper.
        /// </summary>
        private void TriggerDetection(Collider other)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(meleeWeapon.GetDamage(), new DamageInfo(other.transform));
            }

            if (meleeWeapon.GetImpulse() > 0)
            {
                Rigidbody rigidbody = other.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    Vector3 direction = other.transform.position - transform.position;
                    rigidbody.AddForce(direction.normalized * meleeWeapon.GetImpulse(), ForceMode.Impulse);
                }
            }

        }

        /// <summary>
        /// On collision enter wrapper.
        /// </summary>
        private void CollisionDetection(Collision collision)
        {
            ContactPoint contact = collision.GetContact(0);

            IDamageable damageable = collision.transform.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(meleeWeapon.GetDamage(), new DamageInfo(transform, contact.point, contact.normal));
            }

            if (meleeWeapon.GetImpulse() > 0)
            {
                Rigidbody rigidbody = collision.transform.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    Quaternion qDirection = Quaternion.LookRotation(contact.point, contact.normal);
                    rigidbody.AddForce(qDirection.eulerAngles.normalized * meleeWeapon.GetImpulse(), ForceMode.Impulse);
                }
            }

            Decal.Spawn(meleeWeapon.GetDecalMapping(), contact);
        }

        /// <summary>
        /// Start detection impacts of melee weapon.
        /// <b>Use it for more precise control of collision handling.</b>
        /// <b>Can be use for handle collision by animation event.</b>
        /// </summary>
        public void StartDetection()
        {
            meleeWeapon.enabled = true;
        }

        /// <summary>
        /// Stop detection impacts of melee weapon.
        /// <b>Use it for more precise control of collision handling.</b>
        /// <b>Can be use for handle collision by animation event.</b>
        /// </summary>
        public void StopDetection()
        {
            meleeWeapon.enabled = false;
        }

        /// <summary>
        /// Wait for animation complete and hide weapon in inventory.
        /// </summary>
        private IEnumerator QuickAttack()
        {
            OnAttackCallback?.Invoke();
            yield return new WaitForSeconds(animationTime);
            inventorySystem.UseItem(lastWeaponItem);
        }

        #region [Input Actions Wrapper]
        private void OnAttackCallbackAction(InputAction.CallbackContext context)
        {
            if (!blockAttack && (lastAttackTime < 0 || Time.time - lastAttackTime > animationTime))
            {
                OnAttackCallback?.Invoke();
                lastAttackTime = Time.time;
            }
        }

        private void OnInactiveAction(InputAction.CallbackContext context)
        {
            if (!blockAttack && !gameObject.activeSelf)
            {
                lastWeaponItem = inventorySystem.GetEquippedItem();
                inventorySystem.UseItem(weaponItem);
                inactiveRequest = true;
                blockAttack = true;
            }
        }
        #endregion

        #region [Event Callback Functions]
        public event Action OnAttackCallback;
        #endregion
    }
}