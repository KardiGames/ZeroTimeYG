using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldCharacter : MonoBehaviour
{
    private const int LEVEL_UP_EXPERIENCE_MULTIPLER=25;
	private const int START_ATTRIBUTE_POINTS=20;
	
	[SerializeField] private Equipment _equipment;
    [SerializeField] private Inventory _inventory;
	[SerializeField] private Skills _skills;

    [SerializeField] private string _charName;
	public int X { get; private set; } = 0;
	public int Y { get; private set; } = 0;
    public int Level { get; private set; } = 0;
    public int ST {get; private set;} = 5; //Strength
    public int PE {get; private set;} = 5; //Perception
    public int EN {get; private set;} = 5; //Endurance
    public int IN {get; private set;} = 5; //Intelligence
    public int AG {get; private set;} = 5; //Agility
    public int Experience { get; private set; } = 0;
    public string CharacterName => _charName;
    public Equipment Equipment => _equipment;	
    public Skills Skills => _skills;

	public int ExperienceToLevelUp => (Level+1)*LEVEL_UP_EXPERIENCE_MULTIPLER;
	public int AttributePoints => Level-START_ATTRIBUTE_POINTS;
		
	public void FulfillCharacter(string name, int strength, int perception, int endurance, int agility, int intelligence)
    {
        if (_charName != "")
            return;
        _charName = name;
        ST = strength;
        PE = perception;
        EN = endurance;
        AG = agility;
        IN = intelligence;
    }

    public void CollectExperience (int experience)
    {
        if (experience<0)
        {
            print("Error! Can't collect negative experience");
            return;
        }
        Experience += experience;
		if (Experience>=ExperienceToLevelUp)
			LevelUp();
    }
	
	private void LevelUp () {
		if (Experience<ExperienceToLevelUp)
			return;
		
		Experience-=ExperienceToLevelUp;
		Level++;
		_skills.LevelUp();
	}
	
	public string ToJson()
    {
		PlayerJsonData jsonPlayer = new() { 
			x=X, y=Y, name=_charName,
			level=Level,
			experience=Experience,
			ST = ST,
			PE = PE,
			EN = EN,
			AG = AG,
			IN = IN
		};

		jsonPlayer.inventoryJsonString=_inventory.ToJson();
		jsonPlayer.equipmetnJsonString=_equipment.ToJson();
		jsonPlayer.skillsJsonString=_skills.ToJson();		

		return JsonUtility.ToJson(jsonPlayer);
    }

    public void FromJson(string jsonString)
    {
		PlayerJsonData jsonPlayer = JsonUtility.FromJson<PlayerJsonData>(jsonString);
		X = jsonPlayer.x;
		Y = jsonPlayer.y;
		_charName = jsonPlayer.name;
		Level = jsonPlayer.level;
		Experience = jsonPlayer.experience;
		ST = jsonPlayer.ST;
		PE = jsonPlayer.PE;
		EN = jsonPlayer.EN;
		AG = jsonPlayer.AG;
		IN = jsonPlayer.IN;
		
		_inventory.FromJson(jsonPlayer.inventoryJsonString);
		_equipment.FromJson(jsonPlayer.equipmetnJsonString);
		_skills.FromJson(jsonPlayer.skillsJsonString);
	}
	
	[Serializable]
    private class PlayerJsonData
    {
        public string name;
        public int x;
        public int y;
        public int level;
        public int experience;
        public int ST;
        public int PE;
        public int EN;
        public int AG;
        public int IN;
        public string inventoryJsonString;
        public string equipmetnJsonString;
        public string taskTimerJsonString;
        public string skillsJsonString;
    }

}
