/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AuroraFPSRuntime.CoreModules.InputSystem
{
    public static class InputReceiver
    {
        /// <summary>
        /// Current input action map asset.
        /// </summary>
		public static InputActionAsset Asset { get; private set; }

        /// <summary>
        /// Current input config.
        /// </summary>
        public static InputConfig Config { get; private set; }

        /// <summary>
        /// Called once before splash screen.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            Config = Resources.LoadAll<InputConfig>(string.Empty).FirstOrDefault();
            if(Config == null)
            {
                Config = ScriptableObject.CreateInstance<InputConfig>();
            }

            Asset = Resources.LoadAll<InputActionAsset>(string.Empty).FirstOrDefault();
            Debug.Assert(Asset != null, string.Format("<b><color=#FF0000>Input action asset not found!\nCreate or move the current InputActionAsset to resources folder in your project.</color></b>"));
            Asset.Enable();
            MovementVerticalAction = Asset.FindAction(Config.GetMovementVerticalPath(), false);
            MovementHorizontalAction = Asset.FindAction(Config.GetMovementHorizontalPath(), false);
            CameraVerticalAction = Asset.FindAction(Config.GetCameraVerticalPath(), false);
            CameraHorizontalAction = Asset.FindAction(Config.GetCameraHorizontalPath(), false);
            JumpAction = Asset.FindAction(Config.GetJumpPath(), false);
            CrouchAction = Asset.FindAction(Config.GetCrouchPath(), false);
            SprintAction = Asset.FindAction(Config.GetSprintPath(), false);
            LightWalkAction = Asset.FindAction(Config.GetLightWalkPath(), false);
            InteractAction = Asset.FindAction(Config.GetInteractPath(), false);
            ZoomAction = Asset.FindAction(Config.GetZoomPath(), false);
            AttackAction = Asset.FindAction(Config.GetAttackPath(), false);
            ReloadAction = Asset.FindAction(Config.GetReloadPath(), false);
            SwitchFireModeAction = Asset.FindAction(Config.GetSwitchFireModePath(), false);
            ScrollItemsAction = Asset.FindAction(Config.GetScrollItemsPath(), false);
            HideItemAction = Asset.FindAction(Config.GetHideItemPath(), false);
            TossItemAction = Asset.FindAction(Config.GetTossItemPath(), false);
            GrabObjectAction = Asset.FindAction(Config.GetGrabObjectPath(), false);
            ThrowObjectAction = Asset.FindAction(Config.GetThrowObjectPath(), false);
        }

        public static void EnableMap(string name)
        {
            InputActionMap actionMap = Asset.FindActionMap(name, false);
            if (actionMap != null && (EnableMapPredicate?.Invoke(name) ?? true))
            {
                actionMap.Enable();
            }
        }

        public static void DisableMap(string name)
        {
            InputActionMap actionMap = Asset.FindActionMap(name, false);
            actionMap?.Disable();
        }

        public static void EnableAction(string path)
        {
            InputAction action = Asset.FindAction(path, false);
            action?.Enable();
        }

        public static void DisableAction(string path)
        {
            InputAction action = Asset.FindAction(path, false);
            action?.Disable();
        }

        public static void HardwareCursor(bool value)
        {
            Cursor.lockState = value ? CursorLockMode.Confined : CursorLockMode.Locked;
            Cursor.visible = value;
        }

        #region [Event Callback Functions]
        /// <summary>
        /// Additional condition for enabling specified input map.
        /// <br><i><b>Parameter type of (string)</b>: Specified map name of input map asset.</i></br>
        /// </summary>
        public static event System.Predicate<string> EnableMapPredicate;
        #endregion

        #region [Getter]
        public static InputAction MovementVerticalAction { get; private set; }
        public static InputAction MovementHorizontalAction { get; private set; }
        public static InputAction CameraVerticalAction { get; private set; }
        public static InputAction CameraHorizontalAction { get; private set; }
        public static InputAction JumpAction { get; private set; }
        public static InputAction CrouchAction { get; private set; }
        public static InputAction SprintAction { get; private set; }
        public static InputAction LightWalkAction { get; private set; }
        public static InputAction InteractAction { get; private set; }
        public static InputAction ZoomAction { get; private set; }
        public static InputAction AttackAction { get; private set; }
        public static InputAction ReloadAction { get; private set; }
        public static InputAction SwitchFireModeAction { get; private set; }
        public static InputAction ScrollItemsAction { get; private set; }
        public static InputAction HideItemAction { get; private set; }
        public static InputAction TossItemAction { get; private set; }
        public static InputAction GrabObjectAction { get; private set; }
        public static InputAction ThrowObjectAction { get; private set; }
        #endregion
    }
}