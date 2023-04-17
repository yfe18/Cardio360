using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Page : MonoBehaviour{
    [SerializeField] private string _title;

    public string Title {
        get { return _title; }
    }

    public PagePanel ParentPanel {
        get; 
        set;
    }
}
