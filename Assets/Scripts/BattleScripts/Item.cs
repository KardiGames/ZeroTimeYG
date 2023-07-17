using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ICloneable
{
    static private float rangeBalansingParametr = 1; //Multiplies max ranges of weapons & characters. Basicly 2 for FalloutPNP. 

    public static List<Item> items = new List<Item>();

    public string itemName;
    public bool rangedAttack = true;
    private int damageRandomTo = 6; //1dX (10 for 1d10)
    private int damageRandomMultipler = 1; //3 for 3d6
    private int damageAddition = 0; //5 for 2d8+5

    public int _range = 2;
    public int Range
    {
        get
        {
            return (int)(_range * (1 / rangeBalansingParametr));
        }
        set
        {
            _range = value;
        }
    }
    public int apCost = 5;



    public string skillname = "";

    // Start is called before the first frame update
    public static void LoadItems()
    {
        if (items.Count != 0)
            return;

        Item thisItem = new Item();
        thisItem.itemName = "Fist";
        thisItem.SetDamage(1, 4, 0);
        thisItem.Range = 1;
        thisItem.apCost = 3;
        thisItem.rangedAttack = false;
        thisItem.skillname = "unarmed";
        items.Add(thisItem);

        thisItem = new Item();
        thisItem.itemName = "Knife";
        thisItem.SetDamage(1, 10, 0);
        thisItem.Range = 1;
        thisItem.apCost = 3;
        thisItem.rangedAttack = false;
        thisItem.skillname = "melee";
        items.Add(thisItem);

        thisItem = new Item();
        thisItem.itemName = "Sword";
        thisItem.SetDamage(3, 8, 0);
        thisItem.Range = 1;
        thisItem.apCost = 3;
        thisItem.rangedAttack = false;
        thisItem.skillname = "melee";
        items.Add(thisItem);

        thisItem = new Item();
        thisItem.itemName = "Pistol";
        thisItem.SetDamage(1, 8, 3);
        thisItem.Range = 4;
        thisItem.apCost = 4;
        thisItem.skillname = "guns";
        items.Add(thisItem);

        thisItem = new Item();
        thisItem.itemName = "Rifle";
        thisItem.SetDamage(1, 10, 4);
        thisItem.Range = 7;
        thisItem.apCost = 5;
        thisItem.skillname = "guns";
        items.Add(thisItem);

        thisItem = new Item();
        thisItem.itemName = "Shootgun";
        thisItem.SetDamage(5, 6, 0);
        thisItem.Range = 3;
        thisItem.apCost = 5;
        thisItem.skillname = "guns";
        items.Add(thisItem);

        thisItem = new Item();
        thisItem.itemName = "Sniper rifle";
        thisItem.SetDamage(2, 20, 0);
        thisItem.Range = 10;
        thisItem.apCost = 8;
        thisItem.skillname = "guns";
        items.Add(thisItem);

        thisItem = new Item();
        thisItem.itemName = "Machinegun";
        thisItem.SetDamage(5, 6, 0);
        thisItem.Range = 5;
        thisItem.apCost = 6;
        thisItem.skillname = "guns";
        items.Add(thisItem);


    }

    public int Damage
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

    public void SetDamage(int randomMultipler = 1, int randomTo = 6, int addition = 0)
    {
        damageRandomMultipler = randomMultipler;
        damageRandomTo = randomTo;
        damageAddition = addition;
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

    public string FormDamageDiapason(int flatBonus = 0)
    {
        return (damageRandomMultipler + damageAddition + flatBonus) + "-" + (damageRandomMultipler * damageRandomTo + damageAddition + flatBonus);
    }

    public static Item GetItem(string name)
    {
        foreach (Item item in items)
        {
            if (item.itemName == name)
                return item;
        }
        return null;
    }

    public object Clone() => MemberwiseClone();
}
