using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LifecycleEffectItemData : ItemData
{
    [SerializeField]
    private int _uses = 4;
    public int Uses { get => _uses; set => _uses = value; }
    public override TooltipContent ToTooltipContent(ItemStaticDataManager itemStaticDataManager)
    {
        TooltipContent baseTooltipContent = base.ToTooltipContent(itemStaticDataManager);
        TooltipContent content = new TooltipContentBuilder()
            .Ln()
            .Text("Доступно для использования").Text(_uses.ToString())
            .Build();
        return TooltipContent.Merge(baseTooltipContent, content);
    }

    public override bool Equals(object obj)
    {
        return obj is LifecycleEffectItemData itemData ?
            base.Equals(itemData) && itemData.Uses == _uses : false;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(this.ItemStaticDataName, this._uses);
    }

    public override string ToString()
    {
        return base.ToString() + $"; Uses: {Uses}";
    }
}
