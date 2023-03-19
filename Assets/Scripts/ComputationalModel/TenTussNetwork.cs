using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenTussNetwork : ComputationalModel
{
    public override string Name {
        get { return "Ten Tusscher Network Model"; }
    }

    public override string CurrentUnits {
        get { return "pS/F"; }
    }
    public override string ConcentrationUnits {
        get { return "mmol/L"; }
    }

    public override string TimeUnits {
        get { return "ms"; }
    }

    private float _timeStep;
    public override float TimeStep { 
        get { return _timeStep; } 
    }

    public override IEnumerator Run(string protocol, int stoptime, Action<ModelData?, string> returnCallback, 
        int drugConc = 0, int INa_IC50 = 0, int ICaL_IC50 = 0, int IKr_IC50 = 0) // TODO: remove default params
    {
        yield return APIManager.RunModelServer("dynamic", stoptime, (ModelData? data, string errMessage) => {
                _timeStep = data.GetValueOrDefault().timeStep;
                returnCallback(data, errMessage);
            }, 
            drugConc, INa_IC50, ICaL_IC50, IKr_IC50);
    }
}
