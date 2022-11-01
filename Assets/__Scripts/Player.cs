using System.Collections;
using System.Collections.Generic;
using AFPC;
using UnityEngine;
using Mirror;
using TMPro;

[RequireComponent(typeof(Inventory))]
[RequireComponent(typeof(ItemPicker))]
[RequireComponent(typeof(Entity))]
public class Player : NetworkBehaviour, IInventoryInfo
{
    // UI
    public GameObject floatingInfo;
    public TextMeshPro playerNameText;
    public TextMeshPro playerHealthText;
    public TextMeshPro playerInventoryText;

    private SimpleHUD _HUD;

    // Components
    private Inventory _inventory;
    private ItemPicker _itemPicker;
    private Entity _entity;
    public Overview overview;

    private ItemsUIController _itemsUIController;
    
    // Object fields
    [SyncVar(hook = nameof(OnPlayerNameChanged))]
    private string _syncPlayerName;
    private string _playerName;
    public string PlayerName => _playerName;

    string IInventoryInfo.Title => PlayerName;
    string IInventoryInfo.SubTitle => "";
    Sprite IInventoryInfo.Avatar => null;


    // For test
    [SerializeField]
    private Inventory _otherInventory;

    private void Awake() {
        _inventory = GetComponent<Inventory>();
        _itemPicker = GetComponent<ItemPicker>();
        _entity = GetComponent<Entity>();

        // For test
        _otherInventory = GameObject.Find("OtherInventoryTest").GetComponent<Inventory>();
    }

    private void Start() {
        /* a few apllication settings for more smooth. This is Optional. */
        QualitySettings.vSyncCount = 0;
        Cursor.lockState = CursorLockMode.Locked;

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

        playerInventoryText.text = _inventory.Items.Count.ToString();
        _inventory.InventoryChanged += (SyncList<InventoryItem>.Operation op,
            int index, InventoryItem oldItem, InventoryItem newItem) => {
            playerInventoryText.text = _inventory.Items.Count.ToString();
            Debug.Log($"Inventory is changed! {newItem.itemGameData.itemStaticDataName} is added,"
                + $" {oldItem.itemGameData.itemStaticDataName} is removed");
        };
    }

    public override void OnStartLocalPlayer()
    {
        overview.camera = Camera.main;

        string newPlayerName = $"Player {Random.Range(100, 999)}";
        if (isServer) {
            ChangePlayerName(newPlayerName);
        } else {
            CmdChangePlayerName(newPlayerName);
        }

        BindUI();
    }

    private void BindUI() {
        _HUD = FindObjectOfType<SimpleHUD>();
        if (_HUD is not null) {
            _HUD.SetEntity(gameObject);
            Debug.Log("HUD is set");
        }

        _itemsUIController = FindObjectOfType<ItemsUIController>();
        _itemsUIController.SetPlayer(this);

        _itemsUIController.CloseUI();
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
            foreach (var item in _inventory.Items) {
                if (dict.ContainsKey(item.itemGameData.itemStaticDataName))
                    dict[item.itemGameData.itemStaticDataName]++;
                else
                    dict.Add(item.itemGameData.itemStaticDataName, 1);
            }

            Debug.Log("Инвентарь");
            foreach (var pair in dict)
                Debug.Log($"\t{pair.Key}: {pair.Value}");
        }

        if (Input.GetKeyDown(KeyCode.G)) {
            if (_inventory.Items.Count != 0) {
                Debug.Log("Элемент " + _inventory.Items[0].itemGameData.itemStaticDataName
                    + " будет выброшен");
                _itemPicker.ThrowAway(_inventory.Items[0]);
            }
        }

        if (Input.GetKeyDown(KeyCode.B)) {
            bool isAllAdded = _inventory.AddTestItems();
            if (!isAllAdded) {
                Debug.Log("Не все предметы были добавлены. Нет места в инвентаре для предметов "
                    + "данного размера.");
            }
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

        // Имитация открытия ящика
        if (Input.GetKeyDown(KeyCode.F)) {
            InventoryInfo inventoryInfo = new InventoryInfo() {
                Title = "Ящик",
                SubTitle = "Маленький"
            };
            _itemsUIController.ShowOtherInventory(inventoryInfo, _otherInventory);
            _itemsUIController.ToggleUI();
        }
    }

    private void DamageFX() {
        overview.Shake(0.75f);
    }

    #region Sync
    private void OnPlayerNameChanged(string oldName, string newName) {
        _playerName = newName;
        playerNameText.text = newName;
    }

    [Server]  // Будет вызываться и выполняться только на сервере
    private void ChangePlayerName(string newName) {
        _syncPlayerName = newName;
    }

    [Command]  // Метод выполняется на сервере по запросу клиента
    private void CmdChangePlayerName(string name) {
        ChangePlayerName(name);
    }
    #endregion
}
