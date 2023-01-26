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
    InventoryInfo IInventoryInfoProvider.InventoryInfo => ToInventoryInfo(_characterData);

    private InventoryInfo ToInventoryInfo(CharacterData newData) {
        return new InventoryInfo(null, newData.Name, newData.Subtitle);
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

    private void Awake() {
        CharacterDataChanged += (CharacterData newData) => {
            _inventoryInfoChanged?.Invoke(ToInventoryInfo(newData));
        };
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        // _characterData = _syncCharacterData;
        if (_syncCharacterData != null)
            OnCharacterDataChanged(null, _syncCharacterData);
    }

    /// <summary>
    /// Устанавливает данные, используя объект _initialCharacterData, поля которого определены в инспекторе.
    /// Следует вызывать метод перед тем, как данные CharacterDataProvider будут использоваться
    /// другими компонентами
    /// </summary>
    public void SetInitialData() {
        SetData(_initialCharacterData);
        // SetRandomName();
    }

    // Для теста
    public void SetRandomName() {
        string newPlayerName = $"Player {Random.Range(100, 1000)}";
        CharacterData newData = new CharacterData() {
            Name = newPlayerName,
            AppearanceData = _characterData?.AppearanceData,
            Subtitle = _characterData?.Subtitle
        };
        SetData(newData);
    }

    public void SetData(CharacterData newData) {
        Debug.Log("CharacterDataProvider. SetData. " + newData);
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
        Debug.Log("CharacterDataProvider. SetCharacterData. " + newData);
        _syncCharacterData = newData;
    }

    [Command]  // Метод выполняется на сервере по запросу клиента
    private void CmdSetCharacterData(CharacterData newData) {
        Debug.Log("CharacterDataProvider. CmdSetCharacterData. " + newData);
        SetCharacterData(newData);
    }
    #endregion
}
