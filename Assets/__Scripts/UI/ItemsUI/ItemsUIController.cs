using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// Управляет графическим интерфейсом предметов. Сюда относятся инвентарь, крафт, торговля.
/// </summary>
public class ItemsUIController : ItemsUIControllerCore
{
    private PlayerOverview _playerCamera;

    public void SetPlayer(GameObject playerGO, Player player, PlayerOverview playerOverview)
    {
        _playerCamera = playerOverview;
        SetPlayer(playerGO, player.Inventory, player.GetInventoryInfoProvider());
    }

    public override void OpenUI()
    {
        base.OpenUI();
        _playerCamera.Overview.BanLooking();
        _playerCamera.Overview.BanAiming();
    }

    public override void CloseUI()
    {
        base.CloseUI();
        _playerCamera.Overview.AllowLooking();
        _playerCamera.Overview.AllowAiming();
    }
}
