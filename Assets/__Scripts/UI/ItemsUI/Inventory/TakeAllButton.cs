using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TakeAllButton : MonoBehaviour
{
    private Button _button;
    private IItemsProvider _itemsProvider;
    private CharactersInventory _inventory;
    private void Awake() {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => {
            // var items = _itemsProvider.TakeAllItems();
            // List<ItemData> notAdded = new List<ItemData>();
            // foreach (ItemData item in items) {
            //     bool isAdded = _inventory.MainSection.TryToAddToSection(item);
            //     if (!isAdded) {
            //         notAdded.Add(item);
            //     }
            // }

            // foreach (ItemData notAddedItem in notAdded) {
            //     _itemsProvider.TakeBack(notAddedItem);
            // }

            IInventoryItem invItem;
            do {
                invItem = _itemsProvider.PeekNext();
                if (invItem != null && _inventory.MainSection.CanAddToSection(invItem.ItemData,
                    invItem.Count)) {
                    _inventory.MainSection.TryToAddToSection(invItem.ItemData, invItem.Count);
                    _itemsProvider.RemoveLastPeekedItem();
                }
            } while (invItem != null);
        });
    }

    /// <summary>
    /// Устанавливает инвентарь персонажа, который будет получать предметы по нажатию кнопки
    /// </summary>
    public void SetRecipient(CharactersInventory inventory) {
        _inventory = inventory;
    }

    public void SetItemsProvider(IItemsProvider itemsProvider) {
        _itemsProvider = itemsProvider;
    }
}
