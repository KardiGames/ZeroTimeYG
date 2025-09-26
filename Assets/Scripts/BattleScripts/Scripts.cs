using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scripts : MonoBehaviour
{
    public static List<int[]> Get6CloseTiles(CombatCharacter character)
    {
        return Get6CloseTiles(character.pos[0], character.pos[1]);
    }

     public static List<int[]> Get6CloseTiles(int x, int y)
    {
        List<int[]> list = new List<int[]>();
        int[] xOddCorrArray = new int[] { 1, 1, 1, 0, -1, 0 }; //for Odd row
        int[] xEvenCorrArray = new int[] { 0, 1, 0, -1, -1, -1 }; //for Even row
        int[] yCorrArray = new int[] { 1, 0, -1, -1, 0, 1 };
        
        for (int i=0; i<6; i++)
        {
            int[] tileCoords = new int[2];
            tileCoords[1] = y+yCorrArray[i];

            if (y%2==0)
            {
                tileCoords[0] = x + xEvenCorrArray[i];
            } else
            {
                tileCoords[0] = x + xOddCorrArray[i];
            }
            list.Add(tileCoords);
        }
        
        return list;
    }

    public static int[] TileToTarget (int[] from, int[] to)
    {
        int[] step = new int[2]; //Returnable coordinate array
        int dist = 999999; //Starting ultra large value
        
        List<int[]> closeTiles = Get6CloseTiles(from[0], from[1]);

        for (int i=0; i<6; i++)
        {
            int xCurrentDist = Mathf.Abs(closeTiles[i][0] - to[0]);
            int yCurrentDist= Mathf.Abs(closeTiles[i][1] - to[1]);
            int currentDist = xCurrentDist + yCurrentDist;
            if (currentDist < dist)
            {
                dist = currentDist;
                step[0] = closeTiles[i][0];
                step[1] = closeTiles[i][1];
            }
                
                
        }

        
        return step;
    }

    public static void Ai (NonPlayerCharacter bot, BattleManager battleManager, string ai="rat")
    {
        CombatUnit enemy=null;
        float priority = 0f;
        foreach (CombatUnit cC in battleManager.AllCombatCharacters) {
            if (cC.AI != ""||cC.Dead) continue;
            float currentPriority = (float)Location.Distance(bot.pos, cC.pos)*cC.HP/cC.MaxHP;
            if (enemy==null || currentPriority<priority) {
                enemy = cC;
                priority = currentPriority;
            }
        }
        if (enemy == null)
            return;
        //print("Bot for " + bot.name + " started and 'v chosen "+enemy.charName+" as enemy");

        bool enoughOD = true;
        int i = 0;
        
        while (enoughOD)
        {
            int distanceToTarget = Location.Distance(bot.planningPos, enemy.pos);
            if (distanceToTarget <= 0)
            {
                enoughOD = CombatAction.Attack(bot, enemy, battleManager.Turn); ;
            }
            else if (distanceToTarget == 1)
            {
                //TODO make AI more universal with switching weapons
                int attacksWithoutMove = bot.PlanningAP / bot.RightHandWeapon.APCost;
                if (attacksWithoutMove > 0)
                {
                    int attacksAfterMove = (bot.PlanningAP - Location.map[enemy.pos[0], enemy.pos[1]].AP) / bot.RightHandWeapon.APCost;
                    float chanseToMove = (float)attacksAfterMove / attacksWithoutMove;
                    if (UnityEngine.Random.value < chanseToMove)
                    {
                        enoughOD = Move(bot, enemy.pos[0], enemy.pos[1]);
                    }
                    else
                    {
                        enoughOD = CombatAction.Attack(bot, enemy, battleManager.Turn);
                    }
                } else
                {
                    enoughOD = Move(bot, enemy.pos[0], enemy.pos[1]);
                    if (!enoughOD && bot.PlanningAP > 0)
                        CombatAction.Wait(bot, battleManager.Turn, bot.PlanningAP);
                }
            }
            else
            {
                int[] moveCoordinates = Scripts.TileToTarget(bot.planningPos, enemy.pos);
				enoughOD=Move(bot, moveCoordinates[0], moveCoordinates[1]);
            }
            i++;
            if (i > 20) break;  
        }
		
		bool Move (NonPlayerCharacter bot, int x, int y) {
			    
				bot.personalPlanningList.Add(new CombatAction());
                bool movePlanned = bot.personalPlanningList[(bot.personalPlanningList.Count - 1)].Move(bot, x, y, battleManager.Turn);
                if (movePlanned) {
					//Moving plan position and sprite
					bot.planningPos[0] = x;
					bot.planningPos[1] = y;
				}
				else
					bot.personalPlanningList.RemoveAt(bot.personalPlanningList.Count - 1);
				return movePlanned;
		}
    }

    public static int HitChanse (CombatUnit subject, CombatUnit target, Weapon weapon)
    {
        int overPerceptionHitChenseDecrease = 10;
        /*if (subject.loc == null || target.loc == null) //TODO turn it back after repair Location class
            return 0;*/

        int range;
		if (weapon.RangedAttack)
        {
            range = weapon.Range;
        }
        else
            range = 1;

        int distance = Location.Distance(subject.planningPos, target.pos);
        int perseptionLenght = int.MaxValue;
        if (subject is CombatCharacter player)    
            perseptionLenght= player.PE-1;
		
        if (distance > range)
            return 0;
		
        int hitChanse = subject.GetSkillValue(weapon.SkillName);
        // ADD if (cA.target.ai != "") cA.target.CheckAC();
        hitChanse -= target.AC;
        hitChanse -= target._bonusAC;
		if (distance>perseptionLenght)
			hitChanse-=(distance-perseptionLenght)*overPerceptionHitChenseDecrease;
		if (hitChanse<0) 
			hitChanse=0;

        return hitChanse;
    }
	
	public static int HitChanse (CombatUnit subject, CombatUnit target) => subject.usesOffHand ? HitChanse (subject, target, subject.LeftHandWeapon) : HitChanse (subject, target, subject.RightHandWeapon);
    
    public static int Rating (WorldCharacter player, bool isGameFinished=false)
    {
        int levelMultipler = 100;
        int winGameMultipler = 2;
        int expectedMaxLevel = 30;
        
        int rating = 0;
        rating += player.Level * levelMultipler;

        foreach (string skill in Skills.InmplementedSkills)
        {
            rating += player.Skills.GetSkillValue(skill);
        }

        if (isGameFinished) {
            int maxRating = expectedMaxLevel * levelMultipler + Skills.InmplementedSkills.Count * Skills.MAXIMUM_TOTAL_SKILL;
            rating = maxRating * winGameMultipler - rating;
        }

        return rating;
    }
}
