using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour, IInteractable // basically gamemanager
{
    [SerializeField] private float _simSpeed = 1.0f;
    [SerializeField] private CellInfoUI _cellInfoUI;
    [SerializeField] private CellModelType _modelType = CellModelType.TenTussOrigModel;
    [SerializeField] private TenTussOrigModel _tenTussOrigModel;
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

    void Awake() {
        // Singleton
        if (Instance == null) {
            Instance = this;
        } else {
            GameObject.Destroy(this);
        }

        _cellInfoUI.GraphUIComponent.Speed = _simSpeed;
    }

    void Start() {
        LoadCellModel(_modelType);
    }

    void Update() {
        if (_currentCellModel) {
            _time += Time.deltaTime * _simSpeed; // treat realtime as ms (1 second -> 1 ms)
            _cellInfoUI.GraphUIComponent.SetXValue(_time);
            _currentCellModel.SetTime(_time);
            //Debug.Log(_time);
        }
    }

    public void LoadCellModel(CellModelType modelType) {
        Debug.Log("loading cell model");
        switch (modelType) {
            case CellModelType.TenTussOrigModel:
                ReplaceCellModel(_tenTussOrigModel);
                break;
            default:
                ReplaceCellModel(null);
                break;
        }
        _time = 0.0f;
        _currentCellModel.SetTime(_time);
    }

    private void ReplaceCellModel(CellModel model) {
        foreach (Transform child in transform) { // assuming only children of this gameobject are the models
            child.gameObject.SetActive(false);
        }

        if (model != null) {
            model.gameObject.SetActive(true);
            _cellInfoUI.Init(model);
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
