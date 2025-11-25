/* ================================================================
   ---------------------------------------------------
   Project   :    Aurora FPS Engine
   Publisher :    Renowned Games
   Author    :    Tamerlan Shakirov
   ---------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.InputSystem;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Hardware/Cursor/Cursor Handler")]
    [DisallowMultipleComponent]
    public sealed class CursorHandler : MonoBehaviour
    {
        private enum Action
        {
            Show,
            Hide
        }

        [SerializeField]
        private Action action = Action.Show;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            switch (action)
            {
                case Action.Show:
                    InputReceiver.HardwareCursor(true);
                    break;
                case Action.Hide:
                    InputReceiver.HardwareCursor(false);
                    break;
            }
        }
    }
}