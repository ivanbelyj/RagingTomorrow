using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItemIcon : MonoBehaviour
{
    [SerializeField]
    private int _slotSize;

    [SerializeField]
    private ItemIcon _itemIcon;

    public void ShowIcon(ItemData item) {
        _itemIcon.Initialize(item, _slotSize, 0);
        _itemIcon.gameObject.SetActive(true);
    }
    public void HideIcon() {
        _itemIcon.gameObject.SetActive(false);
    }
}
