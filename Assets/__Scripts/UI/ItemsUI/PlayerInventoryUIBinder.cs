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

    private bool _isInventoryOpened;

    private void Awake() {
        _player = GetComponent<Player>();
        _inventory = GetComponent<Inventory>();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        _invUI = GameObject.FindGameObjectWithTag("PlayersInventory").GetComponent<InventoryUI>();
        _invUI.SetPlayersInventory(_player, _inventory);

        CloseInventory();
    }

    public void ToggleInventory() {
        if (_isInventoryOpened) {
            CloseInventory();
        } else {
            OpenInventory();
        }
        _isInventoryOpened = !_isInventoryOpened;
    }

    public void OpenInventory() {
        _invUI.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        _player.overview.BanLooking();
    }

    public void CloseInventory() {
        _invUI.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        _player.overview.AllowLooking();
    }
}
