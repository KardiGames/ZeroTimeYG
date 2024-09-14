using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCreator : MonoBehaviour
{
    [SerializeField] private WorldCharacter _playerCharacter;
    [SerializeField] private CharacterUI _characterUI;
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private TMP_InputField _nameField;
    [SerializeField] private TextMeshProUGUI _errorText;
    [SerializeField] private Button _setNameButton;
    [SerializeField] private Button _closeCross;
    [SerializeField] private TextMeshProUGUI _nameText;

    private string[] nameList = { "Jessie", "Lenee", "Sights", "Shay", "Shaw", "Stylez", 
        "Jennings", "Sindee", "Lomeli", "Stella", "Sophie", "Summer", "Stevie", "Lee", "Sins", 
        "Xander", "Danny", "Bruce", "Deen", "Nixon", "Diesel", "Ryan", "Kristof", "Derrick", 
        "Logan", "Brick", "Rocco", "Toni", "Jeremy", "Wylde", "Quinton", "Parker", "Brass", 
        "Karlo", "Anthony", "Tyler" };


    private void OnEnable()
    {
        print("Character creation 5");
        _nameText.gameObject.SetActive(false);
        _nameField.gameObject.SetActive(true);
        _setNameButton.gameObject.SetActive(true);
        _closeCross.gameObject.SetActive(false);
        _mainMenu.SetActive(false);
        _nameField.text = nameList[Random.Range(0, nameList.Length)];
    }

    public void CreateCharacter()
    {
        if (_nameField.text=="")
        {
            _errorText.gameObject.SetActive(true);
            _errorText.text = "ERROR! You must enter you name!";
            return;
        }

        print("Character creation 7");
        _playerCharacter.SetName(_nameField.text);
        _errorText.gameObject.SetActive(false);
        _setNameButton.gameObject.SetActive(false);
        _nameField.gameObject.SetActive(false);
        _nameText.gameObject.SetActive(true);
        _characterUI.ShowNameLevelText();
        _mainMenu.SetActive(true);
        _closeCross.gameObject.SetActive(true);

        print("Character creation 8 - looks like completed. Check is CharacterCreator script disabled");
        this.enabled = false;
    }

    //TODO Bad plase for this method. Move some way or delete it.
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

}