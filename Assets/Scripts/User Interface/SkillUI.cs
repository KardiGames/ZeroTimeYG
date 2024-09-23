using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
	public DateTime FinishTime { get; private set; }
	[SerializeField] private string _name;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private TextMeshProUGUI _countdownText;
    [SerializeField] private RectTransform _emptyLine;
    [SerializeField] private RectTransform _minimalLine;
    [SerializeField] private RectTransform _trainedLine;
    [SerializeField] private RectTransform _improvedLine;
    [SerializeField] private Button _improveButton;
    [SerializeField] private Button _trainButton;

    private CharacterUI _characterUI;
    private Skills _characterSkills;


    public string Name => _name;
    public TextMeshProUGUI NameText => _nameText;
    public TextMeshProUGUI ValueText => _valueText;
    public TextMeshProUGUI CountdownText => _countdownText;
    /*public RectTransform EmptyLine => _emptyLine;
    public RectTransform MinimalLine => _minimalLine;
    public RectTransform TrainedLine => _trainedLine;
    public RectTransform ImprovedLine => _improvedLine;*/
    public Button ImproveButton => _improveButton;
    public Button TrainButton => _trainButton;

    public void Init (string name, Skills characterSkills, CharacterUI characterUI)
    {
        _name = name;
        _characterSkills = characterSkills;
        _characterUI = characterUI;

        _nameText.text = name;

        float lineWidth = _emptyLine.rect.width;
        int maximalValue = characterSkills.GetMaximalValue(name);

        Vector2 lineSizeDelta = _emptyLine.sizeDelta;
        lineSizeDelta.x = lineWidth * characterSkills.GetImprovedValue(name) / maximalValue;
        _improvedLine.sizeDelta = lineSizeDelta;
        lineSizeDelta.x = lineWidth * characterSkills.GetTrainedValue(name) / maximalValue;
        _trainedLine.sizeDelta = lineSizeDelta;
        lineSizeDelta.x = lineWidth * characterSkills.GetMinimalValue(name) / maximalValue;
        _minimalLine.sizeDelta = lineSizeDelta;

        //print($"{name} Min {characterSkills.GetMinimalValue(name)} Train {characterSkills.GetTrainedValue(name)} Imp {characterSkills.GetImprovedValue(name)} Max {characterSkills.GetMaximalValue(name)}");

        ShowTrained();

        if (_characterSkills.IsPossibleToTrain(_name))
            _trainButton.interactable = true;
        else
            _trainButton.interactable = false;

        if (_characterSkills.IsPossibleToImprove(_name))
            _improveButton.interactable = true;
        else
            _improveButton.interactable = false;
		
		_countdownText.text = "";
    }

    public void Train () {
        _characterSkills.Train(_name);
		_characterUI.ReloadSkills();
	}

    public void Improve(){
        _characterSkills.Improve(_name);
		_characterUI.ReloadSkills();		
	}

    public void ShowMinimal()
    {
        _valueText.text = _characterSkills.GetMinimalValue(_name).ToString();
    }
    public void ShowTrained()
    {
        _valueText.text = _characterSkills.GetTrainedValue(_name).ToString();
    }
    public void ShowImroved()
    {
        _valueText.text = _characterSkills.GetImprovedValue(_name).ToString();
    }
    public void ShowMaximum ()
    {
        _valueText.text = _characterSkills.GetMaximalValue(_name).ToString();
    }
    public void StartCountdown(DateTime finishTime)
    {
		FinishTime=finishTime;
    }
}
