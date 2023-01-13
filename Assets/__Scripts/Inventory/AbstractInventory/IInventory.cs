using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Компоненты, представляющие не только лишь секцию, а полноценный инвентарь, могут
/// реализовывать данный интерфейс
/// </summary>
public interface IInventory
{
    bool TryToAdd(IInventoryItem item);
    void Remove(IInventoryItem item);
    float TotalWeight { get; }
}
