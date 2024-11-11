using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentUI : MonoBehaviour
{
    [SerializeField] private Equipment _equipment;
    [SerializeField] private WorldUserInterface _worldUI;
    [SerializeField] private List<SlotUI> _slotList = new ();
    private Inventory _inventoryToUnequip;
    private Color _emptySlotColor;

    public Equipment Equipment
    {
        get => _equipment; set
        {
            if (_emptySlotColor == Color.clear)
                _emptySlotColor = _slotList[0].ItemImage.color;
            if (_equipment != value || value == null)
            {
                Unsubscribe();
                _equipment = value;
                SubscribeAndRefresh();
            }
            else
            {
                Fill();
            }
        }
    }

    private void SubscribeAndRefresh()
    {
        if (_equipment != null)
        {
            _equipment.OnEquipmentContentChanged += Fill;
            Fill();
        }
    }

    private void Unsubscribe()
    {
        if (_equipment != null)
        {
            _equipment.OnEquipmentContentChanged -= Fill;
        }
    }

    public void Fill()
    {
        if (_slotList == null && _equipment.SlotsCount()!=_slotList.Count)
        {
            print("Error! Can't fill equipment. EQ size != number of eq slots");
            return;
        }

        if (!_equipment.TryGetComponent<Inventory>(out _inventoryToUnequip)) {
            print("Error! Can't fill equipment. Haven't found connected inventory");
            return;
        }

        for (int i = 0; i < _slotList.Count; i++)
        {
            if (_equipment[i] == null)
            {
                _slotList[i].ItemImage.sprite = null;
                _slotList[i].ItemImage.color = _emptySlotColor;
                _slotList[i].Cross.SetActive(false);
            }
            else
            {
                _slotList[i].ItemImage.sprite = _equipment[i].Icon;
                _slotList[i].ItemImage.color = _equipment[i].IconColor;
                _slotList[i].Cross.SetActive(true);
            }
        }


    }

    public void Unequip(int slot)
    {
        if (slot<0 || slot>=_slotList.Count)
        {
            print("Error! Tryed to unequp missing slot "+slot);
            return;
        }
        _equipment.Unequip((Equipment.Slot)slot, _inventoryToUnequip);
    }
    public void ShowItemInfo(int slotNumber)
    {
        if (Equipment[slotNumber] != null)
            _worldUI.ShowItemInfo(Equipment[slotNumber]);
    }
    public void Scale(int slotNumber)
    {
        if (_equipment[slotNumber] != null)
            _slotList[slotNumber].ItemImage.transform.localScale = new Vector3(InventoryItemUI.ITEM_SCALE_MULTIPLER, InventoryItemUI.ITEM_SCALE_MULTIPLER);


    }
    public void Unscale(int slotNumber) {
        _slotList[slotNumber].ItemImage.transform.localScale = Vector3.one;
    }

    private void OnDisable()
    {
        Equipment = null;
    }

    [Serializable]
    private class SlotUI
    {
        [SerializeField] private Image _itemImage;
        [SerializeField] private GameObject _cross;
        public Image ItemImage => _itemImage;
        public GameObject Cross => _cross;
    }
}
