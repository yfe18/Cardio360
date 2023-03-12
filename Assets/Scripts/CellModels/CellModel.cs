using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;


public enum CellModelType {
    TenTussOrigModel,
    TenTussNetworkModel
}

public abstract class CellModel : VisualizerComponent
{
    public override abstract string Name { get; }
    public abstract string CurrentUnits { get; }
    public abstract string ConcentrationUnits { get; }

    public abstract string TimeUnits { get; }

    private bool _isInitiated = false;
    public bool IsInitiated {
        get { return _isInitiated;} 
        protected set { _isInitiated = value;}
    }

    public abstract void RunModel();
    public abstract void SetTime(float time);

    public override void SetStep(int step) // just so I can use VisualizerComponent TODO: change visualizer component to not have setstep, put that in a new class
    {
        //throw new NotImplementedException();
    }

    // TODO: move to utilites
    protected static Dictionary<T1, T2[]> ColumnedArrayToArrayDictionary<T1, T2>(T1[] varNames, T2[] values) {
        Debug.Log(values.Length);
        Debug.Log(varNames.Length);
        if (values.Length % varNames.Length == 0) {
            Dictionary<T1, T2[]> output = new Dictionary<T1, T2[]>();

            foreach (T1 varName in varNames) {
                output[varName] = new T2[values.Length / varNames.Length];
            }
            
            for (int i = 0; i < values.Length / varNames.Length; i++) { // values.Length
                for (int v = 0; v < varNames.Length; v++) {
                    output[varNames[v]][i]= values[(i * varNames.Length) + v];
                }
            }

            return output;
        } else {
            throw new System.IndexOutOfRangeException("INVALID ARRAY LENGTH: Length of");
        }
    }
}
