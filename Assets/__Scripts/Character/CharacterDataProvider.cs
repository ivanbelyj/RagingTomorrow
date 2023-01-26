using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// Информация о личности персонажа
/// </summary>
public class CharacterDataProvider : NetworkBehaviour, IInventoryInfoProvider
{
    public delegate void CharacterDataChangedEventHandler(CharacterData newData);

    [SerializeField]
    private CharacterData _initialCharacterData;

    [SyncVar(hook = nameof(OnCharacterDataChanged))]
    private CharacterData _syncCharacterData;
    private CharacterData _characterData;
    public CharacterData CharacterData => _characterData;

    public event CharacterDataChangedEventHandler CharacterDataChanged;

    #region InventoryInfoProvider
    private IInventoryInfoProvider.InventoryInfoChangedEventHandler _inventoryInfoChanged;

    /// <summary>
    /// Информация о персонаже может отображаться в инвентаре
    /// </summary>
    InventoryInfo IInventoryInfoProvider.InventoryInfo {
        get => ToInventoryInfo();
    }

    private InventoryInfo ToInventoryInfo() {
        return new InventoryInfo(null, _characterData.Name, _characterData.Subtitle);
    }

    /// <summary>
    /// Событие вызывается, если какие-либо данные о персонаже, касающиеся представления информации
    /// о нем в инвентаре, изменились
    /// </summary>
    event IInventoryInfoProvider.InventoryInfoChangedEventHandler
        IInventoryInfoProvider.InventoryInfoChanged {
            add => _inventoryInfoChanged += value;
            remove => _inventoryInfoChanged -= value;
        }
    #endregion

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        CharacterDataChanged += (CharacterData newData) => {
            _inventoryInfoChanged?.Invoke(ToInventoryInfo());
        };

        SetData(_initialCharacterData);
        SetRandomName();
    }

    // Для теста
    public void SetRandomName() {
        string newPlayerName = $"Player {Random.Range(100, 999)}";
        CharacterData newData = new CharacterData() {
            Name = newPlayerName,
            AppearanceData = _characterData.AppearanceData,
            Subtitle = _characterData.Subtitle
        };
        SetData(newData);
    }

    public void SetData(CharacterData newData) {
        if (isServer) {
            SetCharacterData(newData);
        } else {
            CmdSetCharacterData(newData);
        }
    }

    #region Sync
    private void OnCharacterDataChanged(CharacterData oldCharacterData, CharacterData newCharacterData) {
        _characterData = newCharacterData;
        CharacterDataChanged?.Invoke(newCharacterData);
    }

    [Server]  // Будет вызываться и выполняться только на сервере
    private void SetCharacterData(CharacterData newData) {
        _syncCharacterData = newData;
    }

    [Command]  // Метод выполняется на сервере по запросу клиента
    private void CmdSetCharacterData(CharacterData newData) {
        SetCharacterData(newData);
    }
    #endregion
}
