using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

///<summary>
/// Представляет параметр жизненного цикла существа
///</summary>
[System.Serializable]
public class LifecycleParameter
{
    private enum ValueState { Min, Intermediate, Max };
    // Каким было значение последний раз
    private ValueState _valueState;
    private bool _isInitial;

    [Header("Set in inspector")]
    public float minValue = 0;
    public float maxValue = 20;
    // public string name;

    public event UnityAction OnMin;
    public event UnityAction OnMax;
    public event UnityAction OnInitial;
    public event UnityAction OnNotInitial;

    // Начальное значение (например, максимальное здоровье), к которому возвращают
    // некоторые эффекты
    [SerializeField]
    private float _initialValue;
    public float InitialValue {
        get => _initialValue;
        set {
            float newInitial = Mathf.Clamp(value, minValue, maxValue);
            float oldInitial = _initialValue;

            if (newInitial != oldInitial) {
                _initialValue = newInitial;
                OnInitialChanged?.Invoke(oldInitial, newInitial);
                InitialChangedActions(oldInitial, newInitial);
            }
        }
    }

    public delegate void ValueChanged(float oldValue, float newValue);
    public event ValueChanged OnValueChanged;

    [SerializeField]
    private float _value;
    public float Value {
        get => _value;
        set {
            float oldValue = _value;
            _value = Mathf.Clamp(value, minValue, maxValue);
            if (oldValue != _value)
                OnValueChanged?.Invoke(oldValue, _value);
            else
                return;

            // Если минимум достигнут, причем до этого значение было другим
            if (_value == minValue && _valueState != ValueState.Min) {
                _valueState = ValueState.Min;
                OnMin?.Invoke();
            }
            if (_value == maxValue && _valueState != ValueState.Max) {
                _valueState = ValueState.Max;
                OnMax?.Invoke();
            }
            if (_value > minValue && _value < maxValue
                && _valueState != ValueState.Intermediate) {
                _valueState = ValueState.Intermediate;
                // Событие для промежуточного значения не имеет особого смысла
            }

            if (_value == InitialValue && !_isInitial) {
                _isInitial = true;
                OnInitial?.Invoke();
            }
            if (_value != InitialValue && _isInitial) {
                _isInitial = false;
                OnNotInitial?.Invoke();
            }
        }
    }

    public delegate void InitialChanged(float oldInitial, float newInitial);
    // Например, игрок заболел и у него сократилось максимальное здоровье
    public event InitialChanged OnInitialChanged;

    public void Initialize() {
        // Debug.Log($"{name} is initialized");
        // OnValueChanged += (oldVal, newVal) => Debug.Log($"{name} is changed to {newVal}");
    }
    private void InitialChangedActions(float oldInitial, float newInitial)  {
        // Ситуация 1: норма была 100% здоровья. Игрок заболел, теперь никакие аптечки
        // не восстановят выше 80%.
        // Если у игрока до болезни было 100% (parameter == initial), то
        // болезнь снижает лишь максимум, а не текущее значение.
        // Если до болезни было 20%, опять же, в этом плане ничего не изменилось
        // Однако, если смена initial попала прямо в текущий параметр,
        // например, игрок заболел, initial стано 80%, а у него и так было 80%,
        // нужно запустить OnInitial (например, остановить эффекты, которые 
        // заканчиваются по наступлению initial)
        
        if (oldInitial > newInitial) {
            if (newInitial == _value) {
                _isInitial = true;
                OnInitial?.Invoke();
            }
        }

        // Ситуация 2: Игрок вылечился, норма 80% поднялась до нормы 100%
        if (oldInitial < newInitial) {
            // Если у игрока до болезни было 80% (parameter == initial),
            // то теперь нужно дать NotInitial, чтобы, например, запустить регенерацию
            if (_value < newInitial) {
                _isInitial = false;
                OnNotInitial?.Invoke();
            }
            // Если до выздоровления было 20%, то в этом плане без разницы,
            // является ли максимум 80%, или же 100%

            // Если лечение подняло норму до 100%, а оказалось, что
            // у игрока было выше нормы (not initial), теперь
            // значение совершенно нормально. Например, можно остановить эффекты возврата
            if (_value == newInitial) {
                _isInitial = true;
                OnInitial?.Invoke();
                // Код дублируется, он не объединен для ясности рассуждений
            }
        }

        // oldInitial == newInitial не бывает по логике
    }

    // public void ApplyEffect(LifecycleEffect effect) {
    //     // Если эффект активен, а также либо бесконечен, либо не прошел
    //     if (effect.isActive && (effect.isInfinite || !effect.IsPassed)) {
    //         this.Value += effect.speed * Time.deltaTime;
    //     }

    // }
}
