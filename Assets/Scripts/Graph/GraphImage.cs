using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct GraphImage
{
    public readonly int[] xAxisPosBounds, yAxisPosBounds;
    public readonly Vector2 xyScale;
    public readonly Texture image;

    public GraphImage(Texture image, int[] xAxisPosBounds, int[] yAxisPosBounds, Vector2 xyScale) { // TODO: all int[] should be two long, might switch to proper data type
        this.image = image;
        this.xAxisPosBounds = xAxisPosBounds;
        this.yAxisPosBounds = yAxisPosBounds;
        this.xyScale = xyScale;
    }

    public int[] GraphPositionToImagePosition(float graphX, float graphY) {
        int x = (int)((graphX * xyScale.x) + xAxisPosBounds[0]); // TODO: maybe adding clamping to bounds
        int y = (int)((graphX * xyScale.y) + yAxisPosBounds[0]);

        // Debug.Log(graphX + " : " + x);

        return new int[2] {x , y};
    }
}
