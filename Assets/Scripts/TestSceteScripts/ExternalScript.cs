using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TestSceneChar character = GetComponent<TestSceneChar>();
        Item.LoadItems();
        character.SerializList.Add((Item)Item.GetItem("Sniper rifle").Clone());
        character.SerializList.Add(Item.GetItem("Sniper rifle"));
        character.SerializList.Add((Item)Item.GetItem("Pistol").Clone());


        print ("External String:"+JsonUtility.ToJson(character));

        string jsonString = character.ToJson();
        print("Internal String:" + jsonString);

        character.SerializList.Add(Item.GetItem("Sniper rifle"));
        character.SerializList.RemoveAt(1);
        character.Inventory.Remove(this, "Res");
        character.Inventory.Remove(this, "Res");
        
        character.FromJson(jsonString);
        //print(character.CurrentHP);
        /*
        
        Item rifle= Item.GetItem("Sniper rifle");
        print(rifle.itemName);
        print(JsonUtility.ToJson(rifle));
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
