using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scripts : MonoBehaviour
{
    private void Start()
    {

    }

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

    public static int FindDistance (int[] from, int[] to)
    {
        int distance = 0;

        if ((from[0] == to[0]) && (from[1] == to[1]))
            return 0;

        int[] movingTile = new int[2];
        movingTile[0] = from[0];
        movingTile[1] = from[1];

        while ( !((movingTile[0] == to[0]) && (movingTile[1] == to[1])))
        {
            movingTile = TileToTarget(movingTile, to);
            //print("Current tile " + movingTile[0] + " " + movingTile[1]);
            distance++;
            if (distance == 55) break;
        }

        return distance;
    }

    public static void Ai (CombatCharacter bot, string ai="rat")
    {
        CombatCharacter enemy=null;
        float priority = 0f;
        foreach (CombatCharacter cC in CombatCharacter.cCList) {
            if (cC.ai != ""||cC.Dead) continue;
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
                enoughOD = CombatAction.Attack(bot, enemy);
            }
            else if (distanceToTarget == 1)
            {
                //TODO make AI more universal with switching weapons
                int attacksWithoutMove = bot.PlanningAP / bot.equipment[0].apCost;
                if (attacksWithoutMove > 0)
                {
                    int attacksAfterMove = (bot.PlanningAP - Location.map[enemy.pos[0], enemy.pos[1]].AP) / bot.equipment[0].apCost;
                    float chanseToMove = (float)attacksAfterMove / attacksWithoutMove;
                    if (Random.value < chanseToMove)
                    {
                        enoughOD = Move(bot, enemy.pos[0], enemy.pos[1]);
                    }
                    else
                    {
                        enoughOD = CombatAction.Attack(bot, enemy);
                    }
                } else
                {
                    enoughOD = Move(bot, enemy.pos[0], enemy.pos[1]);
                    if (!enoughOD && bot.PlanningAP > 0)
                        CombatAction.Wait(bot, bot.PlanningAP);
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
		
		bool Move (CombatCharacter bot, int x, int y) {
			    
				bot.personalPlanningList.Add(new CombatAction());
                bool movePlanned = bot.personalPlanningList[(bot.personalPlanningList.Count - 1)].Move(bot, x, y);
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

    public static int HitChanse (CombatCharacter subject, CombatCharacter target, Item weapon)
    {
        int overPerceptionHitChenseDecrease = 10;
        /*if (subject.loc == null || target.loc == null) //TODO turn it back after repair Location class
            return 0;*/

        int range;
		if (weapon.rangedAttack)
        {
            range = weapon.Range;
        }
        else
            range = 1;

        int distance = Location.Distance(subject.planningPos, target.pos);
		int perseptionLenght = subject.PE-1;
		
        if (distance > range)
            return 0;
		
        int hitChanse = subject.skills[weapon.skillname];
        // ADD if (cA.target.ai != "") cA.target.CheckAC();
        hitChanse -= target.AC;
        hitChanse -= target.bonusAC;
		if (distance>perseptionLenght)
			hitChanse-=(distance-perseptionLenght)*overPerceptionHitChenseDecrease;
		if (hitChanse<0) 
			hitChanse=0;

        return hitChanse;
    }
	
	public static int HitChanse (CombatCharacter subject, CombatCharacter target) => subject.usesOffHand ? HitChanse (subject, target, subject.equipment[1]) : HitChanse (subject, target, subject.equipment[0]);
        
}
