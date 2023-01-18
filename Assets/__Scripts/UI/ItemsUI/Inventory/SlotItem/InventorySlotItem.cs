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

    private IInventoryItem _invItem;
    public IInventoryItem InventoryItem => _invItem;

    public void Initialize(IInventoryItem invItem, float slotSize, float gridSpacing)
    {
        _invItem = invItem;
        base.Initialize(invItem.ItemData, slotSize, gridSpacing);
        
        SetItemUIAndCount(invItem.ItemData, invItem.Count);
    }

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
