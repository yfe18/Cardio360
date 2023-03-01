using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CellData
{
    public float[] time;
    public float[] voltage;
    public Dictionary<string, float[]> currents;
}
