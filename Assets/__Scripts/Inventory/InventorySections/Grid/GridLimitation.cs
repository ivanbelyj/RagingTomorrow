using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridLimitation : ISectionLimitation
{
    private List<GridItemData> _items;
    public List<GridItemData> Items => _items;
    
    private FillingMatrix _sectionFilling;

    [SerializeField]
    private int _initialWidth;
    [SerializeField]
    private int _initialHeight;
    
    public int Width => _sectionFilling.Width;
    public int Height => _sectionFilling.Height;
    private ItemStaticDataManager _itemStaticDataManager;

    public void Initialize(ItemStaticDataManager itemStaticDataManager) {
        _itemStaticDataManager = itemStaticDataManager;
        Debug.Log($"Initialize GridLimitation with size {_initialWidth}x{_initialHeight}");
        _sectionFilling = new FillingMatrix(_initialHeight, _initialWidth);
        _items = new List<GridItemData>();
    }

    public void AcceptToSection(InventoryItem itemStack) {
        FindFreePos(itemStack, out int x, out int y);
        GridItemData gridItemData = new GridItemData() {
            InventoryX = x,
            InventoryY = y,
            InventoryItem = itemStack
        };
        SetFillingRect(gridItemData, true);
        _items.Add(gridItemData);
    }

    public bool IsAllowedToAdd(InventoryItem itemGameData)
    {
        Debug.Log($"Поиск места в сетке " + _sectionFilling.Width + "x" + _sectionFilling.Height);
        return FindFreePos(itemGameData, out int x, out int y);
    }

    public void RemoveFromSection(Guid itemId)
    {
        GridItemData data = _items.Find(item => item.InventoryItem.id == itemId);
        SetFillingRect(data, false);
    }

    private void SetFillingRect(GridItemData gridItemData, bool value) {
        ItemStaticData staticData = _itemStaticDataManager.GetItemDataByName(
            gridItemData.InventoryItem.itemGameData.itemStaticDataName);
        
        _sectionFilling.SetRect(staticData.Width, staticData.Height,
            gridItemData.InventoryX, gridItemData.InventoryY, value);
    }

    private bool FindFreePos(InventoryItem itemStack, out int x, out int y) {
        ItemStaticData staticData = _itemStaticDataManager
            .GetItemDataByName(itemStack.itemGameData.itemStaticDataName);
        
        bool hasFreePlace = _sectionFilling.FindFreeRectPos(
            staticData.Width, staticData.Height,
            out int resX, out int resY);
        x = resX;
        y = resY;

        return hasFreePlace;
    }
}
