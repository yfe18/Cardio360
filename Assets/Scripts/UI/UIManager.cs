using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour // TODO: Add singleton class
{
    public static UIManager Instance {
        get;
        private set;
    }

    [SerializeField] private CellInfoUI _cellInfoUI;

    private Dictionary<Type, GameObject> _typePrefabDictionary = new Dictionary<Type, GameObject>();

    void Awake() {
        // Singleton
        if (Instance == null) {
            Instance = this;
        } else {
            GameObject.Destroy(this);
        }

        // Init dictionary
        _typePrefabDictionary.Add(_cellInfoUI.GetType(), _cellInfoUI.gameObject);
    }

    public static T NewUI<T>() where T : MonoBehaviour {
        if(Instance._typePrefabDictionary.TryGetValue(typeof(T), out GameObject prefab)) {
            return GameObject.Instantiate(prefab).GetComponent<T>();
        } else {
            return null;
        }
    }
}
