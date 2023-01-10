using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AFPC;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

///<summary>
/// Компонент живого существа
///</summary>
public class Entity : NetworkBehaviour
{
    #region Parameters and effects
    [Header("Parameters")]
    public LifecycleParameter health;
    public LifecycleParameter endurance;
    public LifecycleParameter satiety;
    public LifecycleParameter bleedingParameter;
    public LifecycleParameter radiation;

    [Header("Effects")]
    // Permanent
    public LifecycleEffect regeneration;
    public LifecycleEffect enduranceRecovery;
    public LifecycleEffect hunger;

    // Temporary
    public LifecycleEffect enduranceDecrease;

    // public LifecycleEffect bleedingEffect;
    // public LifecycleEffect radiationExcretion;
    // public LifecycleEffect radiationPoisoning;
    #endregion

    [Header("Movement")]
    public Movement movement;

    ///<summary>
    /// Все незавершившиеся эффекты, применяемые для изменения параметров жизненного цикла.
    /// Завершившиеся эффекты удаляются после регулярного обхода.
    ///</summary>
    private readonly SyncSet<LifecycleEffect> _effects =
        new SyncSet<LifecycleEffect>(new HashSet<LifecycleEffect>());

    ///<summary>
    /// Все параметры существа, собранные для удобства обхода.
    /// Динамическое добавление / удаление параметров не предполагается
    ///</summary>
    private LifecycleParameter[] _parameters;

    public event UnityAction OnDeath;

    public override void OnStartLocalPlayer() {
        if (!hasAuthority)
            return;

        var effects = new LifecycleEffect[] { regeneration, enduranceRecovery, hunger };
        for (int i = 0; i < effects.Length; i++) {
            // effectId должен быть актуален и на клиенте, поэтому устанавливается не в Command
            // effects[i].effectId = GetVacantEffectId(enduranceDecrease);
            // CmdAddEffect(effects[i]);
            AddEffect(effects[i]);
        }
    }

    private void Awake() {
        // Порядок в массиве напрямую зависит от значений EntityParameterEnum,
        // чтобы связать ограниченный набор значений перечисления с ограниченным
        // набором параметров (причины решения описаны в упомянутом enum)
        _parameters = new [] { health, endurance, satiety };
        if (health.Value > health.minValue) {
            IsAlive = true;
        }
        // Debug.Log($"Is alive: {IsAlive}");
        health.OnMin += Death;

        movement.Initialize();
        movement.OnChangeRunning += (bool isRunning) => {
            // Debug.Log("Running changed. isRunning == " + isRunning);
            if (isRunning) {
                AddEffect(enduranceDecrease);

                // enduranceDecrease.effectId = GetVacantEffectId(enduranceDecrease);
                // CmdAddEffect(enduranceDecrease);
            }
            else {
                CmdRemoveEffect(enduranceDecrease.effectId);
            }
        };
    }

    private void Death() {
        IsAlive = false;
        Debug.Log("Entity is dead");
        OnDeath?.Invoke();
    }

    public bool IsAlive { get; private set; }

    private void Update() {
        if (!IsAlive)
            return;

        /* Control the speed */
        movement.Running();

        /* Control the jumping, ground search... */
        movement.Jumping();

        UpdateEffects();
    }

    private void FixedUpdate() {
        if (!IsAlive)
            return;
        
        movement.Accelerate();
    }

    #region Effects

    private ushort GetVacantEffectId(LifecycleEffect effect) {
        if (_effects.Count == 0)
            return 0;
        // Todo: find vacant id algorithm
        return (ushort)(_effects.Max(x => x.effectId) + 1);
    }

    public void AddEffect(LifecycleEffect effect) {
        effect.effectId = GetVacantEffectId(enduranceDecrease);
        CmdAddEffect(effect);
    }

    [Command]
    private void CmdAddEffect(LifecycleEffect effect) {
        // Debug.Log($"Effect {effect.effectId} is added");
        effect.startTime = NetworkTime.time;

        _effects.Add(effect);
    }

    [Command]
    public void CmdRemoveEffect(ushort effectId) {
        LifecycleEffect effect = _effects.First(effect => effect.effectId == effectId);
        bool isRemoved = _effects.Remove(effect);

        // Debug.Log($"Effect {effectId} is " + (isRemoved ? "removed" : "NOT REMOVED"));
    }

    bool IsPassed(LifecycleEffect effect)
        => effect.startTime + effect.duration <= NetworkTime.time;
        // => _effectsUpdateInfo[effectIndex] >= _effects[effectIndex].UpdateIterations;

    public void ApplyEffect(LifecycleEffect effect) {
        // Если эффект бесконечен или не закончился
        if ((effect.isInfinite || !IsPassed(effect))) {
            LifecycleParameter target = _parameters[(byte)effect.targetParameter];
            // Если параметр восстанавливающийся и сейчас нужно восстанавливать
            if (effect.recoverToInitial) {
                if (target.Value != target.InitialValue) {
                    float sign = target.Value < target.InitialValue ? 1 : -1;
                    target.Value += Mathf.Abs(effect.speed) * sign * Time.deltaTime;
                }
            } else {
                target.Value += effect.speed * Time.deltaTime;
            }
        }
    }

    private void UpdateEffects() {
        List<ushort> _effectsToRemove = new List<ushort>();
        foreach (LifecycleEffect effect in _effects) {
            // Прошедшие временные эффекты откладываются для удаления
            // (нельзя изменять список, пока проходим по нему)
            if (!effect.isInfinite && IsPassed(effect))
                _effectsToRemove.Add(effect.effectId);
            else
                ApplyEffect(effect);
        }
        foreach (ushort effectId in _effectsToRemove)
            CmdRemoveEffect(effectId);
    }
    #endregion
}
