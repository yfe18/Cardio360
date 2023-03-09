using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WristMenuUI : MonoBehaviour
{
    [SerializeField] private Button _simulationButton, _medicationButton;

    [SerializeField] private RectTransform _simulationPanel, _medicationPanel;

    // Start is called before the first frame update
    void Start()
    {
        _simulationButton.onClick.AddListener(() => {OpenScreen(_simulationPanel);});
        _medicationButton.onClick.AddListener(() => {OpenScreen(_medicationPanel);});
    }

    void OpenScreen(RectTransform screen) {
        screen.gameObject.SetActive(!screen.gameObject.activeSelf);
    }
}
