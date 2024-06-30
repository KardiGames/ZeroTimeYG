using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatAction
{
    public const int RELOAD_AP_COST = 3;
	
	public string action;
    public CombatUnit subject;
    public CombatUnit target;
    //public CombatObject targetObject;
    public Item usedItem; 
    public int[] place = new int[2]; //[0] for X; [1] for Y
    public int apCost;
    private int turn;

    //For movie only
    public int DamageDone { get; private set; } = 0;
    public int TargetHPAfter { get; private set; }
    
    public bool Move(CombatUnit subj, int x, int y, int turn)
    {
        if (subj == null) return false;
        apCost = Location.map[x, y].AP;
        if (subj.SpendAP(apCost, true))
        {
            action = "move";
            place[0] = x;
            place[1] = y;
            subject = subj;
            this.turn = turn;
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool Attack (CombatUnit subj, CombatUnit obj, int turn)
    {
        if ((subj == null)||(obj==null)) 
            return false;
        Weapon weapon;
        if (subj.usesOffHand)
            weapon= subj.LeftHandWeapon;
        else
            weapon = subj.RightHandWeapon;

        if (subj.SpendAP(weapon.APCost, true))
        {
            CombatAction thisAttack = new CombatAction();
            thisAttack.turn = turn;
            thisAttack.action = "attack";
            thisAttack.subject = subj;
            thisAttack.target = obj;
            thisAttack.usedItem = weapon;
            thisAttack.apCost = weapon.APCost;
            subj.personalPlanningList.Add(thisAttack);
            return true;
        }
        else
            return false;
    }

    public static bool Wait(CombatUnit subj, int turn, int apCost=2)
    {
        if ((subj == null) || (apCost < 1))
            return false;

        if (subj.SpendAP(apCost, true))
        {
            CombatAction thisAction = new CombatAction();
            thisAction.turn = turn;
            thisAction.apCost = apCost;
            thisAction.action = "wait";
            thisAction.subject = subj;

            subj.personalPlanningList.Add(thisAction);
            return true;
        }
        else
            return false;
    }
	
	public static bool Reload (CombatUnit subj, int turn) {
		if (subj == null)
            return false;
		
		Weapon weapon;
        if (subj.usesOffHand)
            weapon= subj.LeftHandWeapon;
        else
            weapon = subj.RightHandWeapon;
		
		if (subj is CombatCharacter player 
			&& weapon.IsAbleToReload(player.Inventory)
            && subj.SpendAP(RELOAD_AP_COST, true))
		{
			CombatAction thisAction = new CombatAction();
			thisAction.turn = turn;
			thisAction.apCost=RELOAD_AP_COST;
			thisAction.action="reload";
            thisAction.subject = subj;
            thisAction.usedItem = weapon;
            subj.personalPlanningList.Add(thisAction);
            return true;
        }
        return false;
	}
	
    public static bool Exit(CombatUnit subj, int turn)
    {
        if (subj == null)
            return false;

        if (subj.SpendAP(subj.TotalAP, true))
        {
            CombatAction thisAction = new CombatAction();
            thisAction.turn = turn;
            thisAction.apCost = subj.TotalAP;
            thisAction.action = "exit";
            thisAction.subject = subj;

            subj.personalPlanningList.Add(thisAction);
            return true;
        }
        else
            return false;
    }

    public void Perform(BattleManager manager)
    {
        if (subject.Dead == true)
        {
            action = "skip";
            return;
        }


        if (action == "move")
        {
            bool checkList = Mathf.Abs(subject.pos[0] - place[0]) <= 1 && Mathf.Abs(subject.pos[1] - place[1]) <= 1;
            //bool checkList = (!(Mathf.Abs(subject.pos[0] - place[0]) > 1) || (Mathf.Abs(subject.pos[1] - place[1]) > 1)); //TODO Delete comment if works

            //TODO ADD cheching for unavailable place

            //Checking for tyle without characters]

            if (Location.IsBusy (place[0], place[1], manager))
            {
                    checkList = false;
                    Debug.Log("There is another character on the way");
            }
            checkList = checkList && subject.SpendAP(apCost);

            if (checkList)
            {
                manager._combatLog.Add(this);
                subject.pos[0] = place[0];
                subject.pos[1] = place[1];
            }
        }
        else if (action == "attack")
        {
            if (!(usedItem is Weapon usedWeapon))
            {
                Debug.Log("Error! Used item for attack is not a weapon ((");
                return;
            }

            if ((subject.RightHandWeapon!=usedWeapon)&&(subject.LeftHandWeapon != usedWeapon)) {
                Debug.Log("Error! You haven't this weapon to use !!");
                return;
            }

            int range = (usedWeapon.RangedAttack) ? usedWeapon.Range : 1;

            if (target == null)
            {
                //Find target from coordinates place[] and set a CombatCharacter or Object as target
            }
               
            if (range < Location.Distance(subject.pos, target.pos))
                return;

            if (usedWeapon.AmmoType != "" && !usedWeapon.TryToSpendAmmo())
                return;

            apCost = Mathf.Max(apCost, usedWeapon.APCost);

            if (subject.SpendAP(apCost))
            {
                manager._combatLog.Add(this);

                int hitChanse = Scripts.HitChanse(subject, target, usedWeapon);

                if (Random.Range(0, 100) < hitChanse)
                {
                    int damage = GetWeaponDamage (usedWeapon);
                    if (!usedWeapon.RangedAttack && subject._ai=="")
                        if (usedWeapon.TwoHanded)
                            damage += subject.MeleeDamageBonus*2;
                        else
                            damage += subject.MeleeDamageBonus;

					damage = damage * subject.GetSkillValue("Weapon damage")/100;
					if (damage <1)
						damage = 1;
                    target.TakeDamage(damage);
                    //print(cA.target.name + "'s got " + damage + " damage. Hit chance was " + hitChanse);
                    DamageDone = damage;
                    TargetHPAfter = target.HP;
                }
                else  
                {
                    //print(cA.subject.name + " have missed ((( HitChanse was " + hitChanse);
                    if (subject is CombatCharacter player && player._ai=="" && Location.Distance(player.pos, target.pos) <= (player.PE - 1) )
                        player.BoostSkill(usedWeapon.SkillName);
                }
            }
        }
        else if (action == "wait")
        {
            if (subject.SpendAP(apCost))
            {
                subject._bonusAC += apCost;
                manager._combatLog.Add(this);
            }
            else
                Debug.Log(subject.name + " can't wait anymore *(");
        }
        else if (action == "reload") 
        {
            if (usedItem is Weapon weapon
                && subject is CombatCharacter player
                && subject.SpendAP(apCost))
            {
                weapon.ReloadAmmo(player.Inventory);
                manager._combatLog.Add(this);
            }
            else
                Debug.Log(subject.name + " can't reload weapon for some reason (");
        } else if (action == "exit")
        {
            if (subject.SpendAP(apCost))
            {
                manager.ExitBattle();
            }
            else
                Debug.Log(subject.name + " can't exit battle. Not enough AP *(");
        }

        int GetWeaponDamage(Weapon weapon)
        {
            int summ = 0;
            (int multipler, int dice, int addition) damageTuple = weapon.DamageTuple;
            int counterOf1 = 0;
            for (int i = 0; i < damageTuple.multipler; i++)
            {
                int diceRoll = Random.Range(1, (damageTuple.dice + 1));
                summ += diceRoll;
                if (diceRoll == 1)
                    counterOf1++;
            }
            summ += damageTuple.addition;

            if ((counterOf1 >= (damageTuple.multipler / 2)) && subject is CombatCharacter player && player._ai == "")
                player.BoostSkill("Weapon damage");

            return summ;
        }
    }


}
