using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGraphable
{

    public string Name {
        get;
    }

    public float[] XValues {
        get;
    }

    public float[] YValues{
        get;
    }
    
}
