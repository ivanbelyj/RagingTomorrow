using System.Collections;
using System.Collections.Generic;
using AFPC;
using UnityEngine;
using Mirror;
using TMPro;

[RequireComponent(typeof(ItemInteractorStrategy))]
[RequireComponent(typeof(Entity))]
[RequireComponent(typeof(CharactersInventory))]
[RequireComponent(typeof(CharacterInfo))]
[RequireComponent(typeof(Interactor))]
[RequireComponent(typeof(ItemInteractorStrategy))]
public class Player : NetworkBehaviour
{
    // Todo: refactor player; separate responsibilities

    // Todo: Только для теста! Удалить после
    public static bool dontEndDrag;

    // UI
    public GameObject floatingInfo;
    public TextMeshPro playerNameText;
    public TextMeshPro playerHealthText;
    public TextMeshPro playerInventoryText;

    private PickableItemIcon _pickableItemUI;
    private SimpleHUD _HUD;

    // Components
    private CharactersInventory _inventory;
    public CharactersInventory Inventory => _inventory;

    // private ItemPicker _itemPicker;
    private ItemThrower _itemThrower;

    private Interactor _interactor;
    private ItemInteractorStrategy _itemInteractorStrategy;
    private CharacterInfo _characterInfo;
    private Entity _entity;
    public Overview overview;

    private ItemsUIController _itemsUIController;

    private void Awake() {
        // _mainInvSection = GetComponent<GridInventorySection>();
        // _itemPicker = GetComponent<ItemPicker>();
        _entity = GetComponent<Entity>();
        _inventory = GetComponent<CharactersInventory>();
        _itemThrower = GetComponent<ItemThrower>();
        _characterInfo = GetComponent<CharacterInfo>();
        _characterInfo.CharacterInfoChanged += (CharacterInfo.CharacterInfoData newInfo) => {
            // Debug.Log("Character info is changed");
            playerNameText.text = newInfo.Name;
        };

        _itemInteractorStrategy = GetComponent<ItemInteractorStrategy>();
        _itemInteractorStrategy.LookedAtItem += OnLookedAtItem;
        _itemInteractorStrategy.LookedAwayFromItem += OnLookedAwayFromItem;
    }

    private void Start() {
    }

    public override void OnStartClient() {
        /* Initialize lifecycle and add Damage FX */
        // lifecycle.Initialize();
        // entity.lifecycle.AssignDamageAction(DamageFX);

        /* Initialize movement and add camera shake when landing */
        // movement.Initialize();
        // _entity.movement.AssignLandingAction(() => overview.Shake(0.5f));
        BindFloatingInfo();
    }

    private void BindFloatingInfo() {
        SetHealthText(_entity.health.Value);
        _entity.health.OnValueChanged += (old, newVal) => {
            SetHealthText(newVal);
        };
        void SetHealthText(float val) {
            playerHealthText.text = string.Format("{0:F2}", System.Math.Round(val, 2));
        }

        // To fix: если на сервере уже стоял Count, то на только что подключенном клиенте
        // он отображается как 0
        playerInventoryText.text = _inventory.MainSection.Items.Count.ToString();
        _inventory.MainSection.InventoryChanged += (SyncList<GridSectionItem>.Operation op,
            int index, GridSectionItem oldItem, GridSectionItem newItem) => {
            playerInventoryText.text = _inventory.MainSection.Items.Count.ToString();
            // Debug.Log($"Inventory is changed! "
            //     + (newItem is null ? "" : $"{newItem.itemData.itemStaticDataName} is added,")
            //     + (oldItem is null ? "" : $" {oldItem.itemData.itemStaticDataName} is removed. ")
            //     + $" New Count is {playerInventoryText.text}");
        };
    }

    public override void OnStartLocalPlayer()
    {
        // A few apllication settings for more smooth. This is Optional
        QualitySettings.vSyncCount = 0;
        Cursor.lockState = CursorLockMode.Locked;
        overview.camera = Camera.main;

        _interactor = GetComponent<Interactor>();
        Debug.Log($"Initialization of _interactor {_interactor} with transform {overview.camera.transform}");
        _interactor.Initialize(overview.camera.transform);

        // _interactor.LookingToInteractiveObject += OnLookingToItem;

        // Player name must be set
        BindUI();
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
        _itemsUIController.SetPlayer(this);

        _itemsUIController.CloseUI();

        _pickableItemUI = FindObjectOfType<PickableItemIcon>();
        _pickableItemUI.HideIcon();
    }

