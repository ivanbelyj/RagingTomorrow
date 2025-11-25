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
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AuroraFPSRuntime.SystemModules
{
    [HideScriptField]
    [AddComponentMenu(null)]
    [DisallowMultipleComponent]
    public abstract class InteractiveObject : MonoBehaviour
    {
        public enum InvokeType
        {
            Viewer,
            Trigger
        }

        [SerializeField]
        [Order(-999)]
        private InvokeType invokeType = InvokeType.Trigger;

        [SerializeField]
        [NotEmpty]
        [Order(-998)]
        private string inputPath = string.Empty;

        [SerializeField]
        [Foldout("Advanced Settings", Style = "Header")]
        [Order(900)]
        private bool interactable = true;

        [SerializeField]
        [Foldout("Advanced Settings", Style = "Header")]
        [Order(901)]
        private bool refreshMessage = true;

        [SerializeField]
        [Label("Rate")]
        [Foldout("Advanced Settings", Style = "Header")]
        [VisibleIf("refreshMessage", true)]
        [MinValue(0.01f)]
        [Indent(1)]
        [Order(902)]
        private float refreshMessageRate = 0.15f;

        [SerializeField]
        [Foldout("Advanced Settings", Style = "Header")]
        [VisibleIf("invokeType", "Trigger")]
        [Order(904)]
        private LayerMask cullingLayer = 1 << 0;

        // Stored required properties.
        private int storedMessageCode;
        private bool isActive;
        private Transform activator;
        private InputAction inputAction;
        private CoroutineObject coroutineObject;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            inputAction = InputReceiver.Asset.FindAction(inputPath, true);
            coroutineObject = new CoroutineObject(this);
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        protected virtual void OnEnable()
        {
            inputAction.performed += OnInputPerformed;
        }

        /// <summary>
        /// Called when the behaviour becomes disabled.
        /// </summary>
        protected virtual void OnDisable()
        {
            inputAction.performed -= OnInputPerformed;
        }

        /// <summary>
        /// Calculate message code of executing this interactive object by specific object.
        /// </summary>
        /// <param name="other">Target trasform reference.</param>
        /// <param name="messageCode">Output message code.</param>
        protected virtual void CalculateMessageCode(Transform other, out int messageCode)
        {
            messageCode = 0;
        }

        /// <summary>
        /// Refresh message calculation by specific rate time.
        /// </summary>
        private IEnumerator RefreshMessageCalculation()
        {
            WaitForSeconds delay = new WaitForSeconds(refreshMessageRate);
            while (isActive && activator != null)
            {
                CalculateMessageCode(activator, out storedMessageCode);
                OnCalculateMessageCodeCallback?.Invoke(activator, storedMessageCode);
                yield return delay;
            }
        }

        public void Interactable(bool value)
        {
            interactable = value;
        }

        #region [Abstract Methods]
        /// <summary>
        /// Called when interactive object being executing by specific object.
        /// </summary>
        /// <param name="other">Target trasform reference.</param>
        /// <returns>True if loot was successful. Otherwise false.</returns>
        public abstract bool Execute(Transform other);
        #endregion

        #region [Trigger Callback Implementation]
        /// <summary>
        /// Called on the FixedUpdate function when two GameObjects collide. 
        /// The Colliders involved are not always at the point of initial contact.
        /// </summary>
        /// <param name="other">The other Collider involved in this collision.</param>
        private void OnTriggerEnter(Collider other)
        {
            if (interactable &&
                invokeType == InvokeType.Trigger &&
                (cullingLayer & 1 << other.gameObject.layer) > 0)
            {
                if (Execute(other.transform))
                {
                    OnExecutedCallback?.Invoke(other.transform);
                }
            }
        }
        #endregion

        #region [Input Actions]
        public InputAction InputAction { get => inputAction; }
        #endregion

        #region [Input Callback Implementation]
        /// <summary>
        /// Wrapper of custom input action.
        /// </summary>
        /// <param name="context"></param>
        private void OnInputPerformed(InputAction.CallbackContext context)
        {
            if (interactable &&
                invokeType == InvokeType.Viewer &&
                isActive &&
                activator != null &&
                context.performed)
            {
                if (Execute(activator))
                {
                    OnExecutedCallback?.Invoke(activator);
                }
            }
        }
        #endregion

        #region [Event Callback Functions]
        /// <summary>
        /// Called when specific game object executed this interactive object.
        /// </summary>
        public event Action<Transform> OnExecutedCallback;

        /// <summary>
        /// Called when this interactive object become active.
        /// </summary>
        /// <param name="other">Target trasform reference.</param>
        /// <param name="message">Specific message of execution.</param>
        public event Action<Transform, int> OnBecomeActiveCallback;

        /// <summary>
        /// Called when this interactive object become inactive.
        /// </summary>
        /// <param name="other">Target trasform reference.</param>
        /// <param name="message">Specific message of execution.</param>
        public event Action OnBecomeInactiveCallback;

        /// <summary>
        /// Called when message code being calculated.
        /// <br>Note: If the refresh option is enabled, this action will be called every time after a certain period of time (refresh rate).</br>
        /// </summary>
        /// <param name="other">Target trasform reference.</param>
        /// <param name="messageCode">Output message code.</param>
        public event Action<Transform, int> OnCalculateMessageCodeCallback;
        #endregion

        #region [Getter / Setter]
        /// <summary>
        /// Specific message code of executing this interactive object by specific object.
        /// </summary>
        protected int GetMessageCode()
        {
            return storedMessageCode;
        }

        internal bool IsActive()
        {
            return isActive;
        }

        internal void Activate(Transform other)
        {
            if (!isActive || activator != other)
            {
                activator = other;
                isActive = true;
                if (refreshMessage)
                {
                    coroutineObject.Start(RefreshMessageCalculation, true);
                }
                else
                {
                    CalculateMessageCode(activator, out storedMessageCode);
                    OnCalculateMessageCodeCallback?.Invoke(activator, storedMessageCode);
                }
                OnBecomeActiveCallback?.Invoke(activator, storedMessageCode);
            }
        }

        public void Diactivate()
        {
            if (isActive && activator != null)
            {
                activator = null;
                isActive = false;
                if (refreshMessage)
                {
                    coroutineObject.Stop();
                }
                OnBecomeInactiveCallback?.Invoke();
            }
        }

        public InvokeType GetInvokeType()
        {
            return invokeType;
        }

        public void SetInvokeType(InvokeType value)
        {
            invokeType = value;
        }

        public string GetInputPath()
        {
            return inputPath;
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
