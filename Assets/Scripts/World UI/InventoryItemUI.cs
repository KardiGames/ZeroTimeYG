using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private Button transferAllButton;
    [SerializeField] private Button transferPartButton;
    [SerializeField] private Button thirdButton;
    public InventoryUIContentFiller InventoryUI { get; private set; }
    private Item item;


    public void Init(Item item, InventoryUIContentFiller inventoryUI)
    {
        if (this.item == null)
            this.item = item;
        this.InventoryUI = inventoryUI;

        itemName.text = item.ItemName;
        if (item.Stackable)
            itemName.text += " x" + item.Amount;

        if (true) //TODO add here and below condition if we know where to transfer    
            transferAllButton.gameObject.SetActive(true);

        if (true && item.Stackable && item.Amount>1)
            transferPartButton.gameObject.SetActive(true);

        if ((item is Weapon || item is Armor) && inventoryUI.Inventory.gameObject.name == "PlayerCharacter")
        {
            thirdButton.gameObject.SetActive(true);
            thirdButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equip";
            if (inventoryUI.Inventory.gameObject.GetComponent<Equipment>().IsAbleToEquip(item, false))
                thirdButton.onClick.AddListener(Equip);
            else
                thirdButton.interactable = false;
        } else if (item is Blueprint && inventoryUI.Inventory.gameObject.name == "Factory")
        {
            thirdButton.gameObject.SetActive(true);
            thirdButton.GetComponentInChildren<TextMeshProUGUI>().text = "Produce";
            thirdButton.onClick.AddListener(StartProductionInFactory);
        }
    }

    public void TransferAll ()
    {
        if (InventoryUI == null || InventoryUI.Inventory == null || InventoryUI.TargetInventory == null)
            return;

        InventoryUI.Inventory.TransferTo(InventoryUI.Inventory, InventoryUI.TargetInventory, item, item.Amount);
    }

    public void TransferPart()
    {
        if (!item.Stackable || item.Amount <= 1)
            return;

        InventoryUI.TransferPartPanel.gameObject.SetActive(true);
        InventoryUI.TransferPartPanel.Init(InventoryUI, item);
    }


    private void StartProductionInFactory ()
    {
        Factory activeFactory = InventoryUI.Inventory.gameObject.GetComponent<Factory>();
        activeFactory.AddFactoryLine(item as Blueprint);
    }

    private void Equip() => InventoryUI.Inventory.gameObject.GetComponent<Equipment>().Equip(InventoryUI.Inventory, item);
}
