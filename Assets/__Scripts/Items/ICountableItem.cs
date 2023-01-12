using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Перемещать между секциями инвентарей можно не просто отдельные предметы, но предметы
/// в определенном количестве
/// </summary>
public interface ICountableItem
{
    public int Count { get; }
    public ItemData ItemData { get; }
}