    private void Update() {
        if (!isLocalPlayer) {
            floatingInfo.transform.LookAt(Camera.main.transform);
            return;
        }

        if (!hasAuthority)
            return;

        ReadInput();

        if (!_entity.IsAlive)
            return;

        /* Block controller when unavailable */
        // if (!entity.lifecycle.Availability()) return;

        /* Mouse look state */
        overview.Looking();

        /* Change camera FOV state */
        overview.Aiming();

        /* Shake camera state. Required "physical camera" mode on */
        overview.Shaking();

        /* Control the speed */
        // movement.Running();

        /* Control the jumping, ground search... */
        // movement.Jumping();

        /* Control the health and shield recovery */
        // lifecycle.Runtime();
    }

    private void FixedUpdate() {

        /* Block controller when unavailable */
        // if (!entity.lifecycle.Availability()) return;
        if (!_entity.IsAlive)
            return;

        /* Physical rotation with camera */
        overview.RotateRigigbodyToLookDirection(_entity.movement.rb);
    }

    private void LateUpdate() {
        if (!_entity.IsAlive)
            return;
        
        /* Block controller when unavailable */
        // if (!entity.lifecycle.Availability()) return;

        /* Camera following */
        overview.Follow(transform.position);
    }

    /// <summary>
    /// Информация персонажа может отображаться в инвентаре
    /// </summary>
    public IInventoryInfoProvider GetInventoryInfoProvider() {
        return _characterInfo;
    }

    public GridSection GetMainInventorySection() {
        return _inventory.MainSection;
    }

    private void ReadInput() {
        // if (Input.GetKeyDown (KeyCode.R)) entity.lifecycle.Damage(50);
        // if (Input.GetKeyDown (KeyCode.H)) entity.lifecycle.Heal(50);
        // if (Input.GetKeyDown (KeyCode.T)) entity.lifecycle.Respawn();
        overview.lookingInputValues.x = Input.GetAxis("Mouse X");
        overview.lookingInputValues.y = Input.GetAxis("Mouse Y");

        overview.aimingInputValue = Input.GetMouseButton(1);
        _entity.movement.movementInputValues.x = Input.GetAxis("Horizontal");
        _entity.movement.movementInputValues.y = Input.GetAxis("Vertical");
        _entity.movement.jumpingInputValue = Input.GetButtonDown("Jump");
        _entity.movement.runningInputValue = Input.GetKey(KeyCode.LeftShift);

        // Информация об инвентаре
        if (Input.GetKeyDown(KeyCode.N)) {
            var dict = new Dictionary<string, int>();
            foreach (var item in _inventory.MainSection.Items) {
                if (dict.ContainsKey(item.itemData.ItemStaticDataName))
                    dict[item.itemData.ItemStaticDataName] += item.count;
                else
                    dict.Add(item.itemData.ItemStaticDataName, item.count);
            }

            Debug.Log("Инвентарь");
            foreach (var pair in dict)
                Debug.Log($"\t{pair.Key}: {pair.Value}");
        }
        if (Input.GetKeyDown(KeyCode.M)) {
            Debug.Log("Информация о том, что надето на персонажа");
            Debug.Log($"Всего надето/выбрано: {_inventory.WearSection.Slots.Count}");
            foreach (var pair in _inventory.WearSection.Slots) {
                Debug.Log($"\tСлот {pair.Key.ToString()}: {pair.Value.ItemStaticDataName}");
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
            if (_inventory.MainSection.Items.Count != 0) {
                var item = _inventory.MainSection.Items[0];
                // Debug.Log($"Inv. count: {_inventory.MainSection.Items.Count}. Элемент "
                //     + item.itemData.itemStaticDataName
                //     + " будет выброшен");
                _itemThrower.ThrowAwayFromGridSection(item);
            }
        }

        if (Input.GetKeyDown(KeyCode.B)) {
            bool isAllAdded = _inventory.MainSection.AddTestItems();
            if (!isAllAdded) {
                Debug.Log("Не все предметы были добавлены. Нет места в инвентаре для предметов "
                    + "данного размера.");
            }
            bool isAddedToWear = _inventory.WearSection.AddTestItems();
            // if (isAddedToWear)
                // Debug.Log("Добавлен предмет в WearSection");
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

    private void DamageFX() {
        overview.Shake(0.75f);
    }
}
