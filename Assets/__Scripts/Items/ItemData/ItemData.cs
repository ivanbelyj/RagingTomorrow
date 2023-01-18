using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// В инвентаре синхронизируются не просто имена, а объекты, содержащие как имя (позволяет
/// получить неизменные характеристики, которые неэффективно постоянно передавать по сети),
/// так и динамическую информацию
/// </summary>
[Serializable]
public class ItemData
{
    // : устанавливать ItemStaticData при сериализации / десериализации
    [SerializeField]
    private string _itemStaticDataName;
    public string ItemStaticDataName {
        get => _itemStaticDataName;
        set => _itemStaticDataName = value;
    }
    // public ItemDynamicData dynamicData;

    public override bool Equals(object obj)
    {
        return obj is ItemData itemData ? itemData.ItemStaticDataName == this.ItemStaticDataName : false;
    }

    public override int GetHashCode()
    {
        return ItemStaticDataName.GetHashCode();
    }

    /// <summary>
    /// Тип предмета определяет структуру контента соответствующей всплывающей подсказки.
    /// ItemStaticDataManager требуется для получения неизменяемой в ходе игры информации,
    /// которую не имеет смысла хранить в ItemData, сохранять в долговременной памяти
    /// и синхронизировать по сети
    /// </summary>
    public virtual TooltipContent ToTooltipContent(ItemStaticDataManager itemStaticDataManager) {
        ItemStaticData staticData = itemStaticDataManager.GetStaticDataByName(ItemStaticDataName);
        return new TooltipContentBuilder()
            .Header(staticData.ItemName)
            .Ln()
            .Text(staticData.Description)
            .Ln()
            .Text("Масса").Text(staticData.Mass + " кг")
            .Ln()
            .Build();
    }

    public override string ToString()
    {
        return $"{this.GetType().Name}; name: {this._itemStaticDataName}";
    }
}
