using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleUserInterface : MonoBehaviour
{
    private float bigMessageVisionTime = 8f;

    [SerializeField] private BattleManager _battleManager;
    [SerializeField] private TextMeshProUGUI actionPointsText;
    [SerializeField] private TextMeshProUGUI weaponInfoField;
    [SerializeField] private TextMeshProUGUI scoreInfoField;
    [SerializeField] private TextMeshProUGUI playerInfoField;
    [SerializeField] private TextMeshProUGUI bigMessage;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private List<Button> planningButtons;
    public static BattleUserInterface Instance { get; private set; }
    public BattleManager BattleManager => _battleManager; //TODO This is crutch (( Much better to delete this
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);
    }

    public void UpdateAP(CombatUnit character) => actionPointsText.text = character.PlanningAP+" AP";

    public void ChangeWeapon()
    {
        if (BattleManager.Status == "planning")
        {
            _battleManager.AllCombatCharacters[BattleManager.Player].usesOffHand = !_battleManager.AllCombatCharacters[BattleManager.Player].usesOffHand;
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

        string weaponText = $"{weapon.ItemName} [ {weapon.APCost} AP ]\n";
        int meleeDamageBonus = (weapon.RangedAttack) ? 0 : attacker.MeleeDamageBonus;
        weaponText += "Damage: " + weapon.FormDamageDiapason(meleeDamageBonus) + " ";
        if (weapon.RangedAttack)
            weaponText += "Range: "+weapon.Range + "\n";
        else
            weaponText += "Melee\n";
        weaponText += "Skill: " + _battleManager.AllCombatCharacters[_battleManager.Player].GetSkillValue(weapon.SkillName)+" %";
        weaponInfoField.text = weaponText;
    }

    public void RefreshLevelInfo()
    {
        int totalEnemiesLevel = 0;
        int score = 0;
        string scoreInfoText = "";
        foreach (CombatCharacter cChar in _battleManager.AllCombatCharacters)
        {
            if (cChar.ai == "")
            {
                scoreInfoText += cChar.charName + " " + cChar.level + " lvl ";
                score += cChar.CombatExperience;
            }
                
            else if (!cChar.Dead)
            {
                totalEnemiesLevel += cChar.level;
            }
        }
        scoreInfoText += "\nEnemyes "+totalEnemiesLevel+" total lvl";
        scoreInfoText += "\nScore: " + score;
        scoreInfoField.text = scoreInfoText;
    }

    public void RefreshCharInfo() => RefreshCharInfo(_battleManager.AllCombatCharacters[BattleManager.Player] as CombatCharacter);
    public void RefreshCharInfo(CombatCharacter player)
    {
        string charInfoText = $"{player.charName} [ {player.level} lvl ]\n"
                            + player.ExperienceText+"\n\n"
                            + $"ST {player.ST} [+{player.MeleeDamageBonus} melee damage]\n"  //TODO Add melee damage
                            + $"PE {player.PE} [{player.PE-1} aim shoot range]\n" //TODO Change range formula to Property??
                            + $"EN {player.EN} [{player.MaxHP} Max HP]\n"
                            + $"AG {player.AG} [{player.TotalAP} AP, {player.AC} AC]";
        playerInfoField.text = charInfoText;
    }

    public void ShowEnemyInfo(NonPlayerCharacter npc)
    {
        string charInfoText = $"{npc.charName} [ {npc.level} lvl ]\n"
                    + $"HP {npc.HP}/{npc.MaxHP}   AC {npc.AC}   AP {npc.TotalAP}"
                    + $"\n\nWeapon - {npc.RightHandWeapon.ItemName}\nDamage: " + npc.RightHandWeapon.FormDamageDiapason();
        if (npc.RightHandWeapon.RangedAttack)
            charInfoText += " Range: " + npc.RightHandWeapon.Range + "\n";
        else
            charInfoText += " Melee\n";
            charInfoText += "Skill: " + npc.GetSkillValue(npc.RightHandWeapon.SkillName) + " %";

        if (npc.LeftHandWeapon!=null && npc.LeftHandWeapon != npc.RightHandWeapon)
        {
            charInfoText += $"\n\nWeapon 2 - {npc.LeftHandWeapon.ItemName}\n Damage: " + npc.LeftHandWeapon.FormDamageDiapason();
            if (npc.LeftHandWeapon.RangedAttack)
                charInfoText += "Range: " + npc.LeftHandWeapon.Range + "\n";
            else
                charInfoText += "Melee\n";
            charInfoText += "Skill: " + npc.GetSkillValue(npc.LeftHandWeapon.SkillName) + " %";
        }
        playerInfoField.text = charInfoText;
    }

    public void EndTurn()
    {
        if (BattleManager.Status != "planning")
            return;
        CombatUnit activeCharacter = _battleManager.AllCombatCharacters[_battleManager.Player];
        if (activeCharacter.PlanningAP > 0)
            CombatAction.Wait(activeCharacter, activeCharacter.PlanningAP);
        _battleManager.NextPlayer();
    }
    public void Wait()
    {
        if (BattleManager.Status != "planning")
            return;
        CombatAction.Wait(_battleManager.AllCombatCharacters[_battleManager.Player]);
        if (_battleManager.AllCombatCharacters[_battleManager.Player].PlanningAP == 0)
        {
            _battleManager.NextPlayer();
        }
    }

    public void SetPlaningButtons(bool interactible)
    {
        foreach (Button button in planningButtons)
            button.interactable=interactible;
    }
    public void ShowBigMessage (string message)
    {
        if (bigMessage.gameObject.activeSelf)
        {
            print("ERROR!!! Big message "+message+" was not shown");
            return;
        }
        bigMessage.gameObject.SetActive(true);
        Color color = bigMessage.color;
        color.a = 0f;
        bigMessage.color = color;
        bigMessage.text = message;
        StartCoroutine(ShowAndHide());

        IEnumerator ShowAndHide()
        {
            bool show = true;
            float alpha = 0f;
            while (show && (alpha < 1f)) 
            {
                alpha += 3 / bigMessageVisionTime * Time.deltaTime ;
                color.a = alpha;
                bigMessage.color = color;
                yield return null;
            }
            show = false;
            yield return new WaitForSeconds(bigMessageVisionTime / 3);

            while (!show && (alpha>0))
            {
                alpha -= 3 / bigMessageVisionTime * Time.deltaTime;
                color.a = alpha;
                bigMessage.color = color;
                yield return null;
            }
            bigMessage.gameObject.SetActive(false);
        }
    }
    public void StartButtonAction ()
    {
        if (BattleManager.Status != "starting")
            return;

        if (_battleManager.AllCombatCharacters.Count < 1 || _battleManager.AllCombatCharacters.Count > 2)
            return;

        foreach (CombatUnit pc in _battleManager.AllCombatCharacters)
            if (pc.ai != "")
                return;

        gameManager.StartBattle();

    }
}
