using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// will become obsolete if we move to computing model on server
public class Graph
{
    public static readonly Matrix4x4 ViewMatrix = Matrix4x4.TRS(new Vector3(0, 0, -1), Quaternion.identity, Vector3.one);
    
    public readonly float[] xValues;
    public readonly float[] yValues;

    private LineRenderer _lineRenderer = new GameObject().AddComponent<LineRenderer>();
    private Material _lineMaterial = new Material(Shader.Find("Sprites/Default"));

    public Graph(float[] xValues, float[] yValues) {
        if(xValues.Length == yValues.Length) {
            this.xValues = xValues;
            this.yValues = yValues;
            _lineRenderer.enabled = false;
        } else {
            throw new System.IndexOutOfRangeException("INVALID ARRAY LENGTH: Length of xValues must equal length of yValues");
        }
    }

    public Graph(IGraphable obj) : this(obj.XValues, obj.YValues) {}

    public Texture DrawGraph(int width, int height, int horizontalPadding, int verticalPadding,
        Color bgColor, LineProperties xAxisProperties, LineProperties yAxisProperties, LineProperties seriesProperties,
        bool isSeamless = false)
    {
        _lineRenderer.startWidth = seriesProperties.lineWidth;
        _lineRenderer.endWidth = seriesProperties.lineWidth;
        _lineMaterial.color = seriesProperties.lineColor;

        Vector2 xyScale = CalculateGraphScale(xValues, yValues, width, height);

        if (isSeamless) { // makes start position blend with end position
            // TODO: Implement once I figure out how
            _lineRenderer.positionCount = xValues.Length;

            for (int i = 0; i < xValues.Length; i++)
            {
                _lineRenderer.SetPosition(i, new Vector3(xValues[i] * xyScale.x, yValues[i] * xyScale.y, 0));
            }
        } else {
            _lineRenderer.positionCount = xValues.Length;

            for (int i = 0; i < xValues.Length; i++)
            {
                _lineRenderer.SetPosition(i, new Vector3(xValues[i] * xyScale.x, yValues[i] * xyScale.y, 0));
            }
        }

        Mesh chartMesh = new Mesh();
        _lineRenderer.BakeMesh(chartMesh);

        Matrix4x4 projectionMatrix = Matrix4x4.Ortho(
            chartMesh.bounds.min.x, chartMesh.bounds.max.x,
            chartMesh.bounds.min.y, chartMesh.bounds.max.y,
            0.01f, 1
        );//sets bounds to exactly contain mesh (beware of stretching if too different than renderTexture size) 

        RenderTexture renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);

