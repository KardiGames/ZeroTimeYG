using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    public const float ITEM_SCALE_MULTIPLER = 1.1f;
    
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private Image _itemImage;
    [SerializeField] private Button transferAllButton;
    [SerializeField] private Button transferPartButton;
    [SerializeField] private Button thirdButton;
    public InventoryUIContentFiller InventoryUI { get; private set; }
    private Item _item;


    public void Init(Item item, InventoryUIContentFiller inventoryUI)
    {
        if (this._item == null)
            this._item = item;
        this.InventoryUI = inventoryUI;

        itemName.text = GlobalUserInterface.Instance.Localisation.Translate(item.ItemName);
        if (item.Stackable)
            itemName.text += " x" + item.Amount;

        _itemImage.sprite = _item.Icon;
        if (_item.Icon!=null)
            _itemImage.color = _item.IconColor;

        if (true) //TODO add here and below condition if we know where to transfer    
            transferAllButton.gameObject.SetActive(true);

        if (true && item.Stackable && item.Amount>1)
            transferPartButton.gameObject.SetActive(true);

        if ((item is Weapon || item is Armor) && inventoryUI.Inventory.gameObject.name == "PlayerCharacter")
        {
            thirdButton.gameObject.SetActive(true);
            thirdButton.GetComponentInChildren<TextMeshProUGUI>().text = GlobalUserInterface.Instance.Localisation.Translate("Equip");  
            if (inventoryUI.Inventory.gameObject.GetComponent<Equipment>().IsAbleToEquip(item, false))
                thirdButton.onClick.AddListener(Equip);
            else
                thirdButton.interactable = false;
        } else if (item is Blueprint && inventoryUI.Inventory.gameObject.name == "Factory")
        {
            thirdButton.gameObject.SetActive(true);
            thirdButton.GetComponentInChildren<TextMeshProUGUI>().text = GlobalUserInterface.Instance.Localisation.Translate("Produce");
            thirdButton.onClick.AddListener(StartProductionInFactory);
        }
    }

    public void TransferAll ()
    {
        if (InventoryUI == null || InventoryUI.Inventory == null || InventoryUI.TargetInventory == null)
            return;

        InventoryUI.Inventory.TransferTo(InventoryUI.Inventory, InventoryUI.TargetInventory, _item, _item.Amount);
    }

    public void TransferPart()
    {
        if (!_item.Stackable || _item.Amount <= 1)
            return;

        InventoryUI.TransferPartPanel.gameObject.SetActive(true);
        InventoryUI.TransferPartPanel.Init(InventoryUI, _item);
    }

    public void ShowItemInfo() => InventoryUI.ShowItemInfo(_item);

    private void StartProductionInFactory ()
    {
        ActionPoints playerAP = InventoryUI.TargetInventory.gameObject.GetComponent<ActionPoints>();
        Factory activeFactory = InventoryUI.Inventory.gameObject.GetComponent<Factory>();

        if (playerAP == null || activeFactory == null)
        {
            GlobalUserInterface.Instance.ShowError(GlobalUserInterface.Instance.Localisation.Translate("Error #") + "1");
            return;
        }

        if (!(_item as Blueprint).IsAnoughResourses(InventoryUI.Inventory))
        {
            GlobalUserInterface.Instance.ShowError("You have not anough resources to start production.");
            return;
        }

        if (!playerAP.TrySpendAP(1))
        {
            GlobalUserInterface.Instance.ShowError("You need at least 1 AP to start production.");
            return;
        }

        activeFactory.AddFactoryLine(_item as Blueprint, true);
    }

    private void Equip() => InventoryUI.Inventory.gameObject.GetComponent<Equipment>().Equip(InventoryUI.Inventory, _item);
    public void Scale() => _itemImage.transform.localScale = new Vector3(ITEM_SCALE_MULTIPLER, ITEM_SCALE_MULTIPLER);
    public void Unscale() => _itemImage.transform.localScale = Vector3.one;
}
