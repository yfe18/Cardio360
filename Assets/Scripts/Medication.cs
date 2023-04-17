using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Medication
{
    public string name;
    public float INa_IC50, ICaL_IC50, IKr_IC50;
}
