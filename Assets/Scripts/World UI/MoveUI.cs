using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _apText;
    [SerializeField] private Button _moveButton;

    [SerializeField] private Color _canMoveTextColor;
    [SerializeField] private Color _cantMoveTextColor;

    public void Init(int apCost, bool canMove)
    {
        gameObject.SetActive(true);
        _apText.text = apCost + " AP";
        if (canMove)
        {
            _apText.color = _canMoveTextColor;
            _moveButton.interactable = true;
        } else
        {
            _apText.color = _cantMoveTextColor;
            _moveButton.interactable = false;
        }
    }
}
