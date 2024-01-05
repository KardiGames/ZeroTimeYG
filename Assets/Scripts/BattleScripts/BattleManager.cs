using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    //Technical use variables
    [SerializeField] private GameManager _gameManager; //TODO delete this as soon as possible. Or not?
    [SerializeField] private BattleUserInterface _battleUI;
    [SerializeField] private MineNpcRewardData _npcSpawnData;
    [SerializeField] private GameObject _ñombatCharacterPrefab;

    //List of all Combat Characters in the scenes
    public List<CombatUnit> AllCombatCharacters= new(); //TODO Change list type to ICombatCharacter, incapsulate it & make it IEnumerable?

    //Combat logs
    public List<CombatAction> _combatLog = new List<CombatAction>();
    private List<CombatAction> _planningList = new List<CombatAction>();

    public string Status { get; private set; } = "starting";
    public int RewardPoints { get; private set; } = 0;
    public float CombatExperience { get; private set; } = 0;
    private Mine _mine;

    //Variables for "movie" status
    public int MovieAct { get; private set; } = 0;

    //Variables for "planning" status
    public int Turn { get; private set; } = 0;
    public int Player { get; private set; } = 0;

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

    public void CollectExperience(NonPlayerCharacter killedNPC)
    {
        CombatExperience += killedNPC.Level * killedNPC.Difficulty;

        BattleUserInterface.Instance.RefreshCharInfo(AllCombatCharacters[Player] as CombatCharacter);
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

    public void StartBattle (Mine mine)
    {
        _planningList.Clear();
        _combatLog.Clear();
        Turn = 0;
        Player = 0;
        MovieAct = 0;
        RewardPoints = 0;

        PlaceCombatCharacter();
        SpawnEnemies(0, 0f);

        bool readyForBattle = true;
        foreach (CombatUnit checkingCharacter in AllCombatCharacters)
        {
            if (checkingCharacter.attackZone == null) readyForBattle = false;
        }
        if (readyForBattle)
        {
            BattleUserInterface.Instance.SetPlaningButtons(true);
            Status = "planning";
            AllCombatCharacters[Player].StartPlanning();
        }
        else
        {
            print("ERROR!!! Something wrong with Starting game. Game stopped. Investigate this");
        }
    }
    private void NextTurn()
    {
        print("TURN IS FINISHED. Starting turn "+(Turn+1));

        //Cleaning map and spawning
        bool gameOver = true;
        int enemiesNumber = 0;
        float enemiesDifficulty=0f;

        foreach (CombatUnit cChar in AllCombatCharacters)
        {
            if (cChar._ai != "" && cChar is NonPlayerCharacter npc)
            {
                if (npc.Dead)
                {
                    Destroy(npc.gameObject);
                }
                else
                {
                    enemiesDifficulty += npc.Difficulty * npc.Level;
                    enemiesNumber++;
                }
            }
            else if (!cChar.Dead)
                gameOver = false;
        }

        if (gameOver)
        {
            Status = "starting";
            _gameManager.GameOver();
            return;
        }

        SpawnEnemies(enemiesNumber, enemiesDifficulty);
        BattleUserInterface.Instance.RefreshLevelInfo();
        Turn++;

        //Preparing next turn
        foreach (CombatUnit cC in AllCombatCharacters)
        {
            cC.ResetAP();
            cC.ResetPlanning();
        }

        Player = 0;
        Status = "planning";
        BattleUserInterface.Instance.SetPlaningButtons(true);
        AllCombatCharacters[Player].StartPlanning();
    }

    public void CreatePlanningList()
    {
        _planningList.Clear();
        List<(CombatAction action, float priority)> list = new List<(CombatAction action, float priority)>();
        foreach (CombatUnit unit in AllCombatCharacters)
        {
            float cost_amount = 0;
            foreach (CombatAction act in unit.personalPlanningList)
            {
                cost_amount += act.apCost;
                list.Add((act, cost_amount / (float)act.subject.TotalAP));
            }
            unit.personalPlanningList.Clear();
        }

        _planningList = list
            .OrderBy(tuple => tuple.priority)
            .ThenBy(tuple => tuple.action.subject.TotalAP)
            .Select(tuple => tuple.action)
            .ToList();
    }
    void SpawnEnemies(int enemiesNumberInBattle, float enemiesDifficultyInBattle)
    {
        int npcNumberCloseToExpected = 2; //1 - totally random, >1 - closer to expected, <1 - Not warking at all!! ((
        
        if (_mine==null)
        {
            print("Error! Mine is not set. Spawn impossible");
            return;
        }

        int mineLevel = Mathf.Max(_mine.Level, Mine.CalculateMineLevel(RewardPoints));

        float totalEnemiesDifficulty = 2 * Mathf.Pow(10, ((float)mineLevel / 20) - 1); //Formula from TechAss.

        float expectedEnemiesNumber = 1 + Mathf.Pow((float)mineLevel / 20, 2);  //Formula from TechAss.
        int minimumEnemiesNumber = (int)(expectedEnemiesNumber / 3f * 2f);
        int maximumEnemiesNumber = minimumEnemiesNumber * 2;

        int enemiesSpawnNumber = 0;
        for (int i = 0; i < npcNumberCloseToExpected; i++)
            enemiesSpawnNumber += Random.Range(minimumEnemiesNumber, maximumEnemiesNumber + 1);
        enemiesSpawnNumber = enemiesSpawnNumber/npcNumberCloseToExpected - enemiesNumberInBattle;
        if (enemiesNumberInBattle == 0 && enemiesSpawnNumber < 1)
            enemiesSpawnNumber = 1;

        if (enemiesSpawnNumber == 0)
            return;

        float eachEnemyDifficulty = (totalEnemiesDifficulty-enemiesDifficultyInBattle) / enemiesSpawnNumber;

        NpcBlank[] potencialEnemies = _npcSpawnData.GetNpcList(mineLevel, _mine.MineType);
        int chosenEnemyIndex;
        int chosenEnemyLevel;
        for (int i=0; i<enemiesSpawnNumber; i++)
        {
            chosenEnemyIndex = Random.Range(0, potencialEnemies.Length);
            chosenEnemyLevel = Mathf.Min((int)(eachEnemyDifficulty / potencialEnemies[chosenEnemyIndex].difficulty), 1);
            potencialEnemies[chosenEnemyIndex].Spawn(this, chosenEnemyLevel, Location.GetSpawnPosition());
        }
    }
    private void PlaceCombatCharacter ()
    {
        CombatCharacter pc = Instantiate(_ñombatCharacterPrefab).GetComponent<CombatCharacter>();

    }

}

