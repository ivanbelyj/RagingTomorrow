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
    /// <summary>
    /// Representation of translation and rotation. 
    /// </summary>
    [System.Serializable]
    public struct Displacement
    {
        [SerializeField]
        private Vector3 position;

        [SerializeField]
        private Vector3 eulerAngles;

        public Displacement(Vector3 position, Vector3 eulerAngles)
        {
            this.position = position;
            this.eulerAngles = eulerAngles;
        }

        public Displacement(Vector3 position)
        {
            this.position = position;
            this.eulerAngles = Vector3.zero;
        }

        public static Displacement Scale(Displacement a, Displacement b)
        {
            return new Displacement(Vector3.Scale(a.position, b.position),
                Vector3.Scale(b.eulerAngles, a.eulerAngles));
        }

        public static Displacement Lerp(Displacement a, Displacement b, float t)
        {
            return new Displacement(Vector3.Lerp(a.position, b.position, t),
                Vector3.Lerp(a.eulerAngles, b.eulerAngles, t));
        }

        public Displacement ScaledBy(float posScale, float rotScale)
        {
            return new Displacement(position * posScale, eulerAngles * rotScale);
        }

        public Displacement Normalized
        {
            get
            {
                return new Displacement(position.normalized, eulerAngles.normalized);
            }
        }

        #region [Static Methods]
        public static Displacement InsideUnitSpheres()
        {
            return new Displacement(Random.insideUnitSphere, Random.insideUnitSphere);
        }
        #endregion

        #region [Static Readonly]
        public static readonly Displacement zero = new Displacement(Vector3.zero, Vector3.zero);
        #endregion

        #region [Operator Override]
        public static Displacement operator +(Displacement a, Displacement b)
        {
            return new Displacement(a.position + b.position,
                b.eulerAngles + a.eulerAngles);
        }

        public static Displacement operator -(Displacement a, Displacement b)
        {
            return new Displacement(a.position - b.position,
                b.eulerAngles - a.eulerAngles);
        }

        public static Displacement operator -(Displacement disp)
        {
            return new Displacement(-disp.position, -disp.eulerAngles);
        }

        public static Displacement operator *(Displacement coords, float number)
        {
            return new Displacement(coords.position * number,
                coords.eulerAngles * number);
        }

        public static Displacement operator *(float number, Displacement coords)
        {
            return coords * number;
        }

        public static Displacement operator /(Displacement coords, float number)
        {
            return new Displacement(coords.position / number,
                coords.eulerAngles / number);
        }
        #endregion

        #region [Getter / Setter]
        public Vector3 GetPosition()
        {
            return position;
        }

        public void SetPosition(Vector3 value)
        {
            position = value;
        }

        public Vector3 GetEulerAngles()
        {
            return eulerAngles;
        }

        public void SetEulerAngles(Vector3 value)
        {
            eulerAngles = value;
        }

        public void SetEulerAngles(float x, float y, float z)
        {
            eulerAngles.x = x;
            eulerAngles.y = y;
            eulerAngles.z = z;
        }
        #endregion
    }
}