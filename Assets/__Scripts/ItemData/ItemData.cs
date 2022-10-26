using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Данные о предмете, который может храниться в инвентаре
/// </summary>
[CreateAssetMenu(fileName = "New ItemData", menuName = "Item Data/Item Data", order = 51)]
public class ItemData : ScriptableObject
{
    #region Private fields
    // координаты иконки в атласе иконок
    // [SerializeField]
    // private float _iconX;

    // [SerializeField]
    // private float _iconY;
    [SerializeField]
    private Item _item;

    [SerializeField]
    private Sprite _icon;

    // размер иконки в плитках интерфейса инвентаря
    [SerializeField]
    private int _width;

    [SerializeField]
    private int _height;

    [SerializeField]
    private float _mass;

    [SerializeField]
    private string _name;

    [SerializeField]
    private string _description;
    #endregion

    public Item Item => _item;
    public Sprite Sprite => _icon;
    public int Width { get => _width; }
    public int Height { get => _height; }
    public float Mass { get => _mass; }
    public string ItemName => _name;
    public string Description => _description;

}
