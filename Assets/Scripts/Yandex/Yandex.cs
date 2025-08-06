using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Yandex : MonoBehaviour
{
    public static int SAVE_SIZE_LIMIT = 100;
    public static int SAVE_SIZE_WARNING = 90;
    private static int MINUTES_TO_RESET = 5;
    private static int FREE_SLOTS = 75;
    
    [DllImport("__Internal")]
    private static extern void UnityReady();

    [DllImport("__Internal")]
    private static extern void SaveExtern(string jsonSave);

    [SerializeField] private GameManager _gameManager;
    [SerializeField] private TextMeshProUGUI _nameInput;
    private List<int> _spentSlots = new List<int>();

    public bool SaveCompleted {get; private set;} = true;
    public bool Offline {get; private set;} = true;
    public string SaveJsonData { get; private set; } = "";

    public void SetNewCharacterName (string playerName) {
        _nameInput.text = name;
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
    }

    public void StartGameOffline () {
        print ("UNITY does StartGameOffline");
        Offline=true;
        _gameManager.StartGame();
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

    private void OnEnable()
    {
        Timer.Instance.EveryMinuteAction += ResetOldestSlots;
    }
    private void OnDisable()
    {
        Timer.Instance.EveryMinuteAction -= ResetOldestSlots;
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
}