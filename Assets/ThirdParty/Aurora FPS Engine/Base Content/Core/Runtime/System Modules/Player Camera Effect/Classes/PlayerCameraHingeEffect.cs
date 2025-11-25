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

namespace AuroraFPSRuntime.SystemModules.CameraSystems.Effects
{
    [System.Serializable]
    public abstract class PlayerCameraHingeEffect : PlayerCameraEffect
    {
        [SerializeField]
        [NotNull]
        protected Transform hinge;

        #region [Getter / Setter]
        public Transform GetHinge()
        {
            return hinge;
        }

        public void SetHinge(Transform value)
        {
            hinge = value;
        }
        #endregion
    }
}