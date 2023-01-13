using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// UI предмета (а точнее стака предметов) инвентаря
/// </summary>
public class InventorySlotItem : ItemIcon, IInventoryItemProvider
{
    // GameObject, содержащий TextMeshPro для отображения количества предметов
    [SerializeField]
    private GameObject _itemsCountGO;

    [SerializeField]
    private TextMeshProUGUI _itemsCountText;

    // private uint _itemLocalId;
    // public uint ItemLocalId => _itemLocalId;

    // private uint _sectionNetId;
    // public uint SectionNetId => _sectionNetId;
    private IInventoryItem _invItem;
    public IInventoryItem InventoryItem => _invItem;

    /// <summary>
    /// ItemStaticDataManager требуется для получения данных о предмете по имени,
    /// т.к. при установке предмета в слот передаются компактные данные
    /// </summary>
    public void Initialize(IInventoryItem invItem, float slotSize,
        float gridSpacing)
    {
        _invItem = invItem;
        base.Initialize(invItem.ItemData, slotSize, gridSpacing);
        
        SetItemUIAndCount(invItem.ItemData, invItem.Count);
    }

    // public void Initialize(GridSectionItem gridItem, uint itemLocalId,
    //     ItemStaticDataManager itemStaticDataManager,
    //     float slotWidth, float slotHeight, float gridSpacing) {
    //     Initialize(gridItem.itemData, gridItem.Count, , itemLocalId, itemStaticDataManager,
    //         slotWidth, slotHeight, gridSpacing);
    // }

    private void SetItemsCountUI(int itemsCount) {
        if (itemsCount != 1) {
            _itemsCountGO.SetActive(true);
            _itemsCountText.text = itemsCount.ToString();
        } else {
            _itemsCountGO.SetActive(false);
        }
    }
    
    /// <summary>
    /// Устанавливает предмет для UI
    /// </summary>
    private void SetItemUIAndCount(ItemData itemData, int count) {
        SetItemUI(itemData);
        SetItemsCountUI(count);
    }
}
