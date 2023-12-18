using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/NpcAttack")]
public class NpcAttack : Weapon
{
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