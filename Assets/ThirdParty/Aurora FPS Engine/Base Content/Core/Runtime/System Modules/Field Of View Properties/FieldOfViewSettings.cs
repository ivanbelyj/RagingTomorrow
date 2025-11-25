/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;

namespace AuroraFPSRuntime.SystemModules
{
    [System.Serializable]
    public class FieldOfViewSettings
    {
        [SerializeField] 
        private float value = 85;

        [SerializeField] 
        private float duration = 0.25f;

        [SerializeField]
        private AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [SerializeField]
        private bool additive = false;

        public FieldOfViewSettings(float value, float duration)
        {
            this.value = Mathf.Clamp(value, 0.01f, 179);
            this.duration = Mathf.Max(0.01f, duration);
        }

        public FieldOfViewSettings(float value, float duration, AnimationCurve curve) : this(value, duration)
        {
            this.curve = curve;
        }

        public FieldOfViewSettings(float value, float duration, AnimationCurve curve, bool additive) : this(value, duration, curve)
        {
            this.additive = additive;
        }

        public float CalculateFieldOfView(float fieldOfView)
        {
            if (additive)
            {
                return Mathf.Clamp(fieldOfView + value, 0.01f, 179);
            }
            return value;
        }

        public float EvaluateCurve(float time)
        {
            return curve.Evaluate(time);
        }

        #region [Getter / Setter]
        public float GetValue()
        {
            return value;
        }

        public void SetValue(float value)
        {
            this.value = value;
        }

        public float GetDuration()
        {
            return duration;
        }

        public void SetDuration(float value)
        {
            duration = value;
        }

        public AnimationCurve GetCurve()
        {
            return curve;
        }

        public void SetCurve(AnimationCurve value)
        {
            curve = value;
        }

        public bool Additive()
        {
            return additive;
        }

        public void Additive(bool value)
        {
            additive = value;
        }
        #endregion
    }
}