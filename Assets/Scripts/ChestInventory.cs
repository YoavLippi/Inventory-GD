using System;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;

public class ChestInventory : MonoBehaviour
{
    [SerializeField] private List<item> inventory = new List<item>();

    [SerializeField] private List<ItemCounter> sortedInventory = new List<ItemCounter>();

    private bool hasMax = true;

    [SerializeField] private int maxSize;
    
    [SerializeField] private TextMeshProUGUI capacityDisplay;
    
    [SerializeField] private TMP_Dropdown sortChoice;

    [SerializeField] private TMP_Dropdown filterChoice;
    
    public List<item> Inventory => inventory;
    
    public bool HasMax
    {
        get => hasMax;
        set => hasMax = value;
    }

    public int MaxSize
    {
        get => maxSize;
        set => maxSize = value;
    }

    public void UpdateSize(int newSize)
    {
        MaxSize = newSize;
        //hasMax is changed in the main Handler
        if (hasMax)
        {
            capacityDisplay.text = $"{inventory.Count}/{maxSize}";
        }
        else
        {
            //\u22IE is an infinity symbol
            capacityDisplay.text = $"{inventory.Count}/\u221E";
        }
    }

    public void AddItem(item newItem)
    {
        if (inventory.Count >= maxSize && hasMax)
        {
        }
        else
        {
            inventory.Add(newItem);
            Sort();
        }
    }

    public void RemoveItem(item targetItem)
    {
        inventory.Remove(targetItem);
        Sort();
    }
    
    public void Filter()
    {
        FilterTypes filterType = (FilterTypes)filterChoice.value;
        List<item> filteredList = new List<item>();

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

        foreach (var checkItem in filteredList)
        {
            bool foundFlag = false;
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

        sortedInventory = filterWithCount;
    }

    public void Sort()
    {
        Filter();
        
        SortDisplayInventory();
        
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
        
        RenderChest();
    }
    
    private void RenderChest()
    {
        foreach (var Object in GetComponentsInChildren<Transform>())
        {
            if (Object.gameObject.CompareTag("Item"))
            {
                Destroy(Object.gameObject);
            }
        }

        float xPos = -3.5f;
        float yPos = 2.9f;
        foreach (var checkItem in sortedInventory)
        {
            Instantiate(checkItem.Item.gameObject,
                new Vector3(transform.position.x + xPos, transform.position.y + yPos, 0f), Quaternion.identity,
                transform);
            xPos++;
            if (xPos >= 4.5f)
            {
                yPos--;
                xPos = -3.5f;
            }
        }
        
        if (hasMax)
        {
            capacityDisplay.text = $"{inventory.Count}/{maxSize}";
        }
        else
        {
            capacityDisplay.text = $"{inventory.Count}/\u221E";
        }
    }
    
    private void SortDisplayInventory()
    {
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