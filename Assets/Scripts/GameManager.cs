using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const float DEATH_POINTS_DIVIDER = 2f;

    [SerializeField] private SaveData _saveData;
    [SerializeField] private Localisation _localisation;
    [SerializeField] private BattleManager _battleManager;
    [SerializeField] private WorldUserInterface _worldUI;
    [SerializeField] private BattleUserInterface _battleUI;
    [SerializeField] private WorldMap _worldMap;
    [SerializeField] private GameObject _battleMap;
    [SerializeField] private WorldCharacter _player;

    private void Awake()
    {
		Item.LoadItems();
        _localisation.InitLanguage();
        Location.LoadBattleMap();
    }

#if UNITY_EDITOR
    private void Start() {
        print ("START started by UNITY_EDITOR");
        StartGame();
    }
#endif

    public void StartGame()
    {
        if (!_saveData.TryLoad()) {

            _saveData.CreateNewSave();
            if (!_saveData.TryLoad())
            {
                print("ERROR!!! Save wasn't loaded or correctly created ( You can't play :( ");
                return;
            }
            _worldUI.CreateNewCharacter();
        } else
            print ("UNITY has loaded game by 1 time");

        _worldUI.HideLoadingScreen();
        if (_player.Experience == 0)
            GlobalUserInterface.Instance.ShowBlackMessage("@Intro");
    }



    public void NesessaryAction () //Action for test button
    {
        WorldCharacter worldChar = GameObject.Find("PlayerCharacter").GetComponent<WorldCharacter>();
        //worldChar.CollectExperience(worldChar.ExperienceToLevelUp/2+2);
        Item item = Item.GetItem("Game Ending City Kit");
        //foreach (var item in Item.AllItems)
            worldChar.GetComponent<Inventory>().TryToAdd(this, item.Clone());

    }

    public void StartBattle(Mine mine)
    {
        if (mine == null)
        {
            GlobalUserInterface.Instance.ShowError("Failed to start a battle.");
            return;
        }
        if (!_player.ActionPoints.TrySpendAP(1))
        {
            GlobalUserInterface.Instance.ShowError("You need at least 1 AP to start a battle.");
            return;
        }

        _worldUI.gameObject.SetActive(false);
        Camera.main.transform.parent = null;
        Camera.main.transform.position = new Vector3(0, 0, Camera.main.transform.position.z);
        _battleUI.gameObject.SetActive(true);
        _worldMap.gameObject.SetActive(false);
        _player.gameObject.SetActive(false);
        _battleMap.SetActive(true);
        _battleManager.StartBattle(mine, _player);
    }
    public void EndBattle(float killPoints, Mine mine, bool dead)
    {
        _battleUI.gameObject.SetActive(false);
        _worldUI.gameObject.SetActive(true);
        _battleMap.SetActive(false);
        _worldMap.gameObject.SetActive(true);
        _player.gameObject.SetActive(true);
        Camera.main.transform.parent = _player.transform;
        Camera.main.transform.localPosition = new Vector3(0, 0, Camera.main.transform.localPosition.z);

        //TODO Add here option to change dead to survived

        if (dead)
            if (_player.AP >= ActionPoints.ADDITIONAL_DEATH_AP_COST)
                _player.ActionPoints.TrySpendAP(ActionPoints.ADDITIONAL_DEATH_AP_COST);
            else
                _player.ActionPoints.TrySpendAP(_player.AP);

        float mineLevel = Mathf.Max(killPoints, mine.Level);
        if (dead)
            mineLevel = mineLevel / DEATH_POINTS_DIVIDER * _player.Skills.GetSkillMultipler("Dangerous mining");
        mine.Level = (int)mineLevel;

        float experience = killPoints;
        if (dead)
            experience = experience / DEATH_POINTS_DIVIDER * _player.Skills.GetSkillMultipler("Dangerous mining");
        int levelBeforeExpCollecting = _player.Level; //TODO m.b. move it to Action and subscription
        _player.CollectExperience((int)experience);
        if (levelBeforeExpCollecting < _player.Level)
            _worldUI.ShowBigMessage("Level up!!!");

        _player.Equipment.BreakEquipment(dead);

        _saveData.SaveBuilding(mine);

        float rewardPoints = killPoints*(1 + _player.Skills.GetSkillMultipler("Attentive search"));
        if (dead)
            rewardPoints = rewardPoints / DEATH_POINTS_DIVIDER * _player.Skills.GetSkillMultipler("Dangerous mining");
        mine.GetComponent<RewardManager>().GiveReward(rewardPoints);
        _worldUI.OpenPlayerInventory();
        _worldUI.InformationPanelUI.ShowEndBattleInfo((int)killPoints, (int)mineLevel, (int)experience, (int)rewardPoints, dead);
    }
}
