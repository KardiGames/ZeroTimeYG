using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skills : MonoBehaviour, ITimerable
{
	public const int WORKING_SKILLS_NUMBER = 5;
	public const int maximumTotalSkill = 200;
	private const int skillToCostImprove = 50;
	private const int maximumImproveByLevel = 5;
	private const float timeToTrainMultipler = 4.3817804600413289076557582624064f;


	[SerializeField] private WorldCharacter _playerCharacter;
	[SerializeField] private TaskTimer _skillsTimer;

	private static Dictionary<string, int> skillNumbers = new Dictionary<string, int>() {
		{ "Unarmed",0 },
		{ "Melee",1 },
		{ "Pistols",2 },
		{ "Rifles",3 },
		{ "Heavy",4 },
		{ "Beam",5 },
		{ "Reserve 1",6 },
		{ "Weapon damage",7 },
		{ "High-tech weapon damage",8 },
		{ "Improved damage resistance",9 },
		{ "Reserve 2",10 },
		{ "Traveling",11 },
		{ "Road traveling",12 },
		{ "Scouting",13 },
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

	private static List<Func<WorldCharacter, int>> minimalSkills = new List<Func<WorldCharacter, int>>() {
		(WorldCharacter c) => 30 + (2 * (c.ST + c.AG)), //"Unarmed" = 0,
		(WorldCharacter c) => 20 + (2 * (c.ST + c.AG)), //"Melee"=1, 
		(WorldCharacter c) => 10 + 3 * c.AG + c.PE,//"Pistols"=2,
		(WorldCharacter c) => 5 + (2 * (c.PE + c.AG)),//"Rifles"=3,
		(WorldCharacter c) => 0 + 2*c.ST + c.EN + c.PE,//"Heavy"=4, 
		(WorldCharacter c) => 5 + 2*c.IN + c.AG + c.PE,//"Beam"=5,
		(WorldCharacter c) => 0,//"Reserve 1"=6,
		(WorldCharacter c) => 20+(2*(c.IN+c.PE)),//"Weapon damage"=7,
		(WorldCharacter c) => 10+(2*c.IN),//"High-tech weapon damage"=8,
		(WorldCharacter c) => 0+ c.IN + c.EN,//"Improved damage resistance"=9,
		(WorldCharacter c) => 0,//"Reserve 2"=10,
		(WorldCharacter c) => 10+3*c.EN + c.PE,//"Traveling"=11,
		(WorldCharacter c) => 20+ 4*c.EN,//"Road traveling"=12,
		(WorldCharacter c) => 10+ c.IN+c.EN,//"Scouting"=13,
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
	private int SkillPointEachLevel => WORKING_SKILLS_NUMBER+_playerCharacter.IN+_playerCharacter.Level;

	public float GetSkillMiltipler(string skillName)
    {
		return (float)GetSkillValue(skillName)/100.0f;
	}
		
	public int GetSkillValue (string skillName) {
		return GetSkillValue(GetSkillNumber(skillName));
	}
	private int GetSkillValue(int skillNumber)
	{
		return (int)(minimalSkills[skillNumber](_playerCharacter)
			+ _trained[skillNumber]);
	}

	public void LevelUp()
    {
		_unspentPoints += SkillPointEachLevel;
    }

    private bool IsPossibleToImprove(int skillNumber)
    {
        int requiredSkillPoints = (_trained[skillNumber] + _untrained[skillNumber]) / skillToCostImprove + 1;
        if (_unspentPoints >= requiredSkillPoints
                && (_playerCharacter.Level * maximumImproveByLevel) > _trained[skillNumber]
                && GetSkillValue(skillNumber) < maximumTotalSkill)
            return true;
        else
            return false;
    }

	public void Train (string skillName)
    {
		int skillNumber = GetSkillNumber(skillName);
		if (_untrained[skillNumber] > 0 && GetSkillValue(skillNumber) < maximumTotalSkill)
        {
			int timeToTrain = (int)(Mathf.Pow(timeToTrainMultipler*(_trained[skillNumber] + 1), 2) / _playerCharacter.IN);
			_skillsTimer.AddTask(this, timeToTrain, skillName, true);
        }
	}

    public void ApplyActionByTimer(TaskByTimer action)
	{
		if (action.Source != this) {
            print("ERROR! INCORRECT ACTION while Applying Skill training");
			return;
		}
		int skillNumber = GetSkillNumber(action.TaskTag);

		if (_untrained[skillNumber]>0 && GetSkillValue(skillNumber) < maximumTotalSkill)
        {
			_untrained[skillNumber]--;
			_trained[skillNumber]++;
        }
	}
	
	private int GetSkillNumber (string skillName) {
		if (!skillNumbers.ContainsKey(skillName)) {
			print ("ERROR!!! Incorrect skill name!");
			throw new Exception();
		}
		return skillNumbers[skillName];
	}
	
	private void Start () {
		_skillsTimer.SetupTaskTimer(3, 1);
	}

	public string ToJson()
	{
		SkillsJsonData jsonSkills = new()
		{

		};

		jsonSkills.taskTimerJsonString = _skillsTimer.ToJson(this);

		return JsonUtility.ToJson(jsonSkills);
	}

	public void FromJson(string jsonString)
	{
		SkillsJsonData jsonSkills = JsonUtility.FromJson<SkillsJsonData>(jsonString);

		_skillsTimer.FromJson(jsonSkills.taskTimerJsonString, this);
	}

	[Serializable]
	private class SkillsJsonData
	{
		public string taskTimerJsonString;
	}
}