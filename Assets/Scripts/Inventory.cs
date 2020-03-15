using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory
{
    public List<Item> itemList = new List<Item>();

    public void AddItem(Item item)
    {
        itemList.Add(item);
    }
}