using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatAction
{
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
    
    public bool Move(CombatUnit subj, int x, int y)
    {
        if (subj == null) return false;
        apCost = Location.map[x, y].AP;
        if (subj.SpendAP(apCost, true))
        {
            action = "move";
            place[0] = x;
            place[1] = y;
            subject = subj;
            turn = BattleUserInterface.Instance.BattleManager.Turn;
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool Attack (CombatUnit subj, CombatUnit obj)
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
            thisAttack.turn = BattleUserInterface.Instance.BattleManager.Turn;
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

    public static bool Wait(CombatUnit subj, int apCost=2)
    {
        if ((subj == null) || (apCost < 1))
            return false;

        if (subj.SpendAP(apCost, true))
        {
            CombatAction thisAction = new CombatAction();
            thisAction.turn = BattleUserInterface.Instance.BattleManager.Turn;
            thisAction.apCost = apCost;
            thisAction.action = "wait";
            thisAction.subject = subj;

            subj.personalPlanningList.Add(thisAction);
            return true;
        }
        else
            return false;
    }


    public static void Perform(List<CombatAction> pList)
    {
        List<CombatAction> log = BattleUserInterface.Instance.BattleManager._combatLog;

        if ((log.Count != 0)&&(log[(log.Count - 1)].turn >= BattleUserInterface.Instance.BattleManager.Turn))
        {
            Debug.Log("ERROR! Previous turn " + log[(log.Count - 1)].turn + ">= current turn " + BattleUserInterface.Instance.BattleManager.Turn);
            return;
        }
        else
        {
            //ADD here adding "Set" action for every alive combat character (or not)
        }

        foreach (CombatAction cA in pList)
        {
            if (cA.subject.Dead == true)
            {
                cA.action = "skip";
                continue;
            }


            if (cA.action == "move")
            {
                bool checkList = true;
                checkList = checkList && (!(Mathf.Abs(cA.subject.pos[0] - cA.place[0]) > 1) || (Mathf.Abs(cA.subject.pos[1] - cA.place[1]) > 1)); //Checking for close tyle

                //ADD cheching for unavailable place

                //Checking for tyle without characters]
                foreach (CombatUnit cC in BattleUserInterface.Instance.BattleManager.AllCombatCharacters)
                {
                    if ((cC.pos[0] == cA.place[0]) && (cC.pos[1] == cA.place[1]))
                    {
                        checkList = false;
                        Debug.Log("There is another character on the way");
                        Debug.Log("Moving to " + cA.place[0] + " " + cA.place[1] + " but " + cC.name + "is there");
                    }
                }
                checkList = checkList && cA.subject.SpendAP(cA.apCost);

                if (checkList)
                {
                    log.Add(cA);
                    cA.subject.pos[0] = cA.place[0];
                    cA.subject.pos[1] = cA.place[1];
                }

            }
            else if (cA.action == "attack")
            {
                if (!(cA.usedItem is Weapon usedWeapon))
                {
                    Debug.Log("Error! Used item for attack is not a weapon ((");
                    continue;
                }

                if ((cA.subject.RightHandWeapon!=usedWeapon)&&(cA.subject.LeftHandWeapon != usedWeapon)) {
                    Debug.Log("Error! You haven't this weapon to use !!");
                    continue;
                }

                int range = (usedWeapon.RangedAttack) ? usedWeapon.Range : 1;

                if (cA.target == null)
                {
                    //Find target from coordinates place[] and set a CombatCharacter or Object as target
                }
               
                bool checkList = true;

                if (range < Scripts.FindDistance(cA.subject.pos, cA.target.pos))
                    checkList=false;

                //TODO ADD check for obstacle, change target to object if u need
                
                if (!checkList)
                    continue;

                int apCost = Mathf.Max(cA.apCost, usedWeapon.APCost);

                if (checkList && cA.subject.SpendAP(apCost))
                {
                    log.Add(cA);

                    int hitChanse = Scripts.HitChanse(cA.subject, cA.target, usedWeapon);

                    if (Random.Range(0, 100) < hitChanse)
                    {
                        int damage = usedWeapon.Damage;
                        if (!usedWeapon.RangedAttack && cA.subject.ai=="")
                            damage += cA.subject.MeleeDamageBonus;
                        bool deadBeforeDamage = cA.target.Dead;
                        cA.target.TakeDamage(damage);
                        //print(cA.target.name + "'s got " + damage + " damage. Hit chance was " + hitChanse);
                        cA.DamageDone = damage;
                        cA.TargetHPAfter = cA.target.HP;
						if ((cA.target.Dead != deadBeforeDamage) && cA.subject is CombatCharacter player && player.ai!="" && cA.target is NonPlayerCharacter npcTarget)
						{
                            player.CollectExperience(npcTarget);
						}
                    }
                    else  
                    {
                        //print(cA.subject.name + " have missed ((( HitChanse was " + hitChanse);
                        if (cA.subject is CombatCharacter player && player.ai=="" && Location.Distance(player.pos, cA.target.pos) <= (player.PE - 1) )
                            player.BoostSkill(usedWeapon.SkillName);
                    }
                }
            }
            else if (cA.action == "wait")
            {
                if (cA.subject.SpendAP(cA.apCost))
                {
                    cA.subject.bonusAC += cA.apCost;
                    log.Add(cA);

                    
                }
                else
                    Debug.Log(cA.subject.name + " can't wait anymore *(");
            }
        }
    }

 
}
