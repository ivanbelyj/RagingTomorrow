/* ================================================================
   ---------------------------------------------------
   Project   :    Aurora FPS Engine
   Publisher :    Renowned Games
   Author    :    Alexandra Averyanova
   ---------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace AuroraFPSRuntime.SystemModules.Settings
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Editable/Control Button Rebinder")]
    [DisallowMultipleComponent]
    public sealed class ControlButtonRebinder : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField]
        [NotNull]
        private Text inputField;

        [SerializeField]
        private string cancelKeyPath;

        [SerializeField]
        [Foldout("Event Callbacks", Style = "Header")]
        private UnityEvent onRebindWaitCallback;

        [SerializeField]
        [Foldout("Event Callbacks", Style = "Header")]
        private UnityEvent onRebindCompleteCallback;

        [SerializeField]
        [Foldout("Event Callbacks", Style = "Header")]
        private UnityEvent onRebindCanceledCallback;

        private InputActionMap map;
        private InputAction action;
        private int bindingIndex;
        private ControlButtonsCollisionsHandler collisionsDetector;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            collisionsDetector = GetComponentInParent<ControlButtonsCollisionsHandler>();
            OnRebindWaitCallback += () => onRebindWaitCallback?.Invoke();
            OnRebindCompleteCallback += (opertation, binding, oldKey) => onRebindCompleteCallback?.Invoke();
            OnRebindCompleteCallback += collisionsDetector.CheckRebindCollisions;
            OnRebindCanceledCallback += (cancelKey) => onRebindCanceledCallback?.Invoke();

            inputField.text = inputField.text.ToUpper();
        }

        /// <summary>
        /// Called when the input field is clicked.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public void OnPointerDown(PointerEventData eventData)
        {
            string oldKey = inputField.text;
            InputBinding oldBinding = action.bindings[bindingIndex];
            inputField.text = "...";
            action.PerformInteractiveRebinding(bindingIndex).WithCancelingThrough(cancelKeyPath)
                .OnComplete(operation => OnRebind(operation, oldBinding, oldKey))
                .OnCancel(operation => OnCancel(oldKey)).Start();
            OnRebindWaitCallback?.Invoke();
        }

        /// <summary>
        /// Called when rebinding is completed.
        /// </summary>
        /// <param name="operation">Rebinding operation.</param>
        private void OnRebind(InputActionRebindingExtensions.RebindingOperation operation,
            InputBinding oldBinding, string oldKey)
        {
            inputField.text = operation.selectedControl.name.ToUpper();
            InputBinding newBinding = action.bindings[bindingIndex];
            InputBindingsStore.ReplaceBinding(inputField, newBinding);
            OnRebindCompleteCallback?.Invoke(operation, action.bindings[bindingIndex], oldKey);
        }

        /// <summary>
        /// Called when rebinding is being canceled.
        /// </summary>
        /// <param name="oldKey">Old control name.</param>
        private void OnCancel(string oldKey)
        {
            inputField.text = oldKey.ToUpper();
            OnRebindCanceledCallback?.Invoke(oldKey);
        }

        #region [Event Callback Functions]
        /// <summary>
        /// Called when rebinding operation is started.
        /// </summary>
        public event Action OnRebindWaitCallback;

        /// <summary>
        /// Called when rebinding operation is completed.
        /// </summary>
        public event Action<InputActionRebindingExtensions.RebindingOperation, InputBinding, string> 
            OnRebindCompleteCallback;

        /// <summary>
        /// Called when rebinding operation is canceled.
        /// </summary>
        public event Action<string> OnRebindCanceledCallback;
        #endregion

        #region [Getter / Setter]
        public Text GetInputField()
        {
            return inputField;
        }

        public void SetInputField(Text value)
        {
            inputField = value;
        }

        public string GetCancelControl()
        {
            return cancelKeyPath;
        }

        public void SetCancelControl(string value)
        {
            cancelKeyPath = value;
        }

        public InputActionMap GetMap()
        {
            return map;
        }

        public void SetMap(InputActionMap value)
        {
            map = value;
        }

        public InputAction GetAction()
        {
            return action;
        }

        public void SetAction(InputAction value)
        {
            action = value;
        }

        public int GetBindingIndex()
        {
            return bindingIndex;
        }

        public void SetBindingIndex(int value)
        {
            bindingIndex = value;
        }

        public ControlButtonsCollisionsHandler GetCollisionsDetector()
        {
            return collisionsDetector;
        }

        public void SetCollisionsDetector(ControlButtonsCollisionsHandler value)
        {
            collisionsDetector = value;
        }
        #endregion
    }
}
