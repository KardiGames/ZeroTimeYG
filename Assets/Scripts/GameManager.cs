using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SaveData _saveData;
    [SerializeField] private BattleManager _battleManager;
    private WorldUserInterface _worldUI;


    void Awake()
    {
        Item.LoadItems();
		Item.LoadItems();
        Location.LoadMap();
        _saveData.LoadSaveSystem();

        _worldUI = GameObject.Find("World UI").GetComponent<WorldUserInterface>();
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
            
        string winnerName="";
        int winnerScore=0;
        foreach (CombatUnit unit in _battleManager.AllCombatCharacters) {
            if (unit.ai=="" && unit is CombatCharacter cChar)
            {
                if (winnerName == "")
                    winnerName += cChar.charName;
                else
                    winnerName += " & " + cChar.charName;
                winnerScore += cChar.CombatExperience;
            }
            Destroy(unit.gameObject);
        }
    }
    public void StartBattle()
    {
        _battleManager.FirstTurn();
        NonPlayerCharacter.SpawnMiner(_battleManager, 1);
        
        bool readyCheck = true;
        foreach (CombatUnit checkingCharacter in _battleManager.AllCombatCharacters)
        {
            if (checkingCharacter.attackZone == null) readyCheck = false;
        }
        if (readyCheck)
        {
            _battleManager.AllCombatCharacters[_battleManager.Player].StartPlanning();
        } else
        {
            print("ERROR!!! Something wrong with Starting game. Game stopped. Investigate this");
        }
    }
}
