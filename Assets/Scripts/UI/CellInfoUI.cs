using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellInfoUI : MonoBehaviour
{
    [SerializeField] private Text _modelName;
    [SerializeField] private GraphUI _graphUI;

    public GraphUI GraphUIComponent {
        get { return _graphUI; }
    }

    public void Init(IGraphable model) {
        _modelName.text = model.Name;
        Graph graph = new Graph(model);
        _graphUI.Image = graph.DrawGraph(1024, 512, 0, 20, Color.white, new LineProperties(5, Color.red), LineProperties.NONE,
            new LineProperties(10, Color.black));
        _graphUI.Play();
    }

    public void Init(IGraphable model, float speed) {
        Init(model);
        _graphUI.Speed = speed; // TODO: change from speed to set position
    }
}
