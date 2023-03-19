using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleIonChannel : IonChannel
{
    [SerializeField] private Color _minColor = Color.white;
    [SerializeField] private Color _maxColor = Color.black;

    private float _min, _max;

    private Material _material;

    public CellInfoUI InfoUI { get; set; }

    public override void Awake() {
        base.Awake();

        _material = GetComponent<Renderer>().material;
    }

    public override void SetValues(ModelValue modelValue) {
        base.SetValues(modelValue);

        _min = Mathf.Min(Value.yValues);
        _max = Mathf.Max(Value.yValues);

        if (InfoUI == null) { // have to include because setvalues is sometimes called before awake due to activating objects
            InfoUI = UIManager.NewUI<CellInfoUI>();
            InfoUI.transform.SetParent(this.transform);
            InfoUI.transform.localPosition = new Vector3(0, this.transform.localScale.y, 0);
        }

        InfoUI.Init(this);
    }

     public override void SetStep(int step) {
        float percent = (Value.yValues[step] - _min) / (_max - _min);
        _material.color = Color.Lerp(_minColor, _maxColor, percent);
        InfoUI.GraphUIComponent.SetXValue(Value.xValues[step]); // TODO remove conversion from xvalue to step back to xvalue
    }
}
