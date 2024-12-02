using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleUserInterface : MonoBehaviour
{
    [SerializeField] private BattleManager _battleManager;
    [SerializeField] private Localisation _localisation;
    [SerializeField] private TextMeshProUGUI _actionPointsText;
    [SerializeField] private TextMeshProUGUI _weaponInfoField;
    [SerializeField] private TextMeshProUGUI _playerInfoField;
    [SerializeField] private TMP_InputField _spendApInput;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private List<Button> _planningButtons;

    public BattleManager BattleManager => _battleManager; //TODO This is crutch (( Much better to delete this
    
    public void UpdateAP(CombatUnit character) => _actionPointsText.text = character.PlanningAP+" "+Translate("AP");

    public void ChangeWeapon()
    {
        if (_battleManager.Status == "planning")
        {
            _battleManager.AllCombatCharacters[_battleManager.Player].usesOffHand = !_battleManager.AllCombatCharacters[_battleManager.Player].usesOffHand;
            ShowWeaponStats();
        }
    }

    public void ShowWeaponStats() => ShowWeaponStats(_battleManager.AllCombatCharacters[_battleManager.Player]);

    public void ShowWeaponStats(CombatUnit attacker)
    {
        Weapon weapon;
        if (attacker.usesOffHand)
        {
            weapon = attacker.LeftHandWeapon;
        } else
        {
            weapon = attacker.RightHandWeapon;
        }

        StringBuilder weaponText = new StringBuilder($"{Translate(weapon.ItemName)} [ {weapon.APCost} {Translate("AP")} ]");
        if (weapon.AmmoType!="")
            weaponText.Append($" {Translate("Ammo:")}{weapon.AmmoAmount}/{weapon.AmmoMaxAmount}");
        weaponText.Append("\n");
        int meleeDamageBonus = (weapon.RangedAttack) ? 0 : attacker.MeleeDamageBonus;
        weaponText.Append(Translate("Damage: ") + weapon.ApplyDamageModifiers(weapon.MinimalDamage, attacker) + " - " + weapon.ApplyDamageModifiers(weapon.MaximalDamage, attacker) + " ");
        if (weapon.RangedAttack)
            weaponText.Append(Translate("Range: ") + weapon.Range + " ");
        else
            weaponText.Append(Translate("Melee")+" ");
        weaponText.Append(Translate("Skill:")+" " + attacker.GetSkillValue(weapon.SkillName)+" %");
        _weaponInfoField.text = weaponText.ToString();
    }

    public void RefreshCharInfo() {
		if (_battleManager.AllCombatCharacters[_battleManager.Player] is CombatCharacter player)
			RefreshCharInfo(player);
	}
    public void RefreshCharInfo(CombatCharacter player)
    {
        string charInfoText = CombatUnitInfo(player)
                            + "\n" + Translate("Threat level")+ " " + (int)_battleManager.KillPoints;
        if ((int)_battleManager.KillPoints < _battleManager.MineLevel)
            charInfoText += "/" + _battleManager.MineLevel;
        _playerInfoField.text = charInfoText;
    }

    public void ShowEnemyInfo(NonPlayerCharacter npc)
    {
        string charInfoText = CombatUnitInfo(npc)
                    + $"{Translate("Weapon")} - {Translate(npc.RightHandWeapon.ItemName)}\n{Translate("Damage: ")+npc.RightHandWeapon.ApplyDamageModifiers(npc.RightHandWeapon.MinimalDamage, npc)}-{npc.RightHandWeapon.ApplyDamageModifiers(npc.RightHandWeapon.MaximalDamage, npc)}";
        if (npc.RightHandWeapon.RangedAttack)
            charInfoText += " "+Translate("Range: ") + npc.RightHandWeapon.Range + " ";
        else
            charInfoText += " "+Translate("Melee")+" ";
            charInfoText += Translate("Skill:")+" " + npc.GetSkillValue(npc.RightHandWeapon.SkillName) + "%";

        if (npc.LeftHandWeapon!=null && npc.LeftHandWeapon != npc.RightHandWeapon)
        {
            charInfoText += $"\n\n{Translate("Weapon")} 2 - {Translate(npc.LeftHandWeapon.ItemName)}\n{Translate("Damage: ")+npc.LeftHandWeapon.ApplyDamageModifiers(npc.LeftHandWeapon.MinimalDamage, npc)}-{npc.LeftHandWeapon.ApplyDamageModifiers(npc.LeftHandWeapon.MaximalDamage, npc)}";
            if (npc.LeftHandWeapon.RangedAttack)
                charInfoText += " "+Translate("Range: ") + npc.LeftHandWeapon.Range + " ";
            else
                charInfoText += " "+Translate("Melee")+" ";
            charInfoText += Translate("Skill:")+" " + npc.GetSkillValue(npc.LeftHandWeapon.SkillName) + "%";
        }
        _playerInfoField.text = charInfoText;
    }
	
	private string CombatUnitInfo (CombatUnit unit) {
		return  $"{Translate(unit.CharName)} [ {unit.Level} {Translate("lvl")} ] {Translate("HP")} {unit.HP}/{unit.MaxHP} \n"
                + $"{Translate("AP")} {unit.TotalAP}   {Translate("AC")} {unit.AC}   {Translate("DR")} {unit.DamageResistance}\n";
	}

    public void ShowExitInfo ()
    {
        CombatUnit player = _battleManager.AllCombatCharacters[_battleManager.Player];
        string charInfoText = Translate("Exit battle") + "\n"
                            + Translate("Exit costs ") + player.TotalAP + Translate(" AP for you and takes one turn") + "\n"
                            + (player.PlanningAP < player.TotalAP ? Translate("You have not enough AP") : "");
        _playerInfoField.text = charInfoText;
    }

    public void EndTurn()
    {
        if (_battleManager.Status != "planning")
            return;
        CombatUnit activeCharacter = _battleManager.AllCombatCharacters[_battleManager.Player];
        if (activeCharacter.PlanningAP > 0)
            CombatAction.Wait(activeCharacter, _battleManager.Turn, activeCharacter.PlanningAP);
        _battleManager.NextPlayer();
    }
	
    public void Wait()
    {
        if (_battleManager.Status != "planning")
            return;
        int apCost = 0;
        int.TryParse(_spendApInput.text, out apCost);
        if (apCost<1 || apCost>_battleManager.AllCombatCharacters[_battleManager.Player].PlanningAP)
        {
            print($"Error. Can't spent {apCost} AP");
            return;
        }
        CombatAction.Wait(_battleManager.AllCombatCharacters[_battleManager.Player], _battleManager.Turn, apCost);
        if (_battleManager.AllCombatCharacters[_battleManager.Player].PlanningAP == 0)
        {
            _battleManager.NextPlayer();
        }
    }

    public void Reload ()
    {
        if (_battleManager.Status != "planning")
            return;
        CombatAction.Reload(_battleManager.AllCombatCharacters[_battleManager.Player], _battleManager.Turn);
        if (_battleManager.AllCombatCharacters[_battleManager.Player].PlanningAP == 0)
        {
            _battleManager.NextPlayer();
        }
    }
    public void Exit()
    {
        CombatUnit player = _battleManager.AllCombatCharacters[_battleManager.Player];
        if (_battleManager.Status != "planning" || player.TotalAP != player.PlanningAP)
            return;
        CombatAction.Exit(_battleManager.AllCombatCharacters[_battleManager.Player], _battleManager.Turn);
        if (_battleManager.AllCombatCharacters[_battleManager.Player].PlanningAP == 0)
        {
            _battleManager.NextPlayer();
        }
    }

    public void SetPlaningButtons(bool interactible)
    {
        foreach (Button button in _planningButtons)
            button.interactable=interactible;
        _spendApInput.interactable = interactible;
    }


    private string Translate(string text) => _localisation.Translate(text);
}
