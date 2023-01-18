using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Компоненты, представляющие не только лишь секцию, а полноценный инвентарь, могут
/// реализовывать данный интерфейс
/// </summary>
public interface IInventory
{
    bool CanAdd(ItemData itemData, int count);
    bool TryToAdd(ItemData itemData, int count);
    
    /// <summary>
    /// Удаляет из инвентаря предмет по расположению, заданному id
    /// </summary>
    bool Remove(ItemPlacementId itemId);
    float TotalWeight { get; }
}
