using System;
using System.Collections.Generic;
using UnityEngine;

public class Skills : MonoBehaviour, ITimerable
{
	public const int WORKING_SKILLS_NUMBER = 11;
	public const int MAXIMUM_TOTAL_SKILL = 200;
	private const int SKILL_TO_COST_INCREASE = 50;
	private const int MAXIMUM_IMPROVED_BY_LEVEL = 5;
	private const float TIME_TO_TRAIN_MULTIPLER = 4.3817804600413289076557582624064f;


	[SerializeField] private WorldCharacter _playerCharacter;
	[SerializeField] private TaskTimer _skillsTimer;

	private static Dictionary<string, int> _skillNumbers = new Dictionary<string, int>() {
		{ "Unarmed",0 },
		{ "Melee",1 },
		{ "Pistols",2 },
		{ "Rifles",3 },
		{ "Heavy",4 },
		{ "Reserve 1",5 },
		{ "Weapon damage",6 },
		{ "Beam damage",7 },
		{ "High-tech weapon damage",8 },
		{ "Improved damage resistance",9 },
		{ "Reserve 2",10 },
		{ "Traveling",11 },
		{ "Road traveling",12 },
		{ "World exploring",13 },
		{ "Reserve 3",14 },
		{ "Production",15 },
		{ "Production efficiency",16 },
		{ "Production security",17 },
		{ "Mass production",18 },
		{ "Time management",19 },
		{ "Reserve4",20 },
		{ "Mining",21 },
		{ "Attentive search",22 },
		{ "Dangerous mining",23 },
		{ "Familiar paths",24 },
		{ "Reserve 5",25 },
		{ "Science research",26 },
		{ "Deep scripting",27 },
		{ "Science experience",28 },
		{ "Research speed",29 },
		{ "Scripting speed",30 },
		{ "Reserve 6",31 }
	};

	private static List<Func<WorldCharacter, int>> _minimalSkills = new List<Func<WorldCharacter, int>>() {
		(WorldCharacter c) => 30 + (2 * (c.ST + c.AG)), //"Unarmed" = 0,
		(WorldCharacter c) => 20 + (2 * (c.ST + c.AG)), //"Melee"=1, 
		(WorldCharacter c) => 10 + 3 * c.AG + c.PE,//"Pistols"=2,
		(WorldCharacter c) => 5 + (2 * (c.PE + c.AG)),//"Rifles"=3,
		(WorldCharacter c) => 0 + 2*c.ST + c.EN + c.PE,//"Heavy"=4, 
		(WorldCharacter c) => 0,//"Reserve 1"=5,
		(WorldCharacter c) => 20+(2*(c.IN+c.PE)),//"Weapon damage"=6,
		(WorldCharacter c) => 5 + 2*c.IN + c.AG + c.PE,//"Beam damage"=7,
		(WorldCharacter c) => 10+(2*c.IN),//"High-tech weapon damage"=8,
		(WorldCharacter c) => 0+ c.IN + c.EN,//"Improved damage resistance"=9,
		(WorldCharacter c) => 0,//"Reserve 2"=10,
		(WorldCharacter c) => 10+3*c.EN + c.PE,//"Traveling"=11,
		(WorldCharacter c) => 20+ 4*c.EN,//"Road traveling"=12,
		(WorldCharacter c) => 10+ c.IN+c.EN,//"World exploring"=13,
		(WorldCharacter c) => 0,//"Reserve 3"=14,
		(WorldCharacter c) => 10+2*c.IN+c.EN,//"Production"=15,
		(WorldCharacter c) => 0+2*(c.IN+c.EN),//"Production efficiency"=16,
		(WorldCharacter c) => 10+2*(c.IN+c.PE),//"Production security"=17,
		(WorldCharacter c) => 10+2*(c.IN+c.ST),//"Mass production"=18,
		(WorldCharacter c) => 5+c.IN+c.AG,//"Time management"=19,
		(WorldCharacter c) => 0,//"Reserve4"=20,
		(WorldCharacter c) => 15+2*(c.ST + c.EN),//"Mining"=21,
		(WorldCharacter c) => 10+2*(c.PE + c.IN),//"Attentive search"=22,
		(WorldCharacter c) => 0+3*c.EN + c.IN,//"Dangerous mining"=23,
		(WorldCharacter c) => 0+2* (c.IN + c.PE),//"Familiar paths"=24,
		(WorldCharacter c) => 0,//"Reserve 5"=25,
		(WorldCharacter c) => 0+2* c.IN,//"Science research"=26,
		(WorldCharacter c) => 0+2* c.IN + c.PE,//"Deep scripting"=27,
		(WorldCharacter c) => 0+2* c.IN,//"Science experience"=28,
		(WorldCharacter c) => 0+2* c.IN,//"Research speed"=29,
		(WorldCharacter c) => 0+2* c.IN+c.PE,//"Scripting speed"=30,
		(WorldCharacter c) => 0,//"Reserve 6"=31
	};

	[SerializeField] private List<int> _trained;
	[SerializeField] private List<int> _untrained;
	private int _unspentPoints;

