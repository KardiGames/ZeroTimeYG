using UnityEngine;

public class HotKeys : MonoBehaviour
{
    [SerializeField] BattleManager _battleManager;

    private void Update()
    {
        if (_battleManager.Status != "planning" || _battleManager.AllCombatCharacters[_battleManager.Player]._ai!="")
            return;
        if (Input.GetKeyDown(KeyCode.Space))
            _battleManager.BattleUI.EndTurn();
        else if (Input.GetKeyDown(KeyCode.E))
            _battleManager.BattleUI.ChangeWeapon();
    }
}
