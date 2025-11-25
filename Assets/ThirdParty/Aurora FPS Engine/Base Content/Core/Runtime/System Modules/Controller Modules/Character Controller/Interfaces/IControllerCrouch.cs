/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime
{
    public interface IControllerCrouch
    {
        /// <summary>
        /// Crouch controller.
        /// </summary>
        /// <param name="value">True to crouch, false to stand./param>
        void Crouch(bool value);

        /// <summary>
        /// Controller is crouching
        /// </summary>
        bool IsCrouched();

        /// <summary>
        /// Controller speed while is crouching.
        /// </summary>
        /// <returns></returns>
        float GetCrouchSpeed();
    }
}