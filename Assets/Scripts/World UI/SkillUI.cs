using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private TextMeshProUGUI _countdownText;
    [SerializeField] private Image _minimalLine;
    [SerializeField] private Image _trainedLine;
    [SerializeField] private Image _improvedLine;
    [SerializeField] private Button _improveButton;
    [SerializeField] private Button _trainButton;
    private CharacterUI _characterUI;
    private Skills _characterSkills;
    private TimeSpan _countdown;

    public string Name => _name;
    public TextMeshProUGUI NameText => _nameText;
    public TextMeshProUGUI ValueText => _valueText;
    public TextMeshProUGUI CountdownText => _countdownText;
    public Image MinimalLine => _minimalLine;
    public Image TrainedLine => _trainedLine;
    public Image ImprovedLine => _improvedLine;
    public Button ImproveButton => _improveButton;
    public Button TrainButton => _trainButton;

    public void Init (string name, Skills characterSkills, CharacterUI characterUI)
    {
        _name = name;
        _characterSkills = characterSkills;
        _characterUI = characterUI;

        _nameText.text = name;

        //TODO ADD seting Images size

        ShowTrained();

        if (_characterSkills.IsPossibleToTrain(_name))
            _trainButton.interactable = true;
        else
            _trainButton.interactable = false;

        if (_characterSkills.IsPossibleToImprove(_name))
            _improveButton.interactable = true;
        else
            _improveButton.interactable = false;
    }

    public void Train () =>
        _characterSkills.Train(_name);
    public void Improve() =>
        _characterSkills.Improve(_name);

    public void ShowMinimum()
    {
        _valueText.text = _characterSkills.GetMinimalValue(_name).ToString();
    }
    public void ShowTrained()
    {
        _valueText.text = _characterSkills.GetSkillValue(_name).ToString();
    }
    public void ShowImroved()
    {
        _valueText.text = _characterSkills.GetImprovedValue(_name).ToString();
    }
    public void ShowMaximum ()
    {
        _valueText.text = _characterSkills.GetMaximalValue(_name).ToString();
    }
    public void StartCountdown()
    {

    }

    public void UpdateCountdown()
    {

    }
}
