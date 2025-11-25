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
    public static class ApexEditorUtilities
    {
        public static Rect[] SplitRect(Rect rectToSplit, int n)
        {
            Rect[] rects = new Rect[n];

            for (int i = 0; i < n; i++)
                rects[i] = new Rect(rectToSplit.position.x + (i * rectToSplit.width / n) - (EditorGUI.indentLevel * 15), rectToSplit.position.y, (rectToSplit.width / n) + (EditorGUI.indentLevel * 15), rectToSplit.height);

            return rects;
        }

        public static Rect[] SplitRect2(Rect rectToSplit, int n)
        {
            Rect[] rects = new Rect[n];

            for (int i = 0; i < n; i++)
                rects[i] = new Rect(rectToSplit.position.x + (i * rectToSplit.width / n) - (EditorGUI.indentLevel * 15), rectToSplit.position.y, (rectToSplit.width / n) + (EditorGUI.indentLevel * 15), rectToSplit.height);

            int padding = (int)rects[0].width - 50 - (EditorGUI.indentLevel * 17);
            int space = 2;

            rects[0].width -= padding + space;
            rects[1].x -= padding;
            rects[1].width += padding;

            return rects;
        }

        public static bool IndentFoldoutGUI(Rect position, bool value, string label)
        {
            Rect foldoutPosition = new Rect(position.x, position.y, 15, position.height);
            value = EditorGUI.Toggle(foldoutPosition, value, EditorStyles.foldout);

            Rect foldoutLabelPosition = new Rect(position.x + 15, foldoutPosition.y, position.width, position.height);
            GUI.Label(foldoutLabelPosition, label, GetFoldoutLabelStyle());

            return value;
        }

        public static bool IndentFoldoutGUI(Rect position, bool value, string label, bool toggleOnClick)
        {
            float foldoutWidth = toggleOnClick ? position.width : 15;
            Rect foldoutPosition = new Rect(position.x, position.y, foldoutWidth, position.height);
            value = EditorGUI.Toggle(foldoutPosition, value, EditorStyles.foldout);

            Rect foldoutLabelPosition = new Rect(position.x + 15, foldoutPosition.y, position.width, position.height);
            GUI.Label(foldoutLabelPosition, label, GetFoldoutLabelStyle());

            return value;
        }

        public static bool IndentFoldoutGUI(Rect position, bool value, GUIContent label)
        {
            Rect foldoutPosition = new Rect(position.x, position.y, 15, position.height);
            value = EditorGUI.Toggle(foldoutPosition, value, EditorStyles.foldout);

            Rect foldoutLabelPosition = new Rect(position.x + 15, foldoutPosition.y, position.width, position.height);
            GUI.Label(foldoutLabelPosition, label, GetFoldoutLabelStyle());

            return value;
        }

        public static bool IndentFoldoutGUI(Rect position, bool value, GUIContent label, bool toggleOnClick)
        {
            float foldoutWidth = toggleOnClick ? position.width : 15;
            Rect foldoutPosition = new Rect(position.x, position.y, foldoutWidth, position.height);
            value = EditorGUI.Toggle(foldoutPosition, value, EditorStyles.foldout);

            Rect foldoutLabelPosition = new Rect(position.x + 14, foldoutPosition.y, position.width - 14, position.height);
            GUI.Label(foldoutLabelPosition, label, GetFoldoutLabelStyle());

            return value;
        }

        private static GUIStyle GetFoldoutLabelStyle()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleLeft;
            return style;
        }
    }
}
