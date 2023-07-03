using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatAction
{
    public string action;
    public CombatCharacter subject;
    public CombatCharacter target;
    //public CombatObject targetObject;
    public Item usedItem; 
    public int[] place = new int[2]; //[0] for X; [1] for Y
    public int apCost;
    private int turn;

    //For movie only
    public int DamageDone { get; private set; } = 0;
    public int targetHPAfter { get; private set; }
    
    public bool Move(CombatCharacter subj, int x, int y)
    {
        if (subj == null) return false;
        apCost = Location.map[x, y].AP;
        if (subj.SpendAP(apCost, true))
        {
            action = "move";
            place[0] = x;
            place[1] = y;
            subject = subj;
            turn = Status.Turn;
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool Attack (CombatCharacter subj, CombatCharacter obj)
    {
        if ((subj == null)||(obj==null)) 
            return false;
        Item weapon;
        if (subj.usesOffHand)
            weapon= subj.equipment[1];
        else
            weapon = subj.equipment[0];

        if (subj.SpendAP(weapon.apCost, true))
        {
            CombatAction thisAttack = new CombatAction();
            thisAttack.turn = Status.Turn;
            thisAttack.action = "attack";
            thisAttack.subject = subj;
            thisAttack.target = obj;
            thisAttack.usedItem = weapon;
            thisAttack.apCost = weapon.apCost;
            subj.personalPlanningList.Add(thisAttack);
            return true;
        }
        else
            return false;
        
    }

    public static bool Wait(CombatCharacter subj, int apCost=2)
    {
        if ((subj == null) || (apCost < 1))
            return false;

        if (subj.SpendAP(apCost, true))
        {
            CombatAction thisAction = new CombatAction();
            thisAction.turn = Status.Turn;
            thisAction.apCost = apCost;
            thisAction.action = "wait";
            thisAction.subject = subj;

            subj.personalPlanningList.Add(thisAction);
            return true;
        }
        else
            return false;
    }
    public static void CreatePlanningList(List<CombatAction> pList)
    {
        pList.Clear();
        List<ActionToCompare> listToSort = new();

        int spentAP=0;
        int subjectTotalAp;
        foreach (CombatCharacter cC in CombatCharacter.cCList)
        {
            spentAP=0;
            foreach (CombatAction plannedAction in cC.personalPlanningList)
            {
                spentAP += plannedAction.apCost;
                subjectTotalAp = plannedAction.subject.totalAP;
                listToSort.Add(new(plannedAction, (float)spentAP / subjectTotalAp, subjectTotalAp));
            }
        }

        listToSort.Sort();

        for (int i = 0; i < listToSort.Count; i++)
            pList.Add(listToSort[i].Action);

        foreach (CombatCharacter cChar in CombatCharacter.cCList)
            cChar.personalPlanningList.Clear();
    }
    private class ActionToCompare : System.IComparable<ActionToCompare>
    {
        public CombatAction Action { get; private set; }
        private float placeInTurn;
        private int totalAP;
        public ActionToCompare(CombatAction action, float placeInTurn, int totalAP)
        {
            Action = action;
            this.placeInTurn = placeInTurn;
            this.totalAP = totalAP;
        }
        public int CompareTo(ActionToCompare compAction)
        {
            if (compAction == null)
                return 1;
            if (placeInTurn > compAction.placeInTurn)
                return 1;
            if (placeInTurn < compAction.placeInTurn)
                return -1;
            return totalAP - compAction.totalAP;
        }
    }

    public static void Create2PlanningList(List<CombatAction> pList)
    {
        pList.Clear();

        int totalLists = CombatCharacter.cCList.Count;
        int emptyLists = 0;

        for (int i = 0; emptyLists < totalLists; i++)
        {
            emptyLists = 0;
            foreach (CombatCharacter cChar in CombatCharacter.cCList)
            {
                if (i < cChar.personalPlanningList.Count)
                {
                    pList.Add(cChar.personalPlanningList[i]);
                }
                else
                {
                    emptyLists++;
                }
            }
        }

        foreach (CombatCharacter cChar in CombatCharacter.cCList)
            cChar.personalPlanningList.Clear();
    }

    public static void Perform(List<CombatAction> pList)
    {
        List<CombatAction> log = Status.combatLog;

        if ((log.Count != 0)&&(log[(log.Count - 1)].turn >= Status.Turn))
        {
            Debug.Log("ERROR! Previous turn " + log[(log.Count - 1)].turn + ">= current turn " + Status.Turn);
            return;
        }
        else
        {
            //ADD here adding "Set" action for every life combat character (or not)
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
                foreach (CombatCharacter cC in CombatCharacter.cCList)
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
                Item weapon = cA.usedItem;
                if ((cA.subject.equipment[0]!=weapon)&&(cA.subject.equipment[1] != weapon)) {
                    Debug.Log("You haven't this weapon to use !!");
                    continue;
                }

                int range = (weapon.rangedAttack) ? weapon.Range : 1;

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

                int apCost = Mathf.Max(cA.apCost, weapon.apCost);

                if (checkList && cA.subject.SpendAP(apCost))
                {
                    log.Add(cA);

                    int hitChanse = Scripts.HitChanse(cA.subject, cA.target, cA.usedItem);

                    if (Random.Range(0, 100) < hitChanse)
                    {
                        int damage = weapon.Damage;
                        if (!weapon.rangedAttack && cA.subject.ai=="")
                            damage += cA.subject.MeleeDamageBonus;
                        bool deadBeforeDamage = cA.target.Dead;
                        cA.target.GetDamage(damage);
                        //print(cA.target.name + "'s got " + damage + " damage. Hit chance was " + hitChanse);
                        cA.DamageDone = damage;
                        cA.targetHPAfter = cA.target.HP;
						if ((cA.target.Dead != deadBeforeDamage) && cA.target.ai!="")
						{
							NonPlayerCharacter npcTarget = (NonPlayerCharacter)cA.target;
							cA.subject.GetExperience(npcTarget);
						}
                    }
                    else  
                    {
                        //print(cA.subject.name + " have missed ((( HitChanse was " + hitChanse);
                        if (cA.subject.ai=="" && Location.Distance(cA.subject.pos, cA.target.pos) <= (cA.subject.PE - 1) )//TODO Think may be delete this PE check if (especially if BoostSkill difficulty is high enough
                            cA.subject.BoostSkill(weapon.skillname);
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
