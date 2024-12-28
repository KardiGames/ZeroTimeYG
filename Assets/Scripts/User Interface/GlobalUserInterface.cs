using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GlobalUserInterface : MonoBehaviour
{
    [SerializeField] private BattleManager _battleManager;
    [SerializeField] private BattleUserInterface _battleUI;
	[SerializeField] private SaveData _saveSystem; //TODO it was for one of the crutches. Delete this after check
    [SerializeField] private TextMeshProUGUI _errorText;
    [SerializeField] private TextMeshProUGUI _blackMessage;
    [SerializeField] private Localisation _localisation;
    public static GlobalUserInterface Instance { get; private set; }
    public BattleManager BattleManager => _battleManager; //TODO This is crutch (( Much better to delete this
    public BattleUserInterface BattleUI => _battleUI; //TODO This is crutch (( Much better to delete this
    public Localisation Localisation => _localisation;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);
    }

    public void ShowError(string errorText)
    {
        _errorText.transform.parent.gameObject.SetActive(true);
        _errorText.text = _localisation.Translate(errorText) ;
    }
	
	public void ShowBlackMessage (string message)
	{
        _blackMessage.transform.parent.gameObject.SetActive(true);
        _blackMessage.text = _localisation.Translate(message);
    }
}
