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
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AuroraFPSRuntime.WeaponModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/Weapon Modules/Reload/Sequential Reload System")]
    [DisallowMultipleComponent]
    public class WeaponSequentialReloadSystem : WeaponReloadSystem
    {
        [SerializeField]
        [Foldout("Time Settings", Style = "Header")]
        [MinValue(0.0f)]
        [Order(100)]
        private float startTime = 1.0f;

        [SerializeField]
        [Foldout("Time Settings", Style = "Header")]
        [MinValue(0.0f)]
        [Order(101)]
        private float loopTime = 1.25f;

        [SerializeField]
        [Foldout("Time Settings", Style = "Header")]
        [MinValue(0.0f)]
        [Order(102)]
        private float endTime = 1.0f;

        [SerializeField]
        [Tooltip("Break reload loop, when pressed specific button.")]
        [Foldout("Advanced Settings", Style = "Header")]
        [Order(305)]
        private bool breakLoopReload;

        [SerializeField]
        [ReorderableList(ElementLabel = null)]
        [VisibleIf("breakLoopReload", true)]
        [Foldout("Advanced Settings", Style = "Header")]
        [Order(306)]
        private string[] breakLoopReloadInputs;

        // Stored required properties.
        private bool breakLoopReloadValue;
        private InputAction[] inputActions;
        private CoroutineObject reloadCoroutine;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            reloadCoroutine = new CoroutineObject(this);
            inputActions = new InputAction[breakLoopReloadInputs.Length];
            for (int i = 0; i < breakLoopReloadInputs.Length; i++)
            {
                inputActions[i] = InputReceiver.Asset.FindAction(breakLoopReloadInputs[i]);
            }
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if(inputActions != null)
            {
                for (int i = 0; i < inputActions.Length; i++)
                {
                    inputActions[i].performed += BreakReloadAction;
                }
            }
        }

        /// <summary>
        /// Implementation of sequential weapon reload logic.
        /// </summary>
        protected override void MakeReload()
        {
            reloadCoroutine.Start(ReloadProcessing);
        }

        /// <summary>
        /// Implementation of sequential weapon reload logic with time delay.
        /// </summary>
        protected virtual IEnumerator ReloadProcessing()
        {
            OnStartReloadCallback?.Invoke();

            if (GetAmmoCount() > 0)
            {
                OnReloadClipCallback?.Invoke();
            }
            else
            {
                OnReloadCallback?.Invoke();
            }

            yield return new WaitForSeconds(startTime);

            WaitForSeconds waitForIterationTime = new WaitForSeconds(loopTime);
            int requiredBulletCount = GetMaxAmmoCount() - GetAmmoCount();
            while (requiredBulletCount > 0 && GetClipCount() > 0)
            {
                if (breakLoopReloadValue)
                {
                    break;
                }

                OnBeforeLoopReloadTimerCallback?.Invoke();

                yield return waitForIterationTime;

                RemoveClip(1);
                requiredBulletCount--;
                AddAmmo(1);
                OnLoopReloadCallback?.Invoke();
            }

            OnBeforeEndReloadTimerCallback?.Invoke();
            yield return new WaitForSeconds(endTime);
            OnEndReloadCallback?.Invoke();
            breakLoopReloadValue = false;
        }

        /// <summary>
        /// Called when the behaviour becomes disabled or inactive.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            reloadCoroutine.Stop();
            if(inputActions != null)
            {
                for (int i = 0; i < inputActions.Length; i++)
                {
                    inputActions[i].performed -= BreakReloadAction;
                }
            }
        }

        /// <summary>
        /// Force break from reloading loop.
        /// </summary>
        private void BreakReload()
        {
            breakLoopReloadValue = true;
        }

        #region [Input Action Wrapper]
        /// <summary>
        /// Input action wrapper of force break from reloading loop.
        /// </summary>
        private void BreakReloadAction(InputAction.CallbackContext context)
        {
            BreakReload();
        }
        #endregion

        #region [Event Callback Functions]
        /// <summary>
        /// On start reload callback function.
        /// OnStartReload called every time when weapon start reloading.
        /// </summary>
        public override event Action OnStartReloadCallback;

        /// <summary>
        /// On iteration reload callback.
        /// OnIterationReloadCallback called every iteration time.
        /// </summary>
        public event Action OnLoopReloadCallback;

        public event Action OnBeforeLoopReloadTimerCallback;

        /// <summary>
        /// On end reload callback function.
        /// OnEndReloadCallback called every time when weapon end of reloading.
        /// </summary>
        public override event Action OnEndReloadCallback;

        public event Action OnBeforeEndReloadTimerCallback;

        /// <summary>
        /// On reload callback function.
        /// OnBaseReloadCallback called when weapon start base reloading (means that rechargeable clip isn't empty).
        /// </summary>
        public override event Action OnReloadClipCallback;

        /// <summary>
        /// On full reload callback function.
        /// OnFullReloadCallback called when weapon start full reloading (means that rechargeable clip is empty).
        /// </summary>
        public override event Action OnReloadCallback;
        #endregion

        #region [Getter / Setter]
        public float GetStartTime()
        {
            return startTime;
        }

        public void SetStartTime(float value)
        {
            startTime = value;
        }

        public float GetLoopTime()
        {
            return loopTime;
        }

        public void SetLoopTime(float value)
        {
            loopTime = value;
        }

        public float GetEndTime()
        {
            return endTime;
        }

        public void SetEndTime(float value)
        {
            endTime = value;
        }
        #endregion
    }
}