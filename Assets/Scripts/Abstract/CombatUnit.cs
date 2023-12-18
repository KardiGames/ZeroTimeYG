using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatUnit : MonoBehaviour
{
    //Technical use variables
    public GameObject prefabClickZone;
    public GameObject attackZone;
    public Animator characterAnimator;
    public AudioSource characterSound;
    public OverheadMessage OverheadText { get; protected set; }
    [SerializeField]protected BattleManager _battleManager;
    protected static List<CombatUnit> cCList;

    //Variables
    public string charName;
    public int[] pos = new int[2];
    public bool usesOffHand = false;
    public bool Dead { get; protected set; } = false;
    protected int _hp;
    public int level = 1; //TODO move to NPC subclass if already possible

    //[0] for right hand, [1] for left hand
    //public List<Item> equipment = new();

    //Secondary stats variables
    protected int AP; //Action points
    
    public int MD; //Melee damage
    public int bonusAC;

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
    public string ai = "";

    //Planning stuff
    public int PlanningAP { get; protected set; }
    public int[] planningPos = new int[2];
    public List<CombatAction> personalPlanningList = new List<CombatAction>();

    public abstract int GetSkillValue(string skillName);

    public void ResetAP()
    {
        AP = TotalAP;
        bonusAC = 0;
    }
    public void ResetPlanning()
    {
        PlanningAP = TotalAP;
        planningPos[0] = pos[0];
        planningPos[1] = pos[1];
        this.transform.position = new Vector3(CoordArray.cArray[this.pos[0], this.pos[1], 0], CoordArray.cArray[this.pos[0], this.pos[1], 1], 0);
    }

    public abstract void StartPlanning(bool start = true);
    public bool SpendAP(int cost, bool spendPlanningAP = false)
    {
        if (spendPlanningAP == false)
        {
            if (AP < cost)
                return false;
            else
            {
                AP -= cost;
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
                BattleUserInterface.Instance.UpdateAP(this);
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

    private void Awake()
    {
        cCList = _battleManager.AllCombatCharacters;
        cCList.Add(this);
    }

    private void OnDestroy()
    {
        while (cCList.Contains(this))
            cCList.Remove(this);
    }
}
