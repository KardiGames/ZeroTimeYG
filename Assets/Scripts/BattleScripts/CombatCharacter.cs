using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCharacter : CombatUnit
{
    //Technical use variables
    //setting X & Y corrections for 1st circle + List of child GameObjects
    private int[] xOddCorrArray = new int[] { 1, 1, 1, 0, -1, 0 }; //for Odd row
    private int[] xEvenCorrArray = new int[] { 0, 1, 0, -1, -1, -1 }; //for Even row
    private int[] yCorrArray = new int[] { 1, 0, -1, -1, 0, 1 };

    private List<GameObject> clickZones = new List<GameObject>(6);

    //Variables
    [SerializeField] private Weapon _fist;
    private WorldCharacter _playerCharacter;
    private Dictionary<string, int> _skillBoostings = new();

    //Basic stats properties
    public override string CharName => _playerCharacter.CharacterName;
    public override int Level => _playerCharacter.Level;
    public int ST => _playerCharacter.ST;
    public int PE => _playerCharacter.PE;
    public int EN => _playerCharacter.EN;
    public int AG => _playerCharacter.AG;
    public int IN => _playerCharacter.IN;
    public Inventory Inventory => _playerCharacter.Inventory;

    //Secondary stats properties
    public override int MaxHP => 15 + (ST + (2 * EN));
    public override int MeleeDamageBonus => 
        CalculateMeleeDamageBonus(_playerCharacter);
    public override int TotalAP => 
        (AG / 2) + 5;
    public override int AC => 
        AG + _playerCharacter.Equipment.AC + _bonusAC;

	public override int DamageResistance
		=> _playerCharacter.Equipment.DamageResistance;
	
    public override Weapon RightHandWeapon =>
        _playerCharacter.Equipment[Equipment.Slot.RightHand] == null ? _fist : _playerCharacter.Equipment[Equipment.Slot.RightHand] as Weapon;
    public override Weapon LeftHandWeapon => 
        _playerCharacter.Equipment[Equipment.Slot.LeftHand] == null ? _fist : _playerCharacter.Equipment[Equipment.Slot.LeftHand] as Weapon;
    public override int GetSkillValue(string skillName)
    {
        int skillValue = _playerCharacter.Skills.GetSkillValue(skillName);
        if (_skillBoostings.ContainsKey(skillName))
            skillValue += _skillBoostings[skillName];
        return skillValue;
    }

    public void SetCharacter (WorldCharacter player, BattleManager manager)
    {
        if (player == null || manager == null)
            return;

        _playerCharacter = player;
        _battleManager = manager;
    }

    public void PrepareToFight()
    {
        ResetAP();
        HP = MaxHP;
        for (int i = 0; i <= 1; i++)
            if (_playerCharacter.Equipment[i] is Weapon weapon)
                weapon.FillAmmo();
        ResetPlanning();
        CreateClickZones();
    }

    private void OnEnable()
    {
        ResetSkillBoostings();
    }

    public void CreateClickZones()
    {
        KeyCode[] keyCodes = { KeyCode.E, KeyCode.F, KeyCode.D, KeyCode.S, KeyCode.A, KeyCode.W };
        
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
        attackZone.transform.parent = this.transform;
        attackZone.transform.position = new Vector3(CoordArray.cArray[this.pos[0], this.pos[1], 0], CoordArray.cArray[this.pos[0], this.pos[1], 1], attackZone.transform.position.z);
        attackZone.GetComponent<ClickArea>().combatCharacter = this;
        attackZone.GetComponent<ClickArea>().action = "attack";
        attackZone.SetActive(false);
    }

    public override void StartPlanning(bool start = true)
    {
        if (Dead && start)
        {
            _battleManager.NextPlayer();
            return;
        }

        _battleManager.BattleUI.UpdateAP(this);
        _battleManager.BattleUI.ShowWeaponStats();
        _battleManager.BattleUI.RefreshCharInfo();
        
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


                bool check = (xCurrent >= 0) && (xCurrent < Location.xSize) && (yCurrent >= 0) && (yCurrent < Location.ySize);
                foreach (CombatUnit cC in _battleManager.AllCombatCharacters)
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

        foreach (CombatUnit cC in _battleManager.AllCombatCharacters)
        {
            cC.attackZone.SetActive(start);
            if (cC == this || cC.AI=="")
            {
                cC.attackZone.SetActive(false);
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

        personalPlanningList.Add(new CombatAction());
        if (!personalPlanningList[(personalPlanningList.Count - 1)].Move(this, x, y, _battleManager.Turn))
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

                bool check = (xCurrent >= 0) && (xCurrent < Location.xSize) && (yCurrent >= 0) && (yCurrent < Location.ySize);
                foreach (CombatUnit cC in _battleManager.AllCombatCharacters)
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
        //float difficulty = 0.2f; //<1 - much easier to train; >1 - much more difficult; 0-always trains
        float difficulty = -0.0444444f * IN + 0.5444444f; //Formula of dependency difficulty by INtelligence

        if (GetSkillValue(skillname) >= Skills.MAXIMUM_TOTAL_SKILL)
            return;

        float chanse = 1.0f-((float)GetSkillValue(skillname)/ Skills.MAXIMUM_TOTAL_SKILL);

        chanse = Mathf.Pow(chanse, difficulty);
        float roll = Random.value;
        if (roll<chanse)
        {
            if (_skillBoostings.ContainsKey(skillname))
                _skillBoostings[skillname]++;
            else
                _skillBoostings.Add(skillname, 1);
            OverheadText.ShowGreen($"{GlobalUserInterface.Instance.Localisation.Translate(skillname)} {GetSkillValue(skillname)} ( +1)");
        } else
            print("Skill " + skillname + "'v not improved. Chanse " + chanse + " < roll "+roll);
    }
    private void ResetSkillBoostings()
    {
        _skillBoostings.Clear();
    }
    public static int CalculateMeleeDamageBonus (WorldCharacter damager)
        => damager.ST;
}
