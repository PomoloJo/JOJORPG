using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


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




    // 物体被鼠标指到时高亮，以及被鼠标点击时触发人物的自动拾取
    private void OnMouseDown() 
    {
        // 应该先给人物加个存活状态，用来判断人物死没死，暂时先看看它是不是 Null
        if(Player.Instance)
        {          
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // 点到了UI, 所以直接结束
                return;
            }
            else
            {
                var pickupController = Player.Instance.GetComponent<PickupController>();
                pickupController.pickupTarget = gameObject;
                pickupController.isWalking = true;
            }
        }
    }



}
