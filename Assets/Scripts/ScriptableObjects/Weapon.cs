using System;
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
    [SerializeField] private string _ammoType;
    [SerializeField] private int _ammoAmount;
    [SerializeField] private int _ammoMaxAmount;
    [SerializeField] private int _ammoPerShot;
    [SerializeField] private int _quality;
    [SerializeField] private int _maxQuality;


    [SerializeField] protected string _skillname = "";

    public override bool Stackable { get => false; }
    public override long Amount { get => 1; }
    public int Range => _rangedAttack ? _range : 1;
    public bool RangedAttack => _rangedAttack;
    public int APCost => _apCost;
    public bool TwoHanded => _twoHanded;
    public string SkillName => _skillname;
    public virtual string AmmoType => _ammoType;
    public virtual int AmmoAmount => _ammoAmount;
    public virtual int AmmoMaxAmount => _ammoMaxAmount;
    public virtual int AmmoPerShot => _ammoPerShot;
    public virtual int Quality
    {
        get => _quality; 
		set
        {
            if (value > _maxQuality) 
                _quality = value; 
            else if (value < 0) 
                _quality = 0; 
            else 
                _quality = value;
        }
    }

    public virtual int  MaxQuality => _maxQuality;

    public (int multipler, int dice, int addition) DamageTuple => new(damageRandomMultipler, damageRandomTo, damageAddition);

    public string DamageInfo => damageRandomMultipler + "d" + damageRandomTo + "+" + damageAddition;

    public virtual bool TryToSpendAmmo()
    {
        if (_ammoAmount < _ammoPerShot)
            return false;

        _ammoAmount -= _ammoPerShot;
        return true;
    }
    public void ReloadAmmo(Inventory inventoryAmmoFrom)
    {
        if (!IsAbleToReload (inventoryAmmoFrom))
            return;

        long ammoInInventory = inventoryAmmoFrom.GetItemAmount(_ammoType);
        int  reloadingAmmo = _ammoMaxAmount - _ammoAmount;
        if (ammoInInventory < reloadingAmmo)
            reloadingAmmo = (int)ammoInInventory;

        inventoryAmmoFrom.Remove(this, _ammoType, reloadingAmmo);
         _ammoAmount += reloadingAmmo;
    }

    public void FillAmmo() =>
        _ammoAmount = _ammoMaxAmount;

    public bool IsAbleToReload (Inventory inventoryAmmoFrom)
    {
        if (_ammoType == ""
            || _ammoAmount == _ammoMaxAmount
            || inventoryAmmoFrom.GetItem(_ammoType)==null)
            return false;
        else
            return true;
    }

    public string FormDamageDiapason(int flatBonus = 0)
    {
        return (damageRandomMultipler + damageAddition + flatBonus) + "-" + (damageRandomMultipler * damageRandomTo + damageAddition + flatBonus);
    }

    public override bool IsTheSameItem(Item itemToCompare)
    {
        if (!base.IsTheSameItem(itemToCompare))
            return false;

        if (itemToCompare is Weapon weaponToCompare
			&& _quality==_maxQuality
			&& weaponToCompare._quality==weaponToCompare._maxQuality
            && weaponToCompare.RangedAttack == _rangedAttack
            && weaponToCompare.Range == _range
            && weaponToCompare.DamageInfo == DamageInfo
            && weaponToCompare.APCost == _apCost
            && weaponToCompare.TwoHanded == _twoHanded
        )
            return true;
        else
            return false;
    }

    public override string ToJson()
    {
        WeaponJsonData itemJson = new WeaponJsonData()
        {
            q = _quality
        };
        return JsonUtility.ToJson(itemJson);
    }

    public override void FromJson(string jsonString)
    {
        WeaponJsonData jsonItem = JsonUtility.FromJson<WeaponJsonData>(jsonString);
        if (jsonItem == null)
        {
            Debug.Log("Error! Mistake on FromJson() in SrcWeapon (item is destroying)");
            Destroy(this);
        }
        _quality = jsonItem.q;
    }

    [Serializable]
    private class WeaponJsonData
    {
        public int q;
    }
}
