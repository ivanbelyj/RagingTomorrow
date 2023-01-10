using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// [RequireComponent(typeof(Entity))]
public class SimpleHUD : MonoBehaviour
{
    private Entity targetEntity;

    [SerializeField]
    private Slider healthSlider;

    [SerializeField]
    private Slider enduranceSlider;

    [SerializeField]
    private Slider satietySlider;

    // radiation, bleeding

    public void SetEntity(GameObject entity) {
        targetEntity = entity.GetComponent<Entity>();
        InitializeSlider(healthSlider, targetEntity.health);
        InitializeSlider(enduranceSlider, targetEntity.endurance);
        InitializeSlider(satietySlider, targetEntity.satiety);
        
        targetEntity.health.OnValueChanged += (oldValue, newValue) => {
            UpdateSlider(healthSlider, newValue);
        };
        targetEntity.endurance.OnValueChanged += (oldValue, newValue) => {
            UpdateSlider(enduranceSlider, newValue);
        };
        targetEntity.satiety.OnValueChanged += (oldValue, newValue) => {
            UpdateSlider(satietySlider, newValue);
        };
    }
    
    private void UpdateSlider(Slider slider, float newValue) {
        slider.value = newValue;
    }
    private void InitializeSlider(Slider slider, LifecycleParameter parameter) {
        slider.minValue = parameter.minValue;
        slider.maxValue = parameter.maxValue;
        slider.value = parameter.Value;
        // Debug.Log($"Slider is initialized. [{slider.minValue},{slider.maxValue}]."
        //     + $"Current: {slider.value}");
    }
}
