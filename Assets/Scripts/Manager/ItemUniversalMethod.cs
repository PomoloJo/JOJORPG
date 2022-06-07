using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using System;


// 挂载到每个 item 下
public class ItemUniversalMethod
{
    public static int MatchNumInString(string str)
    {
        // 匹配1位或多位的整数数字 
        string numberString = Regex.Replace(str, @"[^0-9+]", "");
        int numberInt = int.Parse(numberString);
        return numberInt;
    }

    public static int FindSlotIndex(Transform slotUI)
    {
        if(slotUI == null)
        {
            Debug.LogError("slotUI 不应该为空");
        }
        int index = MatchNumInString(slotUI.name);
        return index;
    }

    public static Item FindItemByIndex(int index, Dictionary<Item, List<Tuple<int, int>>> itemDictionary)
    {
        // Linq 方法 get first key， 通过 value 找 key
        Item item = itemDictionary.FirstOrDefault(q => q.Value[0].Item1 == index).Key;
        return item;
    }

    // 返回枚举类型的元素个数
    public static int GetEnumElementLength<T>() where T : new()
    {
        T e = new T();
        int length = System.Enum.GetNames(e.GetType()).Length;
        return length;
    }

    // 寻找容器 UI 中空的 slot 对应的 index
    public static int FindEmptySlot(Transform container)
    {
        for(int i = 0; i < container.childCount; i++)
        {
            // 先 get 到 slot， 再 get 到 item
            if(container.GetChild(i).transform.GetChild(0).gameObject.activeSelf == false)
            {
                return i;
            }
        }
        // 满了 或 找不到
        return -1;
    }

    public static Color GetQualityColor(Item.Quality q)
    {
        switch((int)q)
        {
            case 0: return Color.gray;
            case 1: return Color.white;
            case 2: return Color.green;
            case 3: return new Color(0, 0.7f, 1);
            case 4: return new Color(1, 0, 1);
            case 5: return new Color(1, 1, 0);
            case 6: return Color.red;
            default: return Color.gray;
        }
    }

}

