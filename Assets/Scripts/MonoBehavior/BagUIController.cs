using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


// 专门管理背包 UI 的内容，挂载到了 Player 下
public class BagUIController : MonoBehaviour
{
    private static BagUIController _instance;
    public static BagUIController Instance
    {
        get{return _instance;}
    }
    private void Awake() 
    {
        _instance = this;
    }


    public GameObject bagUI;
    
    // 存放物品在 背包UI 中的数量，存放用于显示在哪个格子的索引
    public Dictionary<Item, List<Tuple<int, int>>> itemDictionary;

    private Transform bagGrid;

    // 由 Player 在 start 时传入
    private Inventory bag;
    public void SetBag(Inventory inventory)
    {
        bag = inventory;
    }

    // Start is called before the first frame update
    void Start()
    {
        // List<Tuple<int, int>> ItemStack
        // 对于 unique 物品，list中只有一个 tuple， shared 的可能会有多个， 代表其在背包中被拆分到了多个格子里
        // Tuple<int, int> item1 索引， item2 数量
        itemDictionary = new Dictionary<Item, List<Tuple<int, int>>>();
        bagGrid = bagUI.transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        OpenAndCloseBag();
    }

    public void OpenAndCloseBag()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            TipsUIController.Instance.HideTips();
            bagUI.SetActive(!bagUI.activeSelf);
        }
    }


    // 根据给定的 index、quantity、item 刷新格子
    private void RefreshSlot(int slotIndex, int quantity, Item item)
    {
        Transform slot = bagGrid.GetChild(slotIndex);
        GameObject itemUI = slot.GetChild(0).gameObject;
        itemUI.SetActive(true);

        Image itemBackgroundImage = itemUI.transform.GetChild(0).GetComponent<Image>();
        Image itemImage = itemUI.transform.GetChild(1).GetComponent<Image>();
        Image quantityImage = itemUI.transform.GetChild(2).GetComponent<Image>();
        TextMeshProUGUI quantityText = itemUI.transform.GetChild(3).GetComponent<TextMeshProUGUI>();

        itemImage.sprite = item.gameObject.GetComponentInParent<SpriteRenderer>().sprite;
        itemBackgroundImage.color = item.qualityColor;
        
        if(item.dataType == Item.DataType.Shared)
        {
            quantityImage.gameObject.SetActive(true);
            quantityText.text = quantity.ToString();
            quantityText.gameObject.SetActive(true);
        }
        else if(item.dataType == Item.DataType.Unique)
        {
            quantityImage.gameObject.SetActive(false);
            quantityText.gameObject.SetActive(false);
        }
    }

    // bug, 没有判断背包满了会怎么样
    public void RefreshBagItems()
    {
        // 遍历背包中的每一件物品
        foreach(Item item in bag.ItemList)
        {
            List<Tuple<int, int>> itemStack = null;
            // 在字典中查找此物品在背包 UI 中的 List<Tuple<int, int>> itemStack, 再从 item1 值获得索引， item2 值获得数量
            if(itemDictionary.ContainsKey(item))
            {
                itemStack = itemDictionary[item];
                foreach(var tuple in itemStack)
                {
                    RefreshSlot(tuple.Item1, tuple.Item2, item);
                }
            }
            // 如果没有, 给它创建一个新的 pair
            // 遍历背包 UI 中的 empty slot，给它找个空的位置添加上
            else
            {
                int index = ItemUniversalMethod.FindEmptySlot(bagGrid);
                itemStack = new List<Tuple<int, int>>();
                itemStack.Add(new Tuple<int, int>(index, 1));
                itemDictionary[item] = itemStack;
                RefreshSlot(itemStack[0].Item1, itemStack[0].Item2, item);
            }
            
            // // -1直接返回
            // if(index == -1)
            // {
            //     Debug.LogError("有bug,可能是背包满了");
            //     return;
            // }
            // // 只要不为 -1, 就是找到了，更新显示它的 slot 编号，并更新到字典中
            // itemDictionary[item] = index;
            // // 根据 index 刷新格子
            // RefreshSlot(index, item);
        }
    }

}
