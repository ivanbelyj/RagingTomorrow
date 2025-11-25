/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov, Deryabin Vladimir
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules;
using System;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.CameraSystems
{
    [HideScriptField]
    [AddComponentMenu(null)]
    [DisallowMultipleComponent]
    public abstract class EntityCamera : MonoBehaviour, IRestorable, IKinematic, IDeltaTime
    {
        [SerializeField]
        [NotNull(Format = "Set camera as child of empty game object, which will be used as Hinge and attach it here.", Size = MessageBoxSize.Big)]
        [Order(-998)]
        private Transform hinge;

        [SerializeField]
        [NotNull(Format = "Attach an object relative to which the camera will rotate.")]
        [Order(-997)]
        private Transform target;

        // Stored required properties.
        private bool isKinematic;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            Debug.Assert(hinge != null, $"<b><color=#FF0000>Attach reference of hinge transform to the {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Hinge<i>(field)</i>.</color></b>");
            Debug.Assert(target != null, $"<b><color=#FF0000>Attach reference of target transform to the {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Target<i>(field)</i>.</color></b>");
        }

        /// <summary>
        /// Called every frame, while the controller is enabled.
        /// </summary>
        protected virtual void Update()
        {
            if (!isKinematic && GetUpdateOrder() == UpdateOrder.Update)
            {
                UpdateHinge(hinge);
                UpdateTarget(target);
            }
        }

        /// <summary>
        /// Called every fixed frame-rate frame.
        /// <br>0.02 seconds (50 calls per second) is the default time between calls.</br>
        /// </summary>
        protected virtual void FixedUpdate()
        {
            if (!isKinematic && GetUpdateOrder() == UpdateOrder.FixedUpdate)
            {
                UpdateHinge(hinge);
                UpdateTarget(target);
            }
        }

        /// <summary>
        /// Called after all Update functions have been called.
        /// </summary>
        protected virtual void LateUpdate()
        {
            if (!isKinematic && GetUpdateOrder() == UpdateOrder.Update)
            {
                UpdateHinge(hinge);
                UpdateTarget(target);
            }
        }

        /// <summary>
        /// Update hinge transform.
        /// </summary>
        /// <param name="hinge">Reference of hinge transform.</param>
        protected abstract void UpdateHinge(Transform hinge);

        /// <summary>
        /// Update target transform.
        /// </summary>
        /// <param name="target">Reference of target transform.</param>
        protected abstract void UpdateTarget(Transform target);

        /// <summary>
        /// Camera movement update order.
        /// </summary>
        public abstract UpdateOrder GetUpdateOrder();

        #region [IDeltaTime Implementation]
        public float GetDeltaTime()
        {
            if (GetUpdateOrder() == UpdateOrder.FixedUpdate)
            {
                return Time.fixedDeltaTime;
            }
            return Time.deltaTime;
        }
        #endregion

        #region [IRestorable Implementation]
        /// <summary>
        /// Restore camera to default.
        /// </summary>
        public virtual void Restore()
        {
            hinge.localRotation = Quaternion.identity;
        }
        #endregion

        #region [IKinematic Implementation]
        public void IsKinematic(bool value)
        {
            if(isKinematic != value)
            {
                isKinematic = value;
                OnKinematicCallback?.Invoke();
            }
        }

        public bool IsKinematic()
        {
            return isKinematic;
        }
        #endregion

        #region [Event Callback Functions]
        /// <summary>
        /// Called when camera become kinematic.
        /// </summary>
        public event Action OnKinematicCallback;
        #endregion

        #region [Getter / Setter]
        public Transform GetTarget()
        {
            return target;
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        public Transform GetHinge()
        {
            return hinge;
        }

        public void SetHinge(Transform hinge)
        {
            this.hinge = hinge;
        }
        #endregion
    }
}