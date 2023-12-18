using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "MineNpcRewardData", menuName = "MineData")]
public class MineNpcRewardData : ScriptableObject
{
    [SerializeField] private List<NpcData> _junkNpc;
    [SerializeField] private List<RewardData> _junkReward;

    [Serializable]
    private class NpcData
    {
        public int minMineLevel;
        public GameObject npc;
    }
    [Serializable]
    private class RewardData
    {
        public int minMineLevel;
        public Item rewardItem;
        public float rewardCost;
    }

    public GameObject[] GetNpcList (int mineLevel, string mineType)
    {
        return Get2ListsByMineType(mineType).Item1
            .Where(npc => npc.minMineLevel <= mineLevel)
            .Select(npc => npc.npc)
            .ToArray();
    }

    public Dictionary<Item, float> GetItemsDictionary(int mineLevel, string mineType)
    {
        Dictionary<Item, float> reward = new();

        foreach (RewardData item in Get2ListsByMineType(mineType).Item2)
        {
            if (item.minMineLevel <= mineLevel)
                reward.Add(item.rewardItem, item.rewardCost);
        }

        return reward;
    }

    private (List<NpcData>, List<RewardData>) Get2ListsByMineType (string mineType)
    {
        if (mineType=="Junk")
        {
            return (_junkNpc, _junkReward);
        }
        Debug.Log($"Error! Invalid Type Of Myne {mineType} requested");
        throw new ArgumentException();
        //return (new List<NpcData>(), new List<RewardData>());
    }
}
