using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
	[Header("Location buttons")]
    [SerializeField] private Button _factoryButton;
    [SerializeField] private Button _mineButton;
    [SerializeField] private Button _laboratoryButton;
	
	[Header("Character buttons")]
    [SerializeField] private Button _inventoryButton;
    [SerializeField] private Button _equipmentButton;
    [SerializeField] private Button _skillsButton;

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

    (int x, int y) CurrentCoordinates = (0,0); //TODO Delete this, when coordinates system will work

    public void EnterLocation ()
    {
        string[] buildingTypes = _saveData.TypesOfBuildingsOnLocation(CurrentCoordinates.x, CurrentCoordinates.y);

        if (buildingTypes.Contains("Factory"))
            _factoryButton.interactable = true;
        if (buildingTypes.Contains("Mine"))
            _mineButton.interactable = true;
        if (buildingTypes.Contains("Laboratory"))
            _laboratoryButton.interactable = true;
    }
	
	public void EnterFactory() => EnterBuilding(_factoryDropdown.options[_factoryDropdown.value].text, _factoryOnGameObject);
	public void EnterMine() => EnterBuilding(_mineDropdown.options[_mineDropdown.value].text, _mineOnGameObject);

	
	private void EnterBuilding (string buildingName, IWorldBuilding buildingOnGameObject)
    {
		if (buildingOnGameObject.Name != "")
        {
            print($"Warning. Name of building on entering was not empty. ({buildingOnGameObject.Name}). Starting exit with save");
            buildingOnGameObject.ExitBuilding();
        }
		
		string jsonString=_saveData.GetBuildingJsonString(CurrentCoordinates.x, CurrentCoordinates.y, buildingName);
		
		if (jsonString!="")
            buildingOnGameObject.FromJson(jsonString);
    }

    public void OpenFactoryPanel()
    {
        string[] buildingNames = _saveData.BuildingsOfTypeOnLocation(CurrentCoordinates.x, CurrentCoordinates.y, _factoryOnGameObject);
		if (buildingNames.Length==0)
			return;
        _factoryDropdown.options.Clear();
		_factoryDropdown.AddOptions(new List <string> (buildingNames));
		EnterBuilding(buildingNames[0], _factoryOnGameObject);
    }

    public void OpenMinePanel()
        {
        string[] buildingNames = _saveData.BuildingsOfTypeOnLocation(CurrentCoordinates.x, CurrentCoordinates.y, _mineOnGameObject);
        if (buildingNames.Length == 0)
            return;
        _mineDropdown.options.Clear();
        _mineDropdown.AddOptions(new List<string>(buildingNames));
        EnterBuilding(buildingNames[0], _mineOnGameObject);
    }


    public void OnFactoryDropdownChange()
    {
        if (_factoryDropdown.options[_factoryDropdown.value].text == _factoryOnGameObject.Name)
			_enterFactoryButton.interactable=false;
		else
			_enterFactoryButton.interactable=true;
    }
}
