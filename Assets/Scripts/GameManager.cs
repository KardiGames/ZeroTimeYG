using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private LinkedList<(string, int)> winners = new();

    void Awake()
    {
        Item.LoadItems();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameOver()
    {
        string winnerName="";
        int winnerScore=0;
        foreach (CombatCharacter cChar in CombatCharacter.cCList) {
            if (cChar.ai=="")
            {
                if (winnerName == "")
                    winnerName += cChar.charName;
                else
                    winnerName += " & " + cChar.charName;
                winnerScore += cChar.Experience*(int)Status.Difficulty;
            }
            Destroy(cChar.gameObject);
        }

        if (winners.Count == 0)
            winners.AddFirst((winnerName, winnerScore));
        else
        {
            LinkedListNode<(string, int)> currentNode = winners.First;
            while (currentNode!=null && winnerScore < currentNode.Value.Item2)
                currentNode = currentNode.Next;
            if (currentNode == null)
                winners.AddLast((winnerName, winnerScore));
            else
                winners.AddBefore(currentNode, (winnerName, winnerScore));
        }

        UserInterface.Instance.ShowBestScore(winners);
        UserInterface.Instance.ShowBigMessage("Game Over");
        UserInterface.Instance.ShowMainMenu();

        
    }
    public void StartGame()
    {
        Status.FirstTurn();
        NonPlayerCharacter.SpawnMiner(1);
        
        bool readyCheck = true;
        foreach (CombatCharacter checkingCharacter in CombatCharacter.cCList)
        {
            if (checkingCharacter.attackZone == null) readyCheck = false;
        }
        if (readyCheck)
        {
            CombatCharacter.cCList[Status.Player].StartPlanning();
        } else
        {
            print("ERROR!!! Something wrong with Starting game. Game stopped. Investigate this");
        }
    }
}
