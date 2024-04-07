using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldCharacter : MonoBehaviour
{
    public const int MAX_ATTRIBUTE_VALUE=10;
	private const int LEVEL_UP_EXPERIENCE_MULTIPLER=25;
	private const int START_ATTRIBUTE_POINTS=20;
		
	[SerializeField] private Equipment _equipment;
    [SerializeField] private Inventory _inventory;
	[SerializeField] private Skills _skills;

    [SerializeField] private string _charName;
	public int X { get; private set; } = 0;
	public int Y { get; private set; } = 0;
    public int Level { get; private set; } = 0;
    public int ST {get; private set;} = 4; //Strength
    public int PE {get; private set;} = 4; //Perception
    public int EN {get; private set;} = 4; //Endurance
    public int IN {get; private set;} = 4; //Intelligence
    public int AG {get; private set;} = 4; //Agility
    public int Experience { get; private set; } = 0;
    public string CharacterName => _charName;
    public Equipment Equipment => _equipment;
	public Inventory Inventory=> _inventory;
	public Skills Skills => _skills;

	public int ExperienceToLevelUp => (Level+1)*LEVEL_UP_EXPERIENCE_MULTIPLER;
    public int AttributePoints
    {
        get
        {
            int attributePoints = Level + START_ATTRIBUTE_POINTS - ST - PE - EN - IN - AG;
			return (attributePoints < 0) ? 0 : attributePoints;
				
        }

    }


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
	
	public bool IsImprovable (int attribute) {
		if (attribute>=MAX_ATTRIBUTE_VALUE || AttributePoints<=0) 
			return false;
		else
			return true;
	}
	
	public void ImproveST () {
		if (IsImprovable(ST))
			ST++;
	}
	public void ImprovePE () {
		if (IsImprovable(PE))
			PE++;
	}
	public void ImproveEN () {
		if (IsImprovable(EN))
			EN++;
	}
	public void ImproveIN () {
		if (IsImprovable(IN))
			IN++;
	}
	public void ImproveAG () {
		if (IsImprovable(AG))
			AG++;
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
