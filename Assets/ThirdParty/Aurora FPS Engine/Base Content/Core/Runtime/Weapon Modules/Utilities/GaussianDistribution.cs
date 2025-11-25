/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;

namespace AuroraFPSRuntime
{
    public class GaussianDistribution
    {
        float _spareResult;
        bool _nextResultReady = false;

        public float Next()
        {
            float result;

            if (_nextResultReady)
            {
                result = _spareResult;
                _nextResultReady = false;

            }
            else
            {
                float s = -1f, x, y;

                do
                {
                    x = 2f * Random.value - 1f;
                    y = 2f * Random.value - 1f;
                    s = x * x + y * y;
                } while (s < 0f || s >= 1f);

                s = Mathf.Sqrt((-2f * Mathf.Log(s)) / s);

                _spareResult = y * s;
                _nextResultReady = true;

                result = x * s;
            }

            return result;
        }

        public float Next(float mean, float sigma = 1f) => mean + sigma * Next();

        public float Next(float mean, float sigma, float min, float max)
        {
            float x = min - 1f; while (x < min || x > max) x = Next(mean, sigma);
            return x;
        }
    }
}
