using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RewardManager: MonoBehaviour
{
    [SerializeField] private MineNpcRewardData _rewardData;
    [SerializeField] private Mine mine;
    
    public ScriptableItem[] GetTestReward(int winnerScore)
    {
        return ScriptableItem.GetAllItems().Select(r => r.Clone()).ToArray();
    }
    public ScriptableItem[] GetReward (float rewardPoints)
    {
        Dictionary<ScriptableItem, float> potentialReward = _rewardData.GetItemsDictionary(0, "Junk");
        List<ScriptableItem> reward = new();
        List<ScriptableItem> keys = new List<ScriptableItem>(potentialReward.Keys);
        ScriptableItem chosenRewardItem;

        while (rewardPoints>0)
        {

            chosenRewardItem = keys[Random.Range(0, keys.Count)];
            rewardPoints -= potentialReward[chosenRewardItem];
            if (rewardPoints>0)
                reward.Add(chosenRewardItem.Clone());
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
}
