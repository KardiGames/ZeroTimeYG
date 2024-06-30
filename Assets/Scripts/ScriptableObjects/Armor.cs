using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Armor", menuName = "Items/Armor")]
public class Armor : Item
{
    [SerializeField] private int _ac =0;
	[SerializeField] private Equipment.Slot _slot = Equipment.Slot.Body;
	[SerializeField] private int _quality;
	[SerializeField] private int _maxQuality;
	public int Quality => _quality;
	public int MaxQuality => _maxQuality;

	public override bool Stackable => false;
    public override long Amount { get => 1; }
    public override int AC { get { if (_quality > 0) return _ac; else return 0; } }
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
	
	public override string ToJson ()
    {
		ArmorJsonData itemJson = new ArmorJsonData()
		{
			q = _quality
		};
		return JsonUtility.ToJson(itemJson);
    }

    public override void FromJson (string jsonString)
    {
        ArmorJsonData jsonItem = JsonUtility.FromJson<ArmorJsonData>(jsonString);
        if (jsonItem == null){
			Debug.Log ("Error! Mistake on FromJson() in SrcArmor (item is destroying)");
            Destroy(this);
		}
		_quality=jsonItem.q;
    }
	
	[Serializable]
	private class ArmorJsonData {
		public int q;
	}
}
