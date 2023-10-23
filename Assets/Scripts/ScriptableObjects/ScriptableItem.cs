using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item")]
public class ScriptableItem : ScriptableObject
{
    
    [SerializeField] protected string _itemName = "ErrorItem";
    [SerializeField] private bool _stackable=false;
    [SerializeField] private long _amount = 1;

    public string ItemName => _itemName;
    public bool Stackable { get => _stackable; }
    public long Amount
    {
        get => _amount;
        set //TODO delete setter
        {
            if (_stackable && value > 0)
                _amount = value;
        }
    }
	
	public ScriptableItem Clone()
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

    public virtual bool IsTheSameItem(ScriptableItem itemToCompare)
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
        if (this.ItemName == "ErrorItem")
            Destroy(this);
    }

    protected static ScriptableItem GetItem(string itemName)
    {
        throw new NotImplementedException();
    }
}
