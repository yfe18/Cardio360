using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect), typeof(RectTransform))]
public class GraphImage : MonoBehaviour
{
    private ModelValue _modelValue;
    public ModelValue Value {
        get {
            return _modelValue;
        }
        set {
            _modelValue = value;
            // TODO: fix axes alignment and viewport issues, might have something to do with scaling OR the bboxes from the server and matplotlib
            _plotImage.texture = _modelValue.plot;
            _plotImage.SetNativeSize();
            _plotImage.rectTransform.anchoredPosition = new Vector2(0, _modelValue.plotBbox.yMin);

            if (_modelValue.xAxis != null) {
                _xAxisImage.texture  = _modelValue.xAxis;
                _xAxisImage.SetNativeSize();
                _xAxisImage.rectTransform.anchoredPosition = new Vector2(-_modelValue.plotBbox.xMin, 0); // negative pad for width and padding of y axis
            } else {
                _xAxisImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
                _xAxisImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
            }

            if (_modelValue.yAxis != null) {
                _yAxisImage.texture = _modelValue.yAxis;
                _yAxisImage.SetNativeSize();
            } else {
                _yAxisImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
                _yAxisImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
            }

            _scrollRect.viewport.anchoredPosition = new Vector2(_modelValue.plotBbox.min.x, 0);

            float height = (_yAxisImage.rectTransform.rect.height == 0) ? _plotImage.rectTransform.rect.height : _yAxisImage.rectTransform.rect.height;
            float width = (_xAxisImage.rectTransform.rect.height == 0) ? _plotImage.rectTransform.rect.width : _xAxisImage.rectTransform.rect.width;
            
            _scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            _scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

            float scale = _rectTransform.rect.height / height;
            _wrapper.localScale = Vector3.one * scale;

            //make wrapper same size as graph image, even with scaling
            _wrapper.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            _wrapper.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _rectTransform.rect.width / scale); 
        }
    }

    // plot image must just be the plot
    [SerializeField] private RawImage _plotImage, _xAxisImage, _yAxisImage;
    [SerializeField] private RectTransform _wrapper;

    private RectTransform _rectTransform;
    private ScrollRect _scrollRect;

    public RectTransform rectTransform {
        get {return _rectTransform;}
    }

    public void Awake() {
        _rectTransform = GetComponent<RectTransform>();
        _scrollRect = GetComponent<ScrollRect>();
    }

    public void SetXValue(float xValue) {
        int imageX = GraphXToImageX(xValue);
        float displayX = (_plotImage.rectTransform.rect.width / Value.plot.width) * imageX; // TODO: move to central scale variable, bc this is set multiple times

        _scrollRect.content.anchoredPosition = -new Vector2(displayX, 0);
    }

    private int GraphXToImageX(float graphX) {
        int x = (int)(graphX * Value.xScale); // TODO: maybe adding clamping to bounds

        //Debug.Log(graphX + " : " + x);

        return x;
    }
}
