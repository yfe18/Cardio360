using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ModelData
{
    public Dictionary<string, ModelValue> modelValues;
    public float timeStep;
    public List<float> timeValues;

    public ModelData(float timeStep) {
        modelValues = new Dictionary<string, ModelValue>();
        timeValues = new List<float>();
        this.timeStep = timeStep;
    }

}
