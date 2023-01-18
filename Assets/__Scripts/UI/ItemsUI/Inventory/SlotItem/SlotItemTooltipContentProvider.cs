using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotItemTooltipContentProvider : MonoBehaviour, ITooltipContentProvider
{
    private InventorySlotItem _invSlotItem;
    private ItemStaticDataManager _itemStaticDataManager;

    private void Awake() {
        _invSlotItem = GetComponent<InventorySlotItem>();
        _itemStaticDataManager = FindObjectOfType<ItemStaticDataManager>();
    }
    
    public TooltipContent GetTooltipContent()
    {
        return _invSlotItem.ItemData.ToTooltipContent(_itemStaticDataManager);
    }

    private TooltipContent GetTestContent() {
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
        return content;
    }
}
