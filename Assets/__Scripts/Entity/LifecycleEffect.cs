using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// Структура, хранящая сведения об эффекте, применяемом к параметрам жизненного цикла существа
///</summary>
[System.Serializable]
public struct LifecycleEffect
{
    public bool isInfinite;
    public bool recoverToInitial;
    public float speed;
    public float duration;
    public double startTime;

    // Параметров живого существа ограниченное количество, они не синхронизируются,
    // т.к. имеют сложную структуру, кроме того, их нельзя сделать NetworkBehaviour,
    // поэтому каждый эффект применяется по коду параметра
    public EntityParameterEnum targetParameterIndex;
}
