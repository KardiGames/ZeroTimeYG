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

    private ScriptableItem item;
    private InventoryUIContentFiller inventoryUI;

    public void Set(ScriptableItem item, InventoryUIContentFiller inventoryUI)
    {
        if (this.item == null)
            this.item = item;
        this.inventoryUI = inventoryUI;

        itemName.text = item.ItemName;
        if (item.Stackable)
            itemName.text += " x" + item.Amount;

        if (true) //TODO add here and below condition if we know where to transfer    
            transferAllButton.gameObject.SetActive(true);

        if (true && item.Stackable)
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
        if (inventoryUI == null || inventoryUI.Inventory == null || inventoryUI.TargetInventory == null)
            return;

        inventoryUI.Inventory.TransferTo(inventoryUI.Inventory, inventoryUI.TargetInventory, item, item.Amount);
    }

    public void TransferPart(long amount)
    {
        if (inventoryUI == null || inventoryUI.Inventory == null || inventoryUI.TargetInventory == null || amount > item.Amount)
            return;

        inventoryUI.Inventory.TransferTo(inventoryUI.Inventory, inventoryUI.TargetInventory, item, amount);
    }

    private void StartProductionInFactory ()
    {
        Factory activeFactory = inventoryUI.Inventory.gameObject.GetComponent<Factory>();
        activeFactory.AddFactoryLine(item as Blueprint);
    }

    private void Equip() => inventoryUI.Inventory.gameObject.GetComponent<Equipment>().Equip(inventoryUI.Inventory, item);
}
