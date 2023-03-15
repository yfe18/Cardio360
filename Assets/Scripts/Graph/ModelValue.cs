using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Might be better to separate layout data from value data
public readonly struct ModelValue
{
    public readonly RectInt plotBbox;
    public readonly float xScale; // scale from values to size
    public readonly Texture plot, xAxis, yAxis;
    public readonly float[] yValues, xValues;

    // TODO: all int[] should be two long, might switch to proper data type
    // axis pos bounds are used for entire imag
    // the total bounds of the plot and axes centered and stacked on top of each other

    public ModelValue(float[] yValues, float[] xValues, Texture plot, RectInt plotBbox,
        float xScale)  // TODO: all int[] should be two long, might switch to proper data type
    { 
        this.yValues = yValues;
        this.xValues = xValues;
        this.plot = plot;
        this.plotBbox = plotBbox;
        this.xScale = xScale; // I don't think we need xyScale given, can calculate
        this.xAxis = null;
        this.yAxis = null;
    }

    public ModelValue(float[] yValues, float[] xValues, Texture plot, Texture xAxis, Texture yAxis, RectInt plotBbox)  
    { 
        this.yValues = yValues;
        this.xValues = xValues;
        this.plot = plot;
        int xHalfPadding = (int)((xAxis.width - plot.width) / 2f);
        int yHalfPadding = (int)((yAxis.height - plot.height) / 2f);
        this.plotBbox = plotBbox;
        this.xScale = 0; // to get around error
        this.xAxis = xAxis;
        this.yAxis = yAxis;

        xScale = CalculateXScale(xAxis.width, plot.width, xValues);
    }

    // XYScale calculation is wrong because graph y max and min are different than the y value max and min
    private float CalculateXScale(float totalWidth, float plotWidth, float[] xValues) {
        float xMax = Mathf.Max(xValues);
        float xMin = Mathf.Min(xValues);

        return (plotWidth / (xMax - xMin));
    }
}
