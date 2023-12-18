using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Items/Weapon")]
public class Weapon : Item
{
    [SerializeField] private bool _rangedAttack = true;
    [SerializeField] private bool _twoHanded = false;
    [SerializeField] protected int damageRandomMultipler = 1; //3 for 3d6
    [SerializeField] protected int damageRandomTo = 6; //1dX (10 for 1d10)
    [SerializeField] protected int damageAddition = 0; //5 for 2d8+5
    [SerializeField] private int _range = 2;
    [SerializeField] private int _apCost = 5;
    [SerializeField] private string _skillname = "";

    public override bool Stackable { get => false; }
    public override long Amount { get => 1; }
    public int Range => _rangedAttack ? _range : 1;
    public bool RangedAttack => _rangedAttack;
    public int APCost => _apCost;
    public bool TwoHanded => _twoHanded;
    public string SkillName => _skillname;

    public int Damage //TODO move somewhere to delete
    {
        get
        {
            int summ = 0;
            for (int i = 0; i < damageRandomMultipler; i++)
            {
                summ += UnityEngine.Random.Range(1, (damageRandomTo + 1));
            }
            summ += damageAddition;
            return summ;
        }
    }

    public string DamageInfo => damageRandomMultipler + "d" + damageRandomTo + "+" + damageAddition;

    public void SetValues(string itemName, int range, int apCost, bool rangedAttack, string skillName) //TODO temporal. Delete!
    {
        _itemName = itemName;
        _range = range;
        _apCost = apCost;
        _rangedAttack = rangedAttack;
        _skillname = "skillName";
    }
    public void SetDamage(int randomMultipler = 1, int randomTo = 6, int addition = 0) //TODO temporal. Delete!
    {
        damageRandomMultipler = randomMultipler;
        damageRandomTo = randomTo;
        damageAddition = addition;
    }
    public string FormDamageDiapason(int flatBonus = 0)
    {
        return (damageRandomMultipler + damageAddition + flatBonus) + "-" + (damageRandomMultipler * damageRandomTo + damageAddition + flatBonus);
    }

    public void BoostDamage() //TODO move somewhere or delete
    {
        if (damageRandomTo%2==0 && UnityEngine.Random.Range(0, 10) < damageRandomTo / 3)
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
	
	public override bool IsTheSameItem(Item itemToCompare) 
    {
        if (!base.IsTheSameItem(itemToCompare))
			return false;
	
		if (itemToCompare is Weapon weaponToCompare
			&& weaponToCompare.RangedAttack==_rangedAttack 
			&& weaponToCompare.Range==_range 
			&& weaponToCompare.DamageInfo==DamageInfo
			&& weaponToCompare.APCost==_apCost
			&& weaponToCompare.TwoHanded==_twoHanded
		)
			return true;
		else
			return false;
	}
}
