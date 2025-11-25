/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Serialization.Collections;
using AuroraFPSRuntime.SystemModules.ControllerSystems;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.CameraSystems.Effects
{
    [CreateAssetMenu(fileName = "Head Bob Settings", menuName = "Aurora FPS Engine/Camera/Effect/Head Bob Settings", order = 50)]
    [HideScriptField]
    public class HeadBobSettings : ScriptableObject
    {
        [System.Serializable]
        public struct Multiplier
        {
            [SerializeField] 
            private float amplitude;

            [SerializeField] 
            private float frequency;

            public Multiplier(float amplitude, float frequency)
            {
                this.amplitude = amplitude;
                this.frequency = frequency;
            }

            public static readonly Multiplier none = new Multiplier(0, 0);

            #region [Getter / Setter]
            public float GetAmplitude()
            {
                return amplitude;
            }

            public void SetAmplitude(float value)
            {
                amplitude = value;
            }

            public float GetFrequency()
            {
                return frequency;
            }

            public void SetFrequency(float value)
            {
                frequency = value;
            }
            #endregion
        }

        [System.Serializable]
        internal sealed class Multipliers : SerializableDictionary<ControllerState, Multiplier>
        {
            [SerializeField]
            private ControllerState[] keys;

            [SerializeField]
            private Multiplier[] values;

            protected override ControllerState[] GetKeys()
            {
                return keys;
            }

            protected override Multiplier[] GetValues()
            {
                return values;
            }

            protected override void SetKeys(ControllerState[] keys)
            {
                this.keys = keys;
            }

            protected override void SetValues(Multiplier[] values)
            {
                this.values = values;
            }
        }

        [SerializeField]
        [VisualClamp(0.01f, 20.0f)]
        private float speed = 10.0f;

        [SerializeField]
        private Multipliers multipliers;

        // Position amplitude and frequency properties.
        [SerializeField]
        [TabGroup("Transform Layer", "Position")]
        [Label("Amplitude X")]
        private float xPositionAmplitude = 0.05f;

        [SerializeField]
        [TabGroup("Transform Layer", "Position")]
        [Label("Amplitude Y")]
        private float yPositionAmplitude = 0.1f;

        [SerializeField]
        [TabGroup("Transform Layer", "Position")]
        [Label("Frequency X")]
        private float xPositionFrequency = 2.0f;

        [SerializeField]
        [TabGroup("Transform Layer", "Position")]
        [Label("Frequency X")]
        private float yPositionFrequency = 4.0f;

        [SerializeField]
        [TabGroup("Transform Layer", "Position")]
        [Label("Curve X")]
        private AnimationCurve xPositionCurve = new AnimationCurve(
            new Keyframe(0.0f, 0.0f, 3.633036f, 3.633036f, 0.3333333f, 0.1579228f),
            new Keyframe(0.5f, 1.0f, 0.006286651f, 0.006286651f, 0.3333333f, 0.3333333f),
            new Keyframe(1.0f, 0.0f, -3.619263f, -3.619263f, 0.3333333f, 0.1500669f),
            new Keyframe(1.5f, -1.0f, 0.0f, 0.0f, 0.3333333f, 0.3333333f),
            new Keyframe(2.0f, 0.0f, 3.835659f, 3.835659f, 0.2075124f, 0.3333333f));

        [SerializeField]
        [TabGroup("Transform Layer", "Position")]
        [Label("Curve Y")]
        private AnimationCurve yPositionCurve = new AnimationCurve(
            new Keyframe(0.0f, 0.0f, 3.633036f, 3.633036f, 0.3333333f, 0.1579228f),
            new Keyframe(0.5f, 1.0f, 0.006286651f, 0.006286651f, 0.3333333f, 0.3333333f),
            new Keyframe(1.0f, 0.0f, -3.619263f, -3.619263f, 0.3333333f, 0.1500669f),
            new Keyframe(1.5f, -1.0f, 0.0f, 0.0f, 0.3333333f, 0.3333333f),
            new Keyframe(2.0f, 0.0f, 3.835659f, 3.835659f, 0.2075124f, 0.3333333f));

        [SerializeField]
        [TabGroup("Transform Layer", "Position")]
        [Label("Enabled")]
        private bool positionBobEnabled;

        // Rotation amplitude and frequency properties.
        [SerializeField]
        [TabGroup("Transform Layer", "Rotation")]
        [Label("Amplitude X")]
        private float xRotationAmplitude = 0.05f;

        [SerializeField]
        [TabGroup("Transform Layer", "Rotation")]
        [Label("Amplitude Y")]
        private float yRotationAmplitude = 0.1f;

        [SerializeField]
        [TabGroup("Transform Layer", "Rotation")]
        [Label("Frequency X")]
        private float xRotationFrequency = 2.0f;

        [SerializeField]
        [TabGroup("Transform Layer", "Rotation")]
        [Label("Frequency Y")]
        private float yRotationFrequency = 4.0f;

        [SerializeField]
        [TabGroup("Transform Layer", "Rotation")]
        [Label("Curve X")]
        private AnimationCurve xRotationCurve = new AnimationCurve(
            new Keyframe(0.0f, 0.0f, 3.633036f, 3.633036f, 0.3333333f, 0.1579228f),
            new Keyframe(0.5f, 1.0f, 0.006286651f, 0.006286651f, 0.3333333f, 0.3333333f),
            new Keyframe(1.0f, 0.0f, -3.619263f, -3.619263f, 0.3333333f, 0.1500669f),
            new Keyframe(1.5f, -1.0f, 0.0f, 0.0f, 0.3333333f, 0.3333333f),
            new Keyframe(2.0f, 0.0f, 3.835659f, 3.835659f, 0.2075124f, 0.3333333f));

        [SerializeField]
        [TabGroup("Transform Layer", "Rotation")]
        [Label("Curve Y")]
        private AnimationCurve yRotationCurve = new AnimationCurve(
            new Keyframe(0.0f, 0.0f, 3.633036f, 3.633036f, 0.3333333f, 0.1579228f),
            new Keyframe(0.5f, 1.0f, 0.006286651f, 0.006286651f, 0.3333333f, 0.3333333f),
            new Keyframe(1.0f, 0.0f, -3.619263f, -3.619263f, 0.3333333f, 0.1500669f),
            new Keyframe(1.5f, -1.0f, 0.0f, 0.0f, 0.3333333f, 0.3333333f),
            new Keyframe(2.0f, 0.0f, 3.835659f, 3.835659f, 0.2075124f, 0.3333333f));

        [SerializeField]
        [TabGroup("Transform Layer", "Rotation")]
        [Label("Enabled")]
        private bool rotationBobEnabled;

        public bool TryGetMultiplier(ControllerState state, out Multiplier multiplier)
        {
            return multipliers.TryGetValue(state, out multiplier);
        }

        #region [Getter / Setter]
        public float GetSpeed()
        {
            return speed;
        }

        public void SetSpeed(float value)
        {
            speed = value;
        }

        public float GetPositionAmplitudeX()
        {
            return xPositionAmplitude;
        }

        public void SetPositionAmplitudeX(float value)
        {
            xPositionAmplitude = value;
        }

        public float GetPositionAmplitudeY()
        {
            return yPositionAmplitude;
        }

        public void SetPositionAmplitudeY(float value)
        {
            yPositionAmplitude = value;
        }

        public float GetPositionFrequencyX()
        {
            return xPositionFrequency;
        }

        public void SetPositionFrequencyX(float value)
        {
            xPositionFrequency = value;
        }

        public float GetPositionFrequencyY()
        {
            return yPositionFrequency;
        }

        public void SetPositionFrequencyY(float value)
        {
            yPositionFrequency = value;
        }

        public AnimationCurve GetPositionCurveX()
        {
            return xPositionCurve;
        }

        public void SetPositionCurveX(AnimationCurve value)
        {
            xPositionCurve = value;
        }

        public AnimationCurve GetPositionCurveY()
        {
            return yPositionCurve;
        }

        public void SetPositionCurveY(AnimationCurve value)
        {
            yPositionCurve = value;
        }

        public bool PositionBobEnabled()
        {
            return positionBobEnabled;
        }

        public void PositionBobEnabled(bool value)
        {
            positionBobEnabled = value;
        }

        public float GetRotationAmplitudeX()
        {
            return xRotationAmplitude;
        }

        public void SetRotationAmplitudeX(float value)
        {
            xRotationAmplitude = value;
        }

        public float GetRotationAmplitudeY()
        {
            return yRotationAmplitude;
        }

        public void SetRotationAmplitudeY(float value)
        {
            yRotationAmplitude = value;
        }

        public float GetRotationFrequencyX()
        {
            return xRotationFrequency;
        }

        public void SetRotationFrequencyX(float value)
        {
            xRotationFrequency = value;
        }

        public float GetRotationFrequencyY()
        {
            return yRotationFrequency;
        }

        public void SetRotationFrequencyY(float value)
        {
            yRotationFrequency = value;
        }

        public AnimationCurve GetRotationCurveX()
        {
            return xRotationCurve;
        }

        public void SetRotationCurveX(AnimationCurve value)
        {
            xRotationCurve = value;
        }

        public AnimationCurve GetRotationCurveY()
        {
            return yRotationCurve;
        }

        public void SetRotationCurveY(AnimationCurve value)
        {
            yRotationCurve = value;
        }

        public bool RotationBobEnabled()
        {
            return rotationBobEnabled;
        }

        public void RotationBobEnabled(bool value)
        {
            rotationBobEnabled = value;
        }
        #endregion
    }
}