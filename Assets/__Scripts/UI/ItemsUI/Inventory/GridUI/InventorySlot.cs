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

    private GridSection GetGridSectionByNetId(uint netId) {
        return NetworkClient.spawned[netId].GetComponent<GridSection>();
    }

    public void AcceptDrop(Draggable<DraggedItemData> draggable, DraggedItemData draggedData)
    {
        Debug.Log($"Accepted: {draggedData.DraggingPlayerNetId} " +
            $" {draggedData.InventorySectionNetId} {draggedData.ItemLocalId}");
        
        GridSection fromSection =
            GetGridSectionByNetId(draggable.DraggedData.InventorySectionNetId);
        Debug.Log($"Found section by id. Size: {fromSection.Width}x{fromSection.Height}");
        GridSectionItem gridItem = fromSection
            .Items.Find(x => x.GetLocalIdByInventoryPosition() == draggedData.ItemLocalId);
        Debug.Log($"Found item by local id. {gridItem.itemData.itemStaticDataName}. "
            + "It could be changed during drag and drop");

        bool isAdded = _gridSection.TryToAddToSection(gridItem.itemData);
        if (isAdded) {
            bool isRemoved = fromSection.RemoveFromSection(gridItem);
            if (isRemoved) {
                Destroy(draggable);
            } else {
                Debug.LogError("Item was added, but not removed from old section");
            }
        } else {
            // Todo: вернуть предмет на изначальное место
            Debug.Log("Item was accepted, but not added.");
        }
    }
}
