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

    [SerializeField] private IonRegion _intraRegion;

    private float _time = 0.0f;
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
        _time = 0.0f;
        _step = 0;
    }

    public override void StepTime(float deltaTime) {
        _time += deltaTime;

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

    public override void RunModel() {
        int arrlen = 0;
        //IntPtr arrPtr = run(ref arrlen); // original model without medication effects
        IntPtr arrPtr = runtest(ref arrlen, 241, 1190, 1800, 440); //TODO: make sure units are appropriate (currently usin nM)

        double[] result = new double[arrlen];

        Marshal.Copy(arrPtr, result, 0, arrlen);

        float[] flResult = Array.ConvertAll<double, float>(result, x => (float)x);

        Dictionary<string, float[]> variables = ColumnedArrayToArrayDictionary<string, float>(_variableNames, flResult);
        _cachedVariables = variables;

        foreach (float val in variables["svolt"]) {
            Debug.Log(val + "\n");
        }

        SetVisualizations(variables);
    }

    private void SetVisualizations(Dictionary<string, float[]> variables) {
        _iKr.SetValues(variables["IKr"]);
        _iKs.SetValues(variables["IKs"]);
        _iK1.SetValues(variables["IK1"]);
        _ito.SetValues(variables["Ito"]);
        _iNa.SetValues(variables["INa"]);
        _ibNa.SetValues(variables["IbNa"]);
        _iNaK.SetValues(variables["INaK"]);
        _iCaL.SetValues(variables["ICaL"]);
        _ibCa.SetValues(variables["IbCa"]);
        _iNaCa.SetValues(variables["INaCa"]);
        _irel.SetValues(variables["Irel"]);
        
        _intraRegion.SetConcentration(IonType.Sodium, variables["Nai"]);
    }
}
