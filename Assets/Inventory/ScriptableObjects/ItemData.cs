using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[System.Serializable]
public enum Rarity
{
    None,
    Common,
    Uncommon,
    Rare,
    Epic,
    Ledgendairy
}
[CreateAssetMenu(fileName = "Item Data", menuName = "Item Data", order = 50)]
public class ItemData : ScriptableObject
{
    public string itemName = "Item Name";
    public Sprite icon;
    public Rarity Rarity;
}
