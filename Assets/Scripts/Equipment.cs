using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    [SerializeField] private List<ScriptableItem> equipment = new();

    public ScriptableItem this [int index]
    {
        get =>equipment[index];
    }

    public bool TryToEquip (ScriptableItem item, int slot)
    {
        if (slot>=equipment.Count || slot<0)
        {
            print("Trying to set something in eq slot " + slot);
            return false;
        }

        if (item == null)
            return false;

        equipment[slot] = item;
        return true;
    }
}
