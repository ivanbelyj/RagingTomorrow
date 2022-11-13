using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// Управляет графическим интерфейсом предметов. Сюда относятся инвентарь, крафт, торговля.
/// </summary>
public class ItemsUIController : MonoBehaviour
{
    [Flags]
    private enum ItemsUI
    {
        PlayersInventory,
        OtherInventory,
        PlayerLootsInventory = PlayersInventory | OtherInventory

        // Todo:
        // Персонаж игрока,
        // Компоненты торговли
    }

    [SerializeField]
    private InventoryUI _playersInventoryUI;

    [SerializeField]
    private InventoryUI _otherInventoryUI;

    [SerializeField]
    private GameObject _parentUI;

    private Player _player;

    private bool _isUIOpened;

    private ItemsUI _openedComponents;

    /// <summary>
    /// Устанавливает игрока, информация которого будет отображаться
    /// </summary>
    public void SetPlayer(Player player) {
        _player = player;
        _playersInventoryUI = GameObject.FindGameObjectWithTag("PlayersInventory")
            .GetComponent<InventoryUI>();
        
        _playersInventoryUI.Set(_player.GetInventoryInfoProvider(),
            _player.GetMainInventorySection());
    }

    private void SetOtherInventory(IInventoryInfoProvider inventoryInfo, GridSection inventory) {
        _otherInventoryUI.Set(inventoryInfo, inventory);
    }

    public void ToggleUI() {
        if (_isUIOpened) {
            CloseUI();
        } else {
            OpenUI();
        }
    }

    public void OpenUI() {
        // _playersInventoryUI.gameObject.SetActive(true);
        _parentUI.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        _player.overview.BanLooking();
        _player.overview.BanAiming();
        _isUIOpened = true;
    }

    public void CloseUI() {
        // _playersInventoryUI.gameObject.SetActive(false);
        _parentUI.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        _player.overview.AllowLooking();
        _player.overview.AllowAiming();
        _isUIOpened = false;
    }

    /// <summary>
    /// Делает видимыми компоненты, соответствующие установленным флагам видимости.
    /// Помимо видимости отдельно взятого компонента имеет значение, показан ли весь интерфейс
    /// </summary>
    private void Show(ItemsUI visibleComponents) {
        SetActiveByVisibilityFlag(_playersInventoryUI.gameObject, visibleComponents,
            ItemsUI.PlayersInventory);
        SetActiveByVisibilityFlag(_otherInventoryUI.gameObject, visibleComponents,
            ItemsUI.OtherInventory);
    }

    private void SetActiveByVisibilityFlag(GameObject go,
        ItemsUI visibleComponents,
        ItemsUI bit) {
        bool active = (visibleComponents & bit) == bit;
        // Debug.Log($"Setting active GO {go.name}. {active}");
        go.SetActive(active);
    }

    /// <summary>
    /// Помимо собственного инвентаря игрок может просматривать и сторонний. Предполагается,
    /// что в ходе игры таких временных инвентарей может быть много (ящики, схроны, трупы...),
    /// поэтому открытие нового инвентаря с точки зрения пользователей ItemsUIController
    /// тождественно его установке (в отличие от инвентаря игрока, который постоянен).
    /// </summary>
    public void ShowOtherInventory(IInventoryInfoProvider inventoryInfo,
        GridSection inventory) {
        SetOtherInventory(inventoryInfo, inventory);
        Show(ItemsUI.PlayerLootsInventory);
    }

    public void ShowPlayersInventory() {
        Show(ItemsUI.PlayersInventory);
    }
}
