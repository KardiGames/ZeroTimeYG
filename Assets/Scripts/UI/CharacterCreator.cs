using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCreator : MonoBehaviour
{
    private readonly int statisticsSum = 20;

    [SerializeField] private GameObject characterPrefub;

    [SerializeField] private TMP_InputField ST;
    [SerializeField] private TMP_InputField PE;
    [SerializeField] private TMP_InputField EN;
    [SerializeField] private TMP_InputField AG;
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private TMP_Dropdown mainHandWeapon;
    [SerializeField] private TextMeshProUGUI mainHandText;
    [SerializeField] private TMP_Dropdown offHandWeapon;
    [SerializeField] private TextMeshProUGUI offHandText;

    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private Button createAddCharacterButton;
    [SerializeField] private Button startGameButton;

    List<string> weaponOptions = new();

    private string[] nameList = { "Jessie", "Lenee", "Sights", "Shay", "Shaw", "Stylez", 
        "Jennings", "Sindee", "Lomeli", "Stella", "Sophie", "Summer", "Stevie", "Lee", "Sins", 
        "Xander", "Danny", "Bruce", "Deen", "Nixon", "Diesel", "Ryan", "Kristof", "Derrick", 
        "Logan", "Brick", "Rocco", "Toni", "Jeremy", "Wylde", "Quinton", "Parker", "Brass", 
        "Karlo", "Anthony", "Tyler" };

    // Start is called before the first frame update
    void Start()
    {
        foreach (Item weapon in Item.items)
        {
            weaponOptions.Add(weapon.itemName);
        }
        mainHandWeapon.AddOptions(weaponOptions);
        offHandWeapon.AddOptions(weaponOptions);
        RefreshWeaponTexts();
    }

    private void OnEnable()
    {
        nameField.text = nameList[Random.Range(0, nameList.Length)];
    }

    public void ResetMainMenu() {
        createAddCharacterButton.GetComponentInChildren<TextMeshProUGUI>().text = "Create Character";
        createAddCharacterButton.interactable = true;
        startGameButton.interactable = false;
    }

    // Update is called once per frame
    public void CreateCharacter()
    {
        if (Status.Current != "starting")
            return;
        
        if (nameField.text=="")
        {
            errorText.text = "ERROR! You must enter you name!";
            return;
        }
        
        if (!CheckStatisticSum())
            return;

        Item mainWeapon = Item.GetItem(weaponOptions[mainHandWeapon.value]);
        Item offWeapon = Item.GetItem(weaponOptions[offHandWeapon.value]);
        if (mainWeapon == null || offWeapon == null)
            return;

        GameObject createdCharacter = Instantiate(characterPrefub);
        if (!createdCharacter.GetComponent<CombatCharacter>().FulfillCharacter(nameField.text, int.Parse(ST.text), int.Parse(PE.text), int.Parse(EN.text), int.Parse(AG.text), mainWeapon, offWeapon))
            return;

        if (CombatCharacter.cCList.Count == 1)
        {
            createAddCharacterButton.GetComponentInChildren<TextMeshProUGUI>().text = "Add Character";
            createAddCharacterButton.interactable = true;
        } else if (CombatCharacter.cCList.Count == 2)
        {
            createAddCharacterButton.GetComponentInChildren<TextMeshProUGUI>().text = "Create Character";
        } 
        
        gameObject.SetActive(false);
        startGameButton.interactable = true;
    }

    public void RefreshWeaponTexts()
    {
        Item weapon=Item.GetItem(weaponOptions[mainHandWeapon.value]);
        if (weapon != null)
            mainHandText.text = FormWeaponText(weapon);
        
        weapon= Item.GetItem(weaponOptions[offHandWeapon.value]);
        if (weapon != null)
            offHandText.text = FormWeaponText(weapon);

        string FormWeaponText (Item weapon)
        {
            string weaponText = $"{weapon.itemName} [ {weapon.apCost} AP ]\n";
            weaponText += "Damage: " + weapon.FormDamageDiapason(0) + " ";
            if (weapon.rangedAttack)
                weaponText += "Range: " + weapon.Range + "\n";
            else
                weaponText += "Melee\n";
            return weaponText;
        }
    }

    public bool CheckStatisticSum()
    {
        int st;
        int pe;
        int en;
        int ag;

        if (!(int.TryParse(ST.text, out st) && int.TryParse(PE.text, out pe) && int.TryParse(EN.text, out en) && int.TryParse(AG.text, out ag)))
        {
            errorText.text = "Please input just numbers to statistics";
            return false;
        }
        
        if (st<1 || st>10)
        {
            errorText.text = $"ERROR! ST must be between 1 and 10, but now it's {st}";
            return false;
        }
        if (pe < 1 || pe > 10)
        {
            errorText.text = $"ERROR! PE must be between 1 and 10, but now it's {pe}";
            return false;
        }
        if (en < 1 || en > 10)
        {
            errorText.text = $"ERROR! EN must be between 1 and 10, but now it's {en}";
            return false;
        }
        if (ag < 1 || ag > 10)
        {
            errorText.text = $"ERROR! AG must be between 1 and 10, but now it's {ag}";
            return false;
        }


        if (st+pe+en+ag == statisticsSum)
        {
            errorText.text = "";
            return true;
        } else
        {
            errorText.text = $"ERROR! Sum of statistics must be {statisticsSum}, but now {st + pe + en + ag}";
            return false;
        }
            
    }
}