using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Способ использования предмета. Например, компонент стрельбы может реализовать данный интерфейс,
/// после компонент будет помещен в WaysOfUse
/// </summary>

public interface IItemUsage
{
    //  непосредственно действие
    void UseItem();

    // используется, например, в интерфейсе инвентаря
    string UsageName { get; }
}
