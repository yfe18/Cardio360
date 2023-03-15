using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(RectTransform))]
public class GraphUI : MonoBehaviour
{
    [SerializeField] private GraphImage _displayImage;
    [SerializeField] private float _speed;

    private ModelValue _modelValue;

    private bool _isInitiated = false;
    public bool IsInitiated {
        get { return _isInitiated;}
        private set {_isInitiated = value;}
    }

    public bool IsPlaying {
        get;
        private set;
    }

    public float Speed {
        get { return _speed;  }
        set { _speed = value; }
    }

    // TODO: Implement adding separate axes and plot
    public void Init(ModelValue modelValue) { // TODO: change to a SetImage func
        _modelValue = modelValue;

        _displayImage.Value = _modelValue;
        _displayImage.rectTransform.anchoredPosition = Vector2.zero;
    
        IsInitiated = true;
    }
    
    void Awake() {
        IsPlaying = false;
    }

    public void Play() {
        if (IsInitiated) {
            IsPlaying = true;
        }
    }

    public void Pause() {
        IsPlaying = false;
    }

    public void SetXValue(float xValue) { // TODO: could just make it step
        //Debug.Log("moving x value");
        _displayImage.SetXValue(xValue);
    }

    void Update() {
        if (IsPlaying) {
            MoveImageWithinBounds(_displayImage, _speed);
        }
    }

    private void MoveImageWithinBounds(GraphImage graphImage, float dist) { 
        throw new NotImplementedException();
    }
}
