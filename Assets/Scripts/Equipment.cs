using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public event Action OnEquipmentContentChanged;
    public enum Slot {RightHand = 0, LeftHand = 1, Body=2};
		
	[SerializeField] private ScriptableItem[] equipment = new ScriptableItem[3];
    //TODO Actual sizi (is 3) is set by Inspector. Think if you need to delete [SerializeField] and set it in code

    public ScriptableItem this [int index]
    {
        get =>equipment[index];
    }

    public int SlotsCount() => equipment.Length;
	
	public ScriptableItem this [Slot slotIndex]
    {
        get =>equipment[(int) slotIndex];
    }

    public bool IsAbleToEquip(ScriptableItem item, bool replaceInSlot)
    {
        if (item is Weapon weapon)
        {
            if (weapon.TwoHanded)
            {
                if (replaceInSlot || (equipment[(int)Slot.RightHand ] == null && equipment[(int)Slot.LeftHand] != null))
                    return true;
                else
                    return false;
            } else
            {
                if (replaceInSlot || equipment[(int)Slot.RightHand] == null || equipment[(int)Slot.LeftHand] == null)
                    return true;
                else
                    return false;
            }
        }

        else if (item is Armor armor)
        {
            if (replaceInSlot || equipment[(int)armor.Slot] == null)
                return true;
            else
                return false;
        }
        else
            return false;
    }

    public void Unequip (Slot slot, Inventory inventoryTo)
    {
        ScriptableItem itemInSlot = equipment[(int)slot];

        if (itemInSlot is Weapon weapon && weapon.TwoHanded)
        {
            equipment[0] = null;
            equipment[1] = null;
        } else
        {
            equipment[(int)slot] = null;
        }
        
        if (!inventoryTo.TryToAdd(this, itemInSlot))
        {
            print("Error!!! Item isn't added to inventory on unequip. Item is LOST");
            return;
        }

        OnEquipmentContentChanged?.Invoke();
    }
    public void Equip (Inventory inventoryFrom, ScriptableItem item, bool replaceInSlot=false)
    {
        if (!IsAbleToEquip(item, replaceInSlot))
            return;

        if (TryToEquip(inventoryFrom, item, replaceInSlot))
            inventoryFrom.RemoveThisItem(this, item);
    }
    private bool TryToEquip (object sender, ScriptableItem item, bool replaceInSlot)
    {
        int slotNumber = 0;
        if (item is Armor armor)
            slotNumber = (int)armor.Slot;
        else if (equipment[0] == null)
            slotNumber = 0;
        else if (equipment[1] == null)
            slotNumber = 1;

        if (!replaceInSlot && equipment[slotNumber] != null)
            return false;
        if (item is Weapon weapon && weapon.TwoHanded)
        {
            equipment[0] = item;
            equipment[1] = item;
        }
        else
            equipment[slotNumber] = item;

        OnEquipmentContentChanged?.Invoke();
        return true;
    }
}
