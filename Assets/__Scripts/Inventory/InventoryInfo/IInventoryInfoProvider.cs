
using UnityEngine;

/// <summary>
/// Данный интерфейс реализуется объектами, которые могут предоставлять информацию об инвентаре
/// (например, имя персонажа или название хранилища).
/// Пользователи интерфейса должны подписаться на событие до установки данных, иначе потеряют
/// эти данные
/// </summary>
public interface IInventoryInfoProvider {
    public delegate void InventoryInfoChangedEventHandler(InventoryInfo newInventoryInfo);
    InventoryInfo InventoryInfo { get; }
    event InventoryInfoChangedEventHandler InventoryInfoChanged;
}