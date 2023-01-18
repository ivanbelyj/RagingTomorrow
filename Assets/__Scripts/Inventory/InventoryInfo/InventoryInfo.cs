using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryInfo
{
    [SerializeField]
    private Sprite _avatar;
    [SerializeField]
    private string _title;
    [SerializeField]
    private string _subTitle;

    public Sprite Avatar { get => _avatar; }
    public string Title { get => _title; }
    public string SubTitle { get => _subTitle; }
    public InventoryInfo(Sprite avatar, string title, string subTitle) {
        _avatar = avatar;
        _title = title;
        _subTitle = subTitle;
    }
}
