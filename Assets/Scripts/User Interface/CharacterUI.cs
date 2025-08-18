using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    [SerializeField] private WorldCharacter _worldCharacter;

	[SerializeField] private SaveData _saveData;
	[SerializeField] private Localisation _localisation;
	[SerializeField] private InformationPanelUI _infoPanel;
	[SerializeField] private Button _skillsButton;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI ST; //Strength
	[SerializeField] private Button _improveSTButton;
    [SerializeField] private TextMeshProUGUI PE; //Perception
	[SerializeField] private Button _improvePEButton;
    [SerializeField] private TextMeshProUGUI EN; //Endurance
	[SerializeField] private Button _improveENButton;
    [SerializeField] private TextMeshProUGUI IN; //Intelligence
	[SerializeField] private Button _improveINButton;
    [SerializeField] private TextMeshProUGUI AG; //Agility
	[SerializeField] private Button _improveAGButton;
	[SerializeField] private TextMeshProUGUI _AP; //Action points
	[SerializeField] private TextMeshProUGUI _attributePoints;
	[SerializeField] private TextMeshProUGUI _unspentSkillPoints;
	
    [SerializeField] private List<string> _skills = new ();
	//TODO change _skills to appropriate list of Skills class
	[SerializeField] private List<SkillUI> _skillUIPrefabPool;
    private Dictionary<string, SkillUI> _skillsUI = new Dictionary<string, SkillUI>();
	private List<SkillUI> _timers=new();

	public InformationPanelUI InfoPanel => _infoPanel;
    public void OnEnable () {
        _skillsButton.interactable=false;
		_localisation.OnLanguageChangedEvent += Refresh;
		if (_worldCharacter == null) {
		    gameObject.SetActive(false);			
			return;
		}

		ShowNameLevelText();
		RefreshAP();

		ReloadAttributes();
		
        _worldCharacter.Skills.TaskTimer.CompletePastTasks();
		_saveData.SaveCharacter();

		ReloadSkills();
    }
	
	public void ShowNameLevelText() {
		_nameText.text = _worldCharacter.CharacterName+" ("+_worldCharacter.Level+Translate(" level")+") "+_worldCharacter.Experience+"/"+_worldCharacter.ExperienceToLevelUp+Translate(" exp");
	}

	public void ReloadSkills() {
		if (_worldCharacter==null)
			return;
		
		if (_skillsUI.Count != _skills.Count)
            GetObjectsFromPool();
		
		foreach (string skill in _skills)
        {
            _skillsUI[skill].Init(skill, _worldCharacter.Skills, this);
        }
		
		if (_timers.Count != 0) {
			_timers.Clear();
			Timer.Instance.EverySecondAction-=UpdateTimers;
		}
       
        foreach (TaskByTimer trainingSkill in _worldCharacter.Skills.TaskTimer.GetAllItems())
        {
            _skillsUI[trainingSkill.TaskTag].TrainButton.interactable = false;
            if (trainingSkill.IsStarted())
            {
                _skillsUI[trainingSkill.TaskTag].StartCountdown(trainingSkill.FinishTime);
				if (_timers.Count == 0) 
					Timer.Instance.EverySecondAction+=UpdateTimers;
				_timers.Add(_skillsUI[trainingSkill.TaskTag]);
            }
        }
		if (_timers.Count > 0)
			UpdateTimers();

		_unspentSkillPoints.text = Translate("You have ")+_worldCharacter.Skills.UnspentPoints+Translate(" unspent Skill Points");
	}
	
	private void ReloadAttributes () {
		if (_worldCharacter==null)
			return;
		
		ST.text=_worldCharacter.ST.ToString();
		if (_worldCharacter.IsImprovable(_worldCharacter.ST))
			_improveSTButton.interactable=true;
		else
			_improveSTButton.interactable=false;
		
		PE.text=_worldCharacter.PE.ToString();
		if (_worldCharacter.IsImprovable(_worldCharacter.PE))
			_improvePEButton.interactable=true;
		else
			_improvePEButton.interactable=false;
		
		EN.text=_worldCharacter.EN.ToString();
		if (_worldCharacter.IsImprovable(_worldCharacter.EN))
			_improveENButton.interactable=true;
		else
			_improveENButton.interactable=false;
		
		IN.text=_worldCharacter.IN.ToString();
		if (_worldCharacter.IsImprovable(_worldCharacter.IN))
			_improveINButton.interactable=true;
		else
			_improveINButton.interactable=false;
		
		AG.text=_worldCharacter.AG.ToString();
		if (_worldCharacter.IsImprovable(_worldCharacter.AG))
			_improveAGButton.interactable=true;
		else
			_improveAGButton.interactable=false;
		
		_attributePoints.text= Translate("You have ") + _worldCharacter.AttributePoints + Translate(" unspent Attribute Points");
	}

	private void UpdateTimers () {
		bool existFinishedTimer=false;
		
		foreach (SkillUI timer in _timers) {
			TimeSpan countdown = timer.FinishTime - DateTime.Now;
			if (countdown.TotalSeconds >0) {
				string timerText = "";
				if (countdown.TotalDays >= 1)
					timerText += (int)countdown.TotalDays + "d ";
				timerText += $"{countdown.Hours}:{countdown.Minutes}:{countdown.Seconds}";
				timer.CountdownText.text = timerText;
			} else
				existFinishedTimer=true;
		}
		
		if (existFinishedTimer) {
			_worldCharacter.Skills.TaskTimer.OnTaskOrTimerChanged+=ReloadSkills;
			_worldCharacter.Skills.TaskTimer.CompletePastTasks();
			_worldCharacter.Skills.TaskTimer.OnTaskOrTimerChanged-=ReloadSkills;
		}
	}
	
	private void OnDisable () {
		_localisation.OnLanguageChangedEvent -= Refresh;
		if (_timers.Count != 0) {
			Timer.Instance.EverySecondAction-=UpdateTimers;
			_timers.Clear();
		}
		_skillsButton.interactable=true;
	}

    private void GetObjectsFromPool ()
    {
        foreach (var element in _skillsUI)
            element.Value.gameObject.SetActive(false);
        _skillsUI.Clear();

        int poolElement = 0;
        foreach (string skill in _skills)
        {
            if (poolElement < _skillUIPrefabPool.Count)
            {
                _skillsUI[skill] = _skillUIPrefabPool[poolElement];
                _skillUIPrefabPool[poolElement].gameObject.SetActive(true);
                poolElement++;
            }
            else
                break;
        }
    }

	private void Refresh()
    {
		ShowNameLevelText();
		RefreshAP();
		ReloadAttributes();
		ReloadSkills();
	}

	private void RefreshAP ()
    {
		_AP.text = Translate("AP") + " " + _worldCharacter.AP+"/"+_worldCharacter.ActionPoints.MaxValue;
	}

	private string Translate(string text) => _localisation.Translate(text); 
}
