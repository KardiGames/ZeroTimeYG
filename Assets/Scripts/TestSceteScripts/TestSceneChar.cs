using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSceneChar : MonoBehaviour
{
    private Inventory inventory;    

    public string Name;
    public int HP;
    [SerializeField] private int currentHP = 15;
    [SerializeField] private string inventoryJsonTempString;
    //public string SerializString = "{\"Name\":\"AlexKardi\",\"HP\":25,\"CurrentHP\":25}";
    [SerializeField] private List<Item> serializList = new List<Item>();

    public Inventory Inventory => inventory;
    public List<Item> SerializList => serializList;
    public int CurrentHP { get => currentHP; set => currentHP = value; }

    public void Start()
    {
        inventory = GetComponent<Inventory>();
    }

    public void FromJson(string jsonString)
    {
        JsonUtility.FromJsonOverwrite(jsonString, this);
        inventory.FromJson(inventoryJsonTempString);
        inventoryJsonTempString = "";
    }

    public string ToJson()
    {
        inventoryJsonTempString = inventory.ToJson();
        string returnableJsonStriong = JsonUtility.ToJson(this);
        inventoryJsonTempString = "";
        return returnableJsonStriong;
    }
}
