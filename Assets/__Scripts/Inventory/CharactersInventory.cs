using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Компонент, предоставляющий доступ к секциям инвентаря персонажа
/// </summary>
public class CharactersInventory : MonoBehaviour, ITotalWeight
{
    [Header("Set in inspector")]
    #region Inventory sections fields
    [SerializeField]
    private GridSection _mainSection;
    
    [SerializeField]
    private WearSection _wearSection;
    #endregion

    #region Sections properties
    public GridSection MainSection {
        get => _mainSection;
        private set => _mainSection = value;
    }

    public WearSection WearSection {
        get => _wearSection;
        private set => _wearSection = value;
    }
    #endregion

    public float GetTotalWeight() => MainSection.GetTotalWeight() + WearSection.GetTotalWeight();
}
