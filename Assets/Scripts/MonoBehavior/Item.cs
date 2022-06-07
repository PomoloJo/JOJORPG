using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum DataType
    {
        Shared,
        Unique
    }

    public enum TagType
    {
        装备,
        物品,
        任务道具
    }

    public enum ItemType
    {
        // Unique
        Weapon,
        Armor,

        // Shared
        Coin,
        RedPotion,
        PurplePotion
    }

    public enum Quality
    {
        破损, //Broken,     // gray
        普通, //Ordinary,   // white
        精良, //Delicate,   // green
        卓越, //Excellent,  // blue
        史诗, //Epic,       // purple
        传说, //Legendary,  // orange
        神话, //Mythical    // red
    }    

    
    public DataType dataType;
    public TagType tagType;
    public ItemType itemType;


    public string itemName;

    public Quality quality;
    public Color qualityColor;

    [TextArea]
    public string description;
    public bool isStackable = true;
    public int quantity = 1;
}
