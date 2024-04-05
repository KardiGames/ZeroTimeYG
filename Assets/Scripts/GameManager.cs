using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SaveData _saveData;
    [SerializeField] private BattleManager _battleManager;
    [SerializeField] private WorldUserInterface _worldUI;
    [SerializeField] private BattleUserInterface _battleUI;
    [SerializeField] private WorldCharacter _player;

    private void Awake()
    {
		Item.LoadItems();
        Location.LoadMap();
        _saveData.LoadSaveSystem();
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
    }
    public void StartBattle(Mine mine)
    {
        _worldUI.gameObject.SetActive(false);
        _battleUI.gameObject.SetActive(true);
        _battleManager.StartBattle(mine, _player);
    }
    public void EndBattle(float rewardPoins, Mine mine, bool death)
    {
        _battleUI.gameObject.SetActive(false);
        _worldUI.gameObject.SetActive(true);
        if (!death)
            mine.SetLevel(rewardPoins / 2);
        else
        {
            rewardPoins /= 4;
            mine.SetLevel(rewardPoins);
        }
        _player.CollectExperience((int)rewardPoins);
        mine.GetComponent<RewardManager>().GiveReward(rewardPoins);
        _worldUI.OpenPlayerInventory();
    }

}
