/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEditor;
using UnityEngine;

namespace AuroraFPSEditor.Attributes
{
    public delegate void OnItemClickCallback();

    internal class SearchItem
    {
        internal static class Styles
        {
            public static GUIStyle Normal
            {
                get
                {
                    GUIStyle style = new GUIStyle("IN Title");
                    style.alignment = TextAnchor.MiddleLeft;
                    style.fontStyle = FontStyle.Normal;
                    style.fixedHeight = 20;
                    return style;
                }
            }

            public static GUIStyle Focus
            {
                get
                {
                    GUIStyle style = new GUIStyle("OL Title");
                    style.alignment = TextAnchor.MiddleLeft;
                    style.fontStyle = FontStyle.Normal;
                    style.fixedHeight = 20;
                    return style;
                }
            }
        }

        private GUIContent label;
        private bool isActive;

        private GUIStyle style;
        private Vector2 previousMousePosition;

        public SearchItem(GUIContent label, bool isActive, OnItemClickCallback clickCallback)
        {
            this.label = label;
            this.isActive = isActive;
            this.OnClickCallback = clickCallback;
        }

        public virtual void OnItemGUI()
        {
            Event current = Event.current;
            Rect itemPosition = GUILayoutUtility.GetRect(0, 0);
            itemPosition.height = 20;

            if (isActive &&
                current.mousePosition != previousMousePosition &&
                itemPosition.Contains(current.mousePosition))
            {
                style = Styles.Focus;
                previousMousePosition = current.mousePosition;
            }
            else
            {
                style = Styles.Normal;
            }

            EditorGUI.BeginDisabledGroup(!isActive);
            if (GUILayout.Button(label, style))
            {
                OnClickCallback?.Invoke();
            }
            EditorGUI.EndDisabledGroup();
        }

        #region [Event Callback Function]
        public event OnItemClickCallback OnClickCallback;
        #endregion

        #region [Getter / Setter]
        public GUIContent GetLabel()
        {
            return label;
        }

        public void SetLabel(GUIContent value)
        {
            label = value;
        }

        public bool IsActive()
        {
            return isActive;
        }

        public void IsActive(bool value)
        {
            isActive = value;
        }
        #endregion
    }
}