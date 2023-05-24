using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    [Header("Money Handlers")]
    [SerializeField] private int moneyAmount = 100;
    
    [SerializeField] private TextMeshProUGUI moneyDisplay;

    [Header("Inventories link")]
    [SerializeField] private BackpackInventory Backpack;

    [SerializeField] private ShopInventory Shop;

    [SerializeField] private ChestInventory Chest;
    
    [SerializeField] private GameObject leftInventory, rightInventory;
    
    [Header("Item handling")]
    [SerializeField] private List<item> allItems;

    [Header("Upgrades")]
    [SerializeField] private Button chestUp;

    [SerializeField] private Button backUp;

    [SerializeField] private int numTimesClickedC = 0, numTimesClickedB = 0;


    public GameObject LeftInventory
    {
        get => leftInventory;
        set => leftInventory = value;
    }

    public GameObject RightInventory
    {
        get => rightInventory;
        set => rightInventory = value;
    }
    
    private void Start()
    {
        populateShop();
        StartCoroutine(ShopAddition());
    }

    //Adds a new random stackable item to the shop every 7 seconds
    private IEnumerator ShopAddition()
    {
        while (true)
        {
            yield return new WaitForSeconds(7f);
            item[] tempItems = new item[allItems.Count];
            int index = 0;
            foreach (var item in allItems)
            {
                if (item.Stackable)
                {
                    tempItems[index] = item;
                    index++;
                }
            }

            Shop.AddItem(tempItems[Random.Range(0, index)]);
        }
    }

    public void UpgradeChest()
    {
        //Check to make sure it's been clicked less than 3 times -- could be a bool value instead (bool maxed etc)
        if (numTimesClickedC < 3)
        {
            //Check if the player has enough money
            int oldCost = 10 + 5 * numTimesClickedC;
            if (oldCost <= moneyAmount)
            {
                moneyAmount -= oldCost;
                moneyDisplay.text = "" + moneyAmount;
                numTimesClickedC++;
                //Will increase by 5 each time
                int newCost = 10 + 5 * numTimesClickedC;
                chestUp.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = $"Upgrade Chest ({newCost})";
                
                switch (numTimesClickedC)
                {
                    case 1:
                        Chest.UpdateSize(25);
                        break;
                    case 2:
                        Chest.UpdateSize(50);
                        break;
                    case 3:
                        //Making the chest have no maximum
                        Chest.HasMax = false;
                        Chest.UpdateSize(-1);
                        chestUp.gameObject.GetComponent<Image>().color = Color.red;
                        chestUp.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Chest Maxed";
                        chestUp.interactable = false;
                        break;
                }
            }
        }
    }

    public void UpgradeBack()
    {
        //checking number of times clicked again
        if (numTimesClickedB < 3)
        {
            int oldCost = 10 + 2 * numTimesClickedB;
            if (oldCost <= moneyAmount)
            {
                moneyAmount -= oldCost;
                moneyDisplay.text = "" + moneyAmount;
                numTimesClickedB++;
                int newCost = 10 + 2 * numTimesClickedB;
                backUp.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = $"Upgrade Chest ({newCost})";
                
                switch (numTimesClickedB)
                {
                    case 1:
                        Backpack.UpdateSize(15);
                        break;
                    case 2:
                        Backpack.UpdateSize(25);
                        break;
                    case 3:
                        Backpack.UpdateSize(50);
                        backUp.gameObject.GetComponent<Image>().color = Color.red;
                        backUp.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Backpack Maxed";
                        backUp.interactable = false;
                        break;
                }
            }
        }
    }

    private void populateShop()
    {
        foreach (var originalItem in allItems)
        {
            //numItems will inform the loop how many times it'll run
            int numItems = 0;
            switch (originalItem.ItemType)
            {
                case FilterTypes.Consumable:
                    numItems = 10;
                    break;
                case FilterTypes.Armor:
                case FilterTypes.Weapon:
                    numItems = 2;
                    break;
            }

            if (originalItem.IsUnique)
            {
                numItems = 1;
            }

            for (int i = 0; i < numItems; i++)
            {
                //setting the main so it can access its methods within the item
                originalItem.Main = GetComponent<GameHandler>();
                Shop.AddItem(originalItem);
            }
        }
        //Setting the right colors -- they'll all be green by default
        Shop.ColorCosts(moneyAmount);
    }

    public void moveItem(GameObject original, GameObject location, item movedItem)
    {
        //flag to see if the move can be done
        bool possible = false;

        //checking if the place that will be moved to is allowed
        switch (location.name)
        {
            case "Backpack":
                //if the backpack has less than the max
                if (Backpack.Inventory.Count < Backpack.MaxSize)
                {
                    possible = true;
                }

                break;
            
            case "Shop":
                //it's always possible, so we add the money back, recolor the costs based on the new money, and set possible
                moneyAmount += movedItem.Value;
                Shop.ColorCosts(moneyAmount);
                possible = true;
                break;
            
            case "Chest":
                //possible if less than the max or the chest is unlimited
                if (Chest.Inventory.Count < Chest.MaxSize || !Chest.HasMax)
                {
                    possible = true;
                }

                break;
        }

        //check if the item is coming from the shop
        if (original.name == "Shop")
        {
            //if the time is worth more than the amount of money the player has
            if (movedItem.Value > moneyAmount)
            {
                possible = false;
            }
        }

        if (possible)
        {
            //creating a baseItem (the movedItem is a clone, so it needs to be the same as the original)
            item baseItem = movedItem;
            foreach (var checkItem in allItems)
            {
                if (checkItem.ItemName == movedItem.ItemName)
                {
                    baseItem = checkItem;
                }
            }

            switch (original.name)
            {
                case "Backpack":
                    Backpack.RemoveItem(baseItem);
                    break;
                case "Shop":
                    Shop.RemoveItem(baseItem);
                    moneyAmount -= baseItem.Value;
                    Shop.ColorCosts(moneyAmount);
                    break;
                case "Chest":
                    Chest.RemoveItem(baseItem);
                    break;
            }

            switch (location.name)
            {
                case "Backpack":
                    Backpack.AddItem(baseItem);
                    break;
                case "Shop":
                    Shop.AddItem(baseItem);
                    break;
                case "Chest":
                    Chest.AddItem(baseItem);
                    break;
            }

            moneyDisplay.text = "" + moneyAmount;
        }
    }
    
    //only needs the one inventory to be used, helpful for shift clicking
    public void moveItemAuto(GameObject original, item movedItem)
    {
        if (leftInventory != null && rightInventory != null)
        {
            GameObject orig, location;
            if (leftInventory == original)
            {
                orig = leftInventory;
                location = rightInventory;
            }
            else
            {
                orig = rightInventory;
                location = leftInventory;
            }

            moveItem(orig, location, movedItem);
        }
    }
}