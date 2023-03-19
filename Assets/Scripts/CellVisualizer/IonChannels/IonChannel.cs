using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IonChannel : VisualizerComponent
{

    [SerializeField] protected string _name;

    public override string Name {
        get {
            return _name;
        }
    }

    public CellModel ParentModel { get; private set; }

    public virtual void Awake() {
        if (_name == "") {
            _name = gameObject.name;
        }
        ParentModel = GetComponentInParent<CellModel>();
    }

    public virtual void SetValues(ModelValue modelValue) {
        Value = modelValue;
    }
}
