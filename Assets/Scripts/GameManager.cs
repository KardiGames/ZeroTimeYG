using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SaveData saveData;
    private WorldUserInterface worldUI;
    private RewardManager rewardManager=new();

    void Awake()
    {
        Item.LoadItems();
		ScriptableItem.LoadItems();
        Location.LoadMap();
        saveData.LoadSaveSystem();

        worldUI = GameObject.Find("World UI").GetComponent<WorldUserInterface>();
    }

    public void GameOver()
    {
        Inventory inventoryForReward = GameObject.Find("Mine").GetComponent<Inventory>();
        if (worldUI.transform.Find("TargetInventory").gameObject.activeSelf == true)
        {
            inventoryForReward.ClearInventory(this);
            worldUI.transform.Find("TargetInventory").gameObject.SetActive(false);
            return;
        }
            
        string winnerName="";
        int winnerScore=0;
        foreach (CombatCharacter cChar in CombatCharacter.cCList) {
            if (cChar.ai=="")
            {
                if (winnerName == "")
                    winnerName += cChar.charName;
                else
                    winnerName += " & " + cChar.charName;
                winnerScore += cChar.Experience*(int)BattleManager.Difficulty;
            }
            Destroy(cChar.gameObject);
        }

        if (winnerScore == 0)
            winnerScore = Random.Range(1, (int)(1.5 * rewardManager.RewardCount));

        //TODO delete test realization & make normal
        //ScriptableItem[] currentReward = rewardManager.GetReward(winnerScore);
        //print($"Winner Score : {winnerScore} Reward Count : {currentReward.Length}");
        ScriptableItem[] currentReward = rewardManager.GetTestReward(winnerScore);
        worldUI.ShowBigMessage("Test Reward");

        worldUI.OpenTargetInventory(inventoryForReward);
        rewardManager.GiveReward(currentReward, inventoryForReward);
    }
    public void StartBattle()
    {
        BattleManager.FirstTurn();
        NonPlayerCharacter.SpawnMiner(1);
        
        bool readyCheck = true;
        foreach (CombatCharacter checkingCharacter in CombatCharacter.cCList)
        {
            if (checkingCharacter.attackZone == null) readyCheck = false;
        }
        if (readyCheck)
        {
            CombatCharacter.cCList[BattleManager.Player].StartPlanning();
        } else
        {
            print("ERROR!!! Something wrong with Starting game. Game stopped. Investigate this");
        }
    }
}
