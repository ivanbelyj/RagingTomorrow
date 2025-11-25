/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.SystemModules.InventoryModules;
using UnityEngine;

namespace AuroraFPSRuntime.WeaponModules
{
    [HideScriptField]
    [CreateAssetMenu(fileName = "New Bullet Item", menuName ="Aurora FPS Engine/Weapon/Bullet/Standard Bullet", order = 100)]
    [ComponentIcon("Bullet")]
    public class BulletItem : BaseItem, IBulletItem
    {
        [SerializeField]
        [MinValue(0)]
        [Order(50)]
        private float damage;

        [SerializeField]
        [Slider(0.01f, 1.0f)]
        [Order(51)]
        private float rangeModifier;

        [SerializeField]
        [MinValue(0.0f)]
        [Order(52)]
        private float impactImpulse;

        [SerializeField]
        [NotNull]
        [Order(99)]
        private DecalMapping decalMapping;

        /// <summary>
        /// Damage drops off relative specified distance with rounded to the nearest integer.
        /// </summary>
        public virtual float GetDamageDropsoff(float distance)
        {
            return Mathf.Round(GetDamage() * Mathf.Pow(rangeModifier, distance / 9.5f));
        }

        #region [Getter / Setter]
        public virtual float GetDamage()
        {
            return damage * WeaponUtilities.DamageMultiplier;
        }

        public void SetDamage(float value)
        {
            damage = value;
        }

        public virtual float GetRangeModifier()
        {
            return rangeModifier;
        }

        public void SetRangeModifier(float value)
        {
            rangeModifier = value;
        }

        public virtual float GetImpactImpulse()
        {
            return impactImpulse;
        }

        public void SetImpactImpulse(float value)
        {
            impactImpulse = value;
        }

        public DecalMapping GetDecalMapping()
        {
            return decalMapping;
        }

        public void SetDecalMapping(DecalMapping value)
        {
            decalMapping = value;
        }
        #endregion
    }
}