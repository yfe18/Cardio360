using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour, IInteractable // basically gamemanager
{
    [SerializeField] private float _simSpeed = 1.0f;
    [SerializeField] private int _simStopTime = 1000;
    [SerializeField] private float _naIonDisplayMult, _kIonDisplayMult, _caIonDisplayMult;
    [SerializeField] private CellModelType _modelType = CellModelType.TenTussOrigModel;
    [SerializeField] private CellModel _tenTussOrigModel, _tenTussNetworkModel, _tenTussNetwork3DModel;
    [SerializeField] private Ion _calciumIon, _sodiumIon, _potassiumIon; 

    private float _time = 0.0f; // in milliseconds
    
    private CellModel _currentCellModel;

    public static Cell Instance { // create singleton class
        get;
        private set;
    }

    public static float SimSpeed {
        get {
            return Instance._simSpeed;
        }
        set {
            Instance._simSpeed = value;
        }
    }

    public static int SimStopTime {
        get {
            return Instance._simStopTime;
        }
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
    }

    void Start() {
        StartCoroutine(LoadCellModel(_modelType));
    }

    void Update() {
        if (_currentCellModel) {
            _time += Time.deltaTime * _simSpeed; // treat realtime as ms (1 second -> 1 ms)
            _currentCellModel.SetTime(_time);
        }
    }

    public IEnumerator LoadCellModel(CellModelType modelType) {
        Debug.Log("loading cell model");
        switch (modelType) {
            case CellModelType.TenTussOrigModel:
                yield return ReplaceCellModel(_tenTussOrigModel);
                break;
            case CellModelType.TenTussNetworkModel:
                yield return ReplaceCellModel(_tenTussNetworkModel);
                break;
            case CellModelType.TenTussNetwork3DModel:
                yield return ReplaceCellModel(_tenTussNetwork3DModel);
                break;
            default:
                yield return ReplaceCellModel(null);
                break;
        }
        _time = 0.0f;
        _currentCellModel.SetTime(_time);
    }

    private IEnumerator ReplaceCellModel(CellModel model) {
        foreach (Transform child in transform) { // assuming only children of this gameobject are the models
            child.gameObject.SetActive(false);
        }

        if (model != null) {
            model.gameObject.SetActive(true);

            yield return new WaitUntil(() => {return model.IsInitiated;});
        }

        _currentCellModel = model;
    }

    public void OnInteract(Collider collider) { // don't know if this is the best class to put it in
        if (collider.GetComponent<IonChannel>()) {
            
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
