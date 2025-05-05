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
    
    private List <Item> RewardList(Mine mine)
    {
        return new List<Item> (_rewardData.GetItemsDictionary(int.MaxValue, mine.MineType).Keys);
    }
    public List<Item> RewardList() => RewardList(_mine);
    
    private Item[] GetReward (float rewardPoints)
    {
        Dictionary<Item, float> potentialReward = _rewardData.GetItemsDictionary(rewardPoints, _mine.MineType);
        List<Item> reward = new List<Item>();
        Item chosenRewardItem=null;

        float weightsSum = 0f;
        foreach (var pricedItem in potentialReward) {
            if (pricedItem.Value!=0f)
                weightsSum+=1.0f/pricedItem.Value;
            else
                potentialReward.Remove (pricedItem.Key);
        }

        if (potentialReward.Count == 0 || weightsSum == 0f)
            return reward.ToArray();

        while (rewardPoints > 0)
        {
            float randomFloat=Random.value*weightsSum;
            foreach (KeyValuePair <Item, float> pricedItem in potentialReward)
            {
                float currentWeight =1.0f/pricedItem.Value; 
                if (randomFloat<currentWeight)
                {
                    chosenRewardItem=pricedItem.Key;
                    break;
                } else {
                    randomFloat-=currentWeight;
                }
            }
            rewardPoints -= potentialReward[chosenRewardItem];
            if (rewardPoints > 0)
                reward.Add(chosenRewardItem.Clone());
            chosenRewardItem=null;
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
