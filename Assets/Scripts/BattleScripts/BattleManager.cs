using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    //List of all Combat Characters in the scenes
    public List<CombatUnit> AllCombatCharacters= new(); //TODO Change list type to ICombatCharacter

    //Combat logs
    public List<CombatAction> _planningList = new List<CombatAction>();
    public List<CombatAction> _combatLog = new List<CombatAction>();

    public string Status { get; private set; } = "starting";

    //Variables for "movie" status
    public int MovieAct { get; private set; } = 0;

    //Variables for "planning" status
    public int Turn { get; private set; } = 0;
    public int Player { get; private set; } = 0;

    //Technical use variables
    [SerializeField] private GameManager gameManager; //TODO delete this as soon as possible. Or not?

    public void NextMovieAct ()
    {
        if (Status == "movie")
        {
            MovieAct++;
            if (MovieAct >= _combatLog.Count)
            {
                NextTurn();
            }
        }
    }
    public void NextPlayer()
    {
        AllCombatCharacters[Player].ResetPlanning();
        AllCombatCharacters[Player].StartPlanning(false);
        Player++;
        
        if (Player< AllCombatCharacters.Count)
        {
            AllCombatCharacters[Player].StartPlanning();
        } else
        {
            Player--;

            BattleUserInterface.Instance.SetPlaningButtons(false);
            Status = "performing";

            CreatePlanningList();

            CombatAction.Perform(_planningList);

            Status = "movie";

        }
    }

    public void FirstTurn()
    {
        _planningList.Clear();
        _combatLog.Clear();
        Turn = 0;
        Player = 0;
        MovieAct = 0;
        Status = "planning";
        BattleUserInterface.Instance.SetPlaningButtons(true);
    }
    private void NextTurn()
    {
        print("TURN IS FINISHED. Starting turn "+(Turn+1));

        //Cleaning map and spawning 
        int totalEnemiesLevel=0;
        int totalPlayersLevel=0;
        
        int deadSouls = CleanDeadBodies();
        if (totalPlayersLevel == 0)
        {
            Status = "starting";
            gameManager.GameOver();
            return;
        }

        SpawnEnemies(deadSouls);
        BattleUserInterface.Instance.RefreshLevelInfo();
        Turn++;

        foreach (CombatUnit cC in AllCombatCharacters)
        {
            cC.ResetAP();
            cC.ResetPlanning();
        }

        Player = 0;
        Status = "planning";
        BattleUserInterface.Instance.SetPlaningButtons(true);

        AllCombatCharacters[Player].StartPlanning();

        void SpawnEnemies(int minSpawnNumber)
        {
            if (minSpawnNumber < 1)
                return;
            int maxSpawnLevel = (totalPlayersLevel - totalEnemiesLevel) / minSpawnNumber;

            if (totalEnemiesLevel>1)
            {
                float recuceSpawnChanse = 1.0f / maxSpawnLevel;
                if (Random.value < recuceSpawnChanse)
                    minSpawnNumber--;
            }

            for (int i = 0; i < minSpawnNumber; i++)
            {
                int npcSpawnLevel = Mathf.Min(Random.Range(1, (totalPlayersLevel - totalEnemiesLevel + 1)), maxSpawnLevel);
                totalEnemiesLevel += npcSpawnLevel;
                NonPlayerCharacter.SpawnMiner(this, npcSpawnLevel);
            }

            if (totalEnemiesLevel < totalPlayersLevel)
            {
                float additionalSpawnChanse = 1.0f - ((float)totalEnemiesLevel / totalPlayersLevel);
                float spawnRandom = Random.value;
                if (spawnRandom < additionalSpawnChanse)
                {
                    int npcSpawnLevel = Mathf.Min(Random.Range(1, (totalPlayersLevel - totalEnemiesLevel + 1)), maxSpawnLevel);
                    NonPlayerCharacter.SpawnMiner(this, npcSpawnLevel);
                    print($"Was spawned additional NPC. Chanse was {additionalSpawnChanse}, random rolled {spawnRandom}");
                }
                else
                    print($"Additional NPC haven't spawned. Chanse was {additionalSpawnChanse}, random rolled {spawnRandom}");
            }

        }
        int CleanDeadBodies()
        {
            int cleanedBodies = 0;
            foreach (CombatUnit cChar in AllCombatCharacters)
            {
                if (cChar.ai == "")
                {
                    if (!cChar.Dead)
                    {
                        totalPlayersLevel += cChar.level;
                    }
                }
            else
                {
                    if (cChar.Dead)
                    {
                        Destroy(cChar.transform.gameObject);
                        cleanedBodies++;
                    }
                    else
                        totalEnemiesLevel += cChar.level;
                }
            }
            return cleanedBodies;
        }
    }

    public void CreatePlanningList()
    {
        _planningList.Clear();
        List<ActionToCompare> listToSort = new();

        int spentAP = 0;
        int subjectTotalAp;
        foreach (CombatUnit cC in AllCombatCharacters)
        {
            spentAP = 0;
            foreach (CombatAction plannedAction in cC.personalPlanningList)
            {
                spentAP += plannedAction.apCost;
                subjectTotalAp = plannedAction.subject.TotalAP;
                listToSort.Add(new(plannedAction, (float)spentAP / subjectTotalAp, subjectTotalAp));
            }
        }

        listToSort.Sort();

        for (int i = 0; i < listToSort.Count; i++)
            _planningList.Add(listToSort[i].Action);

        foreach (CombatUnit cChar in AllCombatCharacters)
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

}

