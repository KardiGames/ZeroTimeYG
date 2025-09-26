using UnityEngine;

public class HotKeys : MonoBehaviour
{
    [SerializeField] BattleManager _battleManager;

    private void Update()
    {
        if (_battleManager.Status != "planning" || _battleManager.AllCombatCharacters[_battleManager.Player].AI!="")
            return;
        if (Input.GetKeyDown(KeyCode.Space))
            _battleManager.BattleUI.EndTurn();
        else if (Input.GetKeyDown(KeyCode.Q))
            _battleManager.BattleUI.ChangeWeapon();
        else if (Input.GetKeyDown(KeyCode.R))
            _battleManager.BattleUI.Reload();
    }
}
