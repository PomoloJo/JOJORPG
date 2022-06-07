using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;
using System.Linq;
using System;

public class ItemDragController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    // 携带有 itemDictionary 的 GameObject, 比如和玩家用的背包有关，应该关联到 Player
    public GameObject gameObjectWithItemDictionary;
    private Dictionary<Item, List<Tuple<int, int>>> itemDictionary;

    // 用于记录原始父级, 里面已经包含了原始位置
    private Transform originalParent;

    // 鼠标点击时的 index 和 item，不是 itemUI
    int originalIndex;
    Item originalItem;



    private void Start() 
    {
        itemDictionary = gameObjectWithItemDictionary.GetComponent<BagUIController>().itemDictionary;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 记录原始父级 SlotUI
        originalParent = transform.parent;
        // 提级，避免遮挡
        transform.SetParent(transform.parent.parent.parent);
        // 开启射线穿透
        GetComponent<CanvasGroup>().blocksRaycasts = false;

        // 先确定一开始点击时是点在了 PlayerSlot 还是 Slot
        // 然后找到一开始点击时对应的 item 和 index
        if(originalParent.tag == "PlayerSlot")
        {
            originalIndex = -1;
            originalItem = StatusUIController.Instance.equipmentDictionary[originalParent.name];
        }
        else if(originalParent.tag == "Slot")
        {
            originalIndex = ItemUniversalMethod.FindSlotIndex(originalParent.transform);
            originalItem = ItemUniversalMethod.FindItemByIndex(originalIndex, itemDictionary);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        
        // if(eventData.pointerCurrentRaycast.gameObject)
        //     Debug.Log(eventData.pointerCurrentRaycast.gameObject.tag + ", " + eventData.pointerCurrentRaycast.gameObject.name);
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject pointerCurrentRaycastObject = eventData.pointerCurrentRaycast.gameObject;
        // 若位置错误，扔回原来的位置
        if(pointerCurrentRaycastObject == null)
        {
            transform.SetParent(originalParent);
            transform.position = originalParent.position;
        }
        else
        {
            Transform currentSlotUI;
            Transform currentItemUI;

            // 情况1：物品 移动到 空槽
            if(pointerCurrentRaycastObject.name != "ItemImage")
            {
                Debug.Log("case1");

                // 情况1-1：背包物品 到 背包空槽
                if(originalParent.tag == "Slot" && pointerCurrentRaycastObject.tag == "Slot")
                {
                    Debug.Log("case1-1");
                    // 不能属于原始位置，因为原始位置重复放置会出bug
                    if(ItemUniversalMethod.FindSlotIndex(pointerCurrentRaycastObject.transform) != originalIndex)
                    {
                        Debug.Log("case1-1-1");
                        currentSlotUI = pointerCurrentRaycastObject.transform;
                        currentItemUI = currentSlotUI.GetChild(0);
                        ExchangeItemUI(currentSlotUI, currentItemUI);
                        // 修改 itemDictionary 里的 item 和 index 对应关系
                        itemDictionary[originalItem][0] = Tuple.Create(ItemUniversalMethod.FindSlotIndex(currentSlotUI), itemDictionary[originalItem][0].Item2);
                    }
                    else
                    {
                        Debug.Log("case1-1-0");
                        transform.SetParent(originalParent);
                        transform.position = originalParent.position;
                    }
                }

                // 情况1-2：背包物品 到 装备空槽
                else if(originalParent.tag == "Slot" && pointerCurrentRaycastObject.tag == "PlayerSlot")
                {
                    Debug.Log("case1-2");
                    // 暂时只判断 武器 的移动
                    if(originalItem.itemType == Item.ItemType.Weapon && (pointerCurrentRaycastObject.name == "WeaponSlot" || pointerCurrentRaycastObject.name == "OffhandWeaponSlot"))
                    {
                        Debug.Log("case1-2-1");
                        currentSlotUI = pointerCurrentRaycastObject.transform;
                        currentItemUI = currentSlotUI.GetChild(0);
                        ExchangeItemUI(currentSlotUI, currentItemUI);

                        // 把实例的引用 加到 status字典
                        StatusUIController.Instance.equipmentDictionary[pointerCurrentRaycastObject.name] = originalItem;
                        // 从 bag 把该物体的实例的引用 移除出列表，但它还存在
                        Player.Instance.Bag.RemoveItem(originalItem);
                        // 把该物体的实例的引用 移除出 背包 UI 列表，但它还存在
                        itemDictionary.Remove(originalItem);

                        // 存在装备变更，刷新 状态 && UI
                        Player.Instance.RefreshStatus();
                    }
                    else
                    {
                        Debug.Log("case1-2-0");
                        transform.SetParent(originalParent);
                        transform.position = originalParent.position;
                    }
                }

                // 情况1-3：装备物品 到 背包空槽
                else if(originalParent.tag == "PlayerSlot" && pointerCurrentRaycastObject.tag == "Slot")
                {
                    Debug.Log("case1-3");
                    currentSlotUI = pointerCurrentRaycastObject.transform;
                    currentItemUI = currentSlotUI.GetChild(0);
                    ExchangeItemUI(currentSlotUI, currentItemUI);

                    // 把实例的引用 加到 bag 和 bagUI 字典
                    Player.Instance.Bag.AddItem(originalItem);
                    var newList = new List<Tuple<int, int>>();
                    var newTuple = new Tuple<int, int>(ItemUniversalMethod.FindSlotIndex(currentSlotUI), 1);
                    newList.Add(newTuple);
                    itemDictionary.Add(originalItem, newList);

                    // 从 装备字典中移除
                    StatusUIController.Instance.equipmentDictionary[originalParent.name] = null;

                    // 存在装备变更，刷新 状态 && UI
                    Player.Instance.RefreshStatus();
                }

                // 情况1-4：装备物品 到 装备空槽
                // todo debug 要判断背包满不满
                else if(originalParent.tag == "PlayerSlot" && pointerCurrentRaycastObject.tag == "PlayerSlot")
                {
                    Debug.Log("case1-4");

                    // 不能属于原始位置，因为原始位置重复放置会出bug
                    if(originalParent.name == pointerCurrentRaycastObject.name)
                    {
                        Debug.Log("case1-4-0");
                        transform.SetParent(originalParent);
                        transform.position = originalParent.position;
                    }
                    // 装备物品 到 装备空槽 实际上只涉及 武器 与 副手武器的切换，其他部位都不能切换
                    else if(pointerCurrentRaycastObject.name == "OffhandWeaponSlot" || pointerCurrentRaycastObject.name == "WeaponSlot")
                    {
                        Debug.Log("case1-4-1");              
                        currentSlotUI = pointerCurrentRaycastObject.transform;
                        currentItemUI = currentSlotUI.GetChild(0);
                        ExchangeItemUI(currentSlotUI, currentItemUI);
                        // 把实例的引用 加到 status字典, 字典原位置置空
                        StatusUIController.Instance.equipmentDictionary[pointerCurrentRaycastObject.name] = originalItem;
                        StatusUIController.Instance.equipmentDictionary[originalParent.name] = null;

                        // 存在装备变更，刷新 状态 && UI
                        Player.Instance.RefreshStatus();
                    }
                    else
                    {
                        Debug.Log("case1-4-0");
                        transform.SetParent(originalParent);
                        transform.position = originalParent.position;
                    }
                }
                else
                {
                    Debug.Log("case1-0");
                    transform.SetParent(originalParent);
                    transform.position = originalParent.position;
                }
            }
            // 情况2：物品 与 物品 交换
            else if(pointerCurrentRaycastObject.name == "ItemImage")
            {
                Debug.Log("case2");

                // 情况2-1：背包物品 到 背包物品
                if(originalParent.tag == "Slot" && pointerCurrentRaycastObject.transform.parent.parent.tag == "Slot")
                {
                    Debug.Log("case2-1");
                    // 在当前是 ItemImage 的情况下， 一级 parent 是 item， 再向上一级才是 slot
                    currentItemUI = pointerCurrentRaycastObject.transform.parent;
                    currentSlotUI = currentItemUI.parent;
                    ExchangeItemUI(currentSlotUI, currentItemUI);

                    // 交换 itemDictionary 里的一对 item:index
                    int currentIndex = ItemUniversalMethod.FindSlotIndex(currentSlotUI);
                    Item currentItem = ItemUniversalMethod.FindItemByIndex(currentIndex, itemDictionary);
                    itemDictionary[originalItem][0] = Tuple.Create(currentIndex, itemDictionary[originalItem][0].Item2);
                    itemDictionary[currentItem][0] = Tuple.Create(originalIndex, itemDictionary[currentItem][0].Item2);
                }

                // 情况2-2：背包物品 到 装备物品
                else if(originalParent.tag == "Slot" && pointerCurrentRaycastObject.transform.parent.parent.tag == "PlayerSlot")
                {
                    Debug.Log("case2-2");

                    // 暂时只判断 武器 的移动
                    string currentSlotName = pointerCurrentRaycastObject.transform.parent.parent.name;
                    if(originalItem.itemType == Item.ItemType.Weapon && (currentSlotName == "WeaponSlot" || currentSlotName == "OffhandWeaponSlot"))
                    {
                        Debug.Log("case2-2-1");

                        // 鼠标释放时指向的 目标实例 的引用
                        Item currentItem = StatusUIController.Instance.equipmentDictionary[currentSlotName];

                        // 在当前是 ItemImage 的情况下， 一级 parent 是 itemUI， 再向上一级才是 slotUI
                        currentItemUI = pointerCurrentRaycastObject.transform.parent;
                        currentSlotUI = currentItemUI.parent;
                        ExchangeItemUI(currentSlotUI, currentItemUI);

                        // 改变 status字典 中对应格子指向的 实例的引用
                        StatusUIController.Instance.equipmentDictionary[currentSlotName] = originalItem;
                        // 从 bag 把该物体的实例的引用 移除出列表，但它还存在
                        Player.Instance.Bag.RemoveItem(originalItem);
                        // 把该物体的实例的引用 移除出 背包 UI 列表，但它还存在
                        itemDictionary.Remove(originalItem);

                        // 把 鼠标释放时指向的 目标实例 的引用 加到 bag 和 bagUI 字典
                        Player.Instance.Bag.AddItem(currentItem);
                        var newList = new List<Tuple<int, int>>();
                        // 放到 原物体 的格子里，用的是原来的 index 
                        var newTuple = new Tuple<int, int>(originalIndex, 1);
                        newList.Add(newTuple);
                        itemDictionary.Add(currentItem, newList);

                        // 存在装备变更，刷新 状态 && UI
                        Player.Instance.RefreshStatus();
                    }
                    else
                    {
                        Debug.Log("case2-2-0");
                        transform.SetParent(originalParent);
                        transform.position = originalParent.position;
                    }
                }

                // 情况2-3：装备物品 到 背包物品
                else if(originalParent.tag == "PlayerSlot" && pointerCurrentRaycastObject.transform.parent.parent.tag == "Slot")
                {
                    Debug.Log("case2-3");

                    // 此情况无效，换装应该从背包往装备栏去拖动
                    transform.SetParent(originalParent);
                    transform.position = originalParent.position;
                }

                // 情况2-4：装备物品 到 装备物品
                else if(originalParent.tag == "PlayerSlot" && pointerCurrentRaycastObject.transform.parent.parent.tag == "PlayerSlot")
                {
                    Debug.Log("case2-4");
                    string currentSlotName = pointerCurrentRaycastObject.transform.parent.parent.name;
                    // 装备物品 到 装备物品 实际上只涉及 武器 与 副手武器的切换，其他部位都不能切换
                    if(currentSlotName == "WeaponSlot" || currentSlotName == "OffhandWeaponSlot")
                    {
                        Debug.Log("case2-4-1");
                         // 鼠标释放时指向的 目标实例 的引用
                        Item currentItem = StatusUIController.Instance.equipmentDictionary[currentSlotName];

                        // 在当前是 ItemImage 的情况下， 一级 parent 是 itemUI， 再向上一级才是 slotUI
                        currentItemUI = pointerCurrentRaycastObject.transform.parent;
                        currentSlotUI = currentItemUI.parent;
                        ExchangeItemUI(currentSlotUI, currentItemUI);

                        // 交换 武器 与 副手武器 在 装备字典 中的值
                        StatusUIController.Instance.equipmentDictionary[currentSlotName] = originalItem;
                        StatusUIController.Instance.equipmentDictionary[originalParent.name] = currentItem;

                        // 存在装备变更，刷新 状态 && UI
                        Player.Instance.RefreshStatus();
                    }
                }

                else
                {
                    Debug.Log("case2-0");
                    transform.SetParent(originalParent);
                    transform.position = originalParent.position;
                }
            }
         
            // 其他情况 都不符合规则，都放回原来的位置
            else
            {
                Debug.Log("case0");
                transform.SetParent(originalParent);
                transform.position = originalParent.position;
            }
        }
        // 最终无论结果都应该重新打开射线检测
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }


    public void ExchangeItemUI(Transform currentSlotUI, Transform currentItemUI)
    {
        // 把当前的放进原 slot
        currentItemUI.SetParent(originalParent);
        currentItemUI.position = originalParent.position;
        // 把拖拽物品放进当前 slot
        transform.SetParent(currentSlotUI);
        transform.position = currentSlotUI.position;
    }

}
