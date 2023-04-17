using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewSimPage : Page
{
    [SerializeField] private Button _newSimButton;
    [SerializeField] private Page _descriptionPage;
    [SerializeField] private SelectPanel _selectPanel;

    void Awake() {
        _newSimButton.onClick.AddListener(() => {
            Simulation selectedSim = (Simulation)_selectPanel.SelectedValue;

            ParentPanel.Open(_descriptionPage, selectedSim.name);
            //VisualizationManager.Instance.RunSimulation(selectedSim);
        });
    }

}
