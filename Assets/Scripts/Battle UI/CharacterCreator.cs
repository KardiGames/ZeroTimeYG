using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCreator : MonoBehaviour
{
    private readonly int statisticsSum = 20;

    [SerializeField] GameObject createdCharacter;
    [SerializeField] private GameObject characterPrefub;
    [SerializeField] private BattleManager battleManager;

    [SerializeField] private TMP_InputField ST;
    [SerializeField] private TMP_InputField PE;
    [SerializeField] private TMP_InputField EN;
    [SerializeField] private TMP_InputField AG;
    [SerializeField] private TMP_InputField IN;
    [SerializeField] private TMP_InputField nameField;

    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private Button createCharacterButton;
    [SerializeField] private Button startGameButton;

    private List<string> weaponOptions = new();

    private string[] nameList = { "Jessie", "Lenee", "Sights", "Shay", "Shaw", "Stylez", 
        "Jennings", "Sindee", "Lomeli", "Stella", "Sophie", "Summer", "Stevie", "Lee", "Sins", 
        "Xander", "Danny", "Bruce", "Deen", "Nixon", "Diesel", "Ryan", "Kristof", "Derrick", 
        "Logan", "Brick", "Rocco", "Toni", "Jeremy", "Wylde", "Quinton", "Parker", "Brass", 
        "Karlo", "Anthony", "Tyler" };

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        nameField.text = nameList[Random.Range(0, nameList.Length)];
    }

    // Update is called once per frame
    public void CreateCharacter()
    {
        if (nameField.text=="")
        {
            errorText.text = "ERROR! You must enter you name!";
            return;
        }
        
        if (!CheckStatisticsValues())
            return;

        createdCharacter.GetComponent<WorldCharacter>().FulfillCharacter(nameField.text, int.Parse(ST.text), int.Parse(PE.text), int.Parse(EN.text), int.Parse(AG.text), int.Parse(IN.text));
        gameObject.SetActive(false);
        startGameButton.interactable = true;
    }
    public string FormWeaponText(Weapon weapon)
    {
        string weaponText = $"{weapon.ItemName} [ {weapon.APCost} AP ]\n";
        weaponText += "Damage: " + weapon.FormDamageDiapason(0) + " ";
        if (weapon.RangedAttack)
            weaponText += "Range: " + weapon.Range + "\n";
        else
            weaponText += "Melee\n";
        return weaponText;
    }

    public bool CheckStatisticsValues()
    {
        int st;
        int pe;
        int en;
        int ag;
        int in_;

        if (!(int.TryParse(ST.text, out st) && int.TryParse(PE.text, out pe) && int.TryParse(EN.text, out en) && int.TryParse(AG.text, out ag) && int.TryParse(AG.text, out in_)))
        {
            errorText.text = "Please input just numbers to statistics";
            return false;
        }
        
        bool CheckStatisticsBorders (Dictionary<string, int> statistics) {
            foreach (KeyValuePair<string, int> statistic in statistics)
            {
                if (statistic.Value < 1 || statistic.Value > 10)
                {
                    errorText.text = $"ERROR! {statistic.Key} must be between 1 and 10, but now it's {statistic.Value}";
                    return false;
                }
            }
            return true;
        }

        if (!CheckStatisticsBorders(new Dictionary<string, int>() {
            {"ST", st}, {"PE", pe}, {"EN", en}, {"AG", ag}, {"IN", in_}
        }))
            return false;

        if (st+pe+en+ag+in_ == statisticsSum)
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