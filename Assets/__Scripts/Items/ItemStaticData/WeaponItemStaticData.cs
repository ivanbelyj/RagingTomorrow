using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WeaponItemData",
    menuName = "Item Data/Weapon Item Data",
    order = 51)]
public class WeaponItemStaticData : ItemStaticData
{
    // Пока что enum нужен лишь для логического разделения оружия, чтобы можно было определить,
    // что поместить в слот пистолета, а что - винтовки
    public enum WeaponType {
        Rifle,
        HandGun
    }

    [SerializeField]
    private WeaponType _Type;
    public WeaponType Type => _Type;
}
