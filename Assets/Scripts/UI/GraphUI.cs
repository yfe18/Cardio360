using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class GraphUI : MonoBehaviour
{
    [SerializeField] private RawImage _graphImage;
    [SerializeField] private float _speed;

    private RectTransform _imageContainer;
    private RawImage[] _duplicateImages;

    public bool IsPlaying {
        get;
        private set;
    }

    public float Speed {
        get { return _speed;  }
        set { _speed = value; }
    }

    public Texture Image { // TODO: change to a SetImage func
        get {
            return _graphImage.texture;
        }
        set {
            _graphImage.texture = value;
            _graphImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _imageContainer.rect.height);
            _graphImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _imageContainer.rect.height * ((float)value.width/value.height));
            _graphImage.rectTransform.anchoredPosition = new Vector2(-_graphImage.rectTransform.rect.width, 0);

            int imageCount = (int)(Mathf.Ceil(_imageContainer.rect.width / value.width)) + 1;

            _duplicateImages = new RawImage[imageCount];
            
            for (int i = 0; i < imageCount; i++) {
                _duplicateImages[i] = GameObject.Instantiate<RawImage>(_graphImage, _imageContainer);
                _duplicateImages[i].rectTransform.anchoredPosition = new Vector2(_graphImage.rectTransform.rect.width * i, 0);
            }
        }
    }
    
    void Awake() {
        IsPlaying = false;

        _imageContainer = GetComponent<RectTransform>();

        // Set to upper left
        _graphImage.rectTransform.pivot = new Vector2(0, 1);
        _graphImage.rectTransform.anchorMin = new Vector2(0, 1);
        _graphImage.rectTransform.anchorMax = new Vector2(0, 1);

        _graphImage.rectTransform.anchoredPosition = new Vector2(0, 0);
    }

    public void Play() {
        if (Image) {
            IsPlaying = true;
        }
    }


    public void Pause() {
        IsPlaying = false;
    }

    public void SetCursorX(float xValue) {
        
    }

    void Update() {
        if (IsPlaying) {
            MoveImageWithinBounds(_graphImage);
            
            for (int i = 0; i < _duplicateImages.Length; i++) {
                MoveImageWithinBounds(_duplicateImages[i]);
            }
        }
    }

    private void MoveImageWithinBounds(RawImage image) {
        image.rectTransform.anchoredPosition -= new Vector2(_speed, 0);
        if (image.rectTransform.anchoredPosition.x < -_graphImage.rectTransform.rect.width) {
            image.rectTransform.anchoredPosition = new Vector2(_graphImage.rectTransform.rect.width * _duplicateImages.Length, 0);
        }
    }
}
