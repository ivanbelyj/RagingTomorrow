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
    public enum Degree
    {
        Linear,
        Quadratic,
        Cubic,
        Quadric
    }

    public static class Power
    {
        public static float Evaluate(float value, Degree degree)
        {
            switch (degree)
            {
                case Degree.Linear:
                    return value;
                case Degree.Quadratic:
                    return value * value;
                case Degree.Cubic:
                    return value * value * value;
                case Degree.Quadric:
                    return value * value * value * value;
                default:
                    return value;
            }
        }
    }
}
