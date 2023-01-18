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
    [SerializeField]
    private uint _localId;
    /// <summary>
    /// Однозначно определяет предмет в инвентаре какого-либо типа, но не содержит данных о
    /// экземпляре инвентаря
    /// </summary>
    public uint LocalId { get => _localId; set => _localId = value; }

    [SerializeField]
    private uint _inventorySectionNetId;
    /// <summary>
    /// Однозначно определяет секцию инвентаря, к которой принадлежит предмет, с помощью netId
    /// сетевого компонента Mirror
    /// </summary>
    public uint InventorySectionNetId { get => _inventorySectionNetId; set => _inventorySectionNetId = value; }

    public override bool Equals(object obj)
    {
        return obj is ItemPlacementId id && id.LocalId == LocalId
            && id.InventorySectionNetId == InventorySectionNetId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(LocalId, InventorySectionNetId);
    }

    public override string ToString()
    {
        return $"InventoryNetId: {InventorySectionNetId}; LocalId: {LocalId}";
    }
}
