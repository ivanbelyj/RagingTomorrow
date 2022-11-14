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
public class ItemData : IEquatable<ItemData>
{
    public string itemStaticDataName;
    public ItemDynamicData dynamicData;

    public bool Equals(ItemData other)
    {
        return itemStaticDataName.Equals(other.itemStaticDataName) &&
            (this.dynamicData is null ? true : this.dynamicData.Equals(other.dynamicData));
    }
}
