using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Компонент, предоставляющий доступ к секциям инвентаря персонажа
/// </summary>
public class CharactersInventory : MonoBehaviour, IGridSectionInventory
{
    #region Inventory sections
    [Header("Set in inspector")]
    [SerializeField]
    private GridSection _mainSection;
    
    [SerializeField]
    private WearSection _wearSection;

    public GridSection MainSection {
        get => _mainSection;
        private set => _mainSection = value;
    }

    GridSection IGridSectionInventory.GridSection => MainSection;

    public WearSection WearSection {
        get => _wearSection;
        private set => _wearSection = value;
    }
    #endregion

    public bool CanAdd(ItemData itemData, int count)
    {
        return MainSection.CanAddToSection(itemData, count);
    }

    public bool TryToAdd(ItemData itemData, int count)
    {
        // Todo: Wear section
        return MainSection.TryToAddToSection(itemData, count);
    }

    public float TotalWeight => MainSection.TotalWeight + WearSection.TotalWeight;

    public bool Remove(ItemPlacementId placementId)
    {
        if (placementId.InventorySectionNetId == _mainSection.netId) {
            return _mainSection.RemoveFromSection(placementId.LocalId);
        } else if (placementId.InventorySectionNetId == _wearSection.netId) {
            // Todo: wear section
            return false;
        } else {
            Debug.Log("Предмет нельзя удалить из инвентаря персонажа, т.к. он находится в секции, " +
                "не относящейся к нему");
            return false;
        }
    }
}
