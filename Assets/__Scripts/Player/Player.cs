using System.Collections;
using System.Collections.Generic;
using AFPC;
using UnityEngine;
using Mirror;
using TMPro;
using AppearanceCustomization3D;

[RequireComponent(typeof(EntityLifecycle))]
[RequireComponent(typeof(EntityMovement))]
[RequireComponent(typeof(CharactersInventory))]
[RequireComponent(typeof(CharacterDataProvider))]
[RequireComponent(typeof(Interactor))]
[RequireComponent(typeof(ItemInteractorStrategy))]
[RequireComponent(typeof(PlayerOverview))]
public class Player : NetworkBehaviour
{
    // Todo: Только для теста! Удалить после
    public static bool dontEndDrag;

    private PickableItemIcon pickableItemUI;
    private SimpleHUD hud;

    private ItemsUIController itemsUIController;


    // Components
    private EntityLifecycle lifecycle;
    private EntityMovement movement;
    private CharactersInventory inventory;
    public CharactersInventory Inventory => inventory;
    private ItemThrower itemThrower;
    private Interactor interactor;
    private ItemInteractorStrategy itemInteractorStrategy;
    private CharacterDataProvider characterDataProvider;
    [SerializeField]
    private CustomizableAppearance customizableAppearance;
    private PlayerOverview playerCamera;

    private void Awake() {
        lifecycle = GetComponent<EntityLifecycle>();
        movement = GetComponent<EntityMovement>();
        inventory = GetComponent<CharactersInventory>();
        itemThrower = GetComponent<ItemThrower>();
        characterDataProvider = GetComponent<CharacterDataProvider>();
        playerCamera = GetComponent<PlayerOverview>();

        interactor = GetComponent<Interactor>();

        itemInteractorStrategy = GetComponent<ItemInteractorStrategy>();
        itemInteractorStrategy.LookedAtItem += OnLookedAtItem;
        itemInteractorStrategy.LookedAwayFromItem += OnLookedAwayFromItem;
    }

    public override void OnStartClient() {
        // lifecycle.Initialize();
        // entity.lifecycle.AssignDamageAction(DamageFX);

        // movement.Initialize();
        // _entity.movement.AssignLandingAction(() => overview.Shake(0.5f));
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log($"Initialization of _interactor {interactor} with transform" +
            $" {playerCamera.Overview.camera.transform}");
        interactor.Initialize(Camera.main.transform);

        // _interactor.LookingToInteractiveObject += OnLookingToItem;

        // Данные для игрока устанавливаются и синхронизируются лишь раз, когда он появился
        characterDataProvider.SetInitialData();
    }

    private void OnLookedAtItem(Item item) {
        pickableItemUI.ShowIcon(item.ItemData);
        // Debug.Log("Looked at item");
    }

    private void OnLookedAwayFromItem() {
        pickableItemUI.HideIcon();
        // Debug.Log("Looked away from item");
    }

    private void BindUI() {
        hud = FindObjectOfType<SimpleHUD>();
        if (hud is not null) {
            hud.SetEntity(gameObject);
            // Debug.Log("HUD is set");
        }

        itemsUIController = FindObjectOfType<ItemsUIController>();
        itemsUIController.SetPlayer(this.gameObject, this, playerCamera);

        itemsUIController.CloseUI();

        pickableItemUI = FindObjectOfType<PickableItemIcon>();
        pickableItemUI.HideIcon();
    }

    private void SetAppearance() {
        customizableAppearance.InstantiateByAppearanceData(
            characterDataProvider.CharacterData.AppearanceData);
    }

    private bool _appearanceIsSet;
    private bool _uiIsBinded;

    private void Update() {
        // Данные персонажа устанавливаются в OnStartLocalPlayer, а внешний вид должен установиться
        // на их основе на всех объектах игрока. На практике оказалось, что, видимо,
        // синхронизация CharacterData происходит не сразу, и в методе Start CharacterData == null
        if (!_appearanceIsSet && characterDataProvider.CharacterData != null) {
            SetAppearance();
            _appearanceIsSet = true;
        }
        // BindUI тоже опирается на CharacterData (например, для отображения в инвентаре)
        if (!_uiIsBinded && isLocalPlayer && characterDataProvider.CharacterData != null) {
            BindUI();
            _uiIsBinded = true;
        }
        if (!isLocalPlayer || !hasAuthority || !lifecycle.IsAlive) {
            return;
        }

        ReadInput();
    }

