using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HotKeys : MonoBehaviour
{
    private void Update()
    {
        if (BattleManager.Status != "planning" || CombatCharacter.cCList[BattleManager.Player].ai!="")
            return;
        if (Input.GetKeyDown(KeyCode.E))
            UserInterface.Instance.EndTurn();
        else if (Input.GetKeyDown(KeyCode.Space))
            UserInterface.Instance.ChangeWeapon();
    }
}
