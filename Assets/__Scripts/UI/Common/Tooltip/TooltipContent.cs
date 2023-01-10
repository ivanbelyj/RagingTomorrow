using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Данные, предназначенные для отображения во всплывающей подсказке
/// </summary>
public class TooltipContent
{
    public List<TooltipLine> Elements { get; private set; }
    public TooltipContent(List<TooltipLine> elements) {
        // Пустые линии пропускаются
        Elements = elements.Where(line => line.Elements.Count != 0).ToList();
    }

    public static TooltipContent Merge(TooltipContent c1, TooltipContent c2) {
        List<TooltipLine> allElements = new List<TooltipLine>();
        allElements.AddRange(c1.Elements);
        allElements.AddRange(c2.Elements);
        return new TooltipContent(allElements);
    }
}
