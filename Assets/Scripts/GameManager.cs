using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SaveData _saveData;
    [SerializeField] private BattleManager _battleManager;
    [SerializeField] private WorldUserInterface _worldUI;
    [SerializeField] private BattleUserInterface _battleUI;
    [SerializeField] private WorldMap _worldMap;
    [SerializeField] private GameObject _battleMap;
    [SerializeField] private WorldCharacter _player;

    private void Awake()
    {
		Item.LoadItems();
        Location.LoadBattleMap();
        _saveData.LoadSaveSystem();
        _saveData.LoadFromObject();
    }

    public void NesessaryAction () //Action for some button
    {
        WorldCharacter worldChar = GameObject.Find("PlayerCharacter").GetComponent<WorldCharacter>();
        worldChar.CollectExperience(worldChar.ExperienceToLevelUp/2+2);
        GameObject.Find("PlayerCharacter").GetComponent<Inventory>().TryToAdd(this, Item.GetItem("Pistol Blueprint"));
    }

    private void Start()
    {
        /* PREPARATION ONLY FOR FIRST FACTORY SAVE
        GameObject.Find("Factory").GetComponent<TaskTimer>().SetupTaskTimer(5, 2);
        Inventory factoryStorage = GameObject.Find("Factory").GetComponent<Inventory>();
        for (int i=0; i<8;i++)
            factoryStorage.TryToAdd(this, Item.GetItem("Resource"));
        factoryStorage.TryToAdd(this, Item.GetItem("Pistol Blueprint"));
        GameObject.Find("PlayerCharacter").GetComponent<Inventory>().TryToAdd(this, Item.GetItem("Pistol Blueprint"));
        GameObject.Find("PlayerCharacter").GetComponent<Inventory>().TryToAdd(this, Item.GetItem("Steel Armor"));

        _saveData.SaveBuilding(GameObject.Find("Factory").GetComponent<Factory>());
        _saveData.SaveBuilding(GameObject.Find("Mine").GetComponent<Mine>());
        _saveData.SaveToOject();
        
        
        */
        GameObject.Find("PlayerCharacter").GetComponent<Inventory>().TryToAdd(this, Item.GetItem("Bullet"));
    }
    public void StartBattle(Mine mine)
    {
        _worldUI.gameObject.SetActive(false);
        Camera.main.transform.parent = null;
        Camera.main.transform.position = new Vector3(0, 0, Camera.main.transform.position.z);
        _battleUI.gameObject.SetActive(true);
        _worldMap.gameObject.SetActive(false);
        _battleMap.SetActive(true);
        _battleManager.StartBattle(mine, _player);
    }
    public void EndBattle(float rewardPoins, Mine mine, bool death)
    {
        _battleUI.gameObject.SetActive(false);
        _worldUI.gameObject.SetActive(true);
        _battleMap.SetActive(false);
        _worldMap.gameObject.SetActive(true);
        Camera.main.transform.parent = _player.transform;
        Camera.main.transform.localPosition = new Vector3(0, 0, Camera.main.transform.localPosition.z);
        if (!death)
            _player.CollectExperience((int)rewardPoins);
        else
            _player.CollectExperience((int)rewardPoins / 4);

        if (!death)
            mine.Level=(int)(Mathf.Max(rewardPoins,mine.Level) / 2);
        else
            mine.Level = (int)(Mathf.Max(rewardPoins, mine.Level) / 4);

        mine.GetComponent<RewardManager>().GiveReward(rewardPoins);
        _worldUI.OpenPlayerInventory();
    }

}
