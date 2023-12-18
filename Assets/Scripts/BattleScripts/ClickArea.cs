using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickArea : MonoBehaviour
{
    public string action = "move";
    
    public int xCorrection;
    public int yCorrection;
    public CombatUnit combatCharacter; //Subject for MOVE & target (object) for ATTACK
    public int costAP; //for MOVE only
    public KeyCode hotKey = KeyCode.None;

    private int xPlace;
    private int yPlace;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnMouseEnter()
    {
        if (action=="attack")
        {
            combatCharacter.OverheadText.Show("To hit: "+ Scripts.HitChanse(BattleUserInterface.Instance.BattleManager.AllCombatCharacters[BattleUserInterface.Instance.BattleManager.Player], combatCharacter)+"%");
            BattleUserInterface.Instance.ShowEnemyInfo((NonPlayerCharacter)combatCharacter);
        }
    }

    private void OnMouseExit()
    {
        combatCharacter.OverheadText.ShowHP();
        BattleUserInterface.Instance.RefreshCharInfo();
    }


    private void OnMouseDown()
    {
        if (BattleUserInterface.Instance.BattleManager.Status != "planning")
            return;
        
        //Start move action
        if (action == "move")
        {
            if (combatCharacter.PlanningAP >= costAP)
            {
                xPlace = combatCharacter.planningPos[0] + xCorrection;
                yPlace = combatCharacter.planningPos[1] + yCorrection;
                (combatCharacter as CombatCharacter).MovePlan(xPlace, yPlace);
            }
        }
        else if (action == "attack")
        {
            //Check for and perform move action
            if (Input.GetKey(KeyCode.LeftControl) == true || Input.GetKey(KeyCode.RightControl) == true)
            {
                CombatCharacter planningCharacter = BattleUserInterface.Instance.BattleManager.AllCombatCharacters[BattleUserInterface.Instance.BattleManager.Player] as CombatCharacter;
                xPlace = combatCharacter.planningPos[0];
                yPlace = combatCharacter.planningPos[1];
                planningCharacter.MovePlan(xPlace, yPlace);

            }
            else
            {
                if (CombatAction.Attack(BattleUserInterface.Instance.BattleManager.AllCombatCharacters[BattleUserInterface.Instance.BattleManager.Player], combatCharacter))
                {
                    combatCharacter.OverheadText.ShowHP();
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(hotKey))
            OnMouseDown();
    }
}
