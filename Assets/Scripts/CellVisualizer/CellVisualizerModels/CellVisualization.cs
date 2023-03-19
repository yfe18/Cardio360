using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CellVisualization : VisualizerComponent
{
    [SerializeField] protected CellInfoUI _voltageUI;
    [SerializeField] protected IonChannel[] _ionChannels;
    [SerializeField] protected IonRegion _intraRegion, _extraRegion, _srRegion;

    public void SetVisualizerValues(ModelData modelData) {
        Value = modelData.modelValues["svolt"];

        _voltageUI.Init(this);

        foreach (IonChannel ionChannel in _ionChannels) {
            ionChannel.SetValues(modelData.modelValues[ionChannel.Name]);
        }

        // TODO: port ion regions over to ModelValue setup
        _intraRegion.SetValues(IonType.Sodium, modelData.modelValues["Nai"].yValues);
        _intraRegion.SetValues(IonType.Potassium, modelData.modelValues["Ki"].yValues);
        _intraRegion.SetValues(IonType.Calcium, modelData.modelValues["Cai"].yValues);

        _extraRegion.SetValues(IonType.Sodium, modelData.modelValues["Nao"].yValues);
        _extraRegion.SetValues(IonType.Potassium, modelData.modelValues["Ko"].yValues);
        _extraRegion.SetValues(IonType.Calcium, modelData.modelValues["Cao"].yValues);

        _srRegion.SetValues(IonType.Calcium, modelData.modelValues["CaSR"].yValues);
    }

    public override void SetStep(int step)
    {
        _voltageUI.GraphUIComponent.SetXValue(Value.xValues[step]);

        foreach (VisualizerComponent comp in GetComponentsInChildren<VisualizerComponent>()) {
            if (comp != this) {
                comp.SetStep(step);
            }
        }
    }
}
