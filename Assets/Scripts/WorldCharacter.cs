using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCharacter : MonoBehaviour
{
    [SerializeField] private Equipment _equipment;
    [SerializeField] private Inventory _inventory;
	[SerializeField] private Skills _skills;

    [SerializeField] private string _charName;
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
    }
}
