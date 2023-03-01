using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct GraphImage
{
    public readonly LineProperties yAxisProperties, xAxisProperties;
    public readonly LineProperties seriesProperties;

    public readonly int horizontalPadding, verticalPadding;
    public readonly int absoluteXAxisPos, absoluteYAxisPos;

    public readonly Texture graphImage;
    public readonly Texture graphAxes;

    
}
