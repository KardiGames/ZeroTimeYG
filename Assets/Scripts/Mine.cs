using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour, IWorldBuilding
{
    [SerializeField] SaveData _saveSystem;

    [field: SerializeField] public int X { get; private set; } = 0;
    [field: SerializeField] public int Y { get; private set; } = 0;
    [SerializeField] private string _name;
    [SerializeField] private int _level = 0;
    [SerializeField] private string _mineType = "";

    public string Name => _name;
    public int Level  => _level;
    public string MineType => _mineType;

    public void SetLevel(float rewardPoints)
    {
        _level = CalculateMineLevel(rewardPoints);
    }
    public static int CalculateMineLevel (float rewardPoints)
    {
        return rewardPoints>0 ? 
            (int)(20.0f * Mathf.Log10(rewardPoints)) 
            : 0;
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void FromJson(string jsonString)
    {
        JsonUtility.FromJsonOverwrite(jsonString, this);
    }


    public void ExitBuilding()
    {
        _saveSystem.SaveBuilding(this);
        _mineType = "";
        _name = "";
        X = 0;
        Y = 0;
    }
}
