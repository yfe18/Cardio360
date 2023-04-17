using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectOption : MonoBehaviour
{
    private Button _button;
    private Text _text;

    public Button.ButtonClickedEvent onClick;

    public virtual object Value { // since monobehaviours dont permit generics, probably better way to do this
        get { return gameObject.name; }
    }

    void Awake() {
        _button = GetComponent<Button>();
        _text = GetComponentInChildren<Text>();

        _button.onClick = onClick;
        
        _text.text = Value.ToString();

    }

    public void SetEnabled(bool enabled) {
        _button.interactable = enabled;
    }
}
