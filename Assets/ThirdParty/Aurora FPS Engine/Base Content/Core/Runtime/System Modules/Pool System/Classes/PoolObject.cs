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
using System.Collections;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Pool System/Pool Object")]
    [DisallowMultipleComponent]
    public class PoolObject : MonoBehaviour, IPoolObject
    {
        // Base pool object properties.
        [SerializeField]
        [Foldout("Advanced Settings", Style = "Header")]
        [NotEmpty]
        [Order(999)]
        private string poolObjectID = AuroraExtension.GenerateID(7);

        // Stored required components.
        private PoolManager poolManager;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            poolManager = PoolManager.GetRuntimeInstance();
        }

        /// <summary>
        /// Delayed pool object push coroutine.
        /// Started when pool object being active and disable object after delayed time.
        /// The coroutine can be stopped if the object being disabled before the delayed time expires.
        /// 
        /// Implement this method to make custom delayed push coroutine.
        /// </summary>
        /// <param name="time">Delayed time to push pool object in pool.</param>
        [System.Obsolete("Use AutoPush component.")]
        protected virtual IEnumerator DelayedPush(float time)
        {
            yield return new WaitForSeconds(time);
            Push();
        }

        /// <summary>
        /// Push this object to pool.
        /// </summary>
        public void Push()
        {
            OnBeforePush();
            OnBeforePushCallback?.Invoke();
            poolManager.Push(this);
            OnAfterPush();
            OnAfterPushCallback?.Invoke();
        }

        /// <summary>
        /// Generate new global unique identifier.
        /// </summary>
        /// <param name="length">GUID char length.</param>
        public void GenerateGUID(int length)
        {
            poolObjectID = AuroraExtension.GenerateID(length);
        }

        /// <summary>
        /// Called before pushing object to pool.
        /// </summary>
        protected virtual void OnBeforePush() { }

        /// <summary>
        /// Called after pushing object to pool.
        /// </summary>
        protected virtual void OnAfterPush() { }

        #region [Event Callback Functions]
        public event System.Action OnBeforePushCallback;

        public event System.Action OnAfterPushCallback;
        #endregion

        #region [Getter / Setter]
        public string GetPoolObjectID()
        {
            return poolObjectID;
        }

        public void SetPoolObjectID(string value)
        {
            poolObjectID = value;
        }
        #endregion
    }
}