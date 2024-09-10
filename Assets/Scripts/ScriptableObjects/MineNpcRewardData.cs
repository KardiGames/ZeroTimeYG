using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "MineNpcRewardData", menuName = "MineData")]
public class MineNpcRewardData : ScriptableObject
{
    [SerializeField] private List<NpcData> _junkNpc;
    [SerializeField] private List<NpcData> _standartNpc;
    [SerializeField] private List<RewardData> _junkReward;
    [SerializeField] private List<RewardData> _poorResReward;
    [SerializeField] private List<RewardData> _simpleMaterialsReward;
    [SerializeField] private List<RewardData> _weaponT1Reward;
    [SerializeField] private List<RewardData> _rawResReward;
    [SerializeField] private List<RewardData> _armorT1Reward;
    [SerializeField] private List<RewardData> _highTechReward;
    [SerializeField] private List<RewardData> _weaponT2Reward;
    [SerializeField] private List<RewardData> _rareResReward;
    [SerializeField] private List<RewardData> _alienReward;
    [SerializeField] private List<RewardData> _armorT2Reward;
    [SerializeField] private List<RewardData> _weaponT3Reward;
    [SerializeField] private List<RewardData> _advancedAlienReward;
    [SerializeField] private List<RewardData> _technologicalReward;
    [SerializeField] private List<RewardData> _ammunitionReward;
    [SerializeField] private List<RewardData> _armorT3Reward;
    [SerializeField] private List<RewardData> _geckReward;

    [Serializable]
    private class NpcData
    {
        public int minMineLevel;
        public NpcBlank npc;
    }
    [Serializable]
    private class RewardData
    {
        public int minMineLevel;
        public Item rewardItem;
        public float rewardCost;
    }

    public NpcBlank[] GetNpcList (int mineLevel, string mineType)
    {
        return Get2ListsByMineType(mineType).Item1
            .Where(npc => npc.minMineLevel <= mineLevel)
            .Select(npc => npc.npc)
            .ToArray();
    }

    public Dictionary<Item, float> GetItemsDictionary(float mineLevel, string mineType) => GetItemsDictionary((int)mineLevel, mineType);
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
        else if (mineType== "PoorRes") {
            return (_standartNpc, _poorResReward);
        }
        else if (mineType == "SimpleMaterials")
        {
            return (_standartNpc, _simpleMaterialsReward);
        }
        else if (mineType == "WeaponT1")
        {
            return (_standartNpc, _weaponT1Reward);
        }
        else if (mineType == "RawRes")
        {
            return (_standartNpc, _rawResReward);
        }
        else if (mineType == "ArmorT1")
        {
            return (_standartNpc, _armorT1Reward);
        }
        else if (mineType == "HighTech")
        {
            return (_standartNpc, _highTechReward);
        }
        else if (mineType == "WeaponT2")
        {
            return (_standartNpc, _weaponT2Reward);
        }
        else if (mineType == "RareRes")
        {
            return (_standartNpc, _rareResReward);
        }
        else if (mineType == "Alien")
        {
            return (_standartNpc, _alienReward);
        }
        else if (mineType == "ArmorT2")
        {
            return (_standartNpc, _armorT2Reward);
        }
        else if (mineType == "WeaponT3")
        {
            return (_standartNpc, _weaponT3Reward);
        }
        else if (mineType == "AdvancedAlien")
        {
            return (_standartNpc, _advancedAlienReward);
        }
        else if (mineType == "Technological")
        {
            return (_standartNpc, _technologicalReward);
        }
        else if (mineType == "Ammunition")
        {
            return (_standartNpc, _ammunitionReward);
        }
        else if (mineType == "ArmorT3")
        {
            return (_standartNpc, _armorT3Reward);
        }
        else if (mineType == "Geck")
        {
            return (_standartNpc, _geckReward);
        }
        
        Debug.Log($"Error! Invalid Type Of Myne {mineType} requested");
        throw new ArgumentException();
        //return (new List<NpcData>(), new List<RewardData>());
    }
}
