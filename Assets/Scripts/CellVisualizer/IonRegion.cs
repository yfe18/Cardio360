using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IonRegion : MonoBehaviour
{
    private Dictionary<IonType, List<Ion>> _ions = new Dictionary<IonType, List<Ion>>();

    private Bounds _bounds;

    public void Awake() {
        if (_bounds == default(Bounds)) { Init(); }
    }

    private void Init() {
        Debug.Log("init bounds");
        _bounds = GetComponent<BoxCollider>().bounds;
        Debug.Log(_bounds.min);
        Debug.Log(_bounds.max);
    }


    public void SetConcentration(IonType ionType, float[] concentration) {
        if(_bounds == default(Bounds)) { Init(); }

        if (!_ions.ContainsKey(ionType)) {

            _ions[IonType.Sodium] = new List<Ion>();

            for (int i = 0; i < concentration[0]; i++) {
                Ion ion = Cell.Instance.NewIon(ionType);
                ion.transform.position = Utilities.RandomPointInBounds(_bounds);
                ion.transform.parent = transform;
                _ions[ionType].Add(ion);
            }
        }
        else if (_ions[ionType].Count <= concentration[0]) {
            while (_ions[ionType].Count < concentration[0]) {
                Ion ion = Cell.Instance.NewIon(ionType);
                ion.transform.position = Utilities.RandomPointInBounds(_bounds);
                ion.transform.parent = transform;
                _ions[ionType].Add(ion);
            }
        } else {
            while (_ions[ionType].Count > concentration[0]) {
                GameObject.Destroy(_ions[ionType][0].gameObject);
            }
        }
        Debug.Log('c');
        Debug.Log(concentration[0]);
        Debug.Log(GetComponentsInChildren<Ion>().Length);
    }

}
