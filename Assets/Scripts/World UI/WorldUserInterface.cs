using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUserInterface : MonoBehaviour
{
    [SerializeField] private WorldCharacter playerCharacter;

    private void Start()
    {
        playerCharacter.Equipment.TryToEquip(Instantiate(playerCharacter.Equipment[0]), 0);
    }
    public void ShowDamage ()
    {

        print((playerCharacter.Equipment[0] as Weapon).FormDamageDiapason()+ " "+ (playerCharacter.Equipment[1] as Weapon).FormDamageDiapason());
        (playerCharacter.Equipment[0] as Weapon).BoostDamage();
    }
}
