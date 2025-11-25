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
    [AddComponentMenu("Aurora FPS Engine/Weapon Modules/Shooting/Raycast Shotgun Shooting System")]
    [DisallowMultipleComponent]
    public class WeaponRayShotgunShootingSystem : WeaponRayShootingSystem
    {
        // Stored required properties.
        private ShotgunBulletItem shotgunBulletItem;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            shotgunBulletItem = GetBulletItem() as ShotgunBulletItem;
            Debug.Assert(shotgunBulletItem != null, $"<b><color=#FF0000>Ray Shotgun Shooting System can work only with shotgun bullet item types.\nAttach reference of shotgun bullet item type to {gameObject.name}<i>(gameobject)</i> -> Ray Shotgun Shooting System <i>(component)</i> -> Bullet Item<i>(field)</i>.</color></b>");
        }

        /// <summary>
        /// Implement this method to make logic of shooting. 
        /// </summary>
        /// <param name="origin">Origin vection of shoot.</param>
        /// <param name="direction">Direction vector of shoot.</param>
        protected override void MakeShoot(Vector3 origin, Vector3 direction)
        {
            for (int i = 0; i < shotgunBulletItem.GetBallNumber(); i++)
            {
                base.MakeShoot(origin, shotgunBulletItem.GenerateVariance(direction));
            }
        }
    }
}