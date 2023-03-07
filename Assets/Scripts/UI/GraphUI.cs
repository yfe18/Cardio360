using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class GraphUI : MonoBehaviour
{
    [SerializeField] private RawImage _displayImage;
    [SerializeField] private float _speed;

    private RectTransform _imageContainer; // is set to this object
    private RawImage[] _duplicateImages;

    private GraphImage _graphImage;

    public bool IsPlaying {
        get;
        private set;
    }

    public float Speed {
        get { return _speed;  }
        set { _speed = value; }
    }

    public GraphImage Image { // TODO: change to a SetImage func
        get {
            return _graphImage;
        }
        set {
            _graphImage = value;
            _displayImage.texture = _graphImage.image;
            _displayImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _imageContainer.rect.height);
            _displayImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _imageContainer.rect.height * ((float)_graphImage.image.width/_graphImage.image.height));
            _displayImage.rectTransform.anchoredPosition = new Vector2(-_displayImage.rectTransform.rect.width, 0);

            int imageCount = (int)(Mathf.Ceil(_imageContainer.rect.width / _graphImage.image.width)) + 1;

            _duplicateImages = new RawImage[imageCount];
            
            for (int i = 0; i < imageCount; i++) {
                _duplicateImages[i] = GameObject.Instantiate<RawImage>(_displayImage, _imageContainer);
                _duplicateImages[i].rectTransform.anchoredPosition = new Vector2(_displayImage.rectTransform.rect.width * i, 0);
            }
        }
    }
    
    void Awake() {
        IsPlaying = false;

        _imageContainer = GetComponent<RectTransform>();

        // Set to upper left
        _displayImage.rectTransform.pivot = new Vector2(0, 1);
        _displayImage.rectTransform.anchorMin = new Vector2(0, 1);
        _displayImage.rectTransform.anchorMax = new Vector2(0, 1);

        _displayImage.rectTransform.anchoredPosition = new Vector2(0, 0);
    }

    public void Play() {
        if (Image.image) {
            IsPlaying = true;
        }
    }


    public void Pause() {
        IsPlaying = false;
    }

    public void SetXValue(float xValue) { // TODO: could just make it step
        int imageX = _graphImage.GraphPositionToImagePosition(xValue, 0)[0];
        float displayX = (_displayImage.rectTransform.rect.width / _graphImage.image.width) * imageX; // TODO: move to central scale variable, bc this is set multiple times

        ResetImages(_displayImage, _duplicateImages);

        _displayImage.rectTransform.anchoredPosition -= new Vector2(displayX, 0);

        foreach (RawImage image in _duplicateImages) {
            image.rectTransform.anchoredPosition -= new Vector2(displayX, 0);
        }
    }

    void Update() {
        if (IsPlaying) {
            MoveImageWithinBounds(_displayImage, _speed);
            
            for (int i = 0; i < _duplicateImages.Length; i++) {
                MoveImageWithinBounds(_duplicateImages[i], _speed);
            }
        }
    }

    private void MoveImageWithinBounds(RawImage image, float dist) { 
        image.rectTransform.anchoredPosition -= new Vector2(dist, 0);
        if (image.rectTransform.anchoredPosition.x < -_displayImage.rectTransform.rect.width) {
            image.rectTransform.anchoredPosition = new Vector2(_displayImage.rectTransform.rect.width * _duplicateImages.Length, 0);
        }
    }

    private void ResetImages(RawImage mainImage, params RawImage[] extraImages) {
        // taken from SetImages code above, hence the mess
        mainImage.rectTransform.anchoredPosition = new Vector2(-mainImage.rectTransform.rect.width, 0);

        for (int i = 0; i < extraImages.Length; i++) {
            extraImages[i].rectTransform.anchoredPosition = new Vector2(mainImage.rectTransform.rect.width * i, 0);
        }
    }
}
