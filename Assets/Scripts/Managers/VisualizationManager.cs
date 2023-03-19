using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizationManager : MonoBehaviour
{
    public static VisualizationManager Instance {
        get; private set;
    }

    [SerializeField] private float _simSpeed = 1.0f;
    [SerializeField] private int _simStopTime = 1000;
    [SerializeField] private float _naIonDisplayMult, _kIonDisplayMult, _caIonDisplayMult;
    [SerializeField] private ComputationalModelType _compModelType;
    [SerializeField] private CellVisualization _cellVisualization;
    [SerializeField] private Ion _calciumIon, _sodiumIon, _potassiumIon; 

    private ComputationalModel _compModel;
    
    private Simulation _currentSimulation;

    public bool IsRunning {
        get; private set;
    }

    public float SimTime {
        get; private set;
    }

    public static Dictionary<IonType, float> IonDisplayMult {
        get;
        private set;
    }

    void Awake() {
        // Singleton
        if (Instance == null) {
            Instance = this;
        } else {
            GameObject.Destroy(this);
        }

        IonDisplayMult = new Dictionary<IonType, float>() {
            { IonType.Sodium, _naIonDisplayMult },
            { IonType.Potassium, _kIonDisplayMult },
            { IonType.Calcium, _caIonDisplayMult }
        };

        _cellVisualization.gameObject.SetActive(true);

        switch (_compModelType) {
            case ComputationalModelType.TenTusscherOriginal:
                throw new NotImplementedException();
            case ComputationalModelType.TenTusscherNetwork:
                _compModel = new TenTussNetwork();
                break;
            default:
                _compModel = new TenTussNetwork();
                break;
        }

        IsRunning = false;

        RunSimulation(new Simulation("dynamic", _simStopTime)); // TEMP
    }

    public void RunSimulation(Simulation simulation) {
        IsRunning = false;
        SimTime = 0;
        _currentSimulation = simulation;

        StartCoroutine(_compModel.Run(simulation.protocol, simulation.stoptime,
            (ModelData? data, string errMessage) => {
                if (!data.HasValue) {
                    Debug.LogError(errMessage);
                }
                _cellVisualization.SetVisualizerValues(data.GetValueOrDefault());
                IsRunning = true;
            },
            simulation.drugConc, simulation.INa_IC50, simulation.ICaL_IC50, simulation.IKr_IC50)
        );
    }

    // could move simulation logic to a specialized "Simulation Manager"
    void Update() {
        if (IsRunning) {
            SimTime += Time.deltaTime * _simSpeed; // treat realtime as ms (1 second -> 1 ms)
            Debug.Log(SimTime);
            SimTime = (SimTime % _currentSimulation.stoptime);
            Debug.Log(SimTime);
            int step = (int)(SimTime / _compModel.TimeStep);
            Debug.Log(step);
            _cellVisualization.SetStep(step);
        }
    }

    public Ion NewIon(IonType ionType) {
        switch (ionType) {
            case IonType.Sodium:
                return GameObject.Instantiate<Ion>(_sodiumIon);
            case IonType.Calcium:
                return GameObject.Instantiate<Ion>(_calciumIon);
            case IonType.Potassium:
                return GameObject.Instantiate<Ion>(_potassiumIon);
            default:
                // TODO: add exception
                return null;
        }
    }
}
