using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// Эффект, применяемый к параметру жизненного цикла существа
///</summary>
[System.Serializable]
public struct LifecycleEffect : IEquatable<LifecycleEffect>
{
    public bool isInfinite;
    /// <summary>
    /// true, если эффект восстанавливает параметр, т.е. возвращает к заданному значению
    /// </summary>
    // public bool recover;

    [Tooltip("Значение, прибавляемое / отнимаемое от значения параметра в секунду")]
    /// <summary>
    /// Значение, прибавляемое / отнимаемое от значения параметра в секунду
    /// </summary>
    public float speed;

    
    [Tooltip("Время действия эффекта в секундах")]
    /// <summary>
    /// Время действия эффекта в секундах
    /// </summary>
    public float duration;
    
    // Параметров живого существа ограниченное количество, они не синхронизируются
    // и их нельзя сделать NetworkBehaviour, поэтому каждый эффект применяется по коду параметра
    public LifecycleParameterEnum targetParameter;

    public double StartTime { get; set; }

    public override bool Equals(object obj)
    {
        return obj is LifecycleEffect effect && Equals(effect);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(isInfinite, speed, duration, targetParameter, StartTime);
    }

    public bool Equals(LifecycleEffect other) {
        return isInfinite == other.isInfinite
            && duration == other.duration
            && speed == other.speed
            && targetParameter == other.targetParameter
            && StartTime == other.StartTime;
    }

    public override string ToString()
    {
        return $"isInfinite: {isInfinite}; speed: {speed}; duration: {duration};"
            + $" targetParameter: {targetParameter}; StartTime: {StartTime}";
    }
}
