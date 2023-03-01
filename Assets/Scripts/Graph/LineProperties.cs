using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LineProperties
{

    public static LineProperties NONE = new LineProperties(0, Color.clear);

    public int lineWidth;
    public Color lineColor;

    public LineProperties(int lineWidth, Color lineColor) {
        this.lineWidth = lineWidth;
        this.lineColor = lineColor;
    }
}
