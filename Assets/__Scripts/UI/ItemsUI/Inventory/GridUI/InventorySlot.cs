using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Слот в сетке инвентаря
/// </summary>
public class InventorySlot : MonoBehaviour
{
    private Image _image;
    
    private void Awake() {
        _image = GetComponent<Image>();
    }
}
