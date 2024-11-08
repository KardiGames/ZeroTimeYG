using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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
        if (!_saveData.TryLoadFromObject())
        {
            _saveData.CreateNewSave();
            if (!_saveData.TryLoadFromObject())
            {
                print("ERROR!!! Save wasn't loaded or correctly created ( You can't play :( ");
                return;
            }
            _worldUI.CreateNewCharacter();
        }
    }

    public void NesessaryAction () //Action for some button
    {
        WorldCharacter worldChar = GameObject.Find("PlayerCharacter").GetComponent<WorldCharacter>();
        worldChar.CollectExperience(worldChar.ExperienceToLevelUp/2+2);
        worldChar.GetComponent<Inventory>().TryToAdd(this, Item.GetItem("Pistol"));
        worldChar.GetComponent<Inventory>().TryToAdd(this, Item.GetItem("Knife"));
        worldChar.GetComponent<Inventory>().TryToAdd(this, Item.GetItem("Rifle"));
        worldChar.GetComponent<Inventory>().TryToAdd(this, Item.GetItem("Shotgun"));
        worldChar.GetComponent<Inventory>().TryToAdd(this, Item.GetItem("Sword"));
        worldChar.GetComponent<Inventory>().TryToAdd(this, Item.GetItem("Exoskeleton jacket"));
        GlobalUserInterface.Instance.ShowError("Here some Error\nJust for test...");
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
    public void EndBattle(float rewardPoins, Mine mine, bool dead)
    {
        _battleUI.gameObject.SetActive(false);
        _worldUI.gameObject.SetActive(true);
        _battleMap.SetActive(false);
        _worldMap.gameObject.SetActive(true);
        Camera.main.transform.parent = _player.transform;
        Camera.main.transform.localPosition = new Vector3(0, 0, Camera.main.transform.localPosition.z);
        
        //TODO Add here option to change dead to survived

        if (!dead)
            _player.CollectExperience((int)rewardPoins);
        else
            _player.CollectExperience((int)rewardPoins / 4);

        if (!dead)
            mine.Level=(int)(Mathf.Max(rewardPoins,mine.Level) / 2);
        else
            mine.Level = (int)(Mathf.Max(rewardPoins, mine.Level) / 4);

        mine.GetComponent<RewardManager>().GiveReward(rewardPoins);
        _worldUI.OpenPlayerInventory();
    }

}
