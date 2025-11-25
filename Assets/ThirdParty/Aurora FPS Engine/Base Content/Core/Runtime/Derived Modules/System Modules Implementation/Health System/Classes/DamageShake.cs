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
using UnityEngine;

#region [Unity Editor Section]
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

namespace AuroraFPSRuntime
{
    [System.Serializable]
    public sealed class DamageShake
    {
        [SerializeField]
        [CustomView(ViewGUI = "OnDamageRangeGUI")]
        private Vector2 damageRange = new Vector2(0, 100);

        [SerializeField]
        private Displacement shakeDirection = new Displacement(new Vector3(0, 0, 0), new Vector3(1, 1, 0.5f));

        [SerializeField]
        private KickShake.Settings shakeSettings;

        #region [Unity Editor Section]
#if UNITY_EDITOR
        private void OnDamageRangeGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);
            Rect[] rects = new Rect[2];
            for (int i = 0; i < 2; i++)
                rects[i] = new Rect(position.position.x + (i * position.width / 2) - (EditorGUI.indentLevel * 15), position.position.y, (position.width / 2) + (EditorGUI.indentLevel * 15), position.height);
            damageRange.x = EditorGUI.FloatField(rects[0], damageRange.x);
            damageRange.y = EditorGUI.FloatField(rects[1], damageRange.y);
        }
#endif
        #endregion

        #region [Getter / Setter]
        public Vector2 GetDamageRange()
        {
            return damageRange;
        }

        public void SetDamageRange(Vector2 value)
        {
            damageRange = value;
        }

        public Displacement GetShakeDirection()
        {
            return shakeDirection;
        }

        public void SetShakeDirection(Displacement value)
        {
            shakeDirection = value;
        }

        public KickShake.Settings GetShakeSettings()
        {
            return shakeSettings;
        }

        public void SetShakeSettings(KickShake.Settings value)
        {
            shakeSettings = value;
        }
        #endregion
    }
}