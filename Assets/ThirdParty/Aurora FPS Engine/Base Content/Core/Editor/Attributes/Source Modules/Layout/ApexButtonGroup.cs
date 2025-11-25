/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System.Collections.Generic;
using UnityEngine;

namespace AuroraFPSEditor.Attributes
{
    public sealed class ApexButtonGroup : ApexField
    {
        public readonly string name;
        private List<ApexField> buttons;

        public ApexButtonGroup(string name, List<ApexField> buttons)
        {
            this.name = name;
            this.buttons = buttons;
        }

        public override void DrawFieldLayout()
        {
            Rect position = GUILayoutUtility.GetRect(0, GetFieldHeight());
            DrawField(position);
        }

        public override void DrawField(Rect position)
        {
            int count = buttons.Count;
            Rect[] positions = ApexEditorUtilities.SplitRect(position, count);
            for (int i = 0; i < count; i++)
            {
                ApexField button = buttons[i];
                Rect buttonPosition = positions[i];
                buttonPosition.height = button.GetFieldHeight();
                button.DrawField(buttonPosition);
            }
        }

        public override float GetFieldHeight()
        {
            float maxHeight = 0;
            for (int i = 0; i < buttons.Count; i++)
            {
                float height = buttons[i].GetFieldHeight();
                if (maxHeight < height)
                {
                    maxHeight = height;
                }
            }
            return maxHeight > 0 ? maxHeight : 17.0f;
        }

        public void Add(ApexField button)
        {
            buttons.Add(button);
        }
    }
}