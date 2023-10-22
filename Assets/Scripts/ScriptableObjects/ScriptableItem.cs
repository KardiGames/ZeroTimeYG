using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableItem : ScriptableObject
{
    
    [SerializeField] private string itemName = "ScriptItem";
    [SerializeField] private bool stackable=false;
    [SerializeField] private long _amount = 1;


    public string ItemName => itemName;
    public bool Stackable { get => stackable; }
    public long Amount
    {
        get => _amount;
        set //TODO delete setter
        {
            if (stackable && value > 0)
                _amount = value;
        }
    }
	
	private ScriptableItem Clone()
    {
        return Instantiate(this);
    }

    private ScriptableItem Clone (long newAmount)
    {
        long oldAmount = _amount;
        _amount = newAmount;
        ScriptableItem copiedItem = Instantiate(this);
        _amount = oldAmount;
        return copiedItem;
    }
        
    public ScriptableItem Split (long amountToGet)
    {
        if (_amount <= amountToGet)
            return null;

        //TODO mb. if _amount==amountToGet do Destroy(this) ?? Check logic

        _amount -= amountToGet;
        return Clone(amountToGet);
    }
	
    public bool TryToUnite (ScriptableItem itemToAdd)
    {
        if (!IsTheSameItem(itemToAdd))
            return false;

        _amount += itemToAdd.Amount;
        Destroy(itemToAdd);

        return (itemToAdd==null);
    }

    public virtual bool IsTheSameItem(ScriptableItem itemToCompare) //TODO check logic, m.b. make abstract
    {
        return this.itemName == itemToCompare.itemName;
    }
}
