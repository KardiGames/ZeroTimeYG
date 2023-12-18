using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIContentFiller : MonoBehaviour
{

    [SerializeField] private Inventory inventory;
    [SerializeField] private InventoryUIContentFiller targetInventoryUI;

    [SerializeField] private GameObject objectToFill;
    private RectTransform contentTransform;

    [SerializeField] private float percentSpaceBetweenObjects = 0.05f;
    private float scrollableObjectHeight;
    private List<GameObject> children = new();

    public Inventory Inventory
    {
        get => inventory; set
        {
            if (inventory!= value || value == null)
            {
                Unsubscribe();
                inventory = value;
                SubscribeAndRefresh();
            } else
            {
                Clear();
                Fill();
            }
            
        }
    }
    public Inventory TargetInventory { get => targetInventoryUI.Inventory; }


    private void Start()
    {
        if (contentTransform != null)
            return;
        contentTransform = gameObject.GetComponent<ScrollRect>().content;
        scrollableObjectHeight = objectToFill.GetComponent<RectTransform>().sizeDelta.y;

        if (contentTransform.childCount != 0)
        {
            for (int i = 0; i < contentTransform.childCount; i++)
                children.Add(contentTransform.GetChild(i).gameObject);
        }
        SubscribeAndRefresh();
    }

    private void SubscribeAndRefresh()
    {
        if (inventory != null && contentTransform!=null)
        {
            inventory.OnInventoryContentChanged += Clear;
            inventory.OnInventoryContentChanged += Fill;
            Clear();
            Fill();
        }
    }

    private void Unsubscribe()
    {
        if (inventory != null)
        {
            inventory.OnInventoryContentChanged -= Clear;
            inventory.OnInventoryContentChanged -= Fill;
        }
    }

    private void OnDisable()
    {
        Inventory = null;
    }

    private void Clear()
    {
        foreach (GameObject child in children)
            Destroy(child);
        children.Clear();
    }

    public void Fill() { 
        if (inventory != null ) 
            Fill(inventory.GetAllItems());
    }

    
    public void Fill (IEnumerable<Item> list)
    {
        if (contentTransform == null)
            Start();

        if (list==null)
        {
            print("Error. Fill is called, but there is no inventory to fill");
            return;
        }

        
        float nextYPosition = 0;

        foreach (var scrollableItem in list)
        {
            GameObject newScrollableObject = Instantiate(objectToFill,contentTransform);
            if (newScrollableObject != null)
                children.Add(newScrollableObject);
            else
                continue;

            newScrollableObject.transform.localPosition = new Vector3(0, -nextYPosition);
            newScrollableObject.GetComponent<InventoryItemUI>().Set(scrollableItem, this);

            nextYPosition += scrollableObjectHeight * (1.0f + percentSpaceBetweenObjects);

        }
        contentTransform.sizeDelta = new Vector2(contentTransform.sizeDelta.x, nextYPosition);
    }
}
