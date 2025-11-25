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
using AuroraFPSRuntime.CoreModules.InputSystem;
using AuroraFPSRuntime.SystemModules;
using AuroraFPSRuntime.SystemModules.InventoryModules;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AuroraFPSRuntime.WeaponModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/Weapon Modules/Grenade/Grenade Throw System")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AmmoSystem))]
    public sealed class GrenadeThrowSystem : MonoBehaviour
    {
        public enum InputCallback
        {
            Pressed,
            Released
        }

        [SerializeField]
        private InputCallback inputCallback;

        [SerializeField]
        [NotNull]
        private Transform throwPoint;

        [SerializeField]
        [NotNull]
        private Transform throwDirection;

        [SerializeField]
        [MinValue(0.0f)]
        private float animationTime;

        [SerializeField]
        [MinValue(0.0f)]
        private float force;

        [SerializeField]
        [NotNull]
        private PhysicsGrenade grenade;

        [SerializeField]
        private bool inactiveAccess = false;

        [SerializeField]
        [NotEmpty]
        [VisibleIf("inactiveAccess", true)]
        [Indent(1)]
        private string inputActionName;

        // Stored required components.
        private PoolManager poolManager;
        private WeaponInventorySystem inventorySystem;
        private AmmoSystem ammoSystem;

        // Stored required properties.
        private CoroutineObject throwCoroutine;
        private EquippableItem weaponItem;
        private EquippableItem lastWeaponItem;

        private bool blockThrow;
        private bool inactiveRequiest;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            poolManager = PoolManager.GetRuntimeInstance();
            inventorySystem = transform.GetComponentInParent<WeaponInventorySystem>();
            ammoSystem = GetComponent<AmmoSystem>();

            EquippableObjectIdentifier weaponIdentifier = GetComponent<EquippableObjectIdentifier>();
            weaponItem = weaponIdentifier.GetItem();

            throwCoroutine = new CoroutineObject(this);
            if (inactiveAccess)
            {
                InputAction inputAction = InputReceiver.Asset.FindAction(inputActionName);
                inputAction.performed += OnInactiveAction;
            }
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            RegisterInputCallbacks();

            if (inactiveRequiest)
            {
                OnThrowStarted?.Invoke();
                throwCoroutine.Start(WaitForAnimation);
            }
        }

        /// <summary>
        /// Called when the behaviour becomes disabled.
        /// </summary>
        private void OnDisable()
        {
            UnregisterInputCallbacks();
        }

        /// <summary>
        /// Pull pin and throw grenade.
        /// </summary>
        public void Throw()
        {
            if (ammoSystem.RemoveAmmo(1))
            {
                PhysicsGrenade grenadeClone = poolManager.CreateOrPop<PhysicsGrenade>(grenade, throwPoint.position, throwPoint.rotation);
                grenadeClone.PullPin();
                Rigidbody rigidbody = grenadeClone.GetRigidbody();
                if (rigidbody != null)
                {
                    rigidbody.AddForce(throwDirection.forward * force, ForceMode.Impulse);
                }
                OnThrowCallback?.Invoke(grenadeClone);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerator WaitForAnimation()
        {
            OnAnimationTimeStartedCallback?.Invoke();
            yield return new WaitForSeconds(animationTime);
            if (inactiveRequiest)
            {
                inventorySystem.UseItem(lastWeaponItem);
                inactiveRequiest = false;
                blockThrow = false;
            }
        }

        private void RegisterInputCallbacks()
        {
            InputReceiver.AttackAction.started += OnThrowAction;
            InputReceiver.AttackAction.performed += OnThrowAction;
            InputReceiver.AttackAction.canceled += OnThrowAction;
        }

        private void UnregisterInputCallbacks()
        {
            InputReceiver.AttackAction.started -= OnThrowAction;
            InputReceiver.AttackAction.performed -= OnThrowAction;
            InputReceiver.AttackAction.canceled -= OnThrowAction;
        }

        #region [Input Action Wrapper]
        private void OnThrowAction(InputAction.CallbackContext context)
        {
            if (!blockThrow && ammoSystem.GetAmmoCount() > 0)
            {
                if (context.started)
                {
                    OnThrowStarted?.Invoke();
                }
                else if ((context.performed && inputCallback == InputCallback.Pressed) ||
                    (context.canceled && inputCallback == InputCallback.Released))
                {
                    throwCoroutine.Start(WaitForAnimation);
                }
            }
        }

        private void OnInactiveAction(InputAction.CallbackContext context)
        {
            if (!blockThrow && ammoSystem.GetAmmoCount() > 0)
            {
                lastWeaponItem = inventorySystem.GetEquippedItem();
                inventorySystem.UseItem(weaponItem);
                inactiveRequiest = true;
                blockThrow = true;
            }
        }
        #endregion

        #region [Event Callback Functions]
        /// <summary>
        /// Called once when player pressed on attack button.
        /// </summary>
        public event Action OnThrowStarted;

        /// <summary>
        /// Called once when animation timer being started.
        /// </summary>
        public event Action OnAnimationTimeStartedCallback;

        /// <summary>
        /// Called when grenade threw.
        /// </summary>
        public event Action<PhysicsGrenade> OnThrowCallback;
        #endregion

        #region [Getter / Setter]
        public Transform GetThrowDirection()
        {
            return throwPoint;
        }

        public void SetThrowDirection(Transform value)
        {
            throwPoint = value;
        }

        public float GetForce()
        {
            return force;
        }

        public void SetForce(float value)
        {
            force = value;
        }

        public PhysicsGrenade GetGrenade()
        {
            return grenade;
        }

        public void SetGrenade(PhysicsGrenade value)
        {
            grenade = value;
        }
        #endregion
    }
}