using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    private float bigMessageVisionTime = 8f;

    public static UserInterface Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI actionPointsText;
    [SerializeField] private TextMeshProUGUI weaponInfoField;
    [SerializeField] private TextMeshProUGUI scoreInfoField;
    [SerializeField] private TextMeshProUGUI bestScoreField;
    [SerializeField] private TextMeshProUGUI playerInfoField;
    [SerializeField] private TextMeshProUGUI bigMessage;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private List<Button> planningButtons;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);
    }

    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        if (Status.Current == "starting")
            gameObject.GetComponentInChildren<CharacterCreator>(true).ResetMainMenu();
    }

    public void UpdateAP(CombatCharacter character) => actionPointsText.text = character.PlanningAP+" AP";

    public void ChangeWeapon()
    {
        if (Status.Current == "planning")
        {
            CombatCharacter.cCList[Status.Player].usesOffHand = !CombatCharacter.cCList[Status.Player].usesOffHand;
            ShowWeaponStats();
        }
    }

    public void ShowWeaponStats() => ShowWeaponStats(CombatCharacter.cCList[Status.Player]);

    public void ShowWeaponStats(CombatCharacter attacker)
    {
        Item weapon;
        if (attacker.usesOffHand)
        {
            weapon = attacker.equipment[1];
        } else
        {
            weapon = attacker.equipment[0];
        }

        string weaponText = $"{weapon.itemName} [ {weapon.apCost} AP ]\n";
        int meleeDamageBonus = (weapon.rangedAttack) ? 0 : attacker.MeleeDamageBonus;
        weaponText += "Damage: " + weapon.FormDamageDiapason(meleeDamageBonus) + " ";
        if (weapon.rangedAttack)
            weaponText += "Range: "+weapon.Range + "\n";
        else
            weaponText += "Melee\n";
        weaponText += "Skill: " + CombatCharacter.cCList[Status.Player].skills[weapon.skillname]+" %";
        weaponInfoField.text = weaponText;
    }

    public void RefreshLevelInfo()
    {
        int totalEnemiesLevel = 0;
        int score = 0;
        string scoreInfoText = "";
        foreach (CombatCharacter cChar in CombatCharacter.cCList)
        {
            if (cChar.ai == "")
            {
                scoreInfoText += cChar.charName + " " + cChar.level + " lvl ";
                score += cChar.Experience;
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
    public void ShowBestScore(LinkedList<(string,int)> winners)
    {
        string bestScoreText = "Best Score: \n";
        foreach ((string,int) currentNode in winners)
            bestScoreText += currentNode.Item2 + " - " + currentNode.Item1+"\n";
        bestScoreField.text = bestScoreText;
    }
    public void RefreshCharInfo() => RefreshCharInfo(CombatCharacter.cCList[Status.Player]);
    public void RefreshCharInfo(CombatCharacter player)
    {
        string charInfoText = $"{player.charName} [ {player.level} lvl ]\n"
                            + player.ExperienceText+"\n\n"
                            + $"ST {player.ST} [+{player.MeleeDamageBonus} melee damage]\n"  //TODO Add melee damage
                            + $"PE {player.PE} [{player.PE-1} aim shoot range]\n" //TODO Change range formula to Property??
                            + $"EN {player.EN} [{player.MaxHP} Max HP]\n"
                            + $"AG {player.AG} [{player.totalAP} AP, {player.AC} AC]";
        playerInfoField.text = charInfoText;
    }

    public void ShowEnemyInfo(NonPlayerCharacter npc)
    {
        string charInfoText = $"{npc.charName} [ {npc.level} lvl ]\n"
                    + $"HP {npc.HP}/{npc.MaxHP}   AC {npc.AC}   AP {npc.totalAP}"
                    + $"\n\nWeapon - {npc.equipment[0].itemName}\nDamage: " + npc.equipment[0].FormDamageDiapason();
        if (npc.equipment[0].rangedAttack)
            charInfoText += " Range: " + npc.equipment[0].Range + "\n";
        else
            charInfoText += " Melee\n";
            charInfoText += "Skill: " + npc.skills[npc.equipment[0].skillname] + " %";

        if (npc.equipment[1]!=null && npc.equipment[1]!=npc.equipment[0])
        {
            charInfoText += $"\n\nWeapon 2 - {npc.equipment[1].itemName}\n Damage: " + npc.equipment[1].FormDamageDiapason();
            if (npc.equipment[1].rangedAttack)
                charInfoText += "Range: " + npc.equipment[1].Range + "\n";
            else
                charInfoText += "Melee\n";
            charInfoText += "Skill: " + npc.skills[npc.equipment[1].skillname] + " %";
        }
        playerInfoField.text = charInfoText;
    }

    public void EndTurn()
    {
        if (Status.Current != "planning")
            return;
        CombatCharacter activeCharacter = CombatCharacter.cCList[Status.Player];
        if (activeCharacter.PlanningAP > 0)
            CombatAction.Wait(activeCharacter, activeCharacter.PlanningAP);
        Status.NextPlayer();
    }
    public void Wait()
    {
        if (Status.Current != "planning")
            return;
        CombatAction.Wait(CombatCharacter.cCList[Status.Player]);
        if (CombatCharacter.cCList[Status.Player].PlanningAP == 0)
        {
            Status.NextPlayer();
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
        if (Status.Current != "starting")
            return;

        if (CombatCharacter.cCList.Count < 1 || CombatCharacter.cCList.Count > 2)
            return;

        foreach (CombatCharacter pc in CombatCharacter.cCList)
            if (pc.ai != "")
                return;

        mainMenu.SetActive(false);
        gameManager.StartGame();

    }
}
