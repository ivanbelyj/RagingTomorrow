using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipContentBuilder
{
    private List<TooltipLine> _lines;
    private List<TooltipText> _lastLine;
    public TooltipContentBuilder() {
        _lines = new List<TooltipLine>();
    }

    public TooltipContent Build() {
        return new TooltipContent(_lines);
    }

    public TooltipContentBuilder Ln() {
        _lastLine = new List<TooltipText>();
        _lines.Add(new TooltipLine(_lastLine));
        return this;
    }

    public TooltipContentBuilder Text(string str) {
        return Text(str, Color.white);
    }

    public TooltipContentBuilder Text(string str, Color col) {
        if (_lastLine == null)
            Ln();
        _lastLine.Add(new TooltipText(str, col));
        return this;
    }
}
