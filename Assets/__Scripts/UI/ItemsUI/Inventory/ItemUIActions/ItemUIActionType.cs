using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Тип действия, которое происходит при осуществлении пользователем
/// ввода определенного типа
/// </summary>
public enum ItemUIActionType
{
    MoveToOtherInventory,  // Например, когда открыт другой инвентарь, double click осуществляет перенос
    MoveToWearSection,  // Предмет отправляется на тело игрока
    // Также может быть использование предмета
}
