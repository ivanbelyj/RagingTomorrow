using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Стратегия взаимодействия с интерактивными объектами-хранилищами
/// </summary>
public class InventoryInteractorStrategy : MonoBehaviour, IInteractorStrategy
{
    private ItemsUIController _itemsUIController;

    private void Awake() {
        // Todo: не поможет ли Dependency Injection?
        _itemsUIController = FindObjectOfType<ItemsUIController>();
    }

    public bool CanInteract(Collider col)
    {
        return col.GetComponent<InteractableInventory>() is not null;
    }

    public void InteractObject(Collider col)
    {
        InteractableInventory interactInv = col.GetComponent<InteractableInventory>();
        _itemsUIController.ShowOtherInventory(interactInv, interactInv);

        if (!_itemsUIController.IsUIOpened) {
            _itemsUIController.OpenUI();
        }
    }

    public void LookedAtObject(Collider col)
    {
        
    }

    public void LookedAwayFromObject()
    {
        
    }
}
