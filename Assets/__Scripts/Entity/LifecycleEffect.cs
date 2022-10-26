using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// Объект, хранящий сведения об эффекте, применяемом к параметрам жизненного цикла существа
///</summary>
[System.Serializable]
public class LifecycleEffect
{
    public bool isInfinite;
    public bool recoverToInitial;
    public float speed;
    public float duration;
    
    // Параметров живого существа ограниченное количество, они не синхронизируются,
    // т.к. имеют сложную структуру, кроме того, их нельзя сделать NetworkBehaviour,
    // поэтому каждый эффект применяется по коду параметра
    public EntityParameterEnum targetParameter;

    [HideInInspector]
    public double startTime;
    
    [HideInInspector]
    public ushort effectId;
}
