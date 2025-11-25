/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.CoreModules.InputSystem
{
    /// <summary>
    /// Input name conventions of the all Aurora FPS Engine inputs.
    /// </summary>
    public static class INC
    {
        /*
         *  _____________INPUT AXES_____________
         *
         *  All input name axes that used in SDK.
         *  All these axes must be contained in default Unity input manager.
         */
        public const string CharVertical = "Character Vertical";
        public const string CharHorizontal = "Character Horizontal";
        public const string CamVertical = "Camera Vertical";
        public const string CamHorizontal = "Camera Horizontal";
        public const string MouseWheel = "Mouse ScrollWheel";

        public readonly static string[] Axes = new string[] { CharVertical, CharHorizontal, CamVertical, CamHorizontal, MouseWheel };

        /*
         *  _____________INPUT ACTION_____________
         * 
         *  All input name action that used in SDK.
         *  All these action must be contained in default Unity input manager.
         */
        public const string Sprint = "Sprint";
        public const string LightWalk = "Light Walk";
        public const string Jump = "Jump";
        public const string Crouch = "Crouch";
        public const string Attack = "Fire";
        public const string ChangeFireMode = "Change Fire Mode";
        public const string Zoom = "Zoom";
        public const string Reload = "Reload";
        public const string Grab = "Grab";
        public const string Drop = "Drop";
        public const string LeftTilt = "Left Tilt";
        public const string RightTilt = "Right Tilt";

        public readonly static string[] Buttons = new string[] { Crouch, Sprint, Attack, Reload, Zoom, Grab, ChangeFireMode, LightWalk, Jump, Drop, LeftTilt, RightTilt };
    }
}