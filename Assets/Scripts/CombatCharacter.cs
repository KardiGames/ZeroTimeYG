using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCharacter : MonoBehaviour
{
    //List of all Combat Characters in the scenes
    public static List<CombatCharacter> cCList = new();

    //Balansing variables
    private readonly int levelUpMultipler = 2;

    //Technical use variables
    //setting X & Y corrections for 1st circle + List of child GameObjects
    private int[] xOddCorrArray = new int[] { 1, 1, 1, 0, -1, 0 }; //for Odd row
    private int[] xEvenCorrArray = new int[] { 0, 1, 0, -1, -1, -1 }; //for Even row
    private int[] yCorrArray = new int[] { 1, 0, -1, -1, 0, 1 };
    private bool isCreated = false;
    public GameObject prefabClickZone;
    public GameObject attackZone;
    public Animator characterAnimator;
    public AudioSource characterSound;
    public OverheadMessage OverheadText { get; protected set; }
    public string ExperienceText
    {
        get
        {
            return $"( {experience} / {level * levelUpMultipler}  experience)";
        }
    }
    public int Experience { get => experience + (level-1) * levelUpMultipler;  }
    private List<GameObject> clickZones = new List<GameObject>();


    //Variables
    public string charName;
    public bool Dead { get; protected set; } = false;
    public int level = 1;
    public int[] pos = new int[2];
    private int experience = 0;
    public bool usesOffHand = false;

    //[0] for right hand, [1] for left hand
    public List<Item> equipment = new();
    public Dictionary<string, int> skills = new();

    //Basic stats
    public int ST = 5; //Strenght
    public int PE = 5; //Perception
    public int EN = 5; //Endurance
    public int AG = 5; //Agility

    //Secondary stats vatiables
    public int totalAP; //Action points (ochki deystvija)
    private int AP; //Action points
    public int MaxHP {get => 15 + (ST + (2 * EN));}
    public int AC; //Acmor class
    public int MD; //Melee damage
    public int bonusAC;

    //Secondary stats properties
    protected int _hp;
    public virtual int HP
    {
        get => _hp; protected set
        {
            _hp = value;
            if (_hp > MaxHP) _hp = MaxHP;
        }
    }

    public Location loc { get => Location.GetLocation(pos); }

    //AI Stats
    public string ai = "";

    //Planning stuff
    public int PlanningAP {get; protected set;}
    public int MeleeDamageBonus { get => ST * 2; }

    public int[] planningPos = new int[2];
    public List<CombatAction> personalPlanningList = new List<CombatAction>();

    void Awake()
    {
        cCList.Add(this);
    }

    private void OnDestroy()
    {
        while (cCList.Contains(this)) 
            cCList.Remove(this);
    }

    private void Start()
    {
        if (!isCreated)
        {
            print("ERROR!!! Start() for CombatCharacter started too early");
            return;
        }
        
        //Getting components to technical use variables
        OverheadText = GetComponentInChildren<OverheadMessage>();
        characterAnimator= GetComponentInChildren<Animator>(true);
        characterSound = GetComponent<AudioSource>();

        CalculateSecStats();
        ResetAP();
        HP = MaxHP;
        OverheadText.ShowHP();

        //TEMP setting places for Combatcharacters
        int i = cCList.IndexOf(this);
        CombatCharacter.cCList[i].pos[0] = i;
        CombatCharacter.cCList[i].pos[1] = i;

        //This is not temporary part
        ResetPlanning();
        transform.position = new Vector3(CoordArray.cArray[pos[0], pos[1], 0], CoordArray.cArray[pos[0], pos[1], 1], 0);
        CreateClickZones();
        //ADD when needed if (CombatCharacter.cCList[i].equipment[0]==null) CombatCharacter.cCList[i].equipment[0]=Item.items[0]; if (CombatCharacter.cCList[i].equipment[0]==null) CombatCharacter.cCList[i].equipment[1]=Item.items[0];
        }

    public bool FulfillCharacter(string name, int st, int pe, int en, int ag, Item mainWeapon, Item offWeapon)
    {
        if (isCreated)
            return false;

        if (mainWeapon == null || offWeapon == null)
            return false;

        charName = name;
        ST = st;
        PE = pe;
        EN = en;
        AG = ag;

        equipment.Add((Item)mainWeapon.Clone());
        equipment.Add((Item)offWeapon.Clone());

        if (equipment[0].itemName!=mainWeapon.itemName || equipment[1].itemName != offWeapon.itemName)
        {
            print("ERROR! Weapon check faild on fullfilling created character.");
            return false;
        }

        isCreated = true;
        return true;
    }

    private void CalculateSecStats()
    {
        if (ai != "") return; //Check for NPC
        totalAP = (AG / 2) + 5;
        AC = AG; //ADD here adding AC for armor

        //Calculating skills
        if (!skills.ContainsKey("melee")) 
            skills.Add("melee", 1);
        if (!skills.ContainsKey("guns"))
            skills.Add("guns", 1);
        if (!skills.ContainsKey("unarmed"))
            skills.Add("unarmed", 1);

        Dictionary <string,int> minimalSkills = new();

        minimalSkills.Add("melee", 20 + (2 * (ST + AG)));
        minimalSkills.Add("guns", 5 + 4 * AG);
        minimalSkills.Add("unarmed", 30 + (2 * (ST + AG)));

        foreach (KeyValuePair<string,int> mimimalSkill in minimalSkills)
        {
            skills[mimimalSkill.Key] = Mathf.Max(skills[mimimalSkill.Key], mimimalSkill.Value);
        }
    }

    public void CreateClickZones()
    {
        KeyCode[] keyCodes = { KeyCode.W, KeyCode.F, KeyCode.D, KeyCode.S, KeyCode.A, KeyCode.Q };
        
        //Create correct x+y corrections arrays
        int[] xCorrArray = new int[yCorrArray.Length];
        if ((pos[1] % 2) == 0)
            xCorrArray = xEvenCorrArray;
        else
            xCorrArray = xOddCorrArray;

        //Create array for corrections coordinates of ClickZones
        List<float[]> clickzoneCoordCorrections = new List<float[]>();
        for (int i = 0; i < yCorrArray.Length; i++)
        {
            int x = 1 + xOddCorrArray[i];
            int y = 1 + yCorrArray[i];
            float[] thisCoords = new float[2];

            thisCoords[0] = (CoordArray.cArray[x, y, 0] - CoordArray.cArray[1, 1, 0]);
            thisCoords[1] = (CoordArray.cArray[x, y, 1] - CoordArray.cArray[1, 1, 1]);

            clickzoneCoordCorrections.Add(thisCoords);
        }

        for (int i = 0; i < yCorrArray.Length; i++)
        {
            clickZones.Add(Instantiate<GameObject>(prefabClickZone));
            clickZones[i].transform.parent = this.transform;
            clickZones[i].GetComponent<ClickArea>().combatCharacter = this;
            clickZones[i].GetComponent<ClickArea>().hotKey = keyCodes[i];

            //Setting X & Y corrections for zones
            clickZones[i].GetComponent<ClickArea>().xCorrection = xCorrArray[i];
            clickZones[i].GetComponent<ClickArea>().yCorrection = yCorrArray[i];

            //Setting places for click zones
            clickZones[i].transform.position = new Vector3((this.transform.position.x + clickzoneCoordCorrections[i][0]), (this.transform.position.y + clickzoneCoordCorrections[i][1]), clickZones[i].transform.position.y);

            clickZones[i].SetActive(false); //Turning them off
        }

        attackZone = Instantiate<GameObject>(prefabClickZone);
        /*if (attackZone != null)
            print("AZone for " + name + " is created");
        else
            print("Error!!! AZone for " + name + "isn't created");*/
        attackZone.transform.parent = this.transform;
        attackZone.transform.position = new Vector3(CoordArray.cArray[this.pos[0], this.pos[1], 0], CoordArray.cArray[this.pos[0], this.pos[1], 1], attackZone.transform.position.z);
        attackZone.GetComponent<ClickArea>().combatCharacter = this;
        attackZone.GetComponent<ClickArea>().action = "attack";
        attackZone.SetActive(false);
    }

    public void ResetPlanning()
    {
        PlanningAP = totalAP;
        planningPos[0] = pos[0];
        planningPos[1] = pos[1];
        this.transform.position = new Vector3(CoordArray.cArray[this.pos[0], this.pos[1], 0], CoordArray.cArray[this.pos[0], this.pos[1], 1], 0);
    }

    public virtual void StartPlanning(bool start = true)
    {
        if (Dead && start)
        {
            Status.NextPlayer();
            return;
        }

        UserInterface.Instance.UpdateAP(this);
        UserInterface.Instance.ShowWeaponStats();
        UserInterface.Instance.RefreshCharInfo();
        
        for (int i = 0; i < clickZones.Count; i++)
        {

            if ((planningPos[1] % 2) == 0)
                clickZones[i].GetComponent<ClickArea>().xCorrection = xEvenCorrArray[i];
            else
                clickZones[i].GetComponent<ClickArea>().xCorrection = xOddCorrArray[i];
        }

        foreach (GameObject cz in clickZones)
        {
            if (start)
            {
                int xCurrent = pos[0] + cz.GetComponent<ClickArea>().xCorrection;
                int yCurrent = pos[1] + cz.GetComponent<ClickArea>().yCorrection;

                bool check = true;
                check = (xCurrent >= 0) && (xCurrent < Location.xSize) && (yCurrent >= 0) && (yCurrent < Location.ySize);
                foreach (CombatCharacter cC in CombatCharacter.cCList)
                {
                    if ((xCurrent == cC.planningPos[0]) && (yCurrent == cC.planningPos[1]))
                        check = false;
                }
                if (check)
                {
                    cz.SetActive(true);
                    cz.GetComponent<ClickArea>().costAP = Location.map[xCurrent, yCurrent].AP;
                }
            }
            else
                cz.SetActive(false);
        }

        foreach (CombatCharacter cC in CombatCharacter.cCList)
        {
            cC.attackZone.SetActive(start);
            if (cC == this || cC.ai=="")
            {
                cC.attackZone.SetActive(false);
            }
        }
    }
	
	public void GetDamage(int damage) {
		damage = damage < 0? 0 : damage;
		HP -= damage;
		if (HP<=0)
        {
            Dead=true;
		}
	}

    public void GetExperience(NonPlayerCharacter killedNPC)
    {
        experience += killedNPC.level;
        if (experience>=level*levelUpMultipler)
        {
            //LevelUp protocol
            experience -= level * levelUpMultipler;
            level++;

            string leveuUpText = "Level Up !!! \n";
            int randomStart = 0;
            int statSum = ST + PE + EN + AG;
            if (statSum >= 40) randomStart++; 

            switch (Random.Range(randomStart, 2))
            {
                case 0:
                    //Improving stats
                    int randomResult = Random.Range(1, (statSum + 1));
                    if (randomResult<=ST)
                    {
                        if (ST < 10) ST++;
                        else randomResult = ST + 1;
                    }
                    if ((randomResult > ST) && (randomResult <= ST+PE))
                    {
                        if (PE < 10) PE++;
                        else randomResult = ST+PE+1;
                    }
                    if ((randomResult > ST+PE) && (randomResult <= ST+PE+EN))
                    {
                        if (EN < 10) EN++;
                        else randomResult = ST + PE  + EN+ 1;
                    }
                    if (randomResult> ST + PE + EN)
                    {
                        if (AG < 10) AG++;
                        else if (ST < 10) ST++;
                        else if (PE < 10) PE++;
                        else EN++;
                    }
                    CalculateSecStats();
                    leveuUpText += "Main stat boosted!";
                    break;
                case 1:
                    int randomWeapon = Random.Range(0, 2);
                    equipment[randomWeapon].BoostDamage();
                    leveuUpText += equipment[randomWeapon].itemName + " damage boosted!";
                    break;
                /*case 2:    
                    equipment[3].BoostArmor();
                    break;*/
            }
            HP += (int)(EN / Status.Difficulty);
            print(charName + " " + leveuUpText);
            OverheadText.ShowGreen(leveuUpText);
        }
        UserInterface.Instance.RefreshCharInfo(this);
    }

    public void ResetAP()
    {
        AP = totalAP;
        bonusAC = 0;

    }

    public bool SpendAP(int cost, bool isPlanning = false)
    {
        if (isPlanning == false)
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
                UserInterface.Instance.UpdateAP(this);
                return true;
            }
        }

    }

    public void MovePlan(int x, int y)
    {
        if ((Mathf.Abs(planningPos[0] - x) > 1) || (Mathf.Abs(planningPos[1] - y) > 1))
        {
            print("WRONG coordinates for MovePlan!!!");
            return;
        }

        int oDcost = Location.map[x, y].AP;

        if ((PlanningAP - oDcost) < 0) return;

        //Creating planning combat action
        //print("Triying to add move action to " + x + " " + y);

        bool check;
        personalPlanningList.Add(new CombatAction());
        check = personalPlanningList[(personalPlanningList.Count - 1)].Move(this, x, y);
        if (!check)
        {
            personalPlanningList.RemoveAt(personalPlanningList.Count - 1);
            print("Haven't done this (");
        }
        else
        {
            //Moving plan position and sprite
            planningPos[0] = x;
            planningPos[1] = y;
            this.transform.position = new Vector3(CoordArray.cArray[x, y, 0], CoordArray.cArray[x, y, 1], 0);


            for (int i = 0; i < clickZones.Count; i++)
            {

                if ((planningPos[1] % 2) == 0)
                    clickZones[i].GetComponent<ClickArea>().xCorrection = xEvenCorrArray[i];
                else
                    clickZones[i].GetComponent<ClickArea>().xCorrection = xOddCorrArray[i];
            }

            //Are ClickZones exist or not (can we move somewhere or not)
            foreach (GameObject cz in clickZones)
            {
                int xCurrent = planningPos[0] + cz.GetComponent<ClickArea>().xCorrection;
                int yCurrent = planningPos[1] + cz.GetComponent<ClickArea>().yCorrection;

                check = (xCurrent >= 0) && (xCurrent < Location.xSize) && (yCurrent >= 0) && (yCurrent < Location.ySize);
                foreach (CombatCharacter cC in CombatCharacter.cCList)
                {
                    if ((xCurrent == cC.planningPos[0]) && (yCurrent == cC.planningPos[1]))
                        check = false;
                }

                if (check)
                {
                    cz.SetActive(true);
                    cz.GetComponent<ClickArea>().costAP = Location.map[xCurrent, yCurrent].AP;
                }
                else
                    cz.SetActive(false);
            }
        }
    }

    public void BoostSkill(string skillname)
    {
        float difficalty = 0.1f*Status.Difficulty; //<1 - much easier to train; >1 - much more difficult; 0-always trains
        
        if ((skillname == "") || (!skills.ContainsKey(skillname)))
            return;

        if (skills[skillname] >= 300)
            return;

        float chanse = 1.0f-((float)skills[skillname]/300.0f);

        chanse = Mathf.Pow(chanse, difficalty);
        float roll = Random.value;
        if (roll<chanse)
        {
            skills[skillname]++;
            //print("Skill " + skillname + "'v improved to "+skills[skillname]);
            OverheadText.ShowGreen(skillname + " " + skills[skillname] + " +1");
        } else
            print("Skill " + skillname + "'v not improved. Chanse " + chanse + " < roll "+roll);
    }

}
