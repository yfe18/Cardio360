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

    [SerializeField] private CellInfoUI _voltageUI;

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

    private string[] _variableNames = new string[19] {"time", "IKr", "IKs", "IK1", "Ito", 
        "INa", "IbNa", "INaK", "ICaL", "IbCa", "INaCa", "Irel", "svolt", "Nai", "Nao", 
        "Ki", "Ko", "Cai", "Cao"
    };

    void Awake() {
        if (!IsInitiated) {
            RunModel();
            _voltageUI.Init(this, Cell.SimSpeed);
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

    public override void SetStep(int step)
    {
        _voltageUI.GraphUIComponent.SetXValue(Value.xValues[step]);
    }

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
        IsInitiated = true;
    }

    // TODO: currently each channel gets a copy of their own data, may use too much memory. Perhaps switch to setting the value of each as needed
    private void SetVisualizations(Dictionary<string, float[]> variables) {
        Value = GraphValues(variables["time"], variables["svolt"]);

        _iKr.SetValues(GraphValues(variables["time"], variables["IKr"])); // TODO: can change this to loop and just use the name of each channel as the dict index
        _iKs.SetValues( GraphValues(variables["time"], variables["IKs"]));
        _iK1.SetValues(GraphValues(variables["time"], variables["IK1"]));
        _ito.SetValues(GraphValues(variables["time"], variables["Ito"]));
        _iNa.SetValues(GraphValues(variables["time"], variables["INa"]));
        _ibNa.SetValues(GraphValues(variables["time"], variables["IbNa"])); // TODO: IbNa graphs wrong
        _iNaK.SetValues(GraphValues(variables["time"], variables["INaK"]));
        _iCaL.SetValues(GraphValues(variables["time"], variables["ICaL"]));
        _ibCa.SetValues(GraphValues(variables["time"], variables["IbCa"])); // TODO: IbCa graphs wrong, both are < 0 and have large offset from x axis
        _iNaCa.SetValues(GraphValues(variables["time"], variables["INaCa"]));
        _irel.SetValues(GraphValues(variables["time"], variables["Irel"]));
        
        _intraRegion.SetValues(IonType.Sodium, variables["Nai"]);
        _intraRegion.SetValues(IonType.Potassium, variables["Ki"]);
        _intraRegion.SetValues(IonType.Calcium, variables["Cai"]);

        _extraRegion.SetValues(IonType.Sodium, variables["Nao"]);
        _extraRegion.SetValues(IonType.Potassium, variables["Ko"]);
        _extraRegion.SetValues(IonType.Calcium, variables["Cao"]);
    }

    private ModelValue GraphValues(float[] xValues, float[] yValues) {
        Graph graph = new Graph(xValues, yValues);
        return graph.DrawGraph(1024, 512, 0, 20, Color.white, new LineProperties(5, Color.red), LineProperties.NONE,
            new LineProperties(10, Color.black));
    }
}
