using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

///<summary>
/// Компонент, отвечающий за жизненный цикл существа. Содержит параметры и эффекты, применяемые
/// к ним. Набор параметров неизменен, каждый из параметров может изменяться только посредством
/// применяемых эффектов.
/// Синхронизируются только эффекты
///</summary>
[System.Serializable]
public class EntityLifecycle : NetworkBehaviour
{
    #region Parameters and effects
    [Header("Parameters. Set in inspector")]
    public LifecycleParameter health;
    public LifecycleParameter endurance;
    public LifecycleParameter satiety;
    public LifecycleParameter bleedingParameter;
    public LifecycleParameter radiation;

    [Header("Permanent effects. Set in inspector")]
    public LifecycleEffect regeneration;
    public LifecycleEffect enduranceRecovery;
    public LifecycleEffect hunger;

    // public LifecycleEffect bleedingEffect;
    // public LifecycleEffect radiationExcretion;
    // public LifecycleEffect radiationPoisoning;
    #endregion

    // Все активные незавершившиеся эффекты. Неактивный эффект при добавлении
    // становится активным
    private readonly SyncList<LifecycleEffect> _effects = new SyncList<LifecycleEffect>();

    // Несмотря на уникальность каждого параметра с точки зрения семантики,
    // некоторая логика работы с параметрами одинакова.
    // Динамическое добавление / удаление параметров не предполагается
    private LifecycleParameter[] _parameters;

    public event UnityAction OnDeath;

    public override void OnStartLocalPlayer() {
        if (!hasAuthority)
            return;
        
        foreach (var effect in new List<LifecycleEffect> { regeneration, enduranceRecovery, hunger } ) {
            CmdAddEffect(effect);
        }
    }

    private void Start() {
        _parameters = new [] { health, endurance, satiety };
        foreach (var parameter in _parameters)
            parameter.Initialize();
        if (health.Value > health.minValue) {
            IsAlive = true;
        }
        health.OnMin += Death;
    }

    private void Death() {
        IsAlive = false;
        Destroy(gameObject);
        OnDeath?.Invoke();
    }

    public bool IsAlive { get; private set; }

    private void Update() {
        if (!IsAlive)
            return;
        
        UpdateEffects();
    }

    #region Effects
    [Command]
    public void CmdAddEffect(LifecycleEffect effect) {
        // effect.isActive = true;
        Debug.Log($"Effect to {effect.targetParameterIndex}, speed {effect.speed} is added");
        effect.startTime = NetworkTime.time;
        _effects.Add(effect);
    }

    [Command]
    public void CmdRemoveEffect(LifecycleEffect effect) {
        _effects.Remove(effect);
    }

    bool IsPassed(LifecycleEffect effect)
        => effect.startTime + effect.duration <= NetworkTime.time;
        // => _effectsUpdateInfo[effectIndex] >= _effects[effectIndex].UpdateIterations;

    public void ApplyEffect(LifecycleEffect effect) {
        // Если эффект бесконечен или не закончился
        if ((effect.isInfinite || !IsPassed(effect))) {
            LifecycleParameter target = _parameters[effect.targetParameterIndex];
            // Если параметр восстанавливающийся и сейчас нужно восстанавливать
            if (effect.recoverToInitial) {
                if (target.Value != target.InitialValue) {
                    float sign = target.Value < target.InitialValue ? 1 : -1;
                    target.Value += Mathf.Abs(effect.speed) * sign * Time.deltaTime;
                }
            } else {
                if (effect.speed < 0)
                    Debug.Log("Apply effect to " + effect.targetParameterIndex + ". " + effect.speed);
                target.Value += effect.speed * Time.deltaTime;
            }
        }
    }

    private void UpdateEffects() {
        List<LifecycleEffect> _effectToRemove = new List<LifecycleEffect>();
        foreach (LifecycleEffect effect in _effects) {
            // Прошедшие временные эффекты откладываются для удаления
            // (нельзя изменять список, пока проходим по нему)
            if (!effect.isInfinite && IsPassed(effect))
                _effectToRemove.Add(effect);
            else
                ApplyEffect(effect);
        }
        if (isLocalPlayer) {
            foreach (LifecycleEffect effect in _effectToRemove)
                CmdRemoveEffect(effect);
        }
    }
    #endregion
}
