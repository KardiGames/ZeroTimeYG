using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Yandex : MonoBehaviour
{
    public const int SAVE_SIZE_LIMIT = 100;
    public const int SAVE_SIZE_WARNING = 90;
    public const string INNER_TOKEN = "Unity editor token";
    private const int MINUTES_TO_RESET = 5;
    private const int FREE_SLOTS = 75;
    
    [DllImport("__Internal")]
    public static extern void BuyVipExtern();
    [DllImport("__Internal")]
    private static extern void ConsumeLostPurchasesExtern();
    [DllImport("__Internal")]
    private static extern void ConsumeTokenExtern(string token);

    [DllImport("__Internal")]
    private static extern void UnityReady();

    [DllImport("__Internal")]
    private static extern void CallLoadingApiReadyExtern();

    [DllImport("__Internal")]
    private static extern void SaveExtern(string jsonSave);

    [DllImport("__Internal")]
    private static extern void ShowAdExtern();

    [DllImport("__Internal")]
    private static extern void RequestVipPriceExtern();

    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Localisation _localisation;
    [SerializeField] private SaveData _saveData;
    [SerializeField] private TextMeshProUGUI _nameInput;
    [SerializeField] private Button _buyVip;
    [SerializeField] ActionPoints _actionPoints;
    [SerializeField] private List<GameObject> _languageButtons;
    private List<int> _spentSlots = new List<int>();
    private event Action<bool> _onAdShown;
    private bool isStartCalled = false;
    private bool isGameStarted = false;
    public bool SaveCompleted {get; private set;} = true;
    public bool Offline {get; private set;} = true;
    public string SaveJsonData { get; private set; } = "";
    public string VipPriceText { get; private set; } = "";

    public void SetNewCharacterName (string playerName) {
        _nameInput.text = name;
    }
    
    public void SetVipPrice (string vipPrice)
    {
        if (VipPriceText != "")
        {
            print("UNITY got more then 1 Vip price! Didn't handled it. Error?");
            return;
        }
        VipPriceText = vipPrice;
    }

    public void LoadGame (string saveJsonData) {
        if (SaveJsonData != "")
        {
            print("UNITY doesn't LoadGame.");
            print(SaveJsonData);
            return;
        }
        Offline=false;
        SaveJsonData=saveJsonData;
        
        print ("UNITY does LoadGame");
        _gameManager.StartGame();
        ConsumeLostPurchasesExtern();
        RequestVipPriceExtern();
        
        isGameStarted = true;
        CallLoadingApiReady();
    }

    public void StartGameOffline () {
        print ("UNITY does StartGameOffline");
        Offline=true;
        _buyVip.interactable = false;
        _gameManager.StartGame();
        
        isGameStarted = true;
        CallLoadingApiReady();
    }

    public void SetNewSaveJson(SaveScrObj newSaveJson)
    {
        SaveJsonData = newSaveJson.Save;
    }

    public void Save (string saveJson, bool force=false) {
        int saveSize = saveJson.Length / 1000;
        
        if (Offline)
        {
            print("Not saving. Offline mod. " + saveSize + "K");
            return;
        }

        string logText = "Saving. " + saveSize + "K /";
        foreach (int slots in _spentSlots)
            logText += " " + slots;
        print(logText);

        SaveJsonData = saveJson;

        if (saveSize > SAVE_SIZE_LIMIT)
        {
            GlobalUserInterface.Instance.ShowError("Save hasn't happen. Save file is too large.");
            return;
        } else if (saveSize > SAVE_SIZE_WARNING) {
            GlobalUserInterface.Instance.ShowError("You are close to save file limit. Try to consume some items or drop them away after battle.");
        }

        if (force || HaveFreeSaveSlot())
        {
            SaveExtern(saveJson);
            SpendSaveSlot();
        }
    }

    public void SetSkillinfoName(string name) { if (name != "") _nameInput.text = name; }
     
    public void SetLanguage (string lang)
    {
        if (lang == "ru")
            _localisation.CurrentLanguage = "ru_ru";
        else
            _localisation.CurrentLanguage = "en_en";
            
        if (_languageButtons!=null)
        {
            foreach (var button in _languageButtons)
                button.SetActive(false);
        }
    }

    public void ShowAdForReward(Action<bool> onAdShown)
    {
        if (onAdShown == null)
        {
            GlobalUserInterface.Instance.ShowError(GlobalUserInterface.Instance.Localisation.Translate("Error #") + "3");
            return;
        }
        _onAdShown= onAdShown;
        ShowAdExtern();
    }

    public void AdShownCallback ()
    {
        bool stillDead = false;
        _onAdShown?.Invoke(stillDead);
        if (_onAdShown == null)
            print("Unity error. Unixpected AdShownCallback");
        _onAdShown = null;
    }
    public void AdDidntShowCallback ()
    {
        bool stillDead = true;
        if (_onAdShown == null)
            print("Unity error. Unixpected AdDidntShowCallback");
        _onAdShown?.Invoke(stillDead);
        _onAdShown = null;
    }

    public void VipBoughtCallback (string token)
    {
        _actionPoints.AddVipTime();
        _actionPoints.Restore();
        if (token != INNER_TOKEN)
        {
            print("Unity VIP token: " + token);
            ConsumeTokenExtern(token);
        }
        _saveData.Save(true);
    }

    private void OnEnable()
    {
        Timer.Instance.EveryMinuteAction += ResetOldestSlots;
    }
    private void OnDisable()
    {
        Timer.Instance.EveryMinuteAction -= ResetOldestSlots;
    }

    private void Start()
    {
        print("UNITY Start() called");
        isStartCalled = true;
        CallLoadingApiReady();
    }
    private bool HaveFreeSaveSlot()
    {
        int spentSlots = 0;
        foreach (int slots in _spentSlots)
        {
            spentSlots += slots;
        }
        return spentSlots<FREE_SLOTS;
    }

    private void SpendSaveSlot()
    {
        if (_spentSlots.Count < 1)
            _spentSlots.Add(0);
        _spentSlots[_spentSlots.Count - 1]++;
    }

    private void ResetOldestSlots ()
    {
        _spentSlots.Add(0);
        if (_spentSlots.Count>MINUTES_TO_RESET)
            _spentSlots.RemoveAt(0);
    }

    private void CallLoadingApiReady()
    {
        if (isGameStarted && isStartCalled)
        {
            CallLoadingApiReadyExtern();
        }
    }
}