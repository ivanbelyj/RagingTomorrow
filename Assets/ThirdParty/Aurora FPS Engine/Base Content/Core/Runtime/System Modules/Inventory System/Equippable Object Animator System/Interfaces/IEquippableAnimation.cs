/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.SystemModules
{
    public interface IEquippableAnimation
    {
        /// <summary>
        /// Play pull animation clip.
        /// </summary>
        void PlayPullAnimation();

        /// <summary>
        /// Play push animation clip.
        /// </summary>
        void PlayPushAnimation();
    }
}