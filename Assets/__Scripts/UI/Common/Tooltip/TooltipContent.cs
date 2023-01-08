using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Данные, предназначенные для отображения во всплывающей подсказке
/// </summary>
public class TooltipContent
{
    public List<TooltipLine> Elements { get; private set; }
    public TooltipContent(List<TooltipLine> elements) {
        Elements = elements;
    }
}