        CommandBuffer commandBuffer = new CommandBuffer(); // this object could be reused
        //may also be a way to do this without commandbuffers
        commandBuffer.SetRenderTarget(renderTexture);
        commandBuffer.ClearRenderTarget(true, true, bgColor);
        commandBuffer.SetViewProjectionMatrices(ViewMatrix, projectionMatrix);
        commandBuffer.DrawMesh(chartMesh, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one), _lineMaterial);
        Graphics.ExecuteCommandBuffer(commandBuffer);

        RenderTexture.active = renderTexture;
        Texture2D outputText = new Texture2D(renderTexture.width + horizontalPadding, renderTexture.height + verticalPadding,
            TextureFormat.RGBA32, false
        );

        // TODO: Fix axes drawing over graph

        // Add padding
        // Fill outputText with background colour
        outputText.SetPixels(0, 0, outputText.width, outputText.height, 
            Enumerable.Repeat<Color>(bgColor, outputText.GetPixels().Length).ToArray<Color>()
        );
        outputText.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), horizontalPadding/2, verticalPadding/2);
        outputText.Apply();

        commandBuffer.Clear();

        Vector2 renderTextureOrigin = WorldToScreenPoint(ViewMatrix, projectionMatrix, new Vector2(width, height), Vector3.zero);

        DrawAxes(outputText, horizontalPadding, verticalPadding, 
            renderTextureOrigin, xAxisProperties, yAxisProperties
        );        
        
        RenderTexture.active = null;
        renderTexture.Release();

        return outputText;
    }

    private Vector2 CalculateGraphScale(float[] xValues, float[] yValues, int width, int height) 
    {
        double maxY = Double.NegativeInfinity;
        double minY = Double.PositiveInfinity;

        for (int i = 0; i < xValues.Length; i++)
        {
            maxY = (yValues[i] > maxY) ? yValues[i] : maxY;
            minY = (yValues[i] < minY) ? yValues[i] : minY;
        }

        float xScale = (float)(width / (xValues[xValues.Length - 1] - 0));  //x min is defined as 0

        if ((maxY > 0 && minY < 0))
        {
            // we know it already includes 0
        }
        else if (maxY <= 0)
        {
            maxY = 0; // round up, bc they are all neg
        }
        else
        {
            minY = 0; // round down, bc they are all pos
        }

        float yScale = (float)(height / (maxY - minY));

        return new Vector2(xScale, yScale);
    }

    private Vector2 WorldToScreenPoint(Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix, Vector2 screenSize, Vector3 pos) {
        Vector3 clipSpacePos = (projectionMatrix * ViewMatrix).MultiplyPoint(pos);
        Vector2 rasterSpaceOrigin = new Vector2(
            (clipSpacePos.x + 1f) * (screenSize.x / 2f),
            (clipSpacePos.y + 1f) * (screenSize.y / 2f)
        ); // ignores clipping

        return rasterSpaceOrigin;
    }

    private void CalculateGraphBounds(float[] x_vals, float[] y_vals, int width, int height,
        out double[] xAxisBounds, out double[] yAxisBounds, out double[] xyScale)
    {
        double maxY = Double.NegativeInfinity;
        double minY = Double.PositiveInfinity;

        for (int i = 0; i < x_vals.Length; i++)
        {
            maxY = (y_vals[i] > maxY) ? y_vals[i] : maxY;
            minY = (y_vals[i] < minY) ? y_vals[i] : minY;
        }

        Debug.Log(minY);
        Debug.Log(maxY);

        float x_scale = (float)(width / (x_vals[x_vals.Length - 1] - 0));  //x min is defined as 0

        if ((maxY > 0 && minY < 0))
        {
            // we know it already includes 0
        }
        else if (maxY <= 0)
        {
            maxY = 0; // round up, bc they are all neg
        }
        else
        {
            minY = 0; // round down, bc they are all pos
        }

        float y_scale = (float)(height / (maxY - minY));

        Debug.Log("Min: " + minY * y_scale);
        Debug.Log("Max: " + maxY * y_scale);
        Debug.Log("Range: " + (maxY - minY) * y_scale);

        // set the outputs

        xAxisBounds = new double[2] { 0, width };
        yAxisBounds = new double[2] { minY * y_scale, maxY * y_scale };
        xyScale = new double[2] { x_scale, y_scale };
    }

    private void DrawAxes(Texture2D graphTexture, int horizontalPadding, int verticalPadding, Vector2 renderTextureOrigin,
        LineProperties xAxisProperties, LineProperties yAxisProperties)
    {
        Vector2 paddedOrigin = new Vector2(renderTextureOrigin.x + horizontalPadding/2f, renderTextureOrigin.y + verticalPadding/2f);

        for (int x = 0; x < graphTexture.width; x++) {
            for (int w = 0; w < verticalPadding/2; w++) {
                //graphTexture.SetPixel(x, w, Color.cyan);
            }
        }

        // draw x-axis
        for (int x = 0; x < graphTexture.width; x++)
        {
            for (int w = 0; w < xAxisProperties.lineWidth; w++)
            {
                graphTexture.SetPixel(x, (int)paddedOrigin.y - (xAxisProperties.lineWidth / 2) + w, xAxisProperties.lineColor);
            }
        }

        // draw y-axis
        for (int y = 0; y < graphTexture.height; y++)
        {
            for (int w = 0; w < yAxisProperties.lineWidth; w++)
            {
                graphTexture.SetPixel((int)paddedOrigin.x - (yAxisProperties.lineWidth / 2) + w, y, yAxisProperties.lineColor);
            }
        }

        graphTexture.SetPixel((int)paddedOrigin.x, (int)paddedOrigin.y, Color.yellow);

        graphTexture.Apply();
    }
}
