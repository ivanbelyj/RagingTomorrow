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
    // public float TotalWeight { get; private set; }
    public InventoryInfo(Sprite avatar, string title, string subTitle/*, float totalWeight*/) {
        _avatar = avatar;
        _title = title;
        _subTitle = subTitle;
        // TotalWeight = totalWeight;
    }
}
