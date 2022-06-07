using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System; 


// 挂载到每个 itemUI 下
public class ItemPointerController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // 携带有 itemDictionary 的 GameObject, 比如和玩家用的背包有关，应该关联到 Player
    public GameObject gameObjectWithItemDictionary;
    private Dictionary<Item, List<Tuple<int, int>>> itemDictionary;


    private void Start() 
    {
        itemDictionary = gameObjectWithItemDictionary.GetComponent<BagUIController>().itemDictionary;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        // 在装备槽与背包中获取 item 的方式不同
        Item item = null;
        if(transform.parent.tag == "Slot")
        {
            int slotIndex = ItemUniversalMethod.FindSlotIndex(transform.parent);
            item = ItemUniversalMethod.FindItemByIndex(slotIndex, itemDictionary);  
        }
        else if(transform.parent.tag == "PlayerSlot")
        {
            string slotName = transform.parent.name;
            item = StatusUIController.Instance.equipmentDictionary[slotName];
        }

        if(item != null)
        {
            if(item.itemType == Item.ItemType.Weapon)
            {
                TipsUIController.Instance.ShowTips((Weapon)item);
            }
            else
            {
                TipsUIController.Instance.ShowTips(item);
            }
        }
        else
        {
            Debug.LogError("BUG, item不应该为空");
        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TipsUIController.Instance.HideTips();
    }

    // 右键点击更换装备
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            // 在当前是 ItemImage 的情况下， 一级 parent 是 item， 再向上一级才是 slot
            Transform currentItemUI = transform;
            Transform currentSlotUI = transform.parent;

            string currentSlotTag = currentSlotUI.tag;

            // 当前右键点击的是 背包UI 中的物品
            if(currentSlotTag == "Slot")
            {
                int currentIndex = ItemUniversalMethod.FindSlotIndex(currentSlotUI.transform);
                Item currentItem = ItemUniversalMethod.FindItemByIndex(currentIndex, itemDictionary);
                // 如果点的是装备
                if(currentItem.itemType == Item.ItemType.Weapon)
                {
                    // 装备槽 WeaponSlot中 没有物品 
                    if(StatusUIController.Instance.equipmentDictionary["WeaponSlot"] == null)
                    {
                        //SAME-----------------------------------------------------------------------------------------------------------------------------
                        Transform targetSlotUI = StatusUIController.Instance.playerUI.transform.Find("WeaponSlot");
                        Transform targetItemUI = StatusUIController.Instance.playerUI.transform.Find("WeaponSlot").GetChild(0);

                        // 把当前物品放进 目标装备槽
                        currentItemUI.SetParent(targetSlotUI);
                        currentItemUI.position = targetSlotUI.position;
                        // 把目标物品放进当前 slot
                        targetItemUI.SetParent(currentSlotUI);
                        targetItemUI.position = currentSlotUI.position;

                        // 改变 status字典 中对应格子指向的 实例的引用
                        StatusUIController.Instance.equipmentDictionary["WeaponSlot"] = currentItem;
                        // 从 bag 把该物体的实例的引用 移除出列表，但它还存在
                        Player.Instance.Bag.RemoveItem(currentItem);
                        // 把该物体的实例的引用 移除出 背包 UI 列表，但它还存在
                        itemDictionary.Remove(currentItem);
                        //SAME-----------------------------------------------------------------------------------------------------------------------------

                    }
                    // 装备槽 WeaponSlot中 有物品
                    else
                    {
                        // 目标实例 的引用
                        Item targetItem = StatusUIController.Instance.equipmentDictionary["WeaponSlot"];

                        //SAME-----------------------------------------------------------------------------------------------------------------------------
                        Transform targetSlotUI = StatusUIController.Instance.playerUI.transform.Find("WeaponSlot");
                        Transform targetItemUI = StatusUIController.Instance.playerUI.transform.Find("WeaponSlot").GetChild(0);

                        // 把当前物品放进 目标装备槽
                        currentItemUI.SetParent(targetSlotUI);
                        currentItemUI.position = targetSlotUI.position;
                        // 把目标物品放进当前 slot
                        targetItemUI.SetParent(currentSlotUI);
                        targetItemUI.position = currentSlotUI.position;

                        // 改变 status字典 中对应格子指向的 实例的引用
                        StatusUIController.Instance.equipmentDictionary["WeaponSlot"] = currentItem;
                        // 从 bag 把该物体的实例的引用 移除出列表，但它还存在
                        Player.Instance.Bag.RemoveItem(currentItem);
                        // 把该物体的实例的引用 移除出 背包 UI 列表，但它还存在
                        itemDictionary.Remove(currentItem);
                        //SAME-----------------------------------------------------------------------------------------------------------------------------



                        // 把 目标实例 的引用 加到 bag 和 bagUI 字典
                        Player.Instance.Bag.AddItem(targetItem);
                        var newList = new List<Tuple<int, int>>();
                        // 放到 当前物体 的格子里，用的是当前右键点击物体的 index
                        var newTuple = new Tuple<int, int>(currentIndex, 1);
                        newList.Add(newTuple);
                        itemDictionary.Add(targetItem, newList);
                    }
                }
            }
            // 当前右键点击的是 装备UI 中的物品
            // todo debug 拆下来的时候，背包满了怎么办
            else if(currentSlotTag == "PlayerSlot")
            {
                string currentSlotName = currentSlotUI.name;
                Item currentItem = StatusUIController.Instance.equipmentDictionary[currentSlotName];

                var bagGrid = BagUIController.Instance.bagUI.transform.GetChild(0);
                // 寻找背包 UI 中空的 slot 的 index
                int validIndex = ItemUniversalMethod.FindEmptySlot(bagGrid);
                if(validIndex == -1)
                {
                    Debug.LogError("BUG,可能是背包满了");
                }
                Item targetItem = ItemUniversalMethod.FindItemByIndex(validIndex, itemDictionary);
                Transform targetSlotUI = bagGrid.GetChild(validIndex);
                Transform targetItemUI = targetSlotUI.GetChild(0);

                // 把当前装备物品放进 背包 目标slot
                currentItemUI.SetParent(targetSlotUI);
                currentItemUI.position = targetSlotUI.position;
                // 把 背包item 放到 当前点击的 装备slot
                targetItemUI.SetParent(currentSlotUI);
                targetItemUI.position = currentSlotUI.position;

                // 把实例的引用 加到 bag 和 bagUI 字典
                Player.Instance.Bag.AddItem(currentItem);
                var newList = new List<Tuple<int, int>>();
                var newTuple = new Tuple<int, int>(validIndex, 1);
                newList.Add(newTuple);
                itemDictionary.Add(currentItem, newList);

                // 从 装备字典中移除
                StatusUIController.Instance.equipmentDictionary[currentSlotName] = null;
            }
            
            
            
            // 只要有右键点击，最后都要刷新 状态 && UI
            Player.Instance.RefreshStatus();
        } 
    }
}
