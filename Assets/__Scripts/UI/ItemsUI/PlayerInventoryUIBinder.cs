using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// Привязывает графический интерфейс инвентаря к игроку
/// </summary>
[RequireComponent(typeof(Player))]
[RequireComponent(typeof(Inventory))]
public class PlayerInventoryUIBinder : NetworkBehaviour
{
    private Player _player;
    private Inventory _inventory;

    private InventoryUI _invUI;

    private void Awake() {
        _player = GetComponent<Player>();
        _inventory = GetComponent<Inventory>();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        _invUI = GameObject.FindGameObjectWithTag("PlayersInventory").GetComponent<InventoryUI>();
        _invUI.SetInventory(_player, _inventory);
    }
}
