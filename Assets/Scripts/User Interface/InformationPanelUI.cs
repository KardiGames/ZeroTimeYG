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

    public void ShowElementInfo(string elementName, string elementType, string infoText)
    {
        gameObject.SetActive(true);
        ClearElements();
        _nameText.text = elementName;
        _typeText.text = elementType;
        _infoText.gameObject.SetActive(true);
        _infoText.text = infoText;
    }

    public void ShowTranslatedElement(string elementName, string elementType, string infoText) =>
        ShowElementInfo(Translate(elementName), Translate(elementType), Translate(infoText));

    public void ShowElementInfo(InfoPanelData data) =>
        ShowTranslatedElement(data.Name, data.Type, data.Text);

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
        _itemInfoText.text = Translate("Armor class (AC): ") + armor.AC + "\n";
        _itemInfoText.text += Translate("Damage resistance (DR): ") + armor.DamageResistance + "\n";
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
            _itemInfoText.text += Translate("Ammo type: ") + Translate(weapon.AmmoType)+ "\n";
            _itemInfoText.text += Translate("Ammo capacity: ") + weapon.AmmoMaxAmount + "\n";
            _itemInfoText.text += Translate("Ammo per shot: ") + weapon.AmmoPerShot+ "\n";
        }

        _itemInfoText.text+="\n"+ Translate("Skill:") +"\n"+ Translate(weapon.SkillName)+" ("+_playerCharacter.Skills.GetSkillValue(weapon.SkillName)+"%)\n";
        _itemInfoText.text += Translate("Result damage: ") + weapon.ApplyDamageModifiers(weapon.MinimalDamage, _playerCharacter) + " - " + weapon.ApplyDamageModifiers(weapon.MaximalDamage, _playerCharacter)+"\n";
    }

    internal void ShowEndBattleInfo(int killPoints, int mineLevel, int experience, int rewardPoints, bool dead)
    {
        string headerText = dead ? "You died in battle!" : "You survived!";
        string pointsText = Translate("You have got") + ":\n" + killPoints + Translate(" battle points");

        string infoText = Translate("Considering the result of the battle, you got:") + "\n";
        infoText += experience + Translate(" experience poins") + (dead ? " (" + Translate("Dangerous mining") + " " + _playerCharacter.Skills.GetSkillValue("Dangerous mining") + "%)" : "") +".\n";

        infoText += Translate("Loot for ") + rewardPoints + Translate (" points") + " (" + Translate("Attentive search")+ " " + _playerCharacter.Skills.GetSkillValue("Attentive search") + "%).\n";
        infoText += Translate("Threat level") +" "+ (dead ? Translate ("decreased to ") : (mineLevel<=killPoints) ? Translate("increased to ") : Translate("remained at ")) + mineLevel + ".\n";
        if (dead) { 
            infoText += "\n" + Translate("Equipment received additional damage.") + "\n";
            infoText += Translate("You have spent additional ") + ActionPoints.ADDITIONAL_DEATH_AP_COST + Translate("AP to restoration") + ".\n"; 
        }

        ShowElementInfo(Translate(headerText), pointsText, infoText);
    }

    public void ShowBlueprintInfo(Blueprint blueprint, Inventory inventory)
    {
        if (blueprint == null)
            return;

        ClearElements();
        _nameText.text = Translate (blueprint.ItemName);
        _typeText.text = Translate("Blueprint");
        _itemIcon.gameObject.SetActive(true);
        _itemIcon.sprite = blueprint.Icon;
        _itemIcon.color = blueprint.IconColor;
        if (blueprint.Stackable)
            _itemTagsText.text += Translate("Amount: ") + blueprint.Amount + "\n";
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
            _itemInfoText.text += blueprint.AmountsOfResourses[i] + " x "+ Translate(blueprint.ListOfResourses[i].ItemName)+"\n";
        }

    }

    public void ShowBlueprintProductionItem()
    {
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
	
	public void ShowMineInfo (Mine mine) {
        if (mine.Name == "")
            return;
        bool firstItem = true;

        string rewardInfo = Translate("Potential loot") + ":\n";
        foreach (var item in mine.GetComponent<RewardManager>().RewardList())
        {
            if (firstItem)
                firstItem = false;
            else
                rewardInfo += ", ";
            rewardInfo += Translate(item.ItemName);
        }


        ShowElementInfo(Translate(mine.Name), Translate("Threat level") + ": " + mine.Level, rewardInfo);
    }

    public void ShowFactoryInfo(Factory factory)
    {
        if (factory.Name == "")
            return;
        TaskTimer timer = factory.TaskTimer;

        ShowElementInfo(Translate(factory.Name), Translate("Lines") + ": " +timer.StartedTasks+"/"+timer.SimultaniouslyTasks + "\n" + Translate("Queue") + ": " + timer.QueuedTasks + "/" + timer.MaximumTasks, Translate("@FactoryGeneralInfo"));
    }

    private void OnDisable()
    {
        _localisation.OnLanguageChangedEvent -= ClosePanel;
    }
    private void ClosePanel() => gameObject.SetActive(false);

    public void Scale() => _producedItemIcon.transform.localScale = new Vector3(InventoryItemUI.ITEM_SCALE_MULTIPLER, InventoryItemUI.ITEM_SCALE_MULTIPLER);
    public void Unscale() => _producedItemIcon.transform.localScale = Vector3.one;

    private string Translate(string text) => _localisation.Translate(text);
}
