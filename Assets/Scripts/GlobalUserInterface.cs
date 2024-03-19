using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GlobalUserInterface : MonoBehaviour
{
    [SerializeField] private BattleManager _battleManager;
    [SerializeField] private BattleUserInterface _battleUI;

    public static GlobalUserInterface Instance { get; private set; }
    public BattleManager BattleManager => _battleManager; //TODO This is crutch (( Much better to delete this
    public BattleUserInterface BattleUI => _battleUI; //TODO This is crutch (( Much better to delete this

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);
    }

    
}
