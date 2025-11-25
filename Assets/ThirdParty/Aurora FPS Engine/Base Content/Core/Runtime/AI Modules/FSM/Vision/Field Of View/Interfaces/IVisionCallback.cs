/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;
using System;

namespace AuroraFPSRuntime.AIModules.Vision
{
    public interface IVisionCallback
    {
        event Action<Transform> OnTargetBecomeVisible;

        event Action OnTargetsBecomeInvisible;
    }
}