using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    private const int SLOTS_NUMBER = 3;

    public event Action OnEquipmentContentChanged;
    public enum Slot { RightHand = 0, LeftHand = 1, Body = 2 };

    [SerializeField] private Item[] _equipment = new Item[SLOTS_NUMBER];
    //TODO Actual size (is 3) and may be not equal SLOTS_NUMBER, it is set by Inspector.

    public int AC
    {
        get
        {
            int ac = 0;
            foreach (Item item in _equipment)
                if (item != null)
                    ac += item.AC;
            return ac;
        }
    }
	public int DamageResistance
	{
		get 
		{
			int dr=0;
			foreach (Item item in _equipment)
				if (item is Armor armor)
					dr+=armor.DamageResistance;
			return dr;
		}
	}
			

    public Item this[int index]
    {
        get => _equipment[index];
    }

    public int SlotsCount() => _equipment.Length;

    public Item this[Slot slotIndex]
    {
        get => _equipment[(int)slotIndex];
    }

    public bool IsAbleToEquip(Item item, bool replaceInSlot)
    {
        if (item is Weapon weapon)
        {
            if (weapon.TwoHanded)
            {
                if (replaceInSlot || (_equipment[(int)Slot.RightHand] == null && _equipment[(int)Slot.LeftHand] != null))
                    return true;
                else
                    return false;
            } else
            {
                if (replaceInSlot || _equipment[(int)Slot.RightHand] == null || _equipment[(int)Slot.LeftHand] == null)
                    return true;
                else
                    return false;
            }
        }

        else if (item is Armor armor)
        {
            if (replaceInSlot || _equipment[(int)armor.Slot] == null)
                return true;
            else
                return false;
        }
        else
            return false;
    }

    public void Unequip(Slot slot, Inventory inventoryTo)
    {
        Item itemInSlot = _equipment[(int)slot];

        if (itemInSlot is Weapon weapon && weapon.TwoHanded)
        {
            _equipment[0] = null;
            _equipment[1] = null;
        } else
        {
            _equipment[(int)slot] = null;
        }

        if (!inventoryTo.TryToAdd(this, itemInSlot))
        {
            print("Error!!! Item isn't added to inventory on unequip. Item is LOST");
            return;
        }

        OnEquipmentContentChanged?.Invoke();
    }



    public void Equip(Inventory inventoryFrom, Item item, bool replaceInSlot = false)
    {
        if (!IsAbleToEquip(item, replaceInSlot))
            return;

        if (TryToEquip(inventoryFrom, item, replaceInSlot))
            inventoryFrom.RemoveThisItem(this, item);
    }
    private bool TryToEquip(object sender, Item item, bool replaceInSlot)
    {
        int slotNumber = 0;
        if (item is Armor armor)
            slotNumber = (int)armor.Slot;
        else if (_equipment[0] == null)
            slotNumber = 0;
        else if (_equipment[1] == null)
            slotNumber = 1;

        if (!replaceInSlot && _equipment[slotNumber] != null)
            return false;
        if (item is Weapon weapon && weapon.TwoHanded)
        {
            _equipment[0] = item;
            _equipment[1] = item;
        }
        else
            _equipment[slotNumber] = item;

        OnEquipmentContentChanged?.Invoke();
        return true;
    }

    public void FromJson(string jsonString)
    {
        _equipment = new Item[SLOTS_NUMBER];
        EquipmentJsonData jsonEquipment = JsonUtility.FromJson<EquipmentJsonData>(jsonString);
        if (jsonEquipment == null || jsonEquipment.eqNames.Count!=jsonEquipment.eqJsons.Count)
			return;
		Item itemToAdd;
		for (int i=0; i< jsonEquipment.eqNames.Count; i++)
        {
			if (jsonEquipment.eqNames[i] == "")
                continue;
            itemToAdd = Item.GetItem(jsonEquipment.eqNames[i]);
			if (itemToAdd == null)
				continue;
			itemToAdd.FromJson(jsonEquipment.eqJsons[i]);
            _equipment[i] = itemToAdd;
        }
    }
	
	public string ToJson()
    {
		EquipmentJsonData jsonEquipment = new();
		for (int i=0; i<_equipment.Length; i++)
        {
            if (_equipment[i] == null) { 
                jsonEquipment.eqNames.Add("");
                jsonEquipment.eqJsons.Add("");
            }
            else
            {
                jsonEquipment.eqNames.Add(_equipment[i].ItemName);
                jsonEquipment.eqJsons.Add(_equipment[i].ToJson());
            }
        }
		
		return JsonUtility.ToJson(jsonEquipment);
    }
	
	[Serializable]
	protected class EquipmentJsonData
	{
        public List<string> eqNames=new();
        public List<string> eqJsons = new();
    }
}
