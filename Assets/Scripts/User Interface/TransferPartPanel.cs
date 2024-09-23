using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TransferPartPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _errorText;
    [SerializeField] private TMP_InputField _amountInput;
    private InventoryUIContentFiller _inventoryUI;
    private Item _item;

    private bool _initialized = false;


    void Start()
    {
        if (!_initialized)
        {
            print("Error! Transfer part button was not initializated");
            return;
        }

    }

    public void Init(InventoryUIContentFiller inventoryUI, Item item)
    {
        if (inventoryUI == null || item == null)
            return;

        _item = item;
        _inventoryUI = inventoryUI;
        _initialized = true;

        _amountInput.text = item.Amount.ToString();
    }

    private void OnDisable()
    {
        _initialized = false;
    }

    public void TransferPart()
    {
        if (_inventoryUI == null
            || _inventoryUI.Inventory == null
            || _inventoryUI.TargetInventory == null
            )
            return;

        long amount;

        if (!long.TryParse(_amountInput.text, out amount))
            ShowErrorText(_amountInput.text + " is not a number");

        if (IsAmountCorrect(amount))
        {
            _inventoryUI.Inventory.TransferTo(_inventoryUI.Inventory, _inventoryUI.TargetInventory, _item, amount);
            gameObject.SetActive(false);
        }
        else
            ShowErrorText("Value must be between 1 and " + _item.Amount);
    }

    public bool IsAmountCorrect(long amount) =>
        (amount > _item.Amount || amount < 1) ? false : true;


    private void ShowErrorText(string text) => _errorText.text = text;
}
