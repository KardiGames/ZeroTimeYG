using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonPlayerCharacter : CombatCharacter
{
    //Variables for NPC
    private int _maxHP;
    public new int MaxHP { get => _maxHP; private set => _maxHP=value;}
    public override int HP
    {
        get => _hp; protected set
        {
            _hp = value;
            if (_hp > MaxHP) _hp = MaxHP;
        }
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
        if (start && Status.Current=="planning")
        {
            if (!Dead)
            {
                Scripts.Ai(this);
            }
            Status.NextPlayer();
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

    public static void SpawnMiner(int level = 1)
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
        npc.ai = ai;
        npc.charName = "Miner";
        npc.MaxHP = maxHP;
        npc.totalAP = totalAP;
        npc.AC = AC;
        npc.level = level;

        Item attack = new Item();
        attack.itemName = "Plasma Cutter";
        attack.Range = damageRange;
        attack.apCost = attackAP;
        attack.rangedAttack = rangedAttack;
        attack.SetDamage(damageMultipler, damageDise, damagePlus);
        attack.skillname = "npcattack";
        npc.equipment.Add(attack);
        npc.equipment.Add(attack);


        if (!npc.skills.ContainsKey(attack.skillname))
            npc.skills.Add(attack.skillname, 0);
        npc.skills[attack.skillname] = 75;

        for (int i=1; i<level*Status.Difficulty; i++)
        {
            npc.MaxHP += maxHP/2;
            int randomStart = 0;
            if (npc.totalAP >= (totalAP * 2))
                randomStart = 1;
            switch (Random.Range(randomStart, 3))
            {
                case 0:
                    npc.totalAP++;
                    break;
                case 1:
                    attack.BoostDamage();
                    break;
                case 2:
                    npc.AC+=AC/2;
                    break;
            }
        }

        npc.Start();
    }

    public void DeathProtocol ()
    {
        
    }
}
