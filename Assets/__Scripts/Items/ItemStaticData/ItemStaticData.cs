using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Неизменные данные о предмете, который может храниться в инвентаре. Вместо того, чтобы
/// передавать их все по сети для синхронизации, можно передать лишь имя ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "New ItemData", menuName = "Item Data/Item Data", order = 51)]
public class ItemStaticData : ScriptableObject
{
    #region Private fields
    [SerializeField]
    private GameObject _itemPrefab;

    [SerializeField]
    private Sprite _icon;

    // Размеры иконки (слоты инвентаря)
    [SerializeField]
    private int _width;

    [SerializeField]
    private int _height;

    [SerializeField]
    private int _stackSize;

    [SerializeField]
    private float _mass;

    [SerializeField]
    private string _name;

    [SerializeField]
    private string _description;
    #endregion

    public GameObject ItemPrefab => _itemPrefab;
    public Sprite Sprite => _icon;
    public int Width { get => _width; }
    public int Height { get => _height; }
    public int StackSize {
        get => _stackSize;
        set {
            if (value < 1)
                Debug.LogError("Incorrect stack size for item");
            _stackSize = value;
        }
    }
    public float Mass { get => _mass; }
    public string ItemName => _name;
    public string Description => _description;
}
