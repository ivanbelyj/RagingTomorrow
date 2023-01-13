using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Идентификатор размещения предмета. Иногда требуется проверять не только
/// идентичность предметов с точки зрения характеристик, но и с точки зрения инвентаря (предметы могут
/// быть одинаковыми по характеристикам, но находиться в различных инвентарях или позициях).
/// Данный идентификатор не зависит от конкретного типа инвентаря: в WearSection он определяется
/// на основе слота, в GridSection - на основе позиции в сетке
/// </summary>
[Serializable]
public struct ItemPlacementId
{
    /// <summary>
    /// Однозначно определяет предмет в инвентаре какого-либо типа, но не содержит данных о
    /// экземпляре инвентаря
    /// </summary>
    public uint LocalId { get; set; }

    /// <summary>
    /// Однозначно определяет секцию инвентаря, к которой принадлежит предмет, с помощью netId
    /// сетевого компонента Mirror
    /// </summary>
    public uint InventoryNetId { get; set; }

    public ItemPlacementId(uint localId, uint inventoryNetId) {
        LocalId = localId;
        InventoryNetId = inventoryNetId;
    }

    public override bool Equals(object obj)
    {
        return obj is ItemPlacementId id && id.LocalId == LocalId
            && id.InventoryNetId == InventoryNetId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(LocalId, InventoryNetId);
    }

    public override string ToString()
    {
        return $"InventoryNetId: {InventoryNetId}; LocalId: {LocalId}";
    }
}
