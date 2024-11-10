using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    //Technical use variables
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private BattleUserInterface _battleUI;
    [SerializeField] private MineNpcRewardData _npcSpawnData;
    [SerializeField] private GameObject _ñombatCharacterPrefab;
    private WorldCharacter _singlePlayer;

    //List of all Combat Characters in the scenes
    public List<CombatUnit> AllCombatCharacters= new(); //TODO Change list type to ICombatCharacter, incapsulate it & make it IEnumerable?

    //Combat logs
    public List<CombatAction> _combatLog = new List<CombatAction>();
    private List<CombatAction> _planningList = new List<CombatAction>();

    public string Status { get; private set; } = "starting";
    public float KillPoints { get; private set; } = 0;

    private Mine _mine;

    //Variables for "movie" status
    public int MovieAct { get; private set; } = 0;

    //Variables for "planning" status
    public int Turn { get; private set; } = 0;
    public int Player { get; private set; } = 0;

    //Public properties to access
    public BattleUserInterface BattleUI => _battleUI;

    

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

            _battleUI.SetPlaningButtons(false);
            Status = "performing";

            CreatePlanningList();

            for (int i=0; i< _planningList.Count; i++ )
                _planningList[i].Perform(this);
            if (Status == "performing")
                Status = "movie";
        }
    }

    public void StartBattle (Mine mine, WorldCharacter player)
    {
        _mine = mine;
        _planningList.Clear();
        _combatLog.Clear();
        Turn = 0;
        Player = 0;
        MovieAct = 0;
        KillPoints = 0;

        PlaceCombatCharacter(player);
        SpawnEnemies(0, 0f);
        _battleUI.RefreshLevelInfo(0, KillPoints, _mine.Level);

        bool readyForBattle = true;
        foreach (CombatUnit checkingCharacter in AllCombatCharacters)
        {
            if (checkingCharacter.attackZone == null) readyForBattle = false;
        }
        if (readyForBattle)
        {
            _battleUI.SetPlaningButtons(true);
            Status = "planning";
            AllCombatCharacters[Player].StartPlanning(_battleUI);
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
            if (cChar.AI != "" && cChar is NonPlayerCharacter npc)
            {
                if (npc.Dead)
                {
                    float killPoints = npc.Level * npc.Difficulty;
                    if (killPoints < _mine.Level && _singlePlayer != null)
                        killPoints += killPoints * _singlePlayer.Skills.GetSkillMultipler("Familiar paths");
                    KillPoints += killPoints;
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
            ExitBattle(true);
            return;
        }

        SpawnEnemies(enemiesNumber, enemiesDifficulty);
        _battleUI.RefreshLevelInfo(enemiesDifficulty, KillPoints, _mine.Level);
        Turn++;

        //Preparing next turn
        foreach (CombatUnit cC in AllCombatCharacters)
        {
            cC.ResetAP();
            cC.ResetPlanning();
        }

        Player = 0;
        Status = "planning";
        _battleUI.SetPlaningButtons(true);
        AllCombatCharacters[Player].StartPlanning(_battleUI);
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
        float difficultyProgressionSpeed = 0.5f; //must be > 0!!!
        int npcNumberCloseToExpected = 2; //1 - totally random, >1 - closer to expected, <1 - Not warking at all!! ((
        
        if (_mine==null)
        {
            print("Error! Mine is not set. Spawn impossible");
            return;
        }

        int mineLevel = Mathf.Max(_mine.Level, (int)KillPoints);

        float totalEnemiesDifficulty = difficultyProgressionSpeed * mineLevel; //Formula from TechAss.

        float expectedEnemiesNumber = Mathf.Pow((float)mineLevel, 1f/3f);  //Formula from TechAss.
        int minimumEnemiesNumber = (int)(expectedEnemiesNumber * 2f / 3f); //Formula from TechAss.
        int maximumEnemiesNumber = (int)(expectedEnemiesNumber * 4f / 3f); //Formula from TechAss.

        int enemiesSpawnNumber = 0;
        for (int i = 0; i < npcNumberCloseToExpected; i++)
            enemiesSpawnNumber += Random.Range(minimumEnemiesNumber, maximumEnemiesNumber + 1);
        enemiesSpawnNumber = enemiesSpawnNumber/npcNumberCloseToExpected - enemiesNumberInBattle;
        if (enemiesNumberInBattle == 0 && enemiesSpawnNumber < 1)
            enemiesSpawnNumber = 1;

        if (enemiesSpawnNumber == 0)
            return;

        enemiesSpawnNumber = Mathf.Min(enemiesSpawnNumber, (Location.xSize + Location.ySize));

        float eachEnemyDifficulty = ((float)totalEnemiesDifficulty-enemiesDifficultyInBattle) / enemiesSpawnNumber;

        NpcBlank[] potencialEnemies = _npcSpawnData.GetNpcList(mineLevel, _mine.MineType);
        int chosenEnemyIndex;
        int chosenEnemyLevel;
        for (int i=0; i<enemiesSpawnNumber; i++)
        {
            chosenEnemyIndex = Random.Range(0, potencialEnemies.Length);
            float enemyFloatLevel = (eachEnemyDifficulty / potencialEnemies[chosenEnemyIndex].difficulty);
            if (enemyFloatLevel > int.MaxValue)
                chosenEnemyLevel = int.MaxValue;
            else
                chosenEnemyLevel = (int)enemyFloatLevel;
            if (chosenEnemyLevel < 1)
                chosenEnemyLevel = 1;
            NonPlayerCharacter spawnedNPC = potencialEnemies[chosenEnemyIndex].Spawn(this, chosenEnemyLevel, Location.GetSpawnPosition());
            if (spawnedNPC != null)
                AllCombatCharacters.Add(spawnedNPC);
            else
                print("Error! Spawn was broken!");

            enemiesDifficultyInBattle += enemyFloatLevel;
            if (enemiesDifficultyInBattle > totalEnemiesDifficulty)
                break;
        }
    }

    internal void ExitBattle(bool death=false)
    {
        Status = "starting";
        AllCombatCharacters.ForEach(u => Destroy(u.gameObject));
        _gameManager.EndBattle(KillPoints, _mine, death);
    }

    private void PlaceCombatCharacter (WorldCharacter player)
    {
        CombatCharacter pc = Instantiate(_ñombatCharacterPrefab).GetComponent<CombatCharacter>();
        AllCombatCharacters.Add(pc);
        pc.SetCharacter(player, this);
        pc.SetPosition(new int[] { 1, 1 });
        pc.PrepareToFight();
    }

}

