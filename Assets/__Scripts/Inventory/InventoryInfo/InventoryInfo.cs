using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryInfo
{
    public Sprite Avatar { get; private set; }
    public string Title { get; private set; }
    public string SubTitle { get; private set; }
    // public float TotalWeight { get; private set; }
    public InventoryInfo(Sprite avatar, string title, string subTitle/*, float totalWeight*/) {
        Avatar = avatar;
        Title = title;
        SubTitle = subTitle;
        // TotalWeight = totalWeight;
    }
}
