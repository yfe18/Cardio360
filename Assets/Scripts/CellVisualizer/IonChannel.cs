using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IonChannel : VisualizerComponent
{
    [SerializeField] private string _name;
    [SerializeField] private Color _minColor = Color.white;
    [SerializeField] private Color _maxColor = Color.black;

    private float _min, _max;

    private Material _material;

    public CellModel ParentModel { get; private set; }

    public CellInfoUI InfoUI { get; set; }

    public override string Name {
        get {
            return _name;
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

    public void SetValues(ModelValue modelValue) {
        Value = modelValue;

        _min = Mathf.Min(Value.yValues);
        _max = Mathf.Max(Value.yValues);

        if (InfoUI == null) { // have to include because setvalues is sometimes called before awake due to activating objects
            Init();
        }

        InfoUI.Init(this, Cell.SimSpeed);
    }

     public override void SetStep(int step) {
        float percent = (Value.yValues[step] - _min) / (_max - _min);
        _material.color = Color.Lerp(_minColor, _maxColor, percent);
        InfoUI.GraphUIComponent.SetXValue(Value.xValues[step]); // TODO remove conversion from xvalue to step back to xvalue
    }
}
