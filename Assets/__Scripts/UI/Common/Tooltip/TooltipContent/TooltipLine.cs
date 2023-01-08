using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Строка данных, отображаемая во всплывающей подсказке
/// </summary>
public class TooltipLine
{
    public List<TooltipText> Elements { get; private set; }
    public TooltipLine(List<TooltipText> elements) {
        Elements = elements;
    }
}
