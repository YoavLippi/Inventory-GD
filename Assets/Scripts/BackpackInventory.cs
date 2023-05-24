using System;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;

public class BackpackInventory : MonoBehaviour
{
    [SerializeField] private List<item> inventory = new List<item>();

    [SerializeField] private List<ItemCounter> sortedInventory = new List<ItemCounter>();

    [SerializeField] private int maxSize = 10;

    [SerializeField] private TextMeshProUGUI capacityDisplay;

    [SerializeField] private TMP_Dropdown sortChoice;

    [SerializeField] private TMP_Dropdown filterChoice;

    public List<item> Inventory => inventory;

    public int MaxSize
    {
        get => maxSize;
        set => maxSize = value;
    }
    
    //Used to upgrade backpack
    public void UpdateSize(int newSize)
    {
        MaxSize = newSize;
        capacityDisplay.text = $"{inventory.Count}/{maxSize}";
    }
    
    public void AddItem(item newItem)
    {
        //check if the inventory is maxed out
        if (inventory.Count >= maxSize)
        {
            return;
        }
        
        inventory.Add(newItem);
        Sort();
    }

    public void RemoveItem(item targetItem)
    {
        //We will only be able to remove items that are in the inventory, so we don't need to catch any errors
        inventory.Remove(targetItem);
        Sort();
    }
    
    //These methods are duplicates of the other inventories, so are only explained here
    public void Filter()
    {
        //get the filter from the menu
        FilterTypes filterType = (FilterTypes)filterChoice.value;
        
        //Will return a list based on the filter selected
        List<item> filteredList = new List<item>();

        //Will only add to the list if the checked items have the same type
        foreach (var checkItem in inventory)
        {
            if (checkItem.ItemType == filterType)
            {
                filteredList.Add(checkItem);
            }
            else if (filterType == FilterTypes.All)
            {
                filteredList.Add(checkItem);
            }
        }

        List<ItemCounter> filterWithCount = new List<ItemCounter>();

        //doing the same thing as with a normal counting list
        foreach (var checkItem in filteredList)
        {
            bool foundFlag = false;
            
            //the changeItem is an ItemCounter, so it has an Item and a count. It's discrete, so we will add to the count if it's found within the
            //filterWithCount and save the fact that we found it. Otherwise we'll add it to the filterWithCount
            for (int i=0;i<filterWithCount.Count;i++)
            {
                var changeItem = filterWithCount[i];
                
                if (checkItem == changeItem.Item && checkItem.Stackable)
                {
                    changeItem.Count++;
                    foundFlag = true;
                }

                filterWithCount[i] = changeItem;
            }

            if (!foundFlag)
            {
                ItemCounter n = new ItemCounter();
                n.Item = checkItem;
                n.Count = 1;
                filterWithCount.Add(n);
            }
        }

        //We make the display inventory the filtered one
        sortedInventory = filterWithCount;
    }

    public void Sort()
    {
        Filter();
        
        SortDisplayInventory();
        
        //We set the text to the right amount
        foreach (var changeItem in sortedInventory)
        {
            changeItem.Item.CostDisplay.text = "";
            if (changeItem.Item.Stackable)
            {
                changeItem.Item.Text.text = "" + changeItem.Count;
            }
            else
            {
                changeItem.Item.Text.text = "";
            }
        }
        
        RenderBackpack();
    }
    
    private void RenderBackpack()
    {
        //We first destroy every instance of an Item that's currently displayed
        foreach (var Object in GetComponentsInChildren<Transform>())
        {
            if (Object.gameObject.CompareTag("Item"))
            {
                Destroy(Object.gameObject);
            }
        }

        //These are starting positions relative to the empty parent position
        float xPos = -3.5f;
        float yPos = 2.9f;
        
        foreach (var checkItem in sortedInventory)
        {
            //We make an instance of the checked item
            Instantiate(checkItem.Item.gameObject, new Vector3(transform.position.x + xPos, transform.position.y + yPos, 0f), Quaternion.identity, transform);
            
            xPos++;
            //This statement is a ticker. If the xPos is above the value in here, we move down the y by one and reset x
            if (xPos >= 4.5f)
            {
                yPos--;
                xPos = -3.5f;
            }
        }
        
        capacityDisplay.text = $"{inventory.Count}/{maxSize}";
    }

    private void SortDisplayInventory()
    {
        //We check what was selected in the sort dropdown
        switch ((SortChoice)sortChoice.value)
        {
            case SortChoice.Alphabetically :
                sortedInventory.Sort((s1, s2) => String.Compare(s1.Item.ItemName, s2.Item.ItemName, StringComparison.Ordinal));
                break;
            case SortChoice.Cost :
                sortedInventory.Sort((s1, s2) => s2.Item.Value.CompareTo(s1.Item.Value));
                break;
            case SortChoice.Type :
                sortedInventory.Sort((s1,s2) => s1.Item.ItemType.CompareTo(s2.Item.ItemType));
                break;
        }
    }
}