using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class Yandex : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void RequestPlayerData();

    [DllImport("__Internal")]
    private static extern void SaveExtern(string jsonSave);

    [SerializeField] TextMeshProUGUI  _nameInput;
    [SerializeField] GameManager _gameManager;

    public bool SaveCompleted {get; private set;} = true;
    public bool Offline {get; private set;} = true;
    public string SaveJsonData {get; private set;}

    public void SetNewCharacterName (string playerName) {
        _nameInput.text = name;
    }

    public void LoadGame (string saveJsonData) {
        //TODO add here check for string
        Offline=false;
        SaveJsonData=saveJsonData;
        _gameManager.StartGame();
    }

    public void StartGameOffline () {
        Offline=true;
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

        SaveExtern(saveJson);
    }
    
    public void HelloButton()
    {
        RequestPlayerData();
    }

    public void SetSkillinfoName(string name) { if (name != "") _nameInput.text = name; }

}