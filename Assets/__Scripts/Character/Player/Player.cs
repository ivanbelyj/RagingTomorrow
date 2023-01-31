using System.Collections;
using System.Collections.Generic;
using AFPC;
using UnityEngine;
using Mirror;
using TMPro;
using AppearanceCustomization3D;

[RequireComponent(typeof(Entity))]
[RequireComponent(typeof(CharactersInventory))]
[RequireComponent(typeof(CharacterDataProvider))]
[RequireComponent(typeof(Interactor))]
[RequireComponent(typeof(ItemInteractorStrategy))]
[RequireComponent(typeof(PlayerOverview))]
public class Player : NetworkBehaviour
{
    // Todo: Только для теста! Удалить после
    public static bool dontEndDrag;

    private PickableItemIcon _pickableItemUI;
    private SimpleHUD _HUD;

    private ItemsUIController _itemsUIController;

    private Entity _entity;

    // Components
    private CharactersInventory _inventory;
    public CharactersInventory Inventory => _inventory;
    private ItemThrower _itemThrower;
    private Interactor _interactor;
    private ItemInteractorStrategy _itemInteractorStrategy;
    private CharacterDataProvider _characterDataProvider;
    [SerializeField]
    private CustomizableAppearance _customizableAppearance;
    private PlayerOverview _playerCamera;

    private void Awake() {
        _entity = GetComponent<Entity>();
        _inventory = GetComponent<CharactersInventory>();
        _itemThrower = GetComponent<ItemThrower>();
        _characterDataProvider = GetComponent<CharacterDataProvider>();
        _playerCamera = GetComponent<PlayerOverview>();

        _interactor = GetComponent<Interactor>();

        _itemInteractorStrategy = GetComponent<ItemInteractorStrategy>();
        _itemInteractorStrategy.LookedAtItem += OnLookedAtItem;
        _itemInteractorStrategy.LookedAwayFromItem += OnLookedAwayFromItem;
    }

    public override void OnStartClient() {
        // lifecycle.Initialize();
        // entity.lifecycle.AssignDamageAction(DamageFX);

        // movement.Initialize();
        // _entity.movement.AssignLandingAction(() => overview.Shake(0.5f));
    }

    public override void OnStartLocalPlayer()
    {
        _playerCamera.InitializeOnLocalPlayer();
        
        Debug.Log($"Initialization of _interactor {_interactor} with transform" +
            $" {_playerCamera.Overview.camera.transform}");
        _interactor.Initialize(_playerCamera.Overview.camera.transform);

        // _interactor.LookingToInteractiveObject += OnLookingToItem;

        // Данные для игрока устанавливаются и синхронизируются лишь раз, когда он появился
        _characterDataProvider.SetInitialData();
    }

    private void OnLookedAtItem(Item item) {
        _pickableItemUI.ShowIcon(item.ItemData);
        // Debug.Log("Looked at item");
    }

    private void OnLookedAwayFromItem() {
        _pickableItemUI.HideIcon();
        // Debug.Log("Looked away from item");
    }

    private void BindUI() {
        _HUD = FindObjectOfType<SimpleHUD>();
        if (_HUD is not null) {
            _HUD.SetEntity(gameObject);
            // Debug.Log("HUD is set");
        }

        _itemsUIController = FindObjectOfType<ItemsUIController>();
        _itemsUIController.SetPlayer(this.gameObject, this, _playerCamera);

        _itemsUIController.CloseUI();

        _pickableItemUI = FindObjectOfType<PickableItemIcon>();
        _pickableItemUI.HideIcon();
    }

    private void SetAppearance() {
        _customizableAppearance.InstantiateByAppearanceData(
            _characterDataProvider.CharacterData.AppearanceData);
    }

    private bool _appearanceIsSet;
    private bool _uiIsBinded;

    private void Update() {
        // Данные персонажа устанавливаются в OnStartLocalPlayer, а внешний вид должен установиться
        // на их основе на всех объектах игрока. На практике оказалось, что, видимо,
        // синхронизация CharacterData происходит не сразу, и в методе Start CharacterData == null
        if (!_appearanceIsSet && _characterDataProvider.CharacterData != null) {
            SetAppearance();
            _appearanceIsSet = true;
        }
        // BindUI тоже опирается на CharacterData (например, для отображения в инвентаре)
        if (!_uiIsBinded && isLocalPlayer && _characterDataProvider.CharacterData != null) {
            BindUI();
            _uiIsBinded = true;
        }
        if (!isLocalPlayer || !hasAuthority || !_entity.IsAlive) {
            return;
        }

        ReadInput();
    }

