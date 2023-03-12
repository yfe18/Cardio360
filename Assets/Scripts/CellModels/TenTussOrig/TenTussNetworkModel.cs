using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenTussNetworkModel : CellModel
{
    [SerializeField] private IonChannel _iKr, _iKs, _iK1, _ito, 
        _iNa, _ibNa, _iNaK, _iCaL, _ibCa, _iNaCa, _irel;

    [SerializeField] private IonRegion _intraRegion, _extraRegion;

    private int _step = 0;
    private float _timeStep = -1;

    public override string Name {
        get {
            return "Ten Tusscher API Model";
        }
    }
    public override string CurrentUnits {
        get {
            return "pS/F";
        }
    }
    public override string ConcentrationUnits {
        get {
            return "mmol/L";
        }
    }

    public override string TimeUnits {
        get {
            return "ms";
        }
    }

    void Awake() {
        if (!IsInitiated) {
            RunModel();
        }
        _step = 0;
    }

    public override void RunModel() {
        StartCoroutine(APIManager.RunModelServer("dynamic", 1000, (ModelData? modelData, string errMessage) => {
            if (modelData.HasValue) {
                SetupModel(modelData.Value);
                IsInitiated = true;
            } else {
                Debug.LogError(errMessage);
                IsInitiated = true; // so the loading can continue, TODO: maybe switch to set an IsErrored or something
            }
        }));
    }

    public override void SetTime(float time) { // time in ms
        // wraps to start if overflow
        _step = ((int)(time / _timeStep)) % (Value.xValues.Length - 1);

        foreach (VisualizerComponent comp in GetComponentsInChildren<VisualizerComponent>()) {
            comp.SetStep(_step);
        }
    }

    private void SetupModel(ModelData modelData) {
        Value = modelData.modelValues["svolt"];
        _timeStep = modelData.timeStep;

        _iKr.SetValues(modelData.modelValues["IKr"]); // TODO: can change this to loop and just use the name of each channel as the dict index
        _iKs.SetValues(modelData.modelValues["IKs"]); // TODO: can just fromJson initially to an array rather than using a list so we dont have to convert everytime
        _iK1.SetValues(modelData.modelValues["IK1"]);
        _ito.SetValues(modelData.modelValues["Ito"]);
        _iNa.SetValues(modelData.modelValues["INa"]);
        _ibNa.SetValues(modelData.modelValues["IbNa"]); // TODO: IbNa graphs wrong
        _iNaK.SetValues(modelData.modelValues["INaK"]);
        _iCaL.SetValues(modelData.modelValues["ICaL"]);
        _ibCa.SetValues(modelData.modelValues["IbCa"]); // TODO: IbCa graphs wrong, both are < 0 and have large offset from x axis
        _iNaCa.SetValues(modelData.modelValues["INaCa"]);
        _irel.SetValues(modelData.modelValues["Irel"]);
        
        /*_intraRegion.SetValues(IonType.Sodium, variables["Nai"].values.ToArray());
        _intraRegion.SetValues(IonType.Potassium, variables["Ki"].values.ToArray());
        _intraRegion.SetValues(IonType.Calcium, variables["Cai"].values.ToArray());

        _extraRegion.SetValues(IonType.Sodium, variables["Nao"].values.ToArray());
        _extraRegion.SetValues(IonType.Potassium, variables["Ko"].values.ToArray());
        _extraRegion.SetValues(IonType.Calcium, variables["Cao"].values.ToArray());*/
    }
}
