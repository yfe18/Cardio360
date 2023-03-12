using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VisualizerComponent : MonoBehaviour
{
    public abstract void SetStep(int step);

    public abstract string Name { get; }

    public ModelValue Value {
        get;
        protected set;
    }
}
