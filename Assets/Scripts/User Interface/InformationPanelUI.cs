using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class InformationPanelUI : MonoBehaviour
{
    [SerializeField] WorldCharacter _playerCharacter;
    [SerializeField] Localisation _localisation;
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] TextMeshProUGUI _typeText;
    [SerializeField] TextMeshProUGUI _infoText;
    [SerializeField] Image _itemIcon;
    [SerializeField] TextMeshProUGUI _itemTagsText;
    [SerializeField] TextMeshProUGUI _itemInfoText;
    [SerializeField] Image _producedItemIcon;
    private Item _blueprintProductionItem;

    // Update is called once per frame
    private void ClearElements()
    {
        _infoText.gameObject.SetActive(false);
        _itemIcon.gameObject.SetActive(false);
        _itemTagsText.text = "";
        _itemInfoText.gameObject.SetActive(false);
        _producedItemIcon.gameObject.SetActive(false);
    }
    public void ShowElementInfo (string elementName, string elementType, string infoText)
    {
        gameObject.SetActive(true);
        ClearElements();
        _nameText.text = Translate(elementName);
        _typeText.text = Translate(elementType);
        _infoText.gameObject.SetActive(true);
        _infoText.text = Translate(infoText);
    }

    public void ShowElementInfo(InfoPanelData data) =>
        ShowElementInfo(data.Name, data.Type, data.Text);

    public void ShowItemInfo (Item item)
    {
        if (item == null)
            return;

        if (item is Blueprint blueprint)
        {
            ShowBlueprintInfo(blueprint, null);
            return;
        }
        
        ClearElements();
        _nameText.text = Translate(item.ItemName);
        _typeText.text = Translate (item.GetType().ToString());
        _itemIcon.gameObject.SetActive(true);
        _itemIcon.sprite = item.Icon;
        _itemIcon.color = item.IconColor;

        if (item.Stackable)
            _itemTagsText.text += Translate("Amount: ") + item.Amount + "\n";
        _itemInfoText.gameObject.SetActive(true);

        if (item is Weapon weapon)
            AddWeaponInfo(weapon);
        else if (item is Armor armor)
            AddArmorInfo(armor);
        else
            _itemInfoText.text = "";
    }

    private void AddArmorInfo(Armor armor)
    {
        _itemTagsText.text += Translate("Quality: ") + armor.Quality + "/" + armor.MaxQuality + "\n";
        _itemInfoText.text = "Armor class (AC): " + armor.AC + "\n";
        _itemInfoText.text += "Damage resistance (DR): " + armor.DamageResistance + "\n";
    }

    private void AddWeaponInfo(Weapon weapon)
    {
        
        _itemTagsText.text += Translate("Quality: ")+weapon.Quality+"/"+weapon.MaxQuality+"\n";
        if (weapon.TwoHanded)
            _itemTagsText.text += Translate("Two handed")+"\n";
        if (weapon.RangedAttack)
            _itemTagsText.text += Translate("Ranged") +"\n";
        else 
            _itemTagsText.text += Translate("Melee") + "\n";
        if (weapon.AmmoType=="Energy Cell")
            _itemTagsText.text += Translate("Beam") + "\n";

        _itemInfoText.text = Translate("Attack cost: ") +weapon.APCost+ " AP" + "\n";
        _itemInfoText.text += Translate("Base damage: ") + weapon.FormDamageDiapason() + "\n";
        if (weapon.RangedAttack)
            _itemInfoText.text += Translate("Range: ") + weapon.Range + "\n";
        if (weapon.AmmoType!="")
        {
            _itemInfoText.text += Translate("Ammo type: ") + weapon.AmmoType+ "\n";
            _itemInfoText.text += Translate("Ammo capacity: ") + weapon.AmmoMaxAmount + "\n";
            _itemInfoText.text += Translate("Ammo per shot: ") + weapon.AmmoPerShot+ "\n";
        }

        _itemInfoText.text+="\n"+ Translate("Skill:") +"\n"+weapon.SkillName+" ("+_playerCharacter.Skills.GetTrainedValue(weapon.SkillName)+"%)\n";
        _itemInfoText.text += "Result damage: " + weapon.ApplyDamageModifiers(weapon.MinimalDamage, _playerCharacter) + " - " + weapon.ApplyDamageModifiers(weapon.MaximalDamage, _playerCharacter)+"\n";
    }

    public void ShowBlueprintInfo(Blueprint blueprint, Inventory inventory)
    {
        if (blueprint == null)
            return;

        ClearElements();
        _nameText.text = blueprint.ItemName;
        _typeText.text = Translate("Blueprint");
        _itemIcon.gameObject.SetActive(true);
        _itemIcon.sprite = blueprint.Icon;
        _itemIcon.color = blueprint.IconColor;
        if (blueprint.Stackable)
            _itemTagsText.text += "Amount: " + blueprint.Amount + "\n";
        _itemInfoText.gameObject.SetActive(true);
        _itemTagsText.text = "=======>\n=======>\n=======>";

        _blueprintProductionItem = blueprint.ItemToCreate; 
        _producedItemIcon.gameObject.SetActive(true);
        _producedItemIcon.sprite = _blueprintProductionItem.Icon;
        _producedItemIcon.color = _blueprintProductionItem.IconColor;
        _itemInfoText.text = Translate("Produses in factory ") + _blueprintProductionItem.ItemName + " x" + _blueprintProductionItem.Amount + "\n";
        _itemInfoText.text += Translate("Time to produse: ") + TaskByTimerUI.FormTimerText((int)blueprint.SecondsToFinish) + "\n";

        _itemInfoText.text += "\n"+ Translate("Resourses:") +"\n";
        for (int i=0; i<blueprint.ListOfResourses.Count; i++)
        {
            if (inventory!=null)
            {
                long currentResourceAmount = inventory.GetItemAmount(blueprint.ListOfResourses[i]);
                if (currentResourceAmount >= blueprint.AmountsOfResourses[i])
                    _itemInfoText.text += blueprint.AmountsOfResourses[i] + " / ";
                else
                    _itemInfoText.text += currentResourceAmount + " / ";
            }
            _itemInfoText.text += blueprint.AmountsOfResourses[i] + " x "+blueprint.ListOfResourses[i].ItemName+"\n";
        }

    }

    public void ShowBlueprintProductionItem()
    {
        print("Click Accepted");
        if (_blueprintProductionItem != null)
        {
            ShowItemInfo(_blueprintProductionItem);
        }
        else
            print("Error! Produced item is null");
    }

    private void OnEnable ()
    {
        _localisation.OnLanguageChangedEvent += ClosePanel;
    }

    private void OnDisable()
    {
        _localisation.OnLanguageChangedEvent -= ClosePanel;
    }
    private void ClosePanel(string s) => gameObject.SetActive(false);

    private string Translate(string text) => _localisation.Translate(text);
}
