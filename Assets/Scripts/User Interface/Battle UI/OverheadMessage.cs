using System.Collections;
using UnityEngine;
using TMPro;

public class OverheadMessage : MonoBehaviour
{
    private float _messageTime = 1.5f;
    private float _messageMoveUpDistance = 0.8f;
    
    [SerializeField] private TextMeshProUGUI _overheadText;
    [SerializeField] private TextMeshProUGUI _redText;
    [SerializeField] private TextMeshProUGUI _greenText;
    private CombatUnit _combatUnit;
    private Localisation _localisation;

    private Vector3 _startPosition;

    // Start is called before the first frame update
    void Start()
    {
        _combatUnit = GetComponentInParent<CombatUnit>();
        _localisation = GlobalUserInterface.Instance.Localisation;
        _startPosition = _redText.transform.localPosition;
        ShowHP();
    }

    public void ShowHP() => _overheadText.text=(_combatUnit?.HP<0 ? _localisation.Translate("Dead") : _combatUnit?.HP.ToString()+ _localisation.Translate(" HP"));
    public void ShowHP(int hp) => _overheadText.text = (hp < 0 ? _localisation.Translate("Dead") : hp.ToString() + _localisation.Translate(" HP"));
    public void Show(string text) => _overheadText.text = text;

    public void ShowRed(string text) => MoveUpMessage(text, _redText); 
    public void ShowGreen(string text) => MoveUpMessage(text, _greenText);

    private void MoveUpMessage(string text, TextMeshProUGUI movingObject)
    {
        RectTransform textTransform = movingObject.gameObject.GetComponent<RectTransform>();
        movingObject.gameObject.SetActive(true);
        movingObject.text = text;

        StartCoroutine(MoveTextUp());

        IEnumerator MoveTextUp()
        {
            while (textTransform.localPosition.y < (_startPosition.y + _messageMoveUpDistance))
            {
                textTransform.Translate(Vector3.up * _messageMoveUpDistance / _messageTime * Time.deltaTime);
                yield return null;
            }
            textTransform.localPosition = _startPosition;
            movingObject.gameObject.SetActive(false);
        }
    }

}
