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
public class Player : NetworkBehaviour
{
    // Todo: refactor player; separate responsibilities

    // Todo: Только для теста! Удалить после
    public static bool dontEndDrag;

    private PickableItemIcon _pickableItemUI;
    private SimpleHUD _HUD;

    private ItemsUIController _itemsUIController;

    private Entity _entity;
    public Overview overview;

    // Components
    private CharactersInventory _inventory;
    public CharactersInventory Inventory => _inventory;
    private ItemThrower _itemThrower;
    private Interactor _interactor;
    private ItemInteractorStrategy _itemInteractorStrategy;
    private CharacterDataProvider _characterDataProvider;
    [SerializeField]
    private CustomizableAppearance _customizableAppearance;

    private void Awake() {
        // _mainInvSection = GetComponent<GridInventorySection>();
        // _itemPicker = GetComponent<ItemPicker>();
        _entity = GetComponent<Entity>();
        _inventory = GetComponent<CharactersInventory>();
        _itemThrower = GetComponent<ItemThrower>();
        _characterDataProvider = GetComponent<CharacterDataProvider>();

        _itemInteractorStrategy = GetComponent<ItemInteractorStrategy>();
        _itemInteractorStrategy.LookedAtItem += OnLookedAtItem;
        _itemInteractorStrategy.LookedAwayFromItem += OnLookedAwayFromItem;
    }

    public override void OnStartClient() {
        /* Initialize lifecycle and add Damage FX */
        // lifecycle.Initialize();
        // entity.lifecycle.AssignDamageAction(DamageFX);

        /* Initialize movement and add camera shake when landing */
        // movement.Initialize();
        // _entity.movement.AssignLandingAction(() => overview.Shake(0.5f));
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
        _itemsUIController.SetPlayer(this);

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
        if (!isLocalPlayer || !hasAuthority) {
            return;
        }

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
        return _characterDataProvider;
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

    private void DamageFX() {
        overview.Shake(0.75f);
    }
}
