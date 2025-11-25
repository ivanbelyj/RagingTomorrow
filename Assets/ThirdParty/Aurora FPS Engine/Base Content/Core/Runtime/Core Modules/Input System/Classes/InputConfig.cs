/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEngine;

namespace AuroraFPSRuntime.CoreModules.InputSystem
{
    [HideScriptField]
    public sealed class InputConfig : ScriptableObject
    {
        [SerializeField]
        [Label("Movement Vertical")]
        [Foldout("Controller Actions", Style = "Header")]
        [NotEmpty]
        [Indent(1)]
        private string movementVerticalPath = "Player/Movement Vertical";

        [SerializeField]
        [Label("Movement Horizontal")]
        [Foldout("Controller Actions", Style = "Header")]
        [NotEmpty]
        [Indent(1)]
        private string movementHorizontalPath = "Player/Movement Horizontal";

        [SerializeField]
        [Label("Camera Vertical")]
        [Foldout("Controller Actions", Style = "Header")]
        [NotEmpty]
        [Indent(1)]
        private string cameraVerticalPath = "Player/Camera Vertical";

        [SerializeField]
        [Label("Camera Horizontal")]
        [Foldout("Controller Actions", Style = "Header")]
        [NotEmpty]
        [Indent(1)]
        private string cameraHorizontalPath = "Player/Camera Horizontal";

        [SerializeField]
        [Label("Jump")]
        [Foldout("Controller Actions", Style = "Header")]
        [NotEmpty]
        [Indent(1)]
        private string jumpPath = "Player/Jump";

        [SerializeField]
        [Label("Crouch")]
        [Foldout("Controller Actions", Style = "Header")]
        [NotEmpty]
        [Indent(1)]
        private string crouchPath = "Player/Crouch";

        [SerializeField]
        [Label("Sprint")]
        [Foldout("Controller Actions", Style = "Header")]
        [NotEmpty]
        [Indent(1)]
        private string sprintPath = "Player/Sprint";

        [SerializeField]
        [Label("Light Walk")]
        [Foldout("Controller Actions", Style = "Header")]
        [NotEmpty]
        [Indent(1)]
        private string lightWalkPath = "Player/Light Walk";

        [SerializeField]
        [Label("Zoom")]
        [Foldout("Controller Actions", Style = "Header")]
        [NotEmpty]
        [Indent(1)]
        private string zoomPath = "Player/Zoom";

        [SerializeField]
        [Label("Attack")]
        [Foldout("Weapon Actions", Style = "Header")]
        [NotEmpty]
        [Indent(1)]
        private string attackPath = "Player/Attack";

        [SerializeField]
        [Label("Reload")]
        [Foldout("Weapon Actions", Style = "Header")]
        [NotEmpty]
        [Indent(1)]
        private string reloadPath = "Player/Reload";

        [SerializeField]
        [Label("Switch Fire Mode")]
        [Foldout("Weapon Actions", Style = "Header")]
        [NotEmpty]
        [Indent(1)]
        private string switchFireModePath = "Player/Switch Fire Mode";

        [SerializeField]
        [Label("Scroll Items")]
        [Foldout("Inventory Actions", Style = "Header")]
        [NotEmpty]
        [Indent(1)]
        private string scrollItemsPath = "Player/Scroll Items";

        [SerializeField]
        [Label("Hide Item")]
        [Foldout("Inventory Actions", Style = "Header")]
        [NotEmpty]
        [Indent(1)]
        private string hideItemPath = "Player/Hide Item";

        [SerializeField]
        [Label("Toss Item")]
        [Foldout("Inventory Actions", Style = "Header")]
        [NotEmpty]
        [Indent(1)]
        private string tossItemPath = "Player/Toss Item";

        [SerializeField]
        [Label("Interact")]
        [Foldout("Other Actions", Style = "Header")]
        [NotEmpty]
        [Indent(1)]
        private string interactPath = "Player/Interact";

        [SerializeField]
        [Label("Grab Object")]
        [Foldout("Other Actions", Style = "Header")]
        [NotEmpty]
        [Indent(1)]
        private string grabObjectPath = "Player/Grab Object";

        [SerializeField]
        [Label("Throw Object")]
        [Foldout("Other Actions", Style = "Header")]
        [NotEmpty]
        [Indent(1)]
        private string throwObjectPath = "Player/Throw Object";

        #region [Getter / Setter]
        public string GetMovementVerticalPath()
        {
            return movementVerticalPath;
        }

        public string GetMovementHorizontalPath()
        {
            return movementHorizontalPath;
        }

        public string GetCameraVerticalPath()
        {
            return cameraVerticalPath;
        }

        public string GetCameraHorizontalPath()
        {
            return cameraHorizontalPath;
        }

        public string GetJumpPath()
        {
            return jumpPath;
        }

        public string GetCrouchPath()
        {
            return crouchPath;
        }

        public string GetSprintPath()
        {
            return sprintPath;
        }

        public string GetLightWalkPath()
        {
            return lightWalkPath;
        }

        public string GetZoomPath()
        {
            return zoomPath;
        }

        public string GetAttackPath()
        {
            return attackPath;
        }

        public string GetReloadPath()
        {
            return reloadPath;
        }

        public string GetSwitchFireModePath()
        {
            return switchFireModePath;
        }

        public string GetScrollItemsPath()
        {
            return scrollItemsPath;
        }

        public string GetHideItemPath()
        {
            return hideItemPath;
        }

        public string GetTossItemPath()
        {
            return tossItemPath;
        }

        public string GetInteractPath()
        {
            return interactPath;
        }

        public string GetGrabObjectPath()
        {
            return grabObjectPath;
        }

        public string GetThrowObjectPath()
        {
            return throwObjectPath;
        }
        #endregion
    }
}