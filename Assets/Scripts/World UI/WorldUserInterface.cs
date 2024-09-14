using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class WorldUserInterface : MonoBehaviour
{
    [SerializeField] private WorldCharacter _playerCharacter;
    [SerializeField] private GameObject _playerUIInventory;
    [SerializeField] private GameObject _targetUIInventory;
    [SerializeField] private TextMeshProUGUI _bigMessage;
    [SerializeField] private CharacterCreator _characterCreator;
    public void ShowDamage ()
    {
        print((_playerCharacter.Equipment[0] as Weapon).FormDamageDiapason()+ " "+ (_playerCharacter.Equipment[1] as Weapon).FormDamageDiapason());
    }

    public void OpenTargetInventory (Inventory inventory)
    {
        if (inventory == null)
            return;
        _targetUIInventory.SetActive(true);
        _targetUIInventory.GetComponent<InventoryUIContentFiller>().Inventory=inventory;
    }

    public void OpenPlayerInventory()
    {
        _playerUIInventory.SetActive(true);
        _playerUIInventory.GetComponent<InventoryUIContentFiller>().Inventory = _playerCharacter.Inventory;
    }
    public void ShowBigMessage(string message)
    {
        if (_bigMessage.gameObject.activeSelf)
        {
            print("ERROR!!! Big message " + message + " was not shown");
            return;
        }
        _bigMessage.gameObject.SetActive(true);
        Color color = _bigMessage.color;
        color.a = 0f;
        _bigMessage.color = color;
        _bigMessage.text = message;
        StartCoroutine(ShowAndHide());

        IEnumerator ShowAndHide()
        {
            bool show = true;
            float alpha = 0f;
            while (show && (alpha < 1f))
            {
                alpha += 3 / 3 * Time.deltaTime;
                color.a = alpha;
                _bigMessage.color = color;
                yield return null;
            }
            show = false;
            yield return new WaitForSeconds(3 / 3);

            while (!show && (alpha > 0))
            {
                alpha -= 3 / 3 * Time.deltaTime;
                color.a = alpha;
                _bigMessage.color = color;
                yield return null;
            }
            _bigMessage.gameObject.SetActive(false);
        }
    }
    public void CreateNewCharacter()
    {
        print("Character creation 2");
        _characterCreator.gameObject.SetActive(true);
        print("Character creation 4");
        _characterCreator.enabled = true;
        print("Character creation 6");
    }
}
