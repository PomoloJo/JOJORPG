using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private List<Item> itemList;
    public List<Item> ItemList
    {
        get{return itemList;}
    }

    public Inventory()
    {
        itemList = new List<Item>();    
    }

    public void AddItem(Item item)
    {
        itemList.Add(item);
    }
    
    public void RemoveItem(Item item)
    {
        itemList.Remove(item);
    }
}
