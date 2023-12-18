using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item")]
public class Item : ScriptableObject
{
    private static List<Item> allItems=new();
    private static List<Item> items = new(); //TODO temporal.delete!
	
	[SerializeField] protected string _itemName = "ErrorItem";
    [SerializeField] private bool _stackable=false;
    [SerializeField] private long _amount = 1;

    public string ItemName => _itemName;
    public virtual bool Stackable { get => _stackable; }
    public virtual long Amount
    {
        get => _amount;
        set //TODO delete setter
        {
            if (_stackable && value > 0)
                _amount = value;
        }
    }
	
	public static void LoadItems() {
		foreach (object tempObject in Resources.LoadAll("", Type.GetType("ScriptableItem", false, true)))
			if (tempObject is Item tempItem)
				if (allItems.Find(item => item.ItemName == tempItem.ItemName) == null)
					allItems.Add(tempItem);
				else
					Debug.Log("Error! Have tryed to load items with same name");

        //TODO next is temporal. Delete!
        if (items.Count != 0)
            return;

        Weapon thisItem = new Weapon();
        thisItem.SetValues("Fist", 1, 3, false, "Unarmed");
        thisItem._itemName = "Fist";
        thisItem.SetDamage(1, 4, 0);
        items.Add(thisItem);

        thisItem = new ();
        thisItem.SetValues("Knife", 1, 3, false, "Melee");
        thisItem.SetDamage(1, 10, 0);
        items.Add(thisItem);

        thisItem = new ();
        thisItem.SetValues("Sword", 1, 3, false, "Melee");
        thisItem.SetDamage(3, 8, 0);
        items.Add(thisItem);

        thisItem = new ();
        thisItem.SetValues("Pistol", 4, 4, true, "Pistols");
        thisItem.SetDamage(1, 8, 3);
        items.Add(thisItem);

        thisItem = new ();
        thisItem.SetValues("Rifle", 7, 5, true, "Rifles");
        thisItem.SetDamage(1, 10, 4);
        items.Add(thisItem);

        thisItem = new ();
        thisItem.SetValues("Shotgun", 3, 5, true, "Rifles");
        thisItem.SetDamage(5, 6, 0);
        items.Add(thisItem);

        thisItem = new ();
        thisItem.SetValues("Sniper rifle", 10, 8, true, "Rifles");
        thisItem.SetDamage(2, 20, 0);
        items.Add(thisItem);

        thisItem = new ();
        thisItem.SetValues("Machinegun", 5, 6, true, "Heavy");
        thisItem.SetDamage(5, 6, 0);
        items.Add(thisItem);
    }
	
	public Item Clone()
    {
        return Instantiate(this);
    }

    private Item Clone (long newAmount)
    {
        long oldAmount = _amount;
        _amount = newAmount;
        Item copiedItem = Instantiate(this);
        _amount = oldAmount;
        return copiedItem;
    }
        
    public Item Split (long amountToGet)
    {
        if (_amount <= amountToGet)
            return null;

        //TODO mb. if _amount==amountToGet do Destroy(this) ?? Check logic

        _amount -= amountToGet;
        return Clone(amountToGet);
    }
	
    public bool TryToUnite (Item itemToAdd)
    {
        if (!IsTheSameItem(itemToAdd))
            return false;

        _amount += itemToAdd.Amount;
        Destroy(itemToAdd);

        return (itemToAdd==null);
    }

    public virtual bool IsTheSameItem(Item itemToCompare)
    {
        return this._itemName == itemToCompare._itemName && this.GetType()==itemToCompare.GetType();
        //TODO make test if the same name but diff types
    }

    public virtual string ToJson ()
    {
        return JsonUtility.ToJson(this);
    }

    public virtual void FromJson (string jsonString)
    {
        JsonUtility.FromJsonOverwrite(jsonString, this);
        if (this.ItemName == "ErrorItem"){
			Debug.Log ("Error! Mistake on FromJson() in SrcItem (item is destroying)");
            Destroy(this);
		}
    }

    public static Item GetItem(string itemName)
    {
        return allItems.Find (item => item.ItemName == itemName).Clone();
    }

    public static Item[] GetAllItems()
    {
        return allItems.Select(item => item.Clone()).ToArray();
    }
}
