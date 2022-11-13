using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Todo: сделать readonly
/// <summary>
/// Предмет, хранимый в инвентаре
/// </summary>
[System.Serializable]
public class InventoryItem
{
    // public int inventoryX;
    // public int inventoryY;

    /// <summary>
    /// Каждый предмет инвентаря относится к его определенной секции.
    /// Хранение данного поля необходимо для обеспечения функциональности секций,
    /// а также для отображения предметов инвентаря в разных секциях UI.
    /// </summary>
    // public string inventorySection;

    /// <summary>
    /// С точки зрения инвентаря несколько одинаковых предметов (сколько именно, зависит от
    /// предмета) представлены одним объектом
    /// </summary>
    public int count;

    public ItemData itemGameData;

    /// <summary>
    /// Для обращения к любому конкретному элементу в инвентаре требуется id.
    /// </summary>
    public Guid id;
}
