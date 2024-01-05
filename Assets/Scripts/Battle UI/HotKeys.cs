using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HotKeys : MonoBehaviour
{
    [SerializeField] BattleManager _battleManager;

    private void Update()
    {
        if (_battleManager.Status != "planning" || _battleManager.AllCombatCharacters[_battleManager.Player]._ai!="")
            return;
        if (Input.GetKeyDown(KeyCode.E))
            BattleUserInterface.Instance.EndTurn();
        else if (Input.GetKeyDown(KeyCode.Space))
            BattleUserInterface.Instance.ChangeWeapon();
    }
}
