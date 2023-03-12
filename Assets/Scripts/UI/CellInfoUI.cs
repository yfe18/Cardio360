using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellInfoUI : MonoBehaviour
{
    [SerializeField] private Text _modelName;
    [SerializeField] private GraphUI _graphUI;

    private ModelValue _modelValue;

    public GraphUI GraphUIComponent {
        get { return _graphUI; }
    }

    public void Init(VisualizerComponent component) {
        _modelName.text = component.Name;
        Debug.Log(component.Name);
        Debug.Log(component.Value);
        _graphUI.Init(component.Value);
    }

    public void Init(VisualizerComponent component, float speed) {
        Init(component);
        _graphUI.Speed = speed;
    }
}
