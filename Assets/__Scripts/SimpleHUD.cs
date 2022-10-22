using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// [RequireComponent(typeof(Entity))]
public class SimpleHUD : MonoBehaviour
{
    private EntityLifecycle lifecycle;

    [Header("Set in inspector")]
    [SerializeField]
    private Slider healthSlider;

    [SerializeField]
    private Slider enduranceSlider;

    [SerializeField]
    private Slider satietySlider;

    // radiation, bleeding

    public void SetEntity(GameObject entity) {
        lifecycle = entity.GetComponent<Entity>().lifecycle;
        InitializeSlider(healthSlider, lifecycle.health);
        InitializeSlider(enduranceSlider, lifecycle.endurance);
        InitializeSlider(satietySlider, lifecycle.satiety);
        
        lifecycle.health.OnValueChanged += (oldValue, newValue) => {
            UpdateSlider(healthSlider, newValue);
        };
        lifecycle.endurance.OnValueChanged += (oldValue, newValue) => {
            UpdateSlider(enduranceSlider, newValue);
        };
        lifecycle.satiety.OnValueChanged += (oldValue, newValue) => {
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
        Debug.Log($"Slider is initialized. [{slider.minValue},{slider.maxValue}]."
            + $"Current: {slider.value}");
    }
}
