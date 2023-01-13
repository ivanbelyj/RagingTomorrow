using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Какая-либо сущность, которая может детерминированно последовательно предоставлять предметы
/// (например, секции инвентаря)
/// </summary>
public interface IItemsProvider
{
    /// <summary>
    /// Возвращает все предметы в качестве результата и удаляет их из хранилища
    /// </summary>
    // IEnumerable<ItemData> TakeAllItems();
    
    /// <summary>
    /// В случае, если получатель не смог принять какой-либо предмет
    /// (например, из-за переполнения инвентаря), то предоставитель может принять предмет обратно
    /// </summary>
    // void TakeBack(ItemData itemData);

    /// <summary>
    /// Получателю может потребоваться "посмотреть" на предмет. Перед тем, как забрать,
    /// он может проверить, возможно ли это. null, если больше элементов в хранилище нет
    /// </summary>
    IInventoryItem PeekNext();

    /// <summary>
    /// Возвращает очередной предмет в качестве результата и удаляет его из хранилища.
    /// null, если больше элементов в хранилище нет
    /// </summary>
    IInventoryItem RemoveLastPeekedItem();
}
