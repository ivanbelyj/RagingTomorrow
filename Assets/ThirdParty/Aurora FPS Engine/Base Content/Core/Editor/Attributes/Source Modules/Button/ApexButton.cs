/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System.Reflection;
using UnityEngine;

namespace AuroraFPSEditor.Attributes
{
    public sealed class ApexButton : ApexField
    {
        private MethodInfo action;
        private Object target;
        private GUIContent label;
        private string style;
        private float height;

        public ApexButton(MethodInfo action, Object target, GUIContent label, string style, float height)
        {
            this.action = action;
            this.target = target;
            this.label = label;
            this.style = style;
            this.height = height;
        }

        public override void DrawField(Rect position)
        {
            if(GUI.Button(position, label, style))
            {
                action.Invoke(target, null);
            }
        }

        public override void DrawFieldLayout()
        {
            Rect position = GUILayoutUtility.GetRect(0, height);
            DrawField(position);
        }

        public override float GetFieldHeight()
        {
            return height;
        }

        #region [Getter / Setter]
        public MethodInfo GetAction()
        {
            return action;
        }

        public void SetAction(MethodInfo value)
        {
            action = value;
        }

        public Object GetTarget()
        {
            return target;
        }

        public void SetTarget(Object value)
        {
            target = value;
        }

        public GUIContent GetLabel()
        {
            return label;
        }

        public void SetLabel(GUIContent value)
        {
            label = value;
        }

        public string GetStyle()
        {
            return style;
        }

        public void SetStyle(string value)
        {
            style = value;
        }

        public float GetHeight()
        {
            return height;
        }

        public void SetHeight(float value)
        {
            height = value;
        }
        #endregion
    }
}