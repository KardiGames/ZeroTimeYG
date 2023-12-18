using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Armor", menuName = "Items/Armor")]
public class Armor : Item
{
    [SerializeField] private int _ac =0;
	[SerializeField] private Equipment.Slot _slot = Equipment.Slot.Body;

    public override bool Stackable => false;
    public override long Amount { get => 1; }
	public int AC => _ac;
	public Equipment.Slot Slot => _slot;

	public override bool IsTheSameItem(Item itemToCompare) 
    {
        if (!base.IsTheSameItem(itemToCompare))
			return false;
	
		if (itemToCompare is Armor armorToCompare
			&& armorToCompare.AC == _ac 
			&& armorToCompare.Slot==_slot
		)
			return true;
		else
			return false;
	}
}
