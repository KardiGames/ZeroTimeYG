using System.Collections.Generic;
using UnityEngine;

public class NonPlayerCharacter : CombatUnit
{
    private const int NPC_ATTACK_SKILL = 75;
    private const int NPC_WEAPON_DAMAGE_SKILL = 100;

    //Variables for NPC
    
    private string _name;
    private int _level;
    private int _maxHP;
    private int _totalAP; 
    private int _ac;
    [SerializeField] private float _difficulty = 1;
    [SerializeField] private string _ai = "";

    private NpcAttack _npcAttack;
    private Dictionary<string, int> _skillValues = new Dictionary<string, int>();

    //Basic stats properties
    public override string CharName => _name;
    public override int Level { get => _level; }
    
    //Secondary stats properties

    public override int MaxHP { get => _maxHP;}
    public override int TotalAP { get => _totalAP; }
    public override int AC
    {
        get => _ac + _bonusAC;
    }

    public override int DamageResistance => 0;

    public override int MeleeDamageBonus => 0;

    public override Weapon RightHandWeapon => _npcAttack;

    public override Weapon LeftHandWeapon => _npcAttack;

    public float Difficulty => _difficulty;
    public override string AI => _ai;

    public override int GetSkillValue(string skillName)
    {
        int skillValue = 0;
        _skillValues.TryGetValue(skillName, out skillValue);
        return skillValue;
    }

    public override void StartPlanning(bool start = true)
    {
        if (start && _battleManager.Status=="planning")
        {
            if (!Dead)
            {
                Scripts.Ai(this, _battleManager);
            }
            _battleManager.NextPlayer();
        }
    }

    public void FillParameters (NpcBlank blank, BattleManager manager)
    {
        if (blank == null || manager==null)
            throw new System.Exception("Error. There isn't parameters to fill to NPC");

        _battleManager = manager;

        _name = blank.npcName;
        _ai = blank.ai;
        _difficulty = blank.difficulty;
        _maxHP = blank.maxHP;
        _totalAP = blank.totalAP;
        _ac = blank.AC;
        _level = 1;
        
        NpcAttack attack = (NpcAttack)ScriptableObject.CreateInstance(System.Type.GetType("NpcAttack"));
        attack.SetValues(blank.attackName, blank.damageRange, blank.attackAP, blank.rangedAttack);
        attack.SetDamage(blank.damageMultipler, blank.damageDise, blank.damagePlus);
        _npcAttack = attack;
        _skillValues[attack.SkillName] = NPC_ATTACK_SKILL;
        _skillValues["Weapon damage"] = NPC_WEAPON_DAMAGE_SKILL;
    }

    public void LevelUp(NpcBlank blank)
    {
        List<int> cases = new List<int>() { 0, 1, 2, 3 };
        _level++;

        _maxHP += blank.maxHP / 2;

        if (TotalAP >= (blank.totalAP * 2))
            cases.Remove(0);
        if (_ac >= 50)
            cases.Remove(2);

        switch (cases[Random.Range(0, cases.Count)])
        {
            case 0:
                _totalAP++;
                break;
            case 1:
                _npcAttack.BoostDamage();
                break;
            case 2:
                _maxHP += blank.maxHP / 2;
                break;
            case 3:
                _ac += 5;
                break;
        }
    }

    public void PrepareToFight()
    {
        ResetAP();
        HP = MaxHP;
        ResetPlanning();

        attackZone = Instantiate<GameObject>(prefabClickZone);
        attackZone.transform.parent = this.transform;
        attackZone.transform.position = new Vector3(CoordArray.cArray[this.pos[0], this.pos[1], 0], CoordArray.cArray[this.pos[0], this.pos[1], 1], attackZone.transform.position.z);
        attackZone.GetComponent<ClickArea>().combatCharacter = this;
        attackZone.GetComponent<ClickArea>().action = "attack";
        attackZone.SetActive(false);
    }



}
