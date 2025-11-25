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
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace AuroraFPSRuntime.SystemModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Event System/Input/Toggle Input Event")]
    public sealed class ToggleInputEvent : MonoBehaviour
    {
        [System.Serializable]
        private class ToggleEvent : UnityEvent<bool> { }

        [SerializeField]
        private bool awakeToggle = false;

        [SerializeField]
        private bool useInputReceiver = true;

        [SerializeField]
        [VisibleIf("globalInput", true)]
        [Indent(1)]
        private string path = "<map>/<action>";

        [SerializeField]
        [VisibleIf("globalInput", false)]
        private InputAction input;

        [SerializeField]
        [Foldout("Event Callbacks", Style = "Header")]
        private ToggleEvent onToggleEvent;

        [SerializeField]
        [Foldout("Event Callbacks", Style = "Header")]
        private UnityEvent onActiveEvent;

        [SerializeField]
        [Foldout("Event Callbacks", Style = "Header")]
        private UnityEvent onDiactiveEvent;

        // Stored required properties.
        private bool active = false;

        /// <summary>
        /// Сalled when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            active = awakeToggle;
            if (useInputReceiver)
            {
                input = InputReceiver.Asset.FindAction(path);
            }
            OnToggleCallback += onToggleEvent.Invoke;
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            input.performed += OnPerformedEvent;
            if (!useInputReceiver)
                input.Enable();
        }

        /// <summary>
        /// Called when the behaviour becomes disabled.
        /// </summary>
        private void OnDisable()
        {
            input.performed -= OnPerformedEvent;
            if(!useInputReceiver)
                input.Disable();
        }

        #region [Input Action Wrapper]
        private void OnPerformedEvent(InputAction.CallbackContext context)
        {
            active = !active;
            if (active)
                onActiveEvent?.Invoke();
            else
                onDiactiveEvent?.Invoke();
            OnToggleCallback?.Invoke(active);
        }
        #endregion

        #region [Event Callback Function]
        public event System.Action<bool> OnToggleCallback;
        #endregion

        #region [Getter / Setter]
        public bool UseInputReceiver()
        {
            return useInputReceiver;
        }

        public void UseInputReceiver(bool value)
        {
            useInputReceiver = value;
        }

        public string GetPath()
        {
            return path;
        }

        public void SetPath(string value)
        {
            path = value;
        }

        public InputAction GetInput()
        {
            return input;
        }

        public void SetInput(InputAction value)
        {
            input = value;
        }

        public bool Active()
        {
            return active;
        }

        public void Active(bool value)
        {
            active = value;
        }
        #endregion
    }
}
