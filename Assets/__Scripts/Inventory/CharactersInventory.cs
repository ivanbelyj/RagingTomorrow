using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Компонент, предоставляющий доступ к секциям инвентаря персонажа
/// </summary>
public class CharactersInventory : MonoBehaviour, IInventory
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

    public WearSection WearSection {
        get => _wearSection;
        private set => _wearSection = value;
    }
    #endregion

    public float TotalWeight => MainSection.GetTotalWeight() + WearSection.GetTotalWeight();

    public bool TryToAdd(ItemData itemData, int count)
    {
        // Todo: Wear section
        return MainSection.TryToAddToSection(itemData, count);
    }

    public void Remove(IInventoryItem item)
    {
        // Todo
    }
}
