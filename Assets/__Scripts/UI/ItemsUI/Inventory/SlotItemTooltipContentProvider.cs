using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotItemTooltipContentProvider : MonoBehaviour, ITooltipContentProvider
{
    private InventorySlotItem _invSlotItem;

    private void Awake() {
        _invSlotItem = GetComponent<InventorySlotItem>();
    }
    
    public TooltipContent GetTooltipContent()
    {
        TooltipContent content = new TooltipContentBuilder()
            .Ln()
            .Text("Название:")
            .Text("Test")
            .Ln()
            .Text("Состояние:")
            .Text("Идеальное")
            .Build();
        return content;  // _invSlotItem.ItemData.ToTooltipData();
    }
}
