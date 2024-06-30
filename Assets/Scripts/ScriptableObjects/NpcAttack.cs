using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/NpcAttack")]
public class NpcAttack : Weapon
{
    public override string AmmoType => "";
    public override int AmmoAmount => 1;
    public override int AmmoMaxAmount => 1;
    public override int AmmoPerShot => 0;
    public override int Quality => 1;
	public override int MaxQuality => 1;
	
	public void SetValues(string itemName, int range, int apCost, bool rangedAttack)
    {
        _itemName = itemName;
        _range = range;
        _apCost = apCost;
        _rangedAttack = rangedAttack;
        _skillname = itemName;
    }
    public void SetDamage(int randomMultipler = 1, int randomTo = 6, int addition = 0)
    {
        damageRandomMultipler = randomMultipler;
        damageRandomTo = randomTo;
        damageAddition = addition;
    }

    public override bool TryToSpendAmmo() {
		return true;
	}
	
	public void BoostDamage(string parametr, int value = 1)
    {
        if (parametr == "addition")
        {
            damageAddition += value;
        }
        else if (parametr == "dice")
        {
            damageRandomTo += value;
        }
        else if (parametr == "multipler")
        {
            damageRandomMultipler += value;
        }
    }

    public void BoostDamage()
    {
        if (damageRandomTo % 2 == 0 && UnityEngine.Random.Range(1, damageRandomTo / 3) > 10)
        {
            damageRandomMultipler++;
            damageRandomTo /= 2;
        }
        else if (UnityEngine.Random.value <= 0.5f)
        {
            damageRandomTo += 3;
        }
        else
        {
            damageAddition += damageRandomMultipler;
        }
    }
}