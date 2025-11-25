/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System.Collections.Generic;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.Vision
{
    public interface IVisionTarget
    {
        IReadOnlyList<Transform> GetVisibleTargets();

        Transform GetFirstTarget();

        Transform GetNearestTarget();

        Transform GetDistantTarget();
    }
}