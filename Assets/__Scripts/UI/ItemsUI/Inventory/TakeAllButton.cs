using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TakeAllButton : MonoBehaviour
{
    private Button _button;
    private IInventory _supplier;
    private IInventory _recipient;
    
    private void Awake() {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => {
            IInventoryItem invItem;

            Debug.Log("Взятие всех предметов инвентаря");
            for (int i = 0; true; i++) {
                // Todo: проход по всем предметам и взятие в инвентарь
                invItem = null;
                if (invItem == null) {
                    break;
                }
                
                Debug.Log("\tПредмет: " + invItem);
                if (!_recipient.CanAdd(invItem.ItemData, invItem.Count))
                    continue;
                
                bool isNotAdded = !_recipient.TryToAdd(invItem.ItemData, invItem.Count);
                Debug.Log("isNotAdded: " + isNotAdded);
                if (isNotAdded) {
                    Debug.LogError("CanAdd вернул true, но TryToAdd завершился неудачей");
                    Debug.Break();
                }

                bool isRemoved = _supplier.Remove(invItem.PlacementId);
                Debug.Log("isRemoved: " + isRemoved);
                if (!isRemoved) {
                    Debug.LogError("Предмет не был удален");
                    Debug.Break();
                    break;
                }
            };
        });
    }

    /// <summary>
    /// Устанавливает инвентарь, который будет получать предметы по нажатию кнопки
    /// </summary>
    public void SetRecipient(IInventory inventory) {
        _recipient = inventory;
    }

    public void SetSupplier(IInventory supplier) {
        _supplier = supplier;
    }
}
