using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatUnit : MonoBehaviour
{
    //Technical use variables
    [SerializeField] protected GameObject prefabClickZone;
    [SerializeField] protected Animator _characterAnimator;
    [SerializeField] protected AudioSource _sound;
    [SerializeField] protected OverheadMessage _overheadText;

    public GameObject attackZone; //TODO this protected
    protected BattleManager _battleManager; //TODO SET this in code
    public OverheadMessage OverheadText => _overheadText;
    public Animator CharacterAnimator => _characterAnimator;
    public AudioSource Sound => _sound;

    //Variables
    public abstract string CharName { get; }
    public abstract int Level { get; }
    public int[] pos = new int[2];
    public Location loc { get => Location.GetLocation(pos); }
    public bool usesOffHand = false;
    public bool Dead { get; protected set; } = false;
    protected int _hp;

    //[0] for right hand, [1] for left hand
    //public List<Item> equipment = new();

    //Secondary stats variables
    protected int _ap; //Action points
    public int _bonusAC;

    //Secondary stats properties
    public abstract int MaxHP { get; }
    public abstract int MeleeDamageBonus { get; }
    public abstract int TotalAP { get; } //Action points (ochki deystvija)
    public abstract int AC { get; } //Acmor class
    public abstract Weapon RightHandWeapon { get; }
    public abstract Weapon LeftHandWeapon { get; }

    public int HP
    {
        get => _hp; protected set
        {
            _hp = value;
            if (_hp > MaxHP) _hp = MaxHP;
        }
    }

    //AI Stats
    public string _ai = "";

    //Planning stuff
    public int PlanningAP { get; protected set; }
    public int[] planningPos = new int[2];
    public List<CombatAction> personalPlanningList = new List<CombatAction>();

    public abstract int GetSkillValue(string skillName);
    public abstract void StartPlanning(bool start = true);

    public void ResetAP()
    {
        _ap = TotalAP;
        _bonusAC = 0;
    }
    public void ResetPlanning()
    {
        PlanningAP = TotalAP;
        planningPos[0] = pos[0];
        planningPos[1] = pos[1];
        this.transform.position = new Vector3(CoordArray.cArray[this.pos[0], this.pos[1], 0], CoordArray.cArray[this.pos[0], this.pos[1], 1], 0);
    }
    public void SetPosition(int[] position)
    {
        pos = position;
        transform.position = new Vector3(CoordArray.cArray[pos[0], pos[1], 0], CoordArray.cArray[pos[0], pos[1], 1], transform.position.z);
    }

    public bool SpendAP(int cost, bool spendPlanningAP = false)
    {
        if (spendPlanningAP == false)
        {
            if (_ap < cost)
                return false;
            else
            {
                _ap -= cost;
                return true;
            }
        }
        else
        {
            if (PlanningAP < cost)
                return false;
            else
            {
                PlanningAP -= cost;
                _battleManager.BattleUI.UpdateAP(this);
                return true;
            }
        }
    }
    public void TakeDamage(int damage)
    {
        damage = damage < 0 ? 0 : damage;
        HP -= damage;
        if (HP <= 0)
        {
            Dead = true;
        }
    }

    private void OnDestroy()
    {
        while (_battleManager.AllCombatCharacters.Contains(this))
            _battleManager.AllCombatCharacters.Remove(this);
    }
}
