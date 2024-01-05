using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Items/Weapon")]
public class Weapon : Item
{
    [SerializeField] protected bool _rangedAttack = true;
    [SerializeField] private bool _twoHanded = false;
    [SerializeField] protected int damageRandomMultipler = 1; //3 for 3d6
    [SerializeField] protected int damageRandomTo = 6; //1dX (10 for 1d10)
    [SerializeField] protected int damageAddition = 0; //5 for 2d8+5
    [SerializeField] protected int _range = 2;
    [SerializeField] protected int _apCost = 5;
    [SerializeField] protected string _skillname = "";

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


    public string FormDamageDiapason(int flatBonus = 0)
    {
        return (damageRandomMultipler + damageAddition + flatBonus) + "-" + (damageRandomMultipler * damageRandomTo + damageAddition + flatBonus);
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
