using System;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    [Serializable]
    public class item : MonoBehaviour
    {

        [SerializeField] private string itemName;
        [SerializeField] private int value;
        [SerializeField] private bool stackable;
        [SerializeField] private FilterTypes itemType;
        [SerializeField] private bool isUnique;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private GameObject clone;
        [SerializeField] private Material transparent;
        [SerializeField] private GameObject fromInventory, toInventory;
        [SerializeField] private GameHandler main;
        [SerializeField] private TextMeshProUGUI costDisplay;

        public TextMeshProUGUI CostDisplay
        {
            get => costDisplay;
            set => costDisplay = value;
        }
        public GameHandler Main
        {
            get => main;
            set => main = value;
        }

        public TextMeshProUGUI Text
        {
            get => text;
            set => text = value;
        }

        public bool IsUnique
        {
            get => isUnique;
            set => isUnique = value;
        }

        public FilterTypes ItemType
        {
            get => itemType;
            set => itemType = value;
        }

        public string ItemName
        {
            get => itemName;
            set => itemName = value;
        }

        public int Value
        {
            get => value;
            set => this.value = value;
        }

        public bool Stackable
        {
            get => stackable;
            set => stackable = value;
        }

        private void OnMouseDown()
        {
            //When clicked, it will send a ray and save the inventory it hits
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Inventory"));
            if (hit.collider)
            {
                fromInventory = hit.collider.gameObject.transform.parent.gameObject;
            }
            
            //if shift is pressed we run the autoMove
            if (Input.GetKey(KeyCode.LeftShift))
            {
                main.moveItemAuto(fromInventory, this);
                clone = null;
            }
            else
            {
                //if shift is not pressed, we make a shadow version of the item and set the position to the mouse
                clone = Instantiate(this.gameObject);
                clone.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                clone.SetActive(true);
                SpriteRenderer sr = clone.GetComponent<SpriteRenderer>();
                sr.sortingOrder = 10;
                sr.material = transparent;
                clone.GetComponent<item>().Text.text = "";
            }
        }

        private void OnMouseUp()
        {
            //clone is null if shift was held
            if (clone != null)
            {
                Destroy(clone);
                
                //we get the inventory the mouse is hovering over, then move the item
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero,
                    Mathf.Infinity, LayerMask.GetMask("Inventory"));
                if (hit.collider)
                {
                    toInventory = hit.collider.gameObject.transform.parent.gameObject;
                    main.moveItem(fromInventory, toInventory, this);
                }
            }
        }

        private void OnMouseDrag()
        {
            //we make sure the clone is following the mouse
            if (clone != null)
            {
                var mousePos = Input.mousePosition;
                mousePos.z = 10f;
                clone.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
            }
        }
    }
}