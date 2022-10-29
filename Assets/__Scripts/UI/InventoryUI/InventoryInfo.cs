using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryInfo : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _title;

    [SerializeField]
    private TextMeshProUGUI _subTitle;

    [SerializeField]
    private Image _image;

    public TextMeshProUGUI Title => _title;
    public TextMeshProUGUI SubTitle => _subTitle;
    public Image Image => _image;
}
