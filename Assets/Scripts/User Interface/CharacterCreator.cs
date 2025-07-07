using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCreator : MonoBehaviour
{
    [SerializeField] private WorldCharacter _playerCharacter;
    [SerializeField] private WorldMap _worldMap;
    [SerializeField] private CharacterUI _characterUI;
    [SerializeField] private GlobalUserInterface _globalUI;
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

    [DllImport("__Internal")]
    private static extern void RequestPlayerName();

    private void OnEnable()
    {
        _worldMap.enabled = false;
        _nameText.gameObject.SetActive(false);
        _nameField.gameObject.SetActive(true);
        _setNameButton.gameObject.SetActive(true);
        _closeCross.gameObject.SetActive(false);
        _mainMenu.SetActive(false);
        _nameField.text = nameList[Random.Range(0, nameList.Length)];
        RequestPlayerName();
        _globalUI.ShowBlackMessage("@Intro");
    }

    public void CreateCharacter()
    {
        if (_nameField.text=="")
        {
            _errorText.gameObject.SetActive(true);
            _errorText.text = _globalUI.Localisation.Translate("ERROR! You must enter you name!");
            return;
        }

        _playerCharacter.SetName(_nameField.text);
        _errorText.gameObject.SetActive(false);
        _setNameButton.gameObject.SetActive(false);
        _nameField.gameObject.SetActive(false);
        _nameText.gameObject.SetActive(true);
        _characterUI.ShowNameLevelText();
        _mainMenu.SetActive(true);
        _closeCross.gameObject.SetActive(true);
        _worldMap.enabled = true;

        this.enabled = false;
    }
}