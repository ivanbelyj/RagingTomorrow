using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(InventorySlotItem))]
public class DraggableItem : Draggable<DraggedItemData>
{
    private InventorySlotItem _slotItem;

    override protected void Awake() {
        base.Awake();
        _slotItem = GetComponent<InventorySlotItem>();
    }

    protected override void Start()
    {
        base.Start();
        uint sectionNetId = _slotItem.SectionNetId;
        uint playerNetId = GetLocalPlayersNetId();
        // К методу Start local id уже должен быть инициализирован
        Debug.Log($"Initialize draggable item: local id {_slotItem.ItemLocalId}; "
            + $"sectionNetId: {sectionNetId}; playerNetId: {playerNetId}");
        DraggedItemData data = new DraggedItemData() {
            ItemLocalId = _slotItem.ItemLocalId,
            InventorySectionNetId = sectionNetId,
            DraggingPlayerNetId = playerNetId,
        };
        this.Initialize(data);
    }

    private uint GetLocalPlayersNetId() {
        return 0;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        // Debug.Log($"Dragged item with " + DraggedData.ItemLocalId);
    }
}
