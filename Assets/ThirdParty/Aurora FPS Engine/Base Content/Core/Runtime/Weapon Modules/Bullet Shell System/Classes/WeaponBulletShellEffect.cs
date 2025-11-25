/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.ValueTypes;
using AuroraFPSRuntime.SystemModules;
using UnityEngine;

namespace AuroraFPSRuntime.WeaponModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/Weapon Modules/Bullet Shell System/Bullet Shell Effect")]
    [DisallowMultipleComponent]
    public sealed class WeaponBulletShellEffect : MonoBehaviour
    {
        [System.Serializable]
        private class ShellThrowEvent : CallbackEvent { }

        [SerializeField]
        private ShellThrowEvent shellThrowEvent;

        [SerializeField] 
        [NotNull]
        private BulletShell bulletShell;

        [SerializeField] 
        [NotNull]
        private Vector3 throwPoint = Vector3.zero;

        [SerializeField]
        private Vector3 throwDirection = Vector3.zero; 

        [SerializeField] 
        [MinValue(0.1f)]
        private float throwForce = 1.5f;


        private void Awake()
        {
            shellThrowEvent.RegisterCallback(ThrowShell);
        }

        private void ThrowShell()
        {
            Vector3 relativeThrowPoint = transform.TransformPoint(throwPoint);
            Vector3 relativeThrowDirection = transform.TransformPoint(throwDirection);
            Vector3 direction = relativeThrowDirection - relativeThrowPoint;
            BulletShell bulletShellClone = PoolManager.GetRuntimeInstance().CreateOrPop<BulletShell>(bulletShell, relativeThrowPoint, Quaternion.LookRotation(RandomizeDirection(direction)) * Quaternion.Euler(180, 90, 0));
            bulletShellClone.Throw(direction.normalized, throwForce);
        }

        private Vector3 RandomizeDirection(Vector3 direction)
        {
            direction.x += Random.Range(-0.5f, 0.5f);
            direction.y += Random.Range(-0.5f, 0.5f);
            return direction;
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 relativeThrowPoint = transform.TransformPoint(throwPoint);
            Vector3 relativeThrowDirection = transform.TransformPoint(throwDirection);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(relativeThrowPoint, relativeThrowDirection);

            Gizmos.color = Color.red;
            relativeThrowPoint.z *= 1.45f;
            relativeThrowDirection.z *= 1.45f;
            Gizmos.DrawLine(relativeThrowPoint, relativeThrowDirection);
        }

        #region [Getter / Setter]
        public BulletShell GetBulletShell()
        {
            return bulletShell;
        }

        public void SetBulletShell(BulletShell value)
        {
            bulletShell = value;
        }

        public float GetThrowForce()
        {
            return throwForce;
        }

        public void SetThrowForce(float value)
        {
            throwForce = value;
        }
        #endregion
    }
}
