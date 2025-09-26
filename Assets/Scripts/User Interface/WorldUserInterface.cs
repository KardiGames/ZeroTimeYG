using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class WorldUserInterface : MonoBehaviour
{
    private const float BIG_MESSAGE_TIME = 8f;

    [SerializeField] private WorldCharacter _playerCharacter;
    [SerializeField] private CharacterCreator _characterCreator;
    [SerializeField] private GameObject _playerUIInventory;
    [SerializeField] private GameObject _targetUIInventory;
    [SerializeField] private InformationPanelUI _informationUI;
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private TextMeshProUGUI _bigMessage;

    public InformationPanelUI InformationPanelUI =>_informationUI;
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
            GlobalUserInterface.Instance.ShowError("ERROR!!! Big message " + message + " was not shown");
            return;
        }
        _bigMessage.gameObject.SetActive(true);
        Color color = _bigMessage.color;
        color.a = 0f;
        _bigMessage.color = color;
        _bigMessage.text = GlobalUserInterface.Instance.Localisation.Translate(message);
        StartCoroutine(ShowAndHide());

        IEnumerator ShowAndHide()
        {
            bool show = true;
            float alpha = 0f;
            while (show && (alpha < 1f))
            {
                alpha += 3 / BIG_MESSAGE_TIME * Time.deltaTime;
                color.a = alpha;
                _bigMessage.color = color;
                yield return null;
            }
            show = false;
            yield return new WaitForSeconds(BIG_MESSAGE_TIME / 3);

            while (!show && (alpha > 0))
            {
                alpha -= 3 / BIG_MESSAGE_TIME * Time.deltaTime;
                color.a = alpha;
                _bigMessage.color = color;
                yield return null;
            }
            _bigMessage.gameObject.SetActive(false);
        }
    }

    public void CreateNewCharacter()
    {
        _characterCreator.gameObject.SetActive(true);
        _characterCreator.enabled = true;
    }

    public void ShowItemInfo(Item item)
    {
        _informationUI.gameObject.SetActive(true);
        _informationUI.ShowItemInfo(item);
    }

    internal void HideLoadingScreen()
    {
        _loadingScreen.SetActive(false);
    }
}
