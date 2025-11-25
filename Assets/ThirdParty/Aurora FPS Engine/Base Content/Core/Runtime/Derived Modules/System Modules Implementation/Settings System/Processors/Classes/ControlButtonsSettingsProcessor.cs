/* ================================================================
   ---------------------------------------------------
   Project   :    Aurora FPS Engine
   Publisher :    Renowned Games
   Author    :    Alexandra Averyanova
   ---------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.InputSystem;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace AuroraFPSRuntime.SystemModules.Settings
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Settings System/Processors/Control Buttons Settings Processor/Control Buttons Settings Processor")]
    public class ControlButtonsSettingsProcessor : SettingsProcessor
    {
        [SerializeField]
        [NotNull]
        private string mapName = "Player";

        [SerializeField]
        [NotNull]
        private string actionName = "<Action>";

        [SerializeField]
        [ReorderableList(ElementLabel = null)]
        private string[] devices;

        [SerializeField]
        [NotNull]
        private bool isBindingComposite;

        [SerializeField]
        [VisibleIf("isBindingComposite")]
        [Indent]
        private string axisName;

        [SerializeField]
        [NotNull]
        private Text inputField;

        [SerializeField]
        [NotNull]
        private ControlButtonRebinder rebinder;
         
        // Stored required properties.
        private InputActionMap map;
        private InputAction action;
        private int bindingIndex;
        private ControlButtonsCollisionsHandler collisionsHandler;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            map = InputReceiver.Asset.FindActionMap(mapName);
            rebinder.SetMap(map);
            action = map.FindAction(actionName);
            rebinder.SetAction(action);
            FindSpecifiedBinding();
            rebinder.SetBindingIndex(bindingIndex);
            collisionsHandler = GetComponentInParent<ControlButtonsCollisionsHandler>();
        }

        /// <summary>
        /// Called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            InputBindingsStore.Add(action.bindings[bindingIndex], inputField);
        }

        /// <summary>
        /// Save new control buttons.
        /// </summary>
        /// <returns>New control buttons as object.</returns>
        protected override object OnSave()
        {
            return action.bindings[bindingIndex].effectivePath;
        }

        /// <summary>
        /// Load control buttons and display them in menu.
        /// </summary>
        /// <param name="value">Control buttons as object.</param>
        protected override void OnLoad(object value)
        {
            string[] pathSplited = ((string)value).Split('/');
            inputField.text = pathSplited[pathSplited.Length - 1].ToUpper();
            RebindIfChanged((string)value);
            collisionsHandler.SetLoadedProcessorCount(collisionsHandler.GetLoadedProcessorCount() + 1);
        }

        /// <summary>
        /// Get default control button value.
        /// </summary>
        /// <returns>Default control name.</returns>
        public override object GetDefaultValue()
        {
            InputBinding binding = action.bindings[bindingIndex];
            return binding.effectivePath;
        }

        /// <summary>
        /// Find index of binding with specified group and axis.
        /// </summary>
        private void FindSpecifiedBinding()
        {
            if (!isBindingComposite) {
                bindingIndex = action.bindings.IndexOf(bind => IsAttachedToSpecifiedDevices(bind));
            }
            else { 
                bindingIndex = action.bindings.IndexOf(bind => IsAttachedToSpecifiedDevices(bind) &&
                bind.isPartOfComposite && bind.name == axisName.ToLower());
            }
        }

        /// <summary>
        /// Check if input binding is attached to any of specified devices.
        /// </summary>
        /// <param name="binding">Input binding.</param>
        /// <returns>The result of checking.</returns>
        private bool IsAttachedToSpecifiedDevices(InputBinding binding) 
        {
            return Array.Exists(devices, d => binding.effectivePath.Contains(d));
        }

        /// <summary>
        /// Rebind path if it's value was changed.
        /// </summary>
        private void RebindIfChanged(string path)
        {
            InputBinding binding = action.bindings[bindingIndex];
            if(path != binding.effectivePath)
            {
                action.ApplyBindingOverride(bindingIndex, path);
                InputBinding newBinding = action.bindings[bindingIndex];
                InputBindingsStore.ReplaceBinding(inputField, newBinding);
            }
        }

        #region [Getter / Setter]
        public string GetMapName()
        {
            return mapName;
        }

        public void SetMapName(string value)
        {
            mapName = value;
        }

        public Text GetInputField()
        {
            return inputField;
        }

        public void SetInputField(Text value)
        {
            inputField = value;
        }

        public string GetActionName()
        {
            return actionName;
        }

        public void SetActionName(string value)
        {
            actionName = value;
        }

        public string[] GetDevices()
        {
            return devices;
        }

        public void SetDevices(string[] value)
        {
            devices = value;
        }

        public bool GetIsBindingComposite()
        {
            return isBindingComposite;
        }

        public void SetIsBindingComposite(bool value)
        {
            isBindingComposite = value;
        }

        public string GetAxisName()
        {
            return axisName;
        }

        public void SetAxisName(string value)
        {
            axisName = value;
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

        public ControlButtonRebinder GetRebinder()
        {
            return rebinder;
        }

        public void SetRebinder(ControlButtonRebinder value)
        {
            rebinder = value;
        }

        public ControlButtonsCollisionsHandler GetCollisionsHandler()
        {
            return collisionsHandler;
        }

        public void SetCollisionsHandler(ControlButtonsCollisionsHandler value)
        {
            collisionsHandler = value;
        }
        #endregion
    }
}