	public TaskTimer TaskTimer => _skillsTimer;
	public int UnspentPoints => _unspentPoints;
	private int SkillPointEachLevel => WORKING_SKILLS_NUMBER+_playerCharacter.IN+_playerCharacter.Level;

	public float GetSkillMultipler(string skillName)
    {
		return (float)GetSkillValue(skillName)/100.0f;
	}
		
	public int GetSkillValue (string skillName) =>
		GetTrainedValue(GetSkillNumber(skillName));

	private int GetTrainedValue(int skillNumber)
	{
		return (int)(_minimalSkills[skillNumber](_playerCharacter)
			+ _trained[skillNumber]);
	}

	public int GetMinimalValue (string skillName)
    {
		return _minimalSkills[GetSkillNumber(skillName)](_playerCharacter);
    }

	public int GetImprovedValue (string skillName)
    {
		return GetImprovedValue(GetSkillNumber(skillName));
	}

	private int GetImprovedValue (int skillNumber)
    {
		return _minimalSkills[skillNumber](_playerCharacter) + _trained[skillNumber] + _untrained[skillNumber];

    }
	public int GetMaximalValue(string skillName)
	{
		int skillNumber = GetSkillNumber(skillName);
		return Mathf.Min(_minimalSkills[skillNumber](_playerCharacter)+_playerCharacter.Level*MAXIMUM_IMPROVED_BY_LEVEL, MAXIMUM_TOTAL_SKILL);
	}

	public void LevelUp()
    {
		_unspentPoints += SkillPointEachLevel;
    }

	public bool IsPossibleToImprove(string skillName) =>
		IsPossibleToImprove(GetSkillNumber(skillName));

	public bool IsPossibleToImprove(int skillNumber)
    {
		if (skillNumber < 0 || skillNumber > _skillNumbers.Count)
			return false;
		
        if (_unspentPoints >= SkillPointsToImprove(skillNumber)
				&& (_playerCharacter.Level * MAXIMUM_IMPROVED_BY_LEVEL) > (_trained[skillNumber]+_untrained[skillNumber])
                && GetImprovedValue(skillNumber) < MAXIMUM_TOTAL_SKILL)
            return true;
        else
            return false;
    }
	public void Improve(string skillName) =>
		Improve(GetSkillNumber(skillName));
	private void Improve (int skillNumber)
    {
		if (!IsPossibleToImprove(skillNumber))
			return;

		_unspentPoints -= SkillPointsToImprove(skillNumber);
		_untrained[skillNumber]++;
    }

	public int SkillPointsToImprove (int skillNumber) =>
		(_trained[skillNumber] + _untrained[skillNumber]) / SKILL_TO_COST_INCREASE + 1;

	public bool IsPossibleToTrain(string skillName) =>
		IsPossibleToTrain(GetSkillNumber(skillName));
	private bool IsPossibleToTrain (int skillNumber)
    {
		return
			_untrained[skillNumber] > 0
			&& GetTrainedValue(skillNumber) < MAXIMUM_TOTAL_SKILL
			&& _skillsTimer.IsPossibleToAdd();

	}
	public void Train (string skillName)
    {
		int skillNumber = GetSkillNumber(skillName);
		if (IsPossibleToTrain(skillNumber))
		{
			int timeToTrain = (int)(Mathf.Pow(TIME_TO_TRAIN_MULTIPLER * (_trained[skillNumber] + 1), 2) / _playerCharacter.IN);
			_skillsTimer.AddTask(this, timeToTrain, skillName, true);
			print($"Skill {skillName} looks started to train for {timeToTrain} seconds.");
		}
		else
			print($"Error. Skill {skillName} can't be trained. Bud it seems the button works (");
	}

    public void ApplyActionByTimer(TaskByTimer action)
	{
		if (action.Source != this) {
            print("ERROR! INCORRECT ACTION while Applying Skill training");
			return;
		}
		int skillNumber = GetSkillNumber(action.TaskTag);

		if (_untrained[skillNumber]>0 && GetTrainedValue(skillNumber) < MAXIMUM_TOTAL_SKILL)
        {
			_untrained[skillNumber]--;
			_trained[skillNumber]++;
        }
	}
	
	private int GetSkillNumber (string skillName) {
		if (!_skillNumbers.ContainsKey(skillName)) {
			print ("ERROR!!! Incorrect skill name! "+ skillName);
			throw new Exception();
		}
		return _skillNumbers[skillName];
	}

	public string ToJson()
	{
		SkillsJsonData jsonSkills = new();
		jsonSkills.trained=_trained;
		jsonSkills.untrained=_untrained;
		jsonSkills.unspent = _unspentPoints;

		return JsonUtility.ToJson(jsonSkills);
	}

	public void FromJson(string jsonString)
	{
		SkillsJsonData jsonSkills = JsonUtility.FromJson<SkillsJsonData>(jsonString);
		_trained=jsonSkills.trained;
		_untrained=jsonSkills.untrained;
		_unspentPoints = jsonSkills.unspent;
	}

	[Serializable]
	private class SkillsJsonData
	{
		public int unspent;
		public List<int> trained;
		public List<int> untrained;
	}
}