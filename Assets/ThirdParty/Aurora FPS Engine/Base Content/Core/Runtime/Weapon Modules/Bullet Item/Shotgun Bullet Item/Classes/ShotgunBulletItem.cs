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

namespace AuroraFPSRuntime.WeaponModules
{
    [HideScriptField]
    [CreateAssetMenu(fileName = "New Shotgun Bullet Item", menuName = "Aurora FPS Engine/Weapon/Bullet/Shotgun Bullet", order = 101)]
    public class ShotgunBulletItem : BulletItem
    {
        private static readonly GaussianDistribution gaussianDistribution = new GaussianDistribution();

        [SerializeField]
        [MinValue(0)]
        [Order(54)]
        private int pelletsNumber = 6;

        [SerializeField]
        [MinMaxSlider(-1f, 1f)]
        [Order(55)]
        private Vector2 pelletsVariance = new Vector2(-0.1f, 0.1f);

        public override float GetDamage()
        {
            return base.GetDamage() / pelletsNumber;
        }

        public override float GetImpactImpulse()
        {
            return base.GetImpactImpulse() / pelletsNumber;
        }

        /// <summary>
        /// Generate variance for fire point.
        /// </summary>
        /// <param name="target">Target transform</param>
        public Vector3 GenerateVariance(Vector3 target)
        {
            target.x += gaussianDistribution.Next(0, 1, pelletsVariance.x, pelletsVariance.y);
            target.y += gaussianDistribution.Next(0, 1, pelletsVariance.x, pelletsVariance.y);
            target.z += gaussianDistribution.Next(0, 1, pelletsVariance.x, pelletsVariance.y);
            return target;
        }

        #region [Getter / Setter]
        public int GetBallNumber()
        {
            return pelletsNumber;
        }

        public void SetBallNumber(int value)
        {
            pelletsNumber = value;
        }

        public Vector2 GetBallVariance()
        {
            return pelletsVariance;
        }

        public void SetBallVariance(Vector2 value)
        {
            pelletsVariance = value;
        }
        #endregion
    }
}