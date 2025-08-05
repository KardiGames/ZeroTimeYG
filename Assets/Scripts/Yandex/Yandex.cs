using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Yandex : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void UnityReady();

    [DllImport("__Internal")]
    private static extern void SaveExtern(string jsonSave);

    [SerializeField] GameManager _gameManager;
    [SerializeField] TextMeshProUGUI _nameInput;

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

    public bool HaveSaveSlot ()
    {
        return true;
        //TODO add saveslots analyzer 
    }

    public void Save (string saveJson, bool force=false) {
        if (Offline)
            return;

        //TODO add here check for size
        //TOD0 add here counter check fo 5Minutes limit 
        SaveJsonData = saveJson;
        if (force || HaveSaveSlot())
            SaveExtern(saveJson);
    }

    public void SetSkillinfoName(string name) { if (name != "") _nameInput.text = name; }
}