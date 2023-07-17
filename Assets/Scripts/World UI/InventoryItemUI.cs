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
    [SerializeField] private Button equipButton;

    private ScriptableItem item;
    private InventoryUIContentFiller inventoryUI;

    public void Set (ScriptableItem item, InventoryUIContentFiller inventoryUI)
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

        if (true && item is Weapon) //TODO add here condition if it is Players's inventory
            equipButton.gameObject.SetActive(true);
    }

    public void TransferAll ()
    {
        if (inventoryUI == null || inventoryUI.Invetory == null || inventoryUI.TargetInventory == null)
            return;

        inventoryUI.Invetory.TransferTo(inventoryUI.Invetory, inventoryUI.TargetInventory, item, item.Amount);
    }

    public void TransferPart(long amount)
    {
        if (inventoryUI == null || inventoryUI.Invetory == null || inventoryUI.TargetInventory == null || amount > item.Amount)
            return;

        inventoryUI.Invetory.TransferTo(inventoryUI.Invetory, inventoryUI.TargetInventory, item, amount);
    }
}
