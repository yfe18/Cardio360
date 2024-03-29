using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Simulation
{
    public string name;
    public string protocol;
    public int stoptime, drugConc, INa_IC50, ICaL_IC50, IKr_IC50;


    public Simulation(string name, string protocol, int stoptime, int drugConc = 0, 
        int INa_IC50 = 0, int ICaL_IC50 = 0, int IKr_IC50 = 0) 
    {
        this.name = name;
        this.protocol = protocol;
        this.stoptime = stoptime;
        this.drugConc = drugConc;
        this.INa_IC50 = INa_IC50;
        this.ICaL_IC50 = ICaL_IC50;
        this.IKr_IC50 = IKr_IC50;
    }

    public override string ToString()
    {
        return name;
    }
}
