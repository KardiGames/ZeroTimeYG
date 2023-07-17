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
        set
        {
            if (stackable && value > 0)
                _amount = value;
        }
    }
	
	ScriptableItem Clone()
    {
        throw new NotImplementedException();
    }
	

	
    


}
