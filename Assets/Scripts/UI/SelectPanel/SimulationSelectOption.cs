using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationSelectOption : SelectOption
{
    [SerializeField] private Simulation _value;

    public override object Value { // since monobehaviours dont permit generics, probably better way to do this
        get { return _value; }
    }
}
