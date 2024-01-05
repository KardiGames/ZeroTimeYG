using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SaveData _saveData;
    [SerializeField] private BattleManager _battleManager;
    [SerializeField] private WorldUserInterface _worldUI;
    [SerializeField] private BattleUserInterface _battleUI;


    void Awake()
    {
		Item.LoadItems();
        Location.LoadMap();
        _saveData.LoadSaveSystem();
    }

    public void GameOver()
    {
        Inventory inventoryForReward = GameObject.Find("Mine").GetComponent<Inventory>();
        if (_worldUI.transform.Find("TargetInventory").gameObject.activeSelf == true)
        {
            inventoryForReward.ClearInventory(this);
            _worldUI.transform.Find("TargetInventory").gameObject.SetActive(false);
            return;
        }
            
        foreach (CombatUnit unit in _battleManager.AllCombatCharacters) {
            Destroy(unit.gameObject);
        }
    }


    public void StartBattle(Mine mine)
    {
        _worldUI.gameObject.SetActive(false);
        _battleUI.gameObject.SetActive(true);
        _battleManager.StartBattle(mine);
    }
}
