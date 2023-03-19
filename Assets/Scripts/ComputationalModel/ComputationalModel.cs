using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ComputationalModel
{

    public abstract string Name { get; }
    public abstract string CurrentUnits { get; }
    public abstract string ConcentrationUnits { get; }

    public abstract string TimeUnits { get; }

    public abstract float TimeStep { get; }

    public abstract IEnumerator Run(string protocol, int stoptime, Action<ModelData?, string> returnCallback, 
        int drugConc = 0, int INa_IC50 = 0, int ICaL_IC50 = 0, int IKr_IC50 = 0);
}
