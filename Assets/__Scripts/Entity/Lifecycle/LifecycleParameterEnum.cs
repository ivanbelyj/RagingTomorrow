using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// Представляет возможность обращения к параметрам жизненного цикла существа (их набор ограничен)
///</summary>
public enum LifecycleParameterEnum : byte
{
    Health,
    Endurance,
    Satiety,
    Bleeding,
    Radiation
}