    /// <summary>
    /// Информация персонажа может отображаться в инвентаре
    /// </summary>
    public IInventoryInfoProvider GetInventoryInfoProvider() {
        return _characterDataProvider;
    }

    private void ReadInput() {
        // if (Input.GetKeyDown (KeyCode.R)) entity.lifecycle.Damage(50);
        // if (Input.GetKeyDown (KeyCode.H)) entity.lifecycle.Heal(50);
        // if (Input.GetKeyDown (KeyCode.T)) entity.lifecycle.Respawn();
        
        _entity.movement.movementInputValues.x = Input.GetAxis("Horizontal");
        _entity.movement.movementInputValues.y = Input.GetAxis("Vertical");
        _entity.movement.jumpingInputValue = Input.GetButtonDown("Jump");
        _entity.movement.runningInputValue = Input.GetKey(KeyCode.LeftShift);

        // Информация об инвентаре
        if (Input.GetKeyDown(KeyCode.N)) {
            var dict = new Dictionary<string, int>();
            foreach (var item in _inventory.MainSection.Items.Values) {
                if (dict.ContainsKey(item.ItemData.ItemStaticDataName))
                    dict[item.ItemData.ItemStaticDataName] += item.Count;
                else
                    dict.Add(item.ItemData.ItemStaticDataName, item.Count);
            }

            Debug.Log("Инвентарь");
            foreach (var pair in dict)
                Debug.Log($"\t{pair.Key}: {pair.Value}");
            
            Debug.Log("Информация о том, что надето на персонажа");
            Debug.Log($"Всего надето/выбрано: {_inventory.WearSection.Items.Count}");
            foreach (ItemData itemData in _inventory.WearSection.Items) {
                Debug.Log($"\t{itemData}");
            }
        }

        if (Input.GetKeyDown(KeyCode.F)) {
            if (_itemsUIController.IsUIOpened) {
                _itemsUIController.CloseUI();
            } else {
                _interactor.Interact();
            }
        }

        if (Input.GetKeyDown(KeyCode.G)) {
            _itemThrower.ThrowAwayFirstIfExists();
        }

        if (Input.GetKeyDown(KeyCode.B)) {
            bool areAllAdded = _inventory.MainSection.TryToAddTestItems();
            if (!areAllAdded) {
                Debug.Log("Не все предметы были добавлены. Нет места в инвентаре для предметов "
                    + "данного размера.");
            }

            bool areAddedToWearSection = _inventory.WearSection.AddTestItems();
            if (areAddedToWearSection) {
                Debug.Log("Предметы успешно добавлены в WearSectin");
            } else {
                Debug.Log("Предметы не были добавлены в WearSection");
            }
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            var oldData = _characterDataProvider.CharacterData;
            string[] fuorianNames = new[] { "Kaur", "Kafwex", "Xafri", "Xesawi", "Fowex", "Fathi", "Ngurae" };
            int rndIndex = Random.Range(0, fuorianNames.Length);
            string newName = fuorianNames[rndIndex] + $" {Random.Range(100, 1000)}";
            CharacterData charData = new CharacterData() {
                AppearanceData = oldData.AppearanceData,
                Subtitle = oldData.Subtitle,
                Name = newName
            };
            _characterDataProvider.SetData(charData);
            Debug.Log("Новые данные игрока " + charData.ToString());
        }

        // Todo: только для теста! убрать после
        if (Input.GetKeyDown(KeyCode.Y)) {
            Player.dontEndDrag = !Player.dontEndDrag;
            var str = Player.dontEndDrag ? "не " : "";
            Debug.Log($"Drag End drop {str}будет заканчиваться!");
            
        }

        LifecycleEffect damage = new LifecycleEffect() {
            speed = -1f,
            duration = 10,
            targetParameter = EntityParameterEnum.Health
        };

        if (Input.GetKeyDown(KeyCode.H)) {
            _entity.AddEffect(damage);
            // entity.CmdAddEffect(damage);
        }

        if (Input.GetKeyDown(KeyCode.I)) {
            _itemsUIController.ShowPlayersInventory();
            _itemsUIController.ToggleUI();
        }
    }
}
