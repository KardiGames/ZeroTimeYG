using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("Location buttons")]
    [SerializeField] private GameObject _locationPanel;
    [SerializeField] private Button _factoryButton;
    [SerializeField] private Button _mineButton;
    [SerializeField] private Button _laboratoryButton;
	
	[Header("Character buttons+")]
    [SerializeField] private Button _inventoryButton;
    [SerializeField] private Button _equipmentButton;
    [SerializeField] private Button _skillsButton;
    [SerializeField] private TextMeshProUGUI _playerAPText;

    [Header("Factory panel")]
    [SerializeField] private Factory _factoryOnGameObject;
    [SerializeField] private GameObject _factoryPanel;
	[SerializeField] private TMP_Dropdown _factoryDropdown;
	[SerializeField] private Button _enterFactoryButton;

    [Header("Mine panel")]
    [SerializeField] private Mine _mineOnGameObject;
    [SerializeField] private GameObject _minePanel;
    [SerializeField] private TMP_Dropdown _mineDropdown;
    [SerializeField] private Button _enterMineButton;

    [Header("Usable links")]
	[SerializeField] private GameManager _gameManager;
    [SerializeField] private SaveData _saveData;
    [SerializeField] private WorldCharacter _character;
    [SerializeField] private WorldMap _map;
    [SerializeField] private InformationPanelUI _informationPanel;
    [SerializeField] private Localisation _localisatiuon;

    private string[] _buildingNames;

    public void EnterLocation ()
    {
        if (!_map.AreBuildingsFound(_character.X, _character.Y))
        {
            GlobalUserInterface.Instance.ShowError("You don't know are there some objects here. Search here before");
            return;
        }

        string[] buildingTypes = _saveData.TypesOfBuildingsOnArea(_character.X, _character.Y);

        if (buildingTypes.Contains("Factory"))
            _factoryButton.interactable = true;
        if (buildingTypes.Contains("Mine"))
            _mineButton.interactable = true;
        if (buildingTypes.Contains("Laboratory"))
            _laboratoryButton.interactable = true;
    }

    public void ExitLocation()
    {
        if (_factoryOnGameObject.Name!="")
        {
            _factoryOnGameObject.ExitBuilding();
            _factoryPanel.SetActive(false);
        }

        if (_mineOnGameObject.Name!="")
        {
            _mineOnGameObject.ExitBuilding();
            _minePanel.SetActive(false);
        }

        _locationPanel.SetActive(true);
        _factoryButton.interactable = false;
        _mineButton.interactable = false;
        _laboratoryButton.interactable = false;
    }
	
	public void EnterFactory() => EnterBuilding(_buildingNames[_factoryDropdown.value], _factoryOnGameObject);
	public void EnterMine() => EnterBuilding(_buildingNames[_mineDropdown.value], _mineOnGameObject);

	
	private void EnterBuilding (string buildingName, IWorldBuilding buildingOnGameObject)
    {
		if (buildingOnGameObject.Name != "")
        {
            print($"Warning. Name of building on entering was not empty. ({buildingOnGameObject.Name}). Starting exit with save");
            buildingOnGameObject.ExitBuilding();
        }
		
		string jsonString=_saveData.GetBuildingJsonString(_character.X, _character.Y, buildingName);
		
		if (jsonString!="")
            buildingOnGameObject.FromJson(jsonString);
    }

    public void OpenFactoryPanel()
    {
        _buildingNames = _saveData.BuildingsOfTypeOnArea(_character.X, _character.Y, _factoryOnGameObject);
		if (_buildingNames.Length==0)
			return;
        _factoryDropdown.options.Clear();
		_factoryDropdown.AddOptions(new List <string> (_buildingNames.Select(n=>Translate(n))));
		EnterBuilding(_buildingNames[0], _factoryOnGameObject);
    }

    public void OpenMinePanel()
        {
        _buildingNames = _saveData.BuildingsOfTypeOnArea(_character.X, _character.Y, _mineOnGameObject);
        if (_buildingNames.Length == 0)
            return;
        _mineDropdown.options.Clear();
        _mineDropdown.AddOptions(new List<string>(_buildingNames.Select(n => Translate(n))));
        EnterBuilding(_buildingNames[0], _mineOnGameObject);
    }


    public void OnFactoryDropdownChange()
    {
        if (_buildingNames[_factoryDropdown.value] == _factoryOnGameObject.Name)
			_enterFactoryButton.interactable=false;
		else
			_enterFactoryButton.interactable=true;
    }

    public void OnMineDropdownChange()
    {
        if (_buildingNames[_mineDropdown.value] == _mineOnGameObject.Name)
            _enterMineButton.interactable = false;
        else
            _enterMineButton.interactable = true;
    }

    private void UpdateAP ()
    {
        _playerAPText.text = _character.AP+ " " + Translate("AP");
    }

    private void OnEnable()
    {
        if (_character.CharacterName != "")
            UpdateAP();
        _character.ActionPoints.OnAPValueChanged += UpdateAP;
        _localisatiuon.OnLanguageChangedEvent += UpdateAP;
    }

    private void OnDisable()
    {
        _character.ActionPoints.OnAPValueChanged -= UpdateAP;
        _localisatiuon.OnLanguageChangedEvent -= UpdateAP;
    }

    private string Translate(string text) =>
        _localisatiuon.Translate(text);
}
