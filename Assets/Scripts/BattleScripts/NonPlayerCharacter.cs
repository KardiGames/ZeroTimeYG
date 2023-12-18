using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonPlayerCharacter : CombatUnit
{
    //Variables for NPC
    private int _maxHP;
    private int _totalAP; 
    private int _ac;
    private NpcAttack _npcAttack;
    private Dictionary<string, int> _skillValues = new Dictionary<string, int>();

    //Secondary stats properties
    public override int MaxHP { get => _maxHP;}
    public override int TotalAP { get => _totalAP; }
    public override int AC
    {
        get => _ac + bonusAC;
    }

    public override int MeleeDamageBonus => 0;

    public override Weapon RightHandWeapon => _npcAttack;

    public override Weapon LeftHandWeapon => _npcAttack;

    public override int GetSkillValue(string skillName)
    {
        int skillValue = 0;
        _skillValues.TryGetValue(skillName, out skillValue);
        return skillValue;
    }
    // Start is called before the first frame update
    private void Start()
    {
        if (attackZone != null) return;
        
        //Getting components to technical use variables
        OverheadText = GetComponentInChildren<OverheadMessage>();
        characterAnimator = GetComponentInChildren<Animator>(true);
        characterSound = GetComponent<AudioSource>();

        ResetAP();
        HP = MaxHP;

        pos = SpawnPosition();

        //This is not temporary part
        ResetPlanning();
        transform.position = new Vector3(CoordArray.cArray[pos[0], pos[1], 0], CoordArray.cArray[pos[0], pos[1], 1], transform.position.z);

        //temp creating attackzone
        attackZone = Instantiate<GameObject>(prefabClickZone);
        if (attackZone == null)
            print("Error!!! AZone for " + name + "isn't created");
        attackZone.transform.parent = this.transform;
        attackZone.transform.position = new Vector3(CoordArray.cArray[this.pos[0], this.pos[1], 0], CoordArray.cArray[this.pos[0], this.pos[1], 1], attackZone.transform.position.z);
        attackZone.GetComponent<ClickArea>().combatCharacter = this;
        attackZone.GetComponent<ClickArea>().action = "attack";
        attackZone.SetActive(false);
    }

    public override void StartPlanning(bool start = true)
    {
        if (start && _battleManager.Status=="planning")
        {
            if (!Dead)
            {
                Scripts.Ai(this);
            }
            _battleManager.NextPlayer();
        }
    }

    private int[] SpawnPosition()
    {
        int[] position = new int[2];
        int side = Random.Range(0, 4);
        switch (side) 
        {
            case 0:
                position[0] = 0;
                position[1] = Random.Range(0, Location.ySize);
                break;
            case 1:
                position[0] = Location.xSize-1;
                position[1] = Random.Range(0, Location.ySize);
                break;
            case 2:
                position[0] = Random.Range(0, Location.xSize);
                position[1] = 0;
                break;
            case 3:
                position[0] = Random.Range(0, Location.xSize);
                position[1] = Location.ySize - 1;
                break;
        }
        if (Location.IsBusy(position[0], position[1]))
            return SpawnPosition();
        else
            return position;
    }

    public static void SpawnMiner(BattleManager manager, int level = 1)
    {
        if (level < 1) return;
        
        string ai = "rat";
        int maxHP = 10;
        int totalAP = 6;
        int AC = 5;
        
        bool rangedAttack = false;
        int damageMultipler = 1;
        int damageDise = 4;
        int damagePlus = 0;
        int damageRange = 1;
        int attackAP=3;

        GameObject npcGameObj = Instantiate(PrefabsList.instance.ratPrefab);
        NonPlayerCharacter npc = npcGameObj.GetComponent<NonPlayerCharacter>();
        npc._battleManager = manager;
        npc.ai = ai;
        npc.charName = "Miner";
        npc._maxHP = maxHP;
        npc._totalAP = totalAP;
        npc._ac = AC;
        npc.level = level;

        NpcAttack attack = (NpcAttack)ScriptableObject.CreateInstance(System.Type.GetType("NpcAttack"));
        attack.SetValues("Plasma Cutter", damageRange, attackAP, rangedAttack, "NpcAttack");
        attack.SetDamage(damageMultipler, damageDise, damagePlus);
        npc._npcAttack =attack;


        if (!npc._skillValues.ContainsKey(attack.SkillName))
            npc._skillValues.Add(attack.SkillName, 0);
        npc._skillValues[attack.SkillName] = 75;

        for (int i=1; i<level; i++)
        {
            npc._maxHP += maxHP/2;
            int randomStart = 0;
            if (npc.TotalAP >= (totalAP * 2))
                randomStart = 1;
            switch (Random.Range(randomStart, 3))
            {
                case 0:
                    npc._totalAP++;
                    break;
                case 1:
                    attack.BoostDamage();
                    break;
                case 2:
                    npc._ac+=npc._ac/2;
                    break;
            }
        }

        npc.Start();
    }
}
