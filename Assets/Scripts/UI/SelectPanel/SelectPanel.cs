using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectPanel : MonoBehaviour
{
    [SerializeField] private RectTransform _selectContainer;
    [SerializeField] private SelectOption _defaultOption;

    private SelectOption _currentSelection = null;

    public object SelectedValue {
        get { return _currentSelection.Value; }
    }

    void Awake() {
        foreach(SelectOption selectOpt in _selectContainer.GetComponentsInChildren<SelectOption>()) {
            selectOpt.onClick.AddListener(() => {
                SetOption(selectOpt);
            });
        }
    }

    void Start() {
        SetOption(_defaultOption);
    }

    private void SetOption(SelectOption option) {
        if (_currentSelection) _currentSelection.SetEnabled(true);

        option.SetEnabled(false);
        _currentSelection = option;
    }
}
