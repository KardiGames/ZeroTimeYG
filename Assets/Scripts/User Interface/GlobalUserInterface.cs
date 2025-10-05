using System;
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
    [Header("Confirm panel")]
    [SerializeField] private GameObject _confirmPanel;
    [SerializeField] private TextMeshProUGUI _confirmHeader;
    [SerializeField] private TextMeshProUGUI _confirmQuestion;
    [SerializeField] private TextMeshProUGUI _confirmTrueText;
    [SerializeField] private TextMeshProUGUI _confirmFalseText;
    private event Action<bool> _onPlayerDecided;

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

    public void AskConfirmation(Action<bool> onPlayerDecidedAction, string question, string trueAnswer, string falseAnswer, string header)
    {
        if (onPlayerDecidedAction == null)
            return;
        _onPlayerDecided=onPlayerDecidedAction;
        _confirmQuestion.text = _localisation.Translate(question);
        _confirmTrueText.text = _localisation.Translate(trueAnswer);
        _confirmFalseText.text = _localisation.Translate(falseAnswer);
        _confirmHeader.text = _localisation.Translate(header);

        _confirmPanel.SetActive(true);
    }

    public void GetConfirmation (bool answer)
    {
        _confirmPanel.SetActive(false);
        _onPlayerDecided?.Invoke(answer);
        _onPlayerDecided = null;
    }
}
