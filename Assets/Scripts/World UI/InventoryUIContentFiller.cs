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

    public Inventory Invetory { get => inventory; }
    public Inventory TargetInventory { get => targetInventoryUI.Invetory; }



    private void Start()
    {
        contentTransform = gameObject.GetComponent<ScrollRect>().content;
        scrollableObjectHeight = objectToFill.GetComponent<RectTransform>().sizeDelta.y;

        if (contentTransform.childCount != 0)
        {
            for (int i = 0; i < contentTransform.childCount; i++)
                children.Add(contentTransform.GetChild(i).gameObject);
        }

        inventory.OnInventoryContentChanged += Clear;
        inventory.OnInventoryContentChanged += Fill;
        Clear();
        Fill(inventory.GetAllItems());
    }

    private void Clear()
    {
        foreach (GameObject child in children)
            Destroy(child);
        children.Clear();
    }

    public void Fill() => Fill(inventory.GetAllItems());
    
    public void Fill (IEnumerable<ScriptableItem> list)
    {
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
