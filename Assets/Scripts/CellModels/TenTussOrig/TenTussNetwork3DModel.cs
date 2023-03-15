using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenTussNetwork3DModel : CellModel
{
    [SerializeField] private IonChannel[] _ionChannels;

    [SerializeField] private IonRegion _intraRegion, _extraRegion, _srRegion;

    [SerializeField] private CellInfoUI _voltageUI; 

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
        StartCoroutine(APIManager.RunModelServer("dynamic", Cell.SimStopTime, (ModelData? modelData, string errMessage) => {
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

    public override void SetStep(int step)
    {
        _voltageUI.GraphUIComponent.SetXValue(Value.xValues[step]);
    }

    private void SetupModel(ModelData modelData) {
        Value = modelData.modelValues["svolt"];
        _timeStep = modelData.timeStep;

        _voltageUI.Init(this, Cell.SimSpeed);

        foreach (IonChannel ionChannel in _ionChannels) {
            ionChannel.SetValues(modelData.modelValues[ionChannel.Name]);
        }
        
        //this is copied from network model, might want to make a base class for both
        // TODO: port ion regions over to ModelValue setup
        _intraRegion.SetValues(IonType.Sodium, modelData.modelValues["Nai"].yValues);
        _intraRegion.SetValues(IonType.Potassium, modelData.modelValues["Ki"].yValues);
        _intraRegion.SetValues(IonType.Calcium, modelData.modelValues["Cai"].yValues);

        _extraRegion.SetValues(IonType.Sodium, modelData.modelValues["Nao"].yValues);
        _extraRegion.SetValues(IonType.Potassium, modelData.modelValues["Ko"].yValues);
        _extraRegion.SetValues(IonType.Calcium, modelData.modelValues["Cao"].yValues);

        _srRegion.SetValues(IonType.Calcium, modelData.modelValues["CaSR"].yValues);
    }
}
