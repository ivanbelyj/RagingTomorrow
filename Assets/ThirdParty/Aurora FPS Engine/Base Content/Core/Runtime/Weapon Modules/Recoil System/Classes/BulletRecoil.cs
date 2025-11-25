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

namespace AuroraFPSRuntime.WeaponModules.RecoilSystem
{
    [System.Serializable]
    public class BulletRecoil
    {
        public enum Mode
        {
            Pass,
            Random,
            Procedural
        }

        [SerializeField]
        private Mode mode = Mode.Procedural;

        [SerializeField]
        [ReorderableList]
        [VisibleIf("mode", "Pass,Random")]
        private Vector2[] variances;

        [SerializeField]
        [VisibleIf("mode", "Procedural")]
        private float ratio = 0.15f;

        [SerializeField]
        [MinMaxSlider(-10, 10)]
        [VisibleIf("mode", "Procedural")]
        private Vector2 range = new Vector2(-0.8f, 0.8f);

        public BulletRecoil(Mode mode, Vector2[] variances)
        {
            this.mode = mode;
            this.variances = variances;
        }

        public void MoveNext(ref int index, ref Vector3 vector)
        {
            if (variances != null && variances.Length > 0)
            {
                switch (mode)
                {
                    case Mode.Pass:
                        index = Mathf.Clamp(++index, 0, variances.Length - 1);
                        Vector2 next = variances[index];
                        vector.x += Random.Range(-next.x, next.x);
                        vector.y += Random.Range(-next.y, next.y);
                        vector.z += Random.Range(-next.y, next.y);
                        break;
                    case Mode.Procedural:
                        float result = Mathf.Clamp(++index * ratio, range.x, range.y);
                        vector.x += Random.Range(-result, result);
                        vector.y += Random.Range(-result, result);
                        vector.z += Random.Range(-result, result);
                        break;
                    case Mode.Random:
                        index = Random.Range(0, variances.Length);
                        Vector2 random = variances[index];
                        vector.x += Random.Range(-random.x, random.x);
                        vector.y += Random.Range(-random.y, random.y);
                        vector.z += Random.Range(-random.y, random.y);
                        break;
                }
            }
        }

        #region [Getter / Setter]
        public Mode GetMode()
        {
            return mode;
        }

        public void SetMode(Mode value)
        {
            mode = value;
        }

        public Vector2[] GetVariances()
        {
            return variances;
        }

        public void SetVariances(Vector2[] value)
        {
            variances = value;
        }

        public float GetRatio()
        {
            return ratio;
        }

        public void SetRatio(float value)
        {
            ratio = value;
        }

        public Vector2 GetRange()
        {
            return range;
        }

        public void SetRange(Vector2 value)
        {
            range = value;
        }
        #endregion
    }
}
