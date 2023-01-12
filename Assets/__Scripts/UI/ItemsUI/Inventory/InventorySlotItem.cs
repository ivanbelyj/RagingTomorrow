using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI предмета (а точнее стака предметов) инвентаря
/// </summary>
public class InventorySlotItem : ItemIcon
{
    // GameObject, содержащий TextMeshPro для отображения количества предметов
    [SerializeField]
    private GameObject _itemsCountGO;

    [SerializeField]
    private TextMeshProUGUI _itemsCountText;

    private uint _itemLocalId;
    public uint ItemLocalId => _itemLocalId;

    private uint _sectionNetId;
    public uint SectionNetId => _sectionNetId;

    /// <summary>
    /// ItemStaticDataManager требуется для получения данных о предмете по имени,
    /// т.к. при установке предмета в слот передаются компактные данные
    /// </summary>
    public void Initialize(ItemData itemData,
        float slotSize, float gridSpacing,
        int count, uint itemLocalId, uint sectionNetId) {
        base.Initialize(itemData, slotSize, gridSpacing);

        _itemLocalId = itemLocalId;
        _sectionNetId = sectionNetId;
        SetItemUIAndCount(itemData, count);
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
