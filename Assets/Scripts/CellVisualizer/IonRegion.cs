using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IonRegion : VisualizerComponent
{
    private Dictionary<IonType, float[]> _concValues = new Dictionary<IonType, float[]>();
    private Dictionary<IonType, List<Ion>> _ions = new Dictionary<IonType, List<Ion>>();

    [SerializeField] private Bounds[] _bounds;
    [SerializeField] private string _name;

    public override string Name {
        get {return _name;}
    }

    private bool _isInitiated = false;
    public bool IsInitiated {
        get { return _isInitiated;} 
        protected set { _isInitiated = value;}
    }

    // TODO: Refactor to use loop
    public override void SetStep(int step) {
        if(IsInitiated) {
            float absSodiumCount = _concValues.TryGetValue(IonType.Sodium, out float[] sodiumCounts) ? sodiumCounts[step] : 0;
            float absPotassiumCount = _concValues.TryGetValue(IonType.Potassium, out float[] potassiumCounts) ? potassiumCounts[step] : 0;
            float absCalciumCount = _concValues.TryGetValue(IonType.Calcium, out float[] calciumCounts) ? calciumCounts[step] : 0;

            absSodiumCount *= Cell.IonDisplayMult[IonType.Sodium];
            absPotassiumCount *= Cell.IonDisplayMult[IonType.Potassium];
            absCalciumCount *= Cell.IonDisplayMult[IonType.Calcium];

            SetCount(IonType.Sodium, (int)absSodiumCount); // TODO, update how to set count, currently doesnt change ion number cuz difference is so small
            SetCount(IonType.Potassium, (int)absPotassiumCount);
            SetCount(IonType.Calcium, (int)absCalciumCount);
        }
    }

    public void SetValues(IonType ionType, float[] values) {
        _concValues[ionType] = values;
        IsInitiated = true;
    }

    public void SetCount(IonType ionType, int count) {
        if (!_ions.ContainsKey(ionType)) {
            _ions[ionType] = new List<Ion>();
        }

        while (_ions[ionType].Count < count) {
                Ion ion = Cell.Instance.NewIon(ionType);
                int randomBoundIndex = Random.Range(0, _bounds.Length);
                ion.transform.position = Utilities.RandomPointInBounds(_bounds[randomBoundIndex]);
                ion.transform.parent = transform;
                ion.IonBounds = _bounds[randomBoundIndex];
                _ions[ionType].Add(ion);
        }

        while (_ions[ionType].Count > count) { // TODO: could change this to make the object inactive instead of destroying it
            GameObject.Destroy(_ions[ionType][0].gameObject);
            _ions[ionType].RemoveAt(0);
        }
    }

    public void OnDrawGizmosSelected() {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        foreach(Bounds bound in _bounds) {
            Gizmos.DrawCube(bound.center, bound.size);
        }
    }

}
