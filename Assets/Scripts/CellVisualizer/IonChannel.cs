using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IonChannel : MonoBehaviour, IGraphable
{
    [SerializeField] private string _name;
    [SerializeField] private Color _minColor = Color.white;
    [SerializeField] private Color _maxColor = Color.black;

    private float[] _values;
    private float _min, _max;

    private Material _material;

    public CellModel ParentModel { get; private set; }

    public CellInfoUI InfoUI { get; set; }

    public string Name {
        get {
            return _name;
        }
    }

    public int Step {
        set {
            float percent = (_values[value] - _min) / (_max - _min);
            _material.color = Color.Lerp(_minColor, _maxColor, percent);
        }
    }

    public float[] XValues {
        get {
            return ParentModel.XValues; // may be better to set give ion channel its own x values (and maybe let it time step itself)
        }
    }

    public float[] YValues {
        get {
            return _values;
        }
    }

    private void Init() {
        _material = GetComponent<Renderer>().material;
        ParentModel = GetComponentInParent<CellModel>();

        InfoUI = UIManager.NewUI<CellInfoUI>();
        InfoUI.transform.SetParent(this.transform);
        InfoUI.transform.localPosition = new Vector3(0, this.transform.localScale.y, 0);
    }

    void Awake() {
        if (ParentModel == null) {
            Init();
        }
    }

    public void SetValues(float[] values) {
        this._values = values;

        _min = Mathf.Min(values);
        _max = Mathf.Max(values);

        if (InfoUI == null) { // have to include because setvalues is sometimes called before awake due to activating objects
            Init();
        }

        InfoUI.Init(this, Cell.SimSpeed);
    }
}
