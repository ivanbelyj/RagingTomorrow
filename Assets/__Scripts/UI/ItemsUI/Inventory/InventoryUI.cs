using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Компонент, предоставляющий доступ для управления инвентарем
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private InventoryInfo _invInfo;

    [SerializeField]
    private InventoryGrid _invGrid;

    [SerializeField]
    private InventoryWeightBar _weightBar;

    public void SetInventory(Player player, Inventory inventory) {
        _invInfo.Title = player.PlayerName;
        _invGrid.SetInventory(inventory);
    }
}
