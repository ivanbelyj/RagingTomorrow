using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField]
    private Image _image;
    // public int Row { get; set; }
    // public int Col { get; set; }
    // public Image Image {
    //     get => _image;
    //     set => _image = value;
    // }

    public void SetSprite(Sprite sprite) {
        _image.gameObject.SetActive(sprite is not null);
        _image.sprite = sprite;
    }
}
