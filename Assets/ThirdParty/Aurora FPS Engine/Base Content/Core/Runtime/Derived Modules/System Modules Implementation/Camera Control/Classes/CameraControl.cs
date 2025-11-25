/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System;

namespace AuroraFPSRuntime.SystemModules.ControllerModules
{
    [System.Serializable]
    [Obsolete("Use Entity Camera implementation instead.")]
    public abstract class CameraControl : ICameraControl, ICameraZoom
    {
        // Stored required components.
        private Controller controller;

        /// <summary>
        /// Initialize camera control instance.
        /// </summary>
        /// <param name="controller">Target character controller reference.</param>
        public virtual void Initialize(Controller controller)
        {
            this.controller = controller;
        }

        /// <summary>
        /// Called every frame, before any controller update functions call.
        /// </summary>
        protected abstract void Update();

        #region [ICameraZoom Implementation]
        public abstract void ZoomIn();

        public abstract void ZoomOut();

        public abstract bool IsZooming();
        #endregion

        #region [Internal Merhods]
        /// <summary>
        /// Internal update wrapper.
        /// </summary>
        internal void Internal_Update()
        {
            Update();
        }
        #endregion

        #region [Event Callback Functions]
        /// <summary>
        /// Called when camera start zooming.
        /// </summary>
        public abstract event Action OnStartZoomCallback;

        /// <summary>
        /// Called when camera stop zooming.
        /// </summary>
        public abstract event Action OnStopZoomCallback;

        /// <summary>
        /// Called every time before start start zooming to check, can be camera start zoom.
        /// </summary>
        public abstract event Func<bool> ZoomConditionCallback;
        #endregion

        #region [Getter / Setter]
        public Controller GetController()
        {
            return controller;
        }
        #endregion
    }
}