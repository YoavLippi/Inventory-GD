using UnityEngine;

public class HidingBehaviourInventories : MonoBehaviour
{
    [SerializeField] private GameObject left, right;

    [SerializeField] private GameHandler main;

    private void OnEnable()
    {
        if (transform.position.x > 0)
        {
            //This is on the right
            main.RightInventory = gameObject;
        }
        else
        {
            //This is on the left
            main.LeftInventory = gameObject;
        }
    }

    private void OnDisable()
    {
        //check which side then set the corresponding inventory to nothing
        if (transform.position.x > 0)
        {
            main.RightInventory = null;
        }
        else
        {
            main.LeftInventory = null;
        }
    }

    public void runDisable()
    {
        //the onDisable wasn't working with the buttons for some reason
        if (transform.position.x > 0)
        {
            main.RightInventory = null;
        }
        else
        {
            main.LeftInventory = null;
        }
    }

    public void activateButtons()
    {
        if (transform.position.x > 0)
        {
            right.SetActive(true);
        }
        else
        {
            left.SetActive(true);
        }
    }
}