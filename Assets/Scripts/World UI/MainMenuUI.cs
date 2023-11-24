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

	[Header("Usable links")]
	[SerializeField] private GameManager _gameManager;
    [SerializeField] private SaveData _saveData;
    [SerializeField] private WorldCharacter _character;

    (int x, int y) CurrentCoordinates = (0,0);

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
	
	public void EnterFactory() => EnterFactory(_factoryDropdown.options[_factoryDropdown.value].text);
	
	private void EnterFactory (string factoryName)
    {
		if (_factoryOnGameObject.Name!="")
			_factoryOnGameObject.ExitBuilding();
		
		string jsonString=_saveData.GetBuildingJsonString(CurrentCoordinates.x, CurrentCoordinates.y, _factoryDropdown.options[_factoryDropdown.value].text);
		
		if (jsonString!="")
			_factoryOnGameObject.FromJson(jsonString);
    }

    public void OpenFactoryPanel()
    {
        string[] buildingNames = _saveData.BuildingsOfTypeOnLocation(CurrentCoordinates.x, CurrentCoordinates.y, _factoryOnGameObject);
		if (buildingNames.Length==0)
			return;
        _factoryDropdown.options.Clear();
		_factoryDropdown.AddOptions(new List <string> (buildingNames));
		EnterFactory(buildingNames[0]);
    }


    public void OnFactoryDropdownChange()
    {
        if (_factoryDropdown.options[_factoryDropdown.value].text == _factoryOnGameObject.Name)
			_enterFactoryButton.interactable=false;
		else
			_enterFactoryButton.interactable=true;
    }
}
