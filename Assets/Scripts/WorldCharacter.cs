using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCharacter : MonoBehaviour
{
    [SerializeField] private Equipment equipment;
    [SerializeField] private Inventory inventory;

    public Equipment Equipment => equipment;

    private void Start()
    {

    }


}
