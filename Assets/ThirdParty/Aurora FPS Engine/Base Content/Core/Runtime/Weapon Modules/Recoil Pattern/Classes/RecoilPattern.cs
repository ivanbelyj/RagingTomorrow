/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.SystemModules;
using UnityEngine;

namespace AuroraFPSRuntime
{
    [System.Serializable]
    [System.Obsolete("Use Recoil Config instead.")]
    public class RecoilPattern
    {
        public enum Iteration
        {
            Pass,
            Loop,
            Random
        }

        [System.Serializable]
        public struct BulletSpread
        {
            public static readonly BulletSpread zero = new BulletSpread(Vector2.zero, Vector2.zero);

            public Vector2 xAxis;
            public Vector2 yAxis;

            public BulletSpread(Vector2 xAxis, Vector2 yAxis)
            {
                this.xAxis = xAxis;
                this.yAxis = yAxis;
            }
        }

        public Iteration recoilIteration = Iteration.Loop;
        public float recoilDuration = 0.07f;
        public Vector2[] recoil;

        public Iteration spreadIteration = Iteration.Pass;
        public BulletSpread[] spread;
        
        public BounceShake.Settings shakeSettings;

        public Vector2 FetchRecoil(ref int index)
        {
            if(recoil != null && recoil.Length > 0)
            {
                switch (recoilIteration)
                {
                    case Iteration.Pass:
                        index = Mathf.Clamp(index + 1, 0, recoil.Length - 1);
                        break;
                    case Iteration.Loop:
                        index = (index + 1) % recoil.Length;
                        break;
                    case Iteration.Random:
                        index = Random.Range(0, recoil.Length);
                        break;
                }
                return recoil[index];
            }
            return Vector2.zero;
        }

        public BulletSpread FetchSpread(ref int index)
        {
            if (spread != null && spread.Length > 0)
            {
                switch (spreadIteration)
                {
                    case Iteration.Pass:
                        index = Mathf.Clamp(index + 1, 0, spread.Length - 1);
                        break;
                    case Iteration.Loop:
                        index = (index + 1) % spread.Length;
                        break;
                    case Iteration.Random:
                        index = Random.Range(0, spread.Length);
                        break;
                }
                return spread[index];
            }
            return BulletSpread.zero;
        }

        #region [Getter / Setter]

        #endregion
    }
}