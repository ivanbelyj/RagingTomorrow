using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipText
{
    public string Text { get; private set; }
    public Color Color { get; private set; }
    public bool IsHeader { get; private set; }
    /// <summary>
    /// Дополнительный отступ снизу для текста (может пригодиться, например, для заголовков)
    /// </summary>
    public float BottomVSpace { get; private set; }
    public TooltipText(string text, Color color, bool isHeader = false, float bottomVSpace = 0) {
        Text = text;
        Color = color;
        IsHeader = isHeader;
        BottomVSpace = bottomVSpace;
    }
}
