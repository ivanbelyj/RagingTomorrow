using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// Живое существо (компонент Enity) обладает ограниченным количеством параметров жизненного цикла.
/// Это необходимо как для избавление от необходимости синхронизации объектов LifecycleParameter
/// (сложных по структуре для синхронизации), так и для обеспечения способа
/// указания целевого параметра вне кода (например, в json описании предметов: аптечек, лекарств...)
///</summary>
public enum EntityParameterEnum : byte
{
    Health = 0,
    Endurance = 1,
    Satiety = 2,
    Bleeding = 3,
    Radiation = 4
}
