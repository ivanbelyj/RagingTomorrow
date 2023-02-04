using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// [RequireComponent(typeof(Entity))]
public class SimpleHUD : MonoBehaviour
{
    private EntityLifecycle targetEntity;

    [SerializeField]
    private Slider healthSlider;

    [SerializeField]
    private Slider enduranceSlider;

    [SerializeField]
    private Slider satietySlider;

    // radiation, bleeding

    public void SetEntity(GameObject entity) {
        targetEntity = entity.GetComponent<EntityLifecycle>();
        InitializeSlider(healthSlider, targetEntity.Parameters[LifecycleParameterEnum.Health]);
        InitializeSlider(enduranceSlider, targetEntity.Parameters[LifecycleParameterEnum.Endurance]);
        InitializeSlider(satietySlider, targetEntity.Parameters[LifecycleParameterEnum.Satiety]);
        
        targetEntity.Parameters[LifecycleParameterEnum.Health].OnValueChanged += (oldValue, newValue) => {
            UpdateSlider(healthSlider, newValue);
        };
        targetEntity.Parameters[LifecycleParameterEnum.Endurance].OnValueChanged += (oldValue, newValue) => {
            UpdateSlider(enduranceSlider, newValue);
        };
        targetEntity.Parameters[LifecycleParameterEnum.Satiety].OnValueChanged += (oldValue, newValue) => {
            UpdateSlider(satietySlider, newValue);
        };
    }
    
    private void UpdateSlider(Slider slider, float newValue) {
        slider.value = newValue;
    }
    private void InitializeSlider(Slider slider, LifecycleParameter parameter) {
        slider.minValue = parameter.MinValue;
        slider.maxValue = parameter.MaxValue;
        slider.value = parameter.Value;
        // Debug.Log($"Slider is initialized. [{slider.minValue},{slider.maxValue}]."
        //     + $"Current: {slider.value}");
    }
}
