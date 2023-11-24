using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RewardManager
{
    List<ScriptableItem> potentialReward;
    public int RewardCount
    {
        get
        {
            if (potentialReward == null)
                LoadReward();

            return potentialReward.Count;
        }
    }
    public ScriptableItem[] GetTestReward(int winnerScore)
    {
        if (potentialReward == null)
            LoadReward();
        return potentialReward.Select(r => r.Clone()).ToArray();
    }
    public ScriptableItem[] GetReward (int rewardPoints)
    {
        if (potentialReward == null)
            LoadReward();
        List<ScriptableItem> reward = new();
        int chosenRewardNumber;
        
        while (rewardPoints>0)
        {
            chosenRewardNumber=Random.Range(0, potentialReward.Count);
            reward.Add(potentialReward[chosenRewardNumber].Clone());
            rewardPoints -= chosenRewardNumber+1;
        }
        return reward.ToArray();
    }

    internal void GiveReward(ScriptableItem[] currentReward, Inventory inventoryForReward)
    {
        //inventoryForReward.
        foreach(ScriptableItem item in currentReward)
        {
            inventoryForReward.TryToAdd(this, item);
        }
    }
    private void LoadReward ()
    {
        if (potentialReward==null)
            potentialReward = new List<ScriptableItem>(ScriptableItem.GetAllItems());
    }


}
