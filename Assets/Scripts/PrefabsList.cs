using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabsList : MonoBehaviour
{
    //Singleton var
    public static PrefabsList instance { get; internal set; }
    
    //Prefabs for different NPC

    //Links to sounds
    public Dictionary<string, AudioClip> weaponSounds;
    public Dictionary<string, AudioClip> characterMoveSounds;

    //TEMP prefabs
    public GameObject coordinatesText;

    private void Awake()
    {
        if ((instance != this) && (instance != null))
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }


}