    /// <summary>
    /// Информация персонажа может отображаться в инвентаре
    /// </summary>
    public IInventoryInfoProvider GetInventoryInfoProvider() {
        return characterDataProvider;
    }

    private void ReadInput() {
        movement.SetInputValues(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        movement.SetJumpingInputValue(Input.GetButtonDown("Jump"));
        movement.SetRunningInputValue(Input.GetKey(KeyCode.LeftShift));

        // Информация об инвентаре
        if (Input.GetKeyDown(KeyCode.N)) {
            var dict = new Dictionary<string, int>();
            foreach (var item in inventory.MainSection.Items.Values) {
                if (dict.ContainsKey(item.ItemData.ItemStaticDataName))
                    dict[item.ItemData.ItemStaticDataName] += item.Count;
                else
                    dict.Add(item.ItemData.ItemStaticDataName, item.Count);
            }

            Debug.Log("Инвентарь");
            foreach (var pair in dict)
                Debug.Log($"\t{pair.Key}: {pair.Value}");
            
            Debug.Log("Информация о том, что надето на персонажа");
            Debug.Log($"Всего надето/выбрано: {inventory.WearSection.Items.Count}");
            foreach (ItemData itemData in inventory.WearSection.Items) {
                Debug.Log($"\t{itemData}");
            }
        }

        if (Input.GetKeyDown(KeyCode.F)) {
            if (itemsUIController.IsUIOpened) {
                itemsUIController.CloseUI();
            } else {
                interactor.Interact();
            }
        }

        if (Input.GetKeyDown(KeyCode.G)) {
            itemThrower.ThrowAwayFirstIfExists();
        }

        if (Input.GetKeyDown(KeyCode.B)) {
            bool areAllAdded = inventory.MainSection.TryToAddTestItems();
            if (!areAllAdded) {
                Debug.Log("Не все предметы были добавлены. Нет места в инвентаре для предметов "
                    + "данного размера.");
            }

            bool areAddedToWearSection = inventory.WearSection.AddTestItems();
            if (areAddedToWearSection) {
                Debug.Log("Предметы успешно добавлены в WearSectin");
            } else {
                Debug.Log("Предметы не были добавлены в WearSection");
            }
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            var oldData = characterDataProvider.CharacterData;
            string[] fuorianNames = new[] { "Kaur", "Kafwex", "Xafri", "Xesawi", "Fowex", "Fathi", "Ngurae" };
            int rndIndex = Random.Range(0, fuorianNames.Length);
            string newName = fuorianNames[rndIndex] + $" {Random.Range(100, 1000)}";
            CharacterData charData = new CharacterData() {
                AppearanceData = oldData.AppearanceData,
                Subtitle = oldData.Subtitle,
                Name = newName
            };
            characterDataProvider.SetData(charData);
            Debug.Log("Новые данные игрока " + charData.ToString());
        }

        // Todo: только для теста! убрать после
        if (Input.GetKeyDown(KeyCode.Y)) {
            Player.dontEndDrag = !Player.dontEndDrag;
            var str = Player.dontEndDrag ? "не " : "";
            Debug.Log($"Drag End drop {str}будет заканчиваться!");
            
        }

        if (Input.GetKeyDown(KeyCode.X)) {
            Debug.Log("Эффекты");
            foreach (var effect in lifecycle.Effects) {
                Debug.Log("\t" + effect);
            }
        }

        LifecycleEffect damage = new LifecycleEffect() {
            speed = -0.1f,
            duration = 5,
            targetParameter = LifecycleParameterEnum.Health
        };

        if (Input.GetKeyDown(KeyCode.H)) {
            lifecycle.AddEffect(damage);
            // entity.CmdAddEffect(damage);
        }

        if (Input.GetKeyDown(KeyCode.I)) {
            itemsUIController.ShowPlayersInventory();
            itemsUIController.ToggleUI();
        }
    }
}
