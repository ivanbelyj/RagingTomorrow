using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipText
{
    public string Text { get; private set; }
    public Color Color { get; private set; }
    public TooltipText(string text, Color color) {
        Text = text;
        Color = color;
    }
}
