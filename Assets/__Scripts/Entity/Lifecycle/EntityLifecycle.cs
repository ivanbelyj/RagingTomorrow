using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AFPC;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

///<summary>
/// Компонент жизненного цикла живого существа
///</summary>
public class EntityLifecycle : NetworkBehaviour
{
    #region Parameters and effects
    [Header("Параметры")]
    [SerializeField]
    private LifecycleParameter health;
    [SerializeField]
    private LifecycleParameter endurance;
    [SerializeField]
    private LifecycleParameter satiety;
    [SerializeField]
    private LifecycleParameter bleed;
    [SerializeField]
    private LifecycleParameter radiation;

    [Header("Постоянные эффекты")]
    [SerializeField]
    private LifecycleEffect regeneration;
    [SerializeField]
    private LifecycleEffect enduranceRecovery;
    [SerializeField]
    private LifecycleEffect hunger;
    [SerializeField]
    private LifecycleEffect radiationExcretion;

    [Header("Временные эффекты")]
    [SerializeField]
    private LifecycleEffect runEnduranceDecrease;
    [SerializeField]
    private LifecycleEffect bleedEffect;
    [SerializeField]
    private LifecycleEffect radiationPoisoning;
    #endregion

    ///<summary>
    /// Все незавершившиеся эффекты, применяемые для изменения параметров жизненного цикла.
    /// Завершившиеся эффекты удаляются после регулярного обхода.
    ///</summary>
    private readonly SyncHashSet<LifecycleEffect> syncEffects =
        new SyncHashSet<LifecycleEffect>();

    private HashSet<LifecycleEffect> effects;
    public HashSet<LifecycleEffect> Effects => effects;

    ///<summary>
    /// Все параметры существа, собранные для удобства обхода.
    /// Динамическое добавление / удаление параметров не предполагается
    ///</summary>
    public IReadOnlyDictionary<LifecycleParameterEnum, LifecycleParameter> Parameters { get; private set; }

    public bool IsAlive { get; private set; }
    public event UnityAction OnDeath;

    private void Awake()
    {
        syncEffects.OnChange += SyncEffects;

        Parameters = new Dictionary<LifecycleParameterEnum, LifecycleParameter>() {
            { LifecycleParameterEnum.Bleeding, bleed },
            { LifecycleParameterEnum.Endurance, endurance },
            { LifecycleParameterEnum.Health, health },
            { LifecycleParameterEnum.Radiation, radiation },
            { LifecycleParameterEnum.Satiety, satiety },

        };

        health.OnMin += Die;

        if (health.Value > health.MinValue)
        {
            IsAlive = true;
        }
        else
        {
            Die();
        }

        effects = new HashSet<LifecycleEffect>();
    }

    public override void OnStartClient()
    {
        // При подключении игрока на сервере уже могли быть эффекты 
        foreach (var effect in syncEffects)
        {
            effects.Add(effect);
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        // Установка и синхронизация начальных эффектов, установленных в инспекторе
        var permanentEffects = new LifecycleEffect[] { regeneration, enduranceRecovery, hunger,
            radiationExcretion };
        for (int i = 0; i < permanentEffects.Length; i++)
        {
            AddEffect(permanentEffects[i]);
        }
    }

    private void SyncEffects(SyncHashSet<LifecycleEffect>.Operation op, LifecycleEffect item)
    {
        switch (op)
        {
            case SyncHashSet<LifecycleEffect>.Operation.OP_ADD:
                {
                    effects.Add(item);
                    break;
                }
            case SyncHashSet<LifecycleEffect>.Operation.OP_REMOVE:
                {
                    effects.Remove(item);
                    break;
                }
        }
    }

    private void Die()
    {
        IsAlive = false;
        Debug.Log("Entity is dead");
        OnDeath?.Invoke();
    }

    #region Add And Remove Effects

    [Server]
    private void AddLifecycleEffect(LifecycleEffect effect)
    {
        syncEffects.Add(effect);
    }

    [Command]
    private void CmdAddLifecycleEffect(LifecycleEffect effect)
    {
        AddLifecycleEffect(effect);
    }

    [Server]
    private void RemoveLifecycleEffect(LifecycleEffect effect)
    {
        syncEffects.Remove(effect);
    }

    [Command]
    private void CmdRemoveLifecycleEffect(LifecycleEffect effect)
    {
        RemoveLifecycleEffect(effect);
    }

    public void RemoveEffect(LifecycleEffect effect)
    {
        if (isServer)
        {
            RemoveLifecycleEffect(effect);
        }
        else
        {
            CmdRemoveLifecycleEffect(effect);
        }
    }

    /// <summary>
    /// Добавляет эффект и возвращает такой же эффект, но с установленным временем начала
    /// (который и был добавлен)
    /// </summary>
    public LifecycleEffect AddEffect(LifecycleEffect effect)
    {
        effect.StartTime = NetworkTime.time;
        if (isServer)
        {
            AddLifecycleEffect(effect);
        }
        else
        {
            CmdAddLifecycleEffect(effect);
        }
        return effect;
    }
    #endregion

    private void Update()
    {
        UpdateEffects();
    }

    private void UpdateEffects()
    {
        List<LifecycleEffect> effectsToRemove = new List<LifecycleEffect>();
        // Todo: применять только то, что не закончилось. Удалять только на сервере
        foreach (var effect in effects)
        {
            if (!effect.isInfinite && IsPassed(effect))
            {
                // Прошедшие временные эффекты откладываются для удаления
                // (нельзя изменять словарь, пока проходим по нему)
                if (isServer)
                    effectsToRemove.Add(effect);
            }
            else
            {
                ApplyEffect(effect);
            }
        }
        if (isServer)
        {
            foreach (var effectId in effectsToRemove)
                RemoveLifecycleEffect(effectId);
        }
    }

    bool IsPassed(LifecycleEffect effect) => effect.StartTime + effect.duration <= NetworkTime.time;

    public void ApplyEffect(LifecycleEffect effect)
    {
        // Если эффект бесконечен или не закончился
        if ((effect.isInfinite || !IsPassed(effect)))
        {
            LifecycleParameter target = Parameters[effect.targetParameter];
            // Если параметр восстанавливающийся и сейчас нужно восстанавливать
            // if (effect.recover) {
            //     if (target.Value != target.RecoveredValue) {
            //         float sign = target.Value < target.RecoveredValue ? 1 : -1;
            //         target.Value += Mathf.Abs(effect.speed) * sign * Time.deltaTime;
            //     }
            //     return;
            // } 
            target.Value += effect.speed * Time.deltaTime;
        }
    }

    private LifecycleEffect runEffect;

    #region Movement
    public void Run()
    {
        runEffect = AddEffect(runEnduranceDecrease);
        Debug.Log("added: " + runEffect);
    }
    public void StopRun()
    {
        RemoveEffect(runEffect);
        Debug.Log("Stop run: " + runEffect);
    }
    #endregion
}
