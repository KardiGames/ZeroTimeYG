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

    void Awake()
    {
		Item.LoadItems();
        Location.LoadMap();
        _saveData.LoadSaveSystem();
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
    }

}
