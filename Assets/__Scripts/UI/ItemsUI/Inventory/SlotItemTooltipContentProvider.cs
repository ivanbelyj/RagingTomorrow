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
            .Header("Название")
            .Ln()
            .Text("Состояние")
            .Text("Идеальное", Color.green)
            .Ln()
            .Text("Вес")
            .Text("3 кг")
            .Ln()
            .Text("Test parameter")
            .Text("Сносно", Color.yellow)
            .Build();
        return content;  // _invSlotItem.ItemData.ToTooltipData();
    }
}
