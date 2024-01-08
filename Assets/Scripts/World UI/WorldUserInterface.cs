using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorldUserInterface : MonoBehaviour
{
    [SerializeField] private WorldCharacter playerCharacter;
    [SerializeField] private GameObject targetUIInventory;
    [SerializeField] private GameObject taskTimer; //TODO Delete it? Why it is here?
 
    [SerializeField] private TextMeshProUGUI bigMessage;
    public void ShowDamage ()
    {
        print((playerCharacter.Equipment[0] as Weapon).FormDamageDiapason()+ " "+ (playerCharacter.Equipment[1] as Weapon).FormDamageDiapason());
    }

    public void OpenTargetInventory (Inventory inventory)
    {
        if (inventory == null)
            return;
        targetUIInventory.SetActive(true);
        targetUIInventory.GetComponent<InventoryUIContentFiller>().Inventory=inventory;
    }

    public void ShowBigMessage(string message)
    {
        if (bigMessage.gameObject.activeSelf)
        {
            print("ERROR!!! Big message " + message + " was not shown");
            return;
        }
        bigMessage.gameObject.SetActive(true);
        Color color = bigMessage.color;
        color.a = 0f;
        bigMessage.color = color;
        bigMessage.text = message;
        StartCoroutine(ShowAndHide());

        IEnumerator ShowAndHide()
        {
            bool show = true;
            float alpha = 0f;
            while (show && (alpha < 1f))
            {
                alpha += 3 / 3 * Time.deltaTime;
                color.a = alpha;
                bigMessage.color = color;
                yield return null;
            }
            show = false;
            yield return new WaitForSeconds(3 / 3);

            while (!show && (alpha > 0))
            {
                alpha -= 3 / 3 * Time.deltaTime;
                color.a = alpha;
                bigMessage.color = color;
                yield return null;
            }
            bigMessage.gameObject.SetActive(false);
        }
    }
}
