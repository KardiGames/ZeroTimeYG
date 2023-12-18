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
    public int ST {get; private set;} = 5; //Strenght
    public int PE {get; private set;} = 5; //Perception
    public int EN {get; private set;} = 5; //Endurance
    public int IN {get; private set;} = 5; //Intelligence
    public int AG {get; private set;} = 5; //Agility
    public string CharacterName => _charName;
    public Equipment Equipment => _equipment;
    public Skills Skills => _skills;



    private void Start()
    {

    }

    public void FulfillCharacter(string name, int _st, int _pe, int _en, int _ag, int _in)
    {
        if (_charName != "")
            return;
        _charName = name;
        ST = _st;
        PE = _pe;
        EN = _en;
        AG = _ag;
        IN = _in;
    }

}
