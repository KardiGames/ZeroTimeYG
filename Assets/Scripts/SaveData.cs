using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    [SerializeField] private List<WorldBuildingData> globalMapBuildings=new(); //TODO check if need to make it SerField
	private string filePath = "savefile.txt";

    public string[] TypesOfBuildingsOnLocation (int x, int y)
    {
        List<string> allBuildingsOnLocations = new (from building in globalMapBuildings where (building.X == x && building.Y == y) select building.type.ToString());
        return allBuildingsOnLocations.Distinct().ToArray();

    }
    
    public string[] BuildingsOfTypeOnLocation (int x, int y, IWorldBuilding objectOfType) {
		WorldBuildingData.BuildingType type = BuildingTypeByObject(objectOfType);
		return globalMapBuildings.Where(
				building=>building.X==x 
				&& building.Y==y
				&& building.type==type)
			.Select(building=>building.Name)
			.ToArray();
	}
	
	public string GetBuildingJsonString (int x, int y, string buildingName) {
		foreach (var building in globalMapBuildings)
			if (building.X==x && building.Y==y && building.Name==buildingName)
				return building.jSonString;
		return "";
	}

    public void SaveBuilding (IWorldBuilding building)
    {
        if (building == null)
            return;

        WorldBuildingData savableData = FindBuildingOnMap(building);
        if (savableData==null)
        {
            savableData = new WorldBuildingData();
            globalMapBuildings.Add(savableData);
            print($"WORNING. Building {building.Name} was not on the map. Have created new one");
        }

        savableData.X = building.X;
        savableData.Y = building.Y;
        savableData.Name = building.Name;
        savableData.type = BuildingTypeByObject(building);
        savableData.jSonString = building.ToJson();
    }
	
	private WorldBuildingData.BuildingType BuildingTypeByName (string typeName) {
		if (typeName is "Factory")
            return WorldBuildingData.BuildingType.Factory;
		else if (typeName is "Mine") 
			return WorldBuildingData.BuildingType.Mine;
		else
			return WorldBuildingData.BuildingType.Laboratory;
	}
		
	private WorldBuildingData.BuildingType BuildingTypeByObject (IWorldBuilding buildingOfType) {
		return BuildingTypeByName(buildingOfType.GetType().Name);
	}

    private WorldBuildingData FindBuildingOnMap(IWorldBuilding building) => globalMapBuildings.
        Find(b => (b.X == building.X && b.Y == building.Y && b.Name == building.Name));

    internal void LoadSaveSystem()
    {
        if (globalMapBuildings.Count != 0)
            return;
        
        //TODO this is temporal part. detete it!
        WorldBuildingData newBuilding = new WorldBuildingData()
        {
            Name = "Factory 1",
            type = WorldBuildingData.BuildingType.Factory,
            jSonString = ""
        };

        globalMapBuildings.Add(newBuilding);
    }

    public void LoadFromFile()
    {
		if (!File.Exists(filePath))
			return;
		string saveText = File.ReadAllText(filePath);
		JsonUtility.FromJsonOverwrite(saveText, this);
    }
	
	public void SaveToFile() 
	{
		string saveText = JsonUtility.ToJson(this);
		File.WriteAllText(filePath, saveText);
	}
	
    [Serializable]
    private class WorldBuildingData
    {
        public enum BuildingType {Factory, Mine, Laboratory}

        public int X = 0;
        public int Y = 0;
        public BuildingType type;
        public string Name;
        public string jSonString;
    }
}
