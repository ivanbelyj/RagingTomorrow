using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Информация об инвентаре. Например, может содержать имя владельца.
/// </summary>
public class InventoryInfo : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _title;

    [SerializeField]
    private TextMeshProUGUI _subTitle;

    [SerializeField]
    private Image _image;

    public string Title {
        get => _title.text;
        set {
            _title.text = value;
        }
    }
    public string SubTitle {
        get => _subTitle.text;
        set {
            _subTitle.text = value;
        }
    }
    public Image Image => _image;
}
