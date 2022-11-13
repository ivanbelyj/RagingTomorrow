using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Компонент, предоставляющий доступ к секциям инвентаря персонажа
/// </summary>
public class CharactersInventoryController : MonoBehaviour
{
    [Header("Set in inspector")]
    #region Inventory sections fields
    [SerializeField]
    private GridInventorySection _mainSection;

    [SerializeField]
    private GridInventorySection _ingredientsSection;

    [SerializeField]
    private GridInventorySection _craftResultsSection;
    
    [SerializeField]
    private InventorySection _donnedSection;
    #endregion

    #region Sections properties
    public GridInventorySection MainSection {
        get => _mainSection;
        private set => _mainSection = value;
    }
    public GridInventorySection IngredientsSection {
        get => _ingredientsSection;
        private set => _ingredientsSection = value;
    }
    public GridInventorySection CraftResultsSection {
        get => _craftResultsSection;
        private set => _craftResultsSection = value;
    }
    public InventorySection DonnedSection {
        get => _donnedSection;
        private set => _donnedSection = value;
    }
    #endregion

    void Awake() {
    }
}
