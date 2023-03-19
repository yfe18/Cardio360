using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedIonChannel : IonChannel
{
    [SerializeField] private Animator _animator;

    public CellInfoUI InfoUI { get; set; }

    private float _min, _max;

    public override void SetValues(ModelValue modelValue)
    {
        base.SetValues(modelValue);

        _min = Mathf.Min(Value.yValues);
        _max = Mathf.Max(Value.yValues);

        if (InfoUI == null) { // have to include because setvalues is sometimes called before awake due to activating objects
            InfoUI = UIManager.NewUI<CellInfoUI>();
            InfoUI.transform.SetParent(this.transform);
            InfoUI.transform.position = this.transform.position + new Vector3(0, 10, 0); // TODO: replace 10 with serializedfield for info ui displacement
        }

        InfoUI.Init(this);
    }
    
    public override void SetStep(int step) {
        float percent = (Value.yValues[step] - _min) / (_max - _min);
        _animator.SetFloat("PercentOpen", percent);

        InfoUI.GraphUIComponent.SetXValue(Value.xValues[step]); // TODO remove conversion from xvalue to step back to xvalue
    }

    
}
