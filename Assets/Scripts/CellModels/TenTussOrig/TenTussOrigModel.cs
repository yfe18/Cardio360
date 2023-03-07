using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class TenTussOrigModel : CellModel
{
    [DllImport("OrigModelLib", CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr run(ref int arrlen);

    [DllImport("DrugModelLib", CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr runtest(ref int arrlen, double conc, double Na_IC50, double CaL_IC50, double IKr_IC50);

    [SerializeField] private IonChannel _iKr, _iKs, _iK1, _ito, 
        _iNa, _ibNa, _iNaK, _iCaL, _ibCa, _iNaCa, _irel;

    [SerializeField] private IonRegion _intraRegion, _extraRegion;

    private float _timeStep = 2f; // time step in data from model
    private int _step = 0;

    private Dictionary<string, float[]> _cachedVariables;

    public override string Name {
        get {
            return "Ten Tusscher Original Model";
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

    public override float[] XValues {
        get {
            return _cachedVariables["time"];
        }
    }

    public override float[] YValues {
        get {
            return _cachedVariables["svolt"];
        }
    }

    private string[] _variableNames = new string[19] {"time", "IKr", "IKs", "IK1", "Ito", 
        "INa", "IbNa", "INaK", "ICaL", "IbCa", "INaCa", "Irel", "svolt", "Nai", "Nao", 
        "Ki", "Ko", "Cai", "Cao"
    };

    void Awake() {
        Debug.Log("awoken");
        if (_cachedVariables == null) {
            RunModel();
        }
        _step = 0;
    }

    public override void SetTime(float time) { // time in ms
        // wraps to start if overflow
        _step = ((int)(time / _timeStep)) % (_cachedVariables["time"].Length - 1);

        foreach (VisualizerComponent comp in GetComponentsInChildren<VisualizerComponent>()) {
            comp.SetStep(_step);
        }
    }

    /*void Update() {
        _time += (Time.deltaTime * SimSpeed);

        if (_time > (_cachedVariables["time"][_cachedVariables["time"].Length - 1]) / 1000) {
            _time = 0.0f;
            _step = 0;
        }

        if (_time > (_cachedVariables["time"][_step] / 1000)) {
            _step++;
            foreach (IonChannel channel in transform.GetComponentsInChildren<IonChannel>()) {
                channel.Step = _step;
            }
        }
    }*/

    public override void RunModel() { // TODO: add all variables to model (all currents and all regions)
        int arrlen = 0;
        //IntPtr arrPtr = run(ref arrlen); // original model without medication effects
        IntPtr arrPtr = runtest(ref arrlen, 241, 1190, 1800, 440); //TODO: make sure units are appropriate (currently usin nM)

        double[] result = new double[arrlen];

        Marshal.Copy(arrPtr, result, 0, arrlen);

        float[] flResult = Array.ConvertAll<double, float>(result, x => (float)x);

        Dictionary<string, float[]> variables = ColumnedArrayToArrayDictionary<string, float>(_variableNames, flResult);
        _cachedVariables = variables;
        SetVisualizations(variables);
    }

    // TODO: currently each channel gets a copy of their own data, may use too much memory. Perhaps switch to setting the value of each as needed
    private void SetVisualizations(Dictionary<string, float[]> variables) {
        _iKr.SetValues(variables["IKr"]); // TODO: can change this to loop and just use the name of each channel as the dict index
        _iKs.SetValues(variables["IKs"]);
        _iK1.SetValues(variables["IK1"]);
        _ito.SetValues(variables["Ito"]);
        _iNa.SetValues(variables["INa"]);
        _ibNa.SetValues(variables["IbNa"]); // TODO: IbNa graphs wrong
        _iNaK.SetValues(variables["INaK"]);
        _iCaL.SetValues(variables["ICaL"]);
        _ibCa.SetValues(variables["IbCa"]); // TODO: IbCa graphs wrong, both are < 0 and have large offset from x axis
        _iNaCa.SetValues(variables["INaCa"]);
        _irel.SetValues(variables["Irel"]);
        
        _intraRegion.SetValues(IonType.Sodium, variables["Nai"]);
        _intraRegion.SetValues(IonType.Potassium, variables["Ki"]);
        _intraRegion.SetValues(IonType.Calcium, variables["Cai"]);

        _extraRegion.SetValues(IonType.Sodium, variables["Nao"]);
        _extraRegion.SetValues(IonType.Potassium, variables["Ko"]);
        _extraRegion.SetValues(IonType.Calcium, variables["Cao"]);
    }
}
