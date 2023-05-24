using UnityEngine;

public class hidingBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject[] inventories = new GameObject[3];

    [SerializeField] private hidingBehaviour otherSide;

    [SerializeField] private int currentInventory = 4;

    private void OnDisable()
    {
        setPos();
        if (otherSide.getCurrentInventory() == currentInventory)
        {
            otherSide.gameObject.SetActive(true);
        }
    }

    private void setPos()
    {
        if (inventories[currentInventory] != null)
        {
            if (inventories[currentInventory].activeSelf)
            {
                inventories[currentInventory].SetActive(false);
                inventories[currentInventory].GetComponent<HidingBehaviourInventories>().runDisable();
            }
            
            inventories[currentInventory].transform.position = this.gameObject.transform.position;
            inventories[currentInventory].SetActive(true);
        }
    }

    public void setCurrentInventory(int index)
    {
        currentInventory = index;
    }

    public int getCurrentInventory()
    {
        return currentInventory;
    }
}