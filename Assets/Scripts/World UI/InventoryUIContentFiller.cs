using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIContentFiller : MonoBehaviour
{

    [SerializeField] private GameObject _objectToFill;
    [SerializeField] private TransferPartPanel _transferPartPanel;
    private RectTransform _contentTransform;

    [SerializeField] private Inventory inventory;
    [SerializeField] private InventoryUIContentFiller targetInventoryUI;
    [SerializeField] private Button _thisInventoryButton;

    [SerializeField] private float _percentSpaceBetweenObjects = 0.05f;
    private float _scrollableObjectHeight;
    private List<GameObject> _children = new();

    public TransferPartPanel TransferPartPanel => _transferPartPanel;
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
        if (_contentTransform != null)
            return;
        _contentTransform = gameObject.GetComponent<ScrollRect>().content;
        _scrollableObjectHeight = _objectToFill.GetComponent<RectTransform>().sizeDelta.y;

        if (_contentTransform.childCount != 0)
        {
            for (int i = 0; i < _contentTransform.childCount; i++)
                _children.Add(_contentTransform.GetChild(i).gameObject);
        }
        SubscribeAndRefresh();
    }

    private void OnEnable()
    {
        if (_thisInventoryButton != null)
            _thisInventoryButton.interactable = false;
    }

    private void OnDisable()
    {
        Inventory = null;
        if (_thisInventoryButton != null)
            _thisInventoryButton.interactable = true;
    }

    private void SubscribeAndRefresh()
    {
        if (inventory != null && _contentTransform!=null)
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

    private void Clear()
    {
        foreach (GameObject child in _children)
            Destroy(child);
        _children.Clear();
    }

    public void Fill() { 
        if (inventory != null ) 
            Fill(inventory.GetAllItems());
    }

    
    public void Fill (IEnumerable<Item> list)
    {
        if (_contentTransform == null)
            Start();

        if (list==null)
        {
            print("Error. Fill is called, but there is no inventory to fill");
            return;
        }

        
        float nextYPosition = 0;

        foreach (var scrollableItem in list)
        {
            GameObject newScrollableObject = Instantiate(_objectToFill,_contentTransform);
            if (newScrollableObject != null)
                _children.Add(newScrollableObject);
            else
                continue;

            newScrollableObject.transform.localPosition = new Vector3(0, -nextYPosition);
            newScrollableObject.GetComponent<InventoryItemUI>().Init(scrollableItem, this);

            nextYPosition += _scrollableObjectHeight * (1.0f + _percentSpaceBetweenObjects);

        }
        _contentTransform.sizeDelta = new Vector2(_contentTransform.sizeDelta.x, nextYPosition);
    }
}
