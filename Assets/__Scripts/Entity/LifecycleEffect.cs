using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Обеспечивает EntityLifecycle всей необходимой информацией для применения эффекта к
// существу. DTO, существует для удобства синхронизации параметров жизненного цикла
[System.Serializable]
public class LifecycleEffect
{
    [Header("Set in inspector")]
    public bool isInfinite;
    public bool recoverToInitial;
    public float speed;
    public float duration;
    public double startTime;

    // Активен ли эффект в данный момент.
    // Эффект может быть неактивным, но незакончившимся (например, здоровье дошло до максимума,
    // эффект не применяется, но если будет урон - он сразу станет активным).
    // public bool isActive;

    // Параметров живого существа ограниченное количество, они не синхронизируются,
    // т.к. имеют сложную структуру, кроме того, их нельзя сделать NetworkBehaviour,
    // поэтому каждый эффект применяется по коду параметра
    public byte targetParameterIndex;

    // public bool IsPassed => UpdateIterationsCurrent < UpdateIterations;

    // private int? _updateIterations;
    // public int UpdateIterations {
    //     get {
    //         if (_updateIterations is null)
    //             _updateIterations = (int)(speed / Time.fixedDeltaTime * duration);
    //         return _updateIterations.Value;
    //     }
    //     private set {
    //         _updateIterations = value;
    //     }
    // }

    // public int UpdateIterationsCurrent { get; set;}

    public LifecycleEffect(float speed, float duration)
    {
        this.speed = speed;
        this.duration = duration;
    }

    // Если поля установлены в инспекторе
    public LifecycleEffect()
    {
        
    }

    // public LifecycleEffect(int updateIterations)
    // {
    //     UpdateIterations = updateIterations;
    // }
}
