/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Coroutines;
using AuroraFPSRuntime.CoreModules.InputSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AuroraFPSRuntime.SystemModules.InventoryModules
{
    [HideScriptField]
    [AddComponentMenu(null)]
    [DisallowMultipleComponent]
    public abstract class InventorySystem : MonoBehaviour, IInventorySystem<InventoryItem>, IInventoryIEnumerable<InventoryItem>, IInputListener
    {
        [SerializeField]
        [NotNull]
        [Foldout("Equippable Object Settings", Style = "Header")]
        [Order(501)]
        private Transform container;

        // Stored required properties.
        private EquippableItem equippedItem;
        private Transform equippedTransfrom;
        private CoroutineObject<string, InventoryItem> itemProcessing;
        private Dictionary<EquippableItem, Transform> equippableObjectsHash;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            Debug.Assert(container != null, $"<b><color=#FF0000>Attach reference of the container to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Container<i>(field)</i>.</color></b>");

            itemProcessing = new CoroutineObject<string, InventoryItem>(this);
            equippableObjectsHash = new Dictionary<EquippableItem, Transform>();
            SaveCacheOfItemObjects();
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled
        /// just before any of the Update methods are called the first time.
        /// </summary>
        protected virtual void Start()
        {
            RegisterItemsCallback(Items);
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        protected virtual void OnEnable()
        {
            RegisterInputActions();
        }

        /// <summary>
        /// Called when the behaviour becomes disabled.
        /// </summary>
        protected virtual void OnDisable()
        {
            RemoveInputActions();
        }

        /// <summary>
        /// Use specified item in inventory system.
        /// </summary>
        /// <param name="item">InventoryItem reference to use.</param>
        public virtual void UseItem(InventoryItem item)
        {
            if (item != null)
            {
                if (item is EquippableItem equippableItem)
                {
                    itemProcessing.Start(ItemProcessing, "Equip", equippableItem, true);
                }
                else
                {
                    OnUseItem(item);
                }
            }
        }

        /// <summary>
        /// Remove item from inventory and throw it at the scene.
        /// </summary>
        /// <param name="item">Inventory item reference.</param>
        public virtual void TossItem(InventoryItem item)
        {
            itemProcessing.Start(ItemProcessing, "Toss", item);
        }

        /// <summary>
        /// Remove equipped item from inventory and throw it at the scene.
        /// </summary>
        public virtual void TossItem()
        {
            if (equippedItem != null)
            {
                TossItem(equippedItem);
            }
        }

        /// <summary>
        /// Remove equipped item from inventory and throw it at the scene without animation.
        /// </summary>
        public virtual void TossItemForce()
        {
            if (equippedItem != null)
            {
                if (equippedTransfrom != null)
                {
                    equippedTransfrom.gameObject.SetActive(false);
                    OnHideCallback?.Invoke();
                }
                OnTossItem(equippedItem);
                RemoveItem(equippedItem);
                equippedItem = null;
                equippedTransfrom = null;
            }
        }

        /// <summary>
        /// Remove all item from inventory and throw it at the scene without animation.
        /// </summary>
        public virtual void TossAllItemsForce()
        {
            List<InventoryItem> items = Items.ToList();
            for (int i = 0; i < items.Count; i++)
            {
                InventoryItem item = items[i];
                if (item == equippedItem && equippedTransfrom != null)
                {
                    equippedTransfrom.gameObject.SetActive(false);
                    OnHideCallback?.Invoke();
                }
                OnTossItem(item);
                RemoveItem(item);
            }
        }

        /// <summary>
        /// Hide equipped item.
        /// </summary>
        public virtual void HideItem(bool hide)
        {
            if (equippedTransfrom != null)
            {
                string action = equippedTransfrom.gameObject.activeSelf ? "Hide" : "Show";
                itemProcessing.Start(ItemProcessing, action, equippedItem);
            }
        }

        /// <summary>
        /// Hide equipped item without animation.
        /// </summary>
        public virtual void HideItemForce(bool hide)
        {
            if (equippedTransfrom != null)
            {
                equippedTransfrom.gameObject.SetActive(false);
                OnHideCallback?.Invoke();
            }
        }

        /// <summary>
        /// True if item is equipped. Otherwise false. 
        /// </summary>
        /// <returns></returns>
        public bool IsEquipped()
        {
            return equippedItem != null
                && equippedTransfrom != null
                && equippedTransfrom.gameObject.activeSelf;
        }

        /// <summary>
        /// Start item processing coroutine object.
        /// </summary>
        protected bool StartItemProcessing(string key, InventoryItem item, bool force = false)
        {
            return itemProcessing.Start(ItemProcessing, key, item, force);
        }

        /// <summary>
        /// Instantiate first person equippable object.
        /// </summary>
        /// <returns>Instantiated game object reference.</returns>
        protected virtual Transform InstantiateItemObject(EquippableItem equippableItem)
        {
            GameObject equippalbeObject = Instantiate(equippableItem.GetFirstPersonObject(), container);
            equippalbeObject.transform.localPosition = equippableItem.GetFirstPersonPosition();
            equippalbeObject.transform.localRotation = Quaternion.Euler(equippableItem.GetFirstPersonRotation());
            equippalbeObject.transform.localScale = equippableItem.GetFirstPersonScale();
            equippalbeObject.gameObject.SetActive(false);

            EquippableObjectIdentifier identifier = equippalbeObject.GetComponent<EquippableObjectIdentifier>();
            if (identifier == null)
                identifier = equippalbeObject.AddComponent<EquippableObjectIdentifier>();

            identifier.SetItem(equippableItem);
            equippableObjectsHash[equippableItem] = identifier.transform;

            return equippalbeObject.transform;
        }

        /// <summary>
        /// Add equippable object hash.
        /// </summary>
        /// <param name="equipplabeItem">Weapon item key.</param>
        /// <param name="equippableTransform">First person weapon transform.</param>
        protected void AddItemHash(EquippableItem equipplabeItem, Transform equippableTransform)
        {
            equippableObjectsHash.Add(equipplabeItem, equippableTransform);
        }

        /// <summary>
        /// Remove equippable object hash.
        /// </summary>
        /// <param name="equippableItem">Target weapon item key.</param>
        protected bool RemoveItemHash(EquippableItem equippableItem)
        {
            return equippableObjectsHash.Remove(equippableItem);
        }

        /// <summary>
        /// Register input receiver actions.
        /// </summary>
        public virtual void RegisterInputActions()
        {
            InputReceiver.TossItemAction.performed += TossLastWeaponAction;
            InputReceiver.HideItemAction.performed += HideWeaponAction;
        }

        /// <summary>
        /// Remove actions from input receiver.
        /// </summary>
        public virtual void RemoveInputActions()
        {
            InputReceiver.TossItemAction.performed -= TossLastWeaponAction;
            InputReceiver.HideItemAction.performed -= HideWeaponAction;
        }

        /// <summary>
        /// Item processing coroutine.
        /// Implement this method to add additional keys.
        /// </summary>
        /// <param name="key">Unique key of processing action.</param>
        /// <param name="item">Inventory item reference.</param>
        protected virtual IEnumerator ItemProcessing(string key, InventoryItem item)
        {
            switch (key)
            {
                case "Equip":
                    {
                        if (item is EquippableItem equippableItem)
                        {
                            yield return WaitForHideItem();
                            yield return WaitForEquipItem(equippableItem);
                            OnUseItem(item);
                        }
                        yield break;
                    }
                case "Toss":
                    {
                        yield return WaitForHideItem();
                        OnTossItem(item);
                        RemoveItem(item);
                        if (item == equippedItem)
                        {
                            equippedItem = null;
                            equippedTransfrom = null;
                        }
                        yield break;
                    }
                case "Hide":
                    {
                        yield return WaitForHideItem();
                        yield break;
                    }
                case "Show":
                    {
                        if (item is EquippableItem equippableItem)
                        {
                            yield return WaitForEquipItem(equippableItem);
                        }
                        yield break;
                    }
            }
        }

        /// <summary>
        /// Instruction to wait until the item is equipped.
        /// </summary>
        /// <param name="item">Inventory item to equip.</param>
        protected IEnumerator WaitForEquipItem(EquippableItem item)
        {
            if (item != null)
            {
                if (!TryGetItemTransform(item, out equippedTransfrom))
                {
                    equippedTransfrom = InstantiateItemObject(item);
                }

                equippedTransfrom.gameObject.SetActive(true);
                OnEquipStartedCallback?.Invoke(item);
                EquippableObjectAnimationSystem animation = equippedTransfrom.GetComponent<EquippableObjectAnimationSystem>();
                if (animation != null)
                {
                    animation.DisableBehaviours();
                    animation.PlayPullAnimation();
                }
                yield return new WaitForSeconds(item.GetSelectTime());
                if (animation != null)
                {
                    animation.EnableBehaviours();
                }
                equippedItem = item;

                OnEquipPerformedCallback?.Invoke(item);
            }
            yield break;
        }

        /// <summary>
        /// Instruction to wait until the item is hidden.
        /// </summary>
        protected IEnumerator WaitForHideItem()
        {
            if (equippedItem != null && equippedTransfrom != null && equippedTransfrom.gameObject.activeSelf)
            {
                EquippableObjectAnimationSystem animation = equippedTransfrom.GetComponent<EquippableObjectAnimationSystem>();
                if (animation != null)
                {
                    animation.DisableBehaviours();
                    animation.PlayPushAnimation();
                }
                yield return new WaitForSeconds(equippedItem.GetHideTime());
                if (animation != null)
                {
                    animation.EnableBehaviours();
                }
                equippedTransfrom.gameObject.SetActive(false);
                OnHideCallback?.Invoke();
            }
            yield break;
        }

        /// <summary>
        /// Base implementation of using inventory item.
        /// </summary>
        protected void OnUseItem(InventoryItem item)
        {
            InvokeItemCallback(item);
            if (!item.Reusable())
            {
                RemoveItem(item);
            }
        }

        /// <summary>
        /// Base implementation of tossing inventory item.
        /// </summary>
        /// <param name="item"></param>
        protected void OnTossItem(InventoryItem item)
        {
            if (item != null && item.GetDropObject() != null)
            {
                Vector3 position = container.position + (container.forward * item.GetDropForwardMultiplier());
                Quaternion rotation = Quaternion.Euler(item.GetDropRotation());
                PoolObject weaponClone = PoolManager.GetRuntimeInstance().CreateOrPop(item.GetDropObject(), position, rotation);
                weaponClone.transform.localScale = item.GetDropScale();
                Rigidbody rigidbody = weaponClone.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    rigidbody.AddForce(container.forward * item.GetDropForce(), ForceMode.Impulse);
                    rigidbody.AddTorque(container.forward * item.GetDropTorque(), ForceMode.Impulse);
                }
                OnTossCallback?.Invoke(item, weaponClone.transform);
                OnTossItemCallback?.Invoke(item);
            }
        }

        /// <summary>
        /// Register event callback function of inventory items.
        /// Called once when the script instance is being loaded.
        /// Implement this method to make custom registration of items callbacks.
        /// </summary>
        private void RegisterItemsCallback(IEnumerable<InventoryItem> items)
        {
            foreach (InventoryItem item in items)
            {
                RegisterItemCallback(this, item);
            }
        }

        /// <summary>
        /// Initialize all equippable objects, which found in the container.
        /// </summary>
        /// <param name="container">Target equippable object container.</param>
        /// <param name="equippableObjectsHash">Initialized dictionary with equippable objects.</param>
        private void SaveCacheOfItemObjects()
        {
            EquippableObjectIdentifier[] equippableObjectIdentifiers = container.GetComponentsInChildren<EquippableObjectIdentifier>(true);
            if (equippableObjectIdentifiers != null)
            {
                for (int i = 0; i < equippableObjectIdentifiers.Length; i++)
                {
                    EquippableObjectIdentifier identifier = equippableObjectIdentifiers[i];
                    equippableObjectsHash[identifier.GetItem()] = identifier.transform;
                }
            }
        }

        #region [IInventorySystem Implementation]
        /// <summary>
        /// Add new item in inventory.
        /// </summary>
        /// <param name="item">Inventory item reference.</param>
        public abstract bool AddItem(InventoryItem item);

        /// <summary>
        /// Remove item from inventory.
        /// </summary>
        /// <param name="item">Inventory item reference.</param>
        public abstract bool RemoveItem(InventoryItem item);
        #endregion

        #region [IInventoryIterator Implemetation]
        /// <summary>
        /// Iterate through all inventory items in inventory.
        /// </summary>
        public abstract IEnumerable<InventoryItem> Items { get; }
        #endregion

        #region [Input Action Wrappers]
        private void TossLastWeaponAction(InputAction.CallbackContext context)
        {
            if (equippedItem != null)
            {
                TossItem(equippedItem);
            }
        }

        private void HideWeaponAction(InputAction.CallbackContext context)
        {
            if (equippedItem != null)
            {
                HideItem(equippedTransfrom.gameObject.activeSelf);
            }
        }
        #endregion

        #region [Event Callback Functions]
        /// <summary>
        /// Called when equipped item. 
        /// </summary>
        public event Action<EquippableItem> OnEquipStartedCallback;

        /// <summary>
        /// Called when equipping object being performed.
        /// </summary>
        public event Action<EquippableItem> OnEquipPerformedCallback;

        /// <summary>
        /// Called when active equipped object was hidden.
        /// </summary>
        public event Action OnHideCallback;

        /// <summary>
        /// Called when remove item from inventory and throw it at the scene.
        /// </summary>
        [Obsolete("Use OnTossCallback instead.")]
        public event Action<InventoryItem> OnTossItemCallback;

        /// <summary>
        /// Called when remove item from inventory and throw it at the scene.
        /// </summary>
        public event Action<InventoryItem, Transform> OnTossCallback;
        #endregion

        #region [Static Methods]
        /// <summary>
        /// Register item event callback.
        /// </summary>
        /// <param name="owner">Event callback fucntion owner.</param>
        /// <param name="item">Target item.</param>
        public static void RegisterItemCallback(Component owner, InventoryItem item)
        {
            item.InitializeEventFunctions(owner);
        }

        /// <summary>
        /// Invoke item event callback.
        /// </summary>
        /// <param name="item">Target item.</param>
        public static void InvokeItemCallback(InventoryItem item)
        {
            item.InvokeEventFunctions();
        }
        #endregion

        #region [Getter / Setter]
        public Transform GetContainer()
        {
            return container;
        }

        public void SetContainer(Transform value)
        {
            container = value;
        }

        /// <summary>
        /// Try to find the item by name in the inventory. 
        /// If the inventory contains another item with the same name, return the first one.
        /// </summary>
        /// <param name="name">Item name.</param>
        /// <returns>InventoryItem reference if item founded in inventory. Otherwise null.</returns>
        public virtual InventoryItem FindItem(string name)
        {
            foreach (InventoryItem iterator in Items)
            {
                if (iterator.GetItemName() == name)
                {
                    return iterator;
                }
            }
            return null;
        }

        /// <summary>
        /// Find all the items by name in the inventory. 
        /// </summary>
        /// <param name="name">Item name.</param>
        /// <returns>List of items with target name.</returns>
        public virtual List<InventoryItem> FindAllItem(string name)
        {
            List<InventoryItem> items = null;
            foreach (InventoryItem iterator in Items)
            {
                if (iterator.GetItemName() == name)
                {
                    if (items == null)
                    {
                        items = new List<InventoryItem>(1);
                    }
                    items.Add(iterator);
                }
            }
            return items;
        }

        /// <summary>
        /// Try to find the item by type in the inventory. 
        /// If the inventory contains another item with the same type, return the first one.
        /// </summary>
        /// <returns>Item reference with target type.</returns>
        public virtual T FindItem<T>() where T : InventoryItem
        {
            foreach (T iterator in Items)
            {
                if (iterator.GetType() == typeof(T))
                {
                    return iterator;
                }
            }
            return null;
        }

        /// <summary>
        /// Find all items with target type.
        /// </summary>
        /// <typeparam name="T">Search target type.</typeparam>
        /// <returns>List of items with target type.</returns>
        public virtual List<T> FindAllItem<T>() where T : InventoryItem
        {
            List<T> items = null;
            foreach (T iterator in Items)
            {
                if (iterator.GetType() == typeof(T))
                {
                    if (items == null)
                    {
                        items = new List<T>(1);
                    }
                    items.Add(iterator);
                }
            }
            return items;
        }

        /// <summary>
        /// Current equipped object transform.
        /// </summary>
        public Transform GetEquippedTransform()
        {
            return equippedTransfrom;
        }

        /// <summary>
        /// Current equipped item.
        /// </summary>
        public EquippableItem GetEquippedItem()
        {
            return equippedItem;
        }

        /// <summary>
        /// Gets the item transform associated with the specified equippable item.
        /// </summary>
        /// <param name="equippableItem">The item of the transform to get.</param>
        /// <param name="equippableTransfrom">
        /// When this method returns, contains the weapon transform associated with the specified equippable item, if the equippable item is found.
        /// Otherwise, null. 
        /// This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// True if the equippable item has an created transform.
        /// Otherwise, false.
        /// </returns>
        public bool TryGetItemTransform(EquippableItem equippableItem, out Transform equippableTransfrom)
        {
            return equippableObjectsHash.TryGetValue(equippableItem, out equippableTransfrom);
        }
        #endregion
    }
}