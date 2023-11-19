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
        /*
        character.SerializList.Add((Item)Item.GetItem("Pistol").Clone());
        character.SerializList.Add(Item.GetItem("Plate"));
        character.SerializList.Add((Armor)Item.GetItem("Plate").Clone());
        */

        print ("External String:"+JsonUtility.ToJson(character));

        string jsonString = character.ToJson();
        print("Internal String:" + jsonString);

        //character.FromJson(jsonString);



        //character.FromJson("{ \"Name\":\"Alex\",\"HP\":25,\"currentHP\":15,\"inventoryJsonTempString\":\"{\\\"inventoryItems\\\":[{\\\"instanceID\\\":25910},{\\\"instanceID\\\":25912},{\\\"instanceID\\\":25916},{\\\"instanceID\\\":25916}]}\"}");
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
