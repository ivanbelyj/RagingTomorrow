/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.SystemModules;
using AuroraFPSRuntime.SystemModules.ControllerModules;
using UnityEngine;

namespace AuroraFPSRuntime.WeaponModules.RecoilSystem
{
    [System.Serializable]
    public sealed class CameraRecoil
    {
        public enum SideForceMode
        {
            Static,
            Pattern,
            Random
        }

        [SerializeField]
        [Slider(0, 90.0f)]
        private float upForce = 2.0f;

        [SerializeField]
        [MinMaxSlider(0, 90.0f)]
        private Vector2 upForceLimit = new Vector2(0, 20);

        [SerializeField]
        private SideForceMode sideForceMode = SideForceMode.Random;

        [SerializeField]
        [Slider(-90, 90)]
        [VisibleIf("sideForceMode", "Static")]
        private float sideForce;

        [SerializeField]
        [VisibleIf("sideForceMode", "Pattern")]
        private bool loopSidePattern = false;

        [SerializeField]
        [ReorderableList]
        [VisibleIf("sideForceMode", "Pattern")]
        private float[] sidePattern;

        [SerializeField]
        [MinMaxSlider(-90, 90)]
        [VisibleIf("sideForceMode", "Random")]
        private Vector2 sideForceRange = new Vector2(-2, 2);

        [SerializeField]
        [MinMaxSlider(-10.0f, 10.0f)]
        private Vector2 sideForceLimit = new Vector2(-10.0f, 10.0f);

        [SerializeField]
        [MinMaxSlider(-90, 90)]
        private Vector2 shakeForce = new Vector2(-0.5f, 0.5f);

        [SerializeField]
        private float snappinessUp = 5.5f;

        [SerializeField]
        private float snappinessBack = 12.5f;

        [SerializeField]
        private bool additionShake = false;

        [SerializeField]
        [VisibleIf("additionShake")]
        private BounceShake.Settings shakeSettings;

        // Stored required components.
        private CameraShaker shaker;

        public CameraRecoil(float upForce, float sideForce, Vector2 shakeForce, float snappiness, float returnSpeed, CameraShaker shaker)
        {
            this.upForce = upForce;
            this.sideForce = sideForce;
            this.shakeForce = shakeForce;
            this.shaker = shaker;
            this.snappinessUp = snappiness;
            this.snappinessBack = returnSpeed;
            sideForceMode = SideForceMode.Static;
            loopSidePattern = false;
        }

        public CameraRecoil(float upForce, float[] sideForcePattern, bool loopSidePattern, Vector2 shakeForce, float snappinessUp, float snappinessBack, CameraShaker shaker)
        {
            this.upForce = upForce;
            this.sidePattern = sideForcePattern;
            this.loopSidePattern = loopSidePattern;
            this.shakeForce = shakeForce;
            this.shaker = shaker;
            this.snappinessUp = snappinessUp;
            this.snappinessBack = snappinessBack;
            sideForceMode = SideForceMode.Pattern;
        }

        public CameraRecoil(float upForce, Vector2 sideForceRange, Vector2 shakeForce, float snappinessUp, float snappinessBack, CameraShaker shaker)
        {
            this.upForce = upForce;
            this.sideForceRange = sideForceRange;
            this.shakeForce = shakeForce;
            this.shaker = shaker;
            this.snappinessUp = snappinessUp;
            this.snappinessBack = snappinessBack;
            sideForceMode = SideForceMode.Random;
            loopSidePattern = false;
        }

        public void MoveNext(ref int index, ref Vector3 vector)
        {
            vector.x = Mathf.Clamp(vector.x - upForce, -upForceLimit.y, upForceLimit.x);
            switch (sideForceMode)
            {
                case SideForceMode.Static:
                    vector.y += sideForce;
                    break;
                case SideForceMode.Pattern:
                    if (sidePattern.Length > 0)
                    {
                        if (loopSidePattern)
                            index = (index + 1) % sidePattern.Length;
                        else
                            index = Mathf.Clamp(index + 1, 0, sidePattern.Length - 1);
                        vector.y += sidePattern[index];
                    }
                    break;
                case SideForceMode.Random:
                    vector.y += Random.Range(sideForceRange.x, sideForceRange.y);
                    break;
            }
            vector.y = Mathf.Clamp(vector.y, sideForceLimit.x, sideForceLimit.y);

            if(additionShake && shaker != null)
            {
                shaker.RegisterShake(new BounceShake(shakeSettings));
            }
        }

        #region [Getter / Setter]
        public float GetUpForce()
        {
            return upForce;
        }

        public void SetUpForce(float value)
        {
            upForce = value;
        }

        public Vector2 GetUpForceLimit()
        {
            return upForceLimit;
        }

        public void SetUpForceLimit(Vector2 value)
        {
            upForceLimit = value;
        }

        public SideForceMode GetSideForceMode()
        {
            return sideForceMode;
        }

        public void SetSideForceMode(SideForceMode value)
        {
            sideForceMode = value;
        }

        public float GetSideForce()
        {
            return sideForce;
        }

        public void SetSideForce(float value)
        {
            sideForce = value;
        }

        public bool LoopSidePattern()
        {
            return loopSidePattern;
        }

        public void LoopSidePattern(bool value)
        {
            loopSidePattern = value;
        }

        public float[] GetSidePattern()
        {
            return sidePattern;
        }

        public void SetSidePattern(float[] value)
        {
            sidePattern = value;
        }

        public float GetSidePatternForce(int index)
        {
            return sidePattern[index];
        }

        public void SetSidePatternForce(int index, float value)
        {
            sidePattern[index] = value;
        }

        public int GetSidePatternLength()
        {
            return sidePattern.Length;
        }

        public Vector2 GetSideForceRange()
        {
            return sideForceRange;
        }

        public void SetSideForceRange(Vector2 value)
        {
            sideForceRange = value;
        }

        public Vector2 GetSideForceLimit()
        {
            return sideForceLimit;
        }

        public void SetSideForceLimit(Vector2 value)
        {
            sideForceLimit = value;
        }

        public Vector2 GetShakeForceRange()
        {
            return shakeForce;
        }

        public void SetShakeForceRange(Vector2 value)
        {
            shakeForce = value;
        }

        public float GetSnappinessUp()
        {
            return snappinessUp;
        }

        public void SetSnappinessUp(float value)
        {
            snappinessUp = value;
        }

        public float GetSnappinessBack()
        {
            return snappinessBack;
        }

        public void SetSnappinessBack(float value)
        {
            snappinessBack = value;
        }

        public bool AdditionShake()
        {
            return additionShake;
        }

        public void AdditionShake(bool value)
        {
            additionShake = value;
        }

        public BounceShake.Settings GetShakeSettings()
        {
            return shakeSettings;
        }

        public void SetShakeSettings(BounceShake.Settings value)
        {
            shakeSettings = value;
        }

        public CameraShaker GetShaker()
        {
            return shaker;
        }

        public void SetShaker(CameraShaker value)
        {
            shaker = value;
        }
        #endregion
    }
}