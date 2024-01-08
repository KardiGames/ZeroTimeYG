using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RewardManager: MonoBehaviour
{
    [SerializeField] private MineNpcRewardData _rewardData;
    [SerializeField] private Mine _mine;
    [SerializeField] private WorldUserInterface _worldUI;
    

    public Item[] GetTestReward(int winnerScore)
    {
        return Item.GetAllItems().Select(r => r.Clone()).ToArray();
    }
    private Item[] GetReward (float rewardPoints)
    {
        Dictionary<Item, float> potentialReward = _rewardData.GetItemsDictionary(Mine.CalculateMineLevel(rewardPoints), "Junk");
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

    private void GiveReward(Item[] currentReward, Inventory inventoryForReward)
    {
        inventoryForReward.ClearInventory(this);
        _worldUI.OpenTargetInventory(inventoryForReward);
        foreach(Item item in currentReward)
        {
            inventoryForReward.TryToAdd(this, item);
        }
    }

    internal void GiveReward(float rewardPoints, Inventory inventoryForReward) => GiveReward(GetReward(rewardPoints), inventoryForReward);
    internal void GiveReward(float rewardPoints) => GiveReward(rewardPoints, _mine.GetComponent<Inventory>());

}
