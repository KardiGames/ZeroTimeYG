using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "NpcBlank", menuName = "NpcBlank")]
public class NpcBlank : ScriptableObject
{
    [field: SerializeField] public GameObject PrefabToFill { get; private set; }
    [field: SerializeField] public float difficulty { get; private set; }
    [field: SerializeField] public string npcName { get; private set; }
    [field: SerializeField] public string ai { get; private set; }
    [field: SerializeField] public int maxHP { get; private set; }
    [field: SerializeField] public int totalAP { get; private set; }
    [field: SerializeField] public int AC { get; private set; }

    [field: SerializeField] public bool rangedAttack { get; private set; }
    [field: SerializeField] public string attackName { get; private set; }
    [field: SerializeField] public int damageMultipler { get; private set; }
    [field: SerializeField] public int damageDise { get; private set; }
    [field: SerializeField] public int damagePlus { get; private set; }
    [field: SerializeField] public int damageRange { get; private set; }
    [field: SerializeField] public int attackAP { get; private set; }

    public NonPlayerCharacter Spawn(BattleManager manager, int level, int[] position)
    {
        if (level < 1) return null;
        NonPlayerCharacter npc = Instantiate(PrefabToFill).GetComponent<NonPlayerCharacter>();
        npc.FillParameters(this, manager);

        for (int i = 1; i < level; i++)
            npc.LevelUp(this);

        npc.SetPosition(position);
        npc.PrepareToFight();
        return npc;
    }
}
