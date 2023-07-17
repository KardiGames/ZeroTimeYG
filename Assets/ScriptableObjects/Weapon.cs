using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Weapon", menuName = "Items/Weapon")]
public class Weapon : ScriptableItem
{
    [SerializeField]private bool rangedAttack = true;
    [SerializeField] private int damageRandomMultipler = 1; //3 for 3d6
    [SerializeField] private int damageRandomTo = 6; //1dX (10 for 1d10)
    [SerializeField] private int damageAddition = 0; //5 for 2d8+5
    [SerializeField] private int range = 2;
    [SerializeField] private int apCost = 5;
    [SerializeField] private string skillname = "";

    public long Amount { get => 1; }

    public string FormDamageDiapason(int flatBonus = 0)
    {
        return (damageRandomMultipler + damageAddition + flatBonus) + "-" + (damageRandomMultipler * damageRandomTo + damageAddition + flatBonus);
    }

    public void BoostDamage()
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
}
