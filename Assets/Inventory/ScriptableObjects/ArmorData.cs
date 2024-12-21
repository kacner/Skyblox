using UnityEngine;
[System.Serializable]
public enum ArmorType
{
    None,
    Helmet,
    Chestplate,
    Leggings,
    Boots
}

[CreateAssetMenu(fileName = "Armor Data", menuName = "Item Data/Armor", order = 53)]
public class ArmorData : ItemData
{
    [Header("Armor Properties")]
    public ArmorType armorType;
}