using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Слот в сетке инвентаря
/// </summary>
public class InventorySlot : MonoBehaviour, IDropAcceptor<DraggedItemData>
{
    private Image _image;
    private GridSection _gridSection;

    private int _row;
    private int _col;
    
    private void Awake() {
        _image = GetComponent<Image>();
    }

    public void Initialize(GridSection gridSection, int row, int col) {
        _gridSection = gridSection;
        _row = row;
        _col = col;
    }

    public void AcceptDrop(Draggable<DraggedItemData> draggable, DraggedItemData draggedData)
    {
        Debug.Log($"Accepted: {draggedData.DraggingPlayerNetId} " +
            $" {draggedData.PlacementId}");
        
        // Удаление элемента из его предыдущей секции инвентаря
        uint netId = draggable.DraggedData.PlacementId.InventorySectionNetId;
        GridSection oldSection = NetworkClient.spawned[netId].GetComponent<GridSection>();
        Debug.Log($"Found old section by id. Size: {oldSection.Width}x{oldSection.Height}. It's items: ");
        foreach (var item in oldSection.Items) {
            Debug.Log($"\t{item}");
        }
        GridSectionItem oldGridItem = oldSection.Items[draggedData.PlacementId.LocalId];
        Debug.Log($"Old item by local id: {oldGridItem.ItemData}");

        GridSectionItem newItem = new GridSectionItem() {
            InventoryX = _col - draggedData.MouseSlotsOffsetX,
            InventoryY = _row - draggedData.MouseSlotsOffsetY,
            Count = oldGridItem.Count,
            ItemData = oldGridItem.ItemData,
            InventoryNetId = _gridSection.netId
        };

        bool isAdded;
        
        if (_gridSection.netId == oldSection.netId &&
            oldGridItem.PlacementId.LocalId
            == newItem.PlacementId.LocalId) {
            // Добавлять элемент в одно и то же место не имеет смысла,
            // ничего не происходит
            isAdded = false;
        } else {
            isAdded = _gridSection.TryToAddGridSectionItem(newItem,
                oldSection.netId == _gridSection.netId ? oldGridItem : null);
            if (isAdded) {
                bool isRemoved = oldSection.RemoveFromSection(oldGridItem.PlacementId.LocalId);
                if (isRemoved) {
                    Debug.Log("Item is removed from old section");
                    Destroy(draggable);
                }
            }
        }
        Debug.Log("Is item added? " + isAdded);
        if (!isAdded) {
            draggable.ResetDrag();
            Debug.Log("Item was accepted, but not added.");
        }
    }
}
