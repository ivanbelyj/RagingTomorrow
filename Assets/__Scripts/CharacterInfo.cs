using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// Информация о личности персонажа
/// </summary>
public class CharacterInfo : NetworkBehaviour, IInventoryInfoProvider
{
    public class CharacterInfoData {
        public string Name { get; set; }
    }

    public delegate void CharacterInfoChangedEventHandler(CharacterInfoData newInfo);

    [SyncVar(hook = nameof(OnCharactersNameChanged))]
    private string _syncCharactersName;
    private string _nameCharacters;
    public string charactersName => _nameCharacters;

    /// <summary>
    /// Событие вызывается, если какие-либо данные о персонаже изменились
    /// </summary>
    public event CharacterInfoChangedEventHandler CharacterInfoChanged;

    #region InventoryInfoProvider
    private InventoryInfo _inventoryInfo;

    /// <summary>
    /// Информация о персонаже может быть использована в инвентаре
    /// </summary>
    InventoryInfo IInventoryInfoProvider.InventoryInfo {
        get => _inventoryInfo;
    }

    private IInventoryInfoProvider.InfoChangedEventHandler _inventoryInfoChanged;

    /// <summary>
    /// Событие вызывается, если какие-либо данные о персонаже, касающиеся представления информации
    /// о нем в инвентаре, изменились
    /// </summary>
    event IInventoryInfoProvider.InfoChangedEventHandler
        IInventoryInfoProvider.InventoryInfoChanged {
            add => _inventoryInfoChanged += value;
            remove => _inventoryInfoChanged -= value;
        }
    #endregion

    private void Awake() {
        // _lastInfo = new InventoryInfo(null, charactersName, "Что-то про игрока");
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        _inventoryInfo = new InventoryInfo(null, charactersName, "Что-то о персонаже");

        CharacterInfoChanged += (CharacterInfoData newInfo) => {
            _inventoryInfo = new InventoryInfo(_inventoryInfo.Avatar, newInfo.Name,
                _inventoryInfo.SubTitle);
            _inventoryInfoChanged?.Invoke(_inventoryInfo);
        };

        string newPlayerName = $"Player {Random.Range(100, 999)}";
        if (isServer) {
            ChangePlayerName(newPlayerName);
        } else {
            CmdChangePlayerName(newPlayerName);
        }
    }

    #region Sync
    private void OnCharactersNameChanged(string oldName, string newName) {
        _nameCharacters = newName;
        // playerNameText.text = newName;

        // var newInfo = new InventoryInfo(_lastInfo.Avatar, newName, _lastInfo.SubTitle);
        // this.InventoryInfoChanged?.Invoke(newInfo);
        CharacterInfoData newData = new CharacterInfoData() {
            Name = newName
        };
        CharacterInfoChanged?.Invoke(newData);
    }

    [Server]  // Будет вызываться и выполняться только на сервере
    private void ChangePlayerName(string newName) {
        _syncCharactersName = newName;
    }

    [Command]  // Метод выполняется на сервере по запросу клиента
    private void CmdChangePlayerName(string name) {
        ChangePlayerName(name);
    }
    #endregion
}
