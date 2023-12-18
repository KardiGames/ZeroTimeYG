using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RewardManager: MonoBehaviour
{
    [SerializeField] private MineNpcRewardData _rewardData;
    [SerializeField] private Mine mine;
    
    public Item[] GetTestReward(int winnerScore)
    {
        return Item.GetAllItems().Select(r => r.Clone()).ToArray();
    }
    public Item[] GetReward (float rewardPoints)
    {
        Dictionary<Item, float> potentialReward = _rewardData.GetItemsDictionary(0, "Junk");
        List<Item> reward = new();
        List<Item> keys = new List<Item>(potentialReward.Keys);
        Item chosenRewardItem;

        while (rewardPoints>0)
        {

            chosenRewardItem = keys[Random.Range(0, keys.Count)];
            rewardPoints -= potentialReward[chosenRewardItem];
            if (rewardPoints>0)
                reward.Add(chosenRewardItem.Clone());
        }

        return reward.ToArray();
    }

    internal void GiveReward(Item[] currentReward, Inventory inventoryForReward)
    {
        //inventoryForReward.
        foreach(Item item in currentReward)
        {
            inventoryForReward.TryToAdd(this, item);
        }
    }
}
