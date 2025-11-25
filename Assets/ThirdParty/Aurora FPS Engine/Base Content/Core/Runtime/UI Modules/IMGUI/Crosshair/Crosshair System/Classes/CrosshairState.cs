/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.SystemModules.ControllerSystems;
using UnityEngine;

namespace AuroraFPSRuntime.UIModules.IMGUI.Crosshair
{
    [System.Serializable]
    public class CrosshairState
    {
        [SerializeField]
        private ControllerState state = ControllerState.Disabled;

        [SerializeField]
        [HideExpandButton]
        private CrosshairSpread spread = new CrosshairSpread(30.0f, 7.0f);

        public CrosshairState() { }

        public CrosshairState(ControllerState state, CrosshairSpread spread)
        {
            this.state = state;
            this.spread = spread;
        }

        #region [Getter / Setter]
        public ControllerState GetState()
        {
            return state;
        }

        public void SetState(ControllerState value)
        {
            state = value;
        }

        public CrosshairSpread GetCrosshairSpread()
        {
            return spread;
        }

        public void SetCrosshairSpread(CrosshairSpread value)
        {
            spread = value;
        }
        #endregion
    }
}
