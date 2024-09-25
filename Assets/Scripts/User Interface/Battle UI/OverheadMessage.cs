using System.Collections;
using UnityEngine;
using TMPro;

public class OverheadMessage : MonoBehaviour
{
    private float messageTime = 1.5f;
    private float messageMoveUpDistance = 0.8f;
    
    [SerializeField] private TextMeshProUGUI overheadText;
    [SerializeField] private TextMeshProUGUI redText;
    [SerializeField] private TextMeshProUGUI greenText;
    private CombatUnit combatUnit;

    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        combatUnit = GetComponentInParent<CombatUnit>();
        startPosition = redText.transform.localPosition;
        ShowHP();
    }

    public void ShowHP() => overheadText.text=(combatUnit?.HP<0 ? "Dead" : combatUnit?.HP.ToString()+ " HP");
    public void ShowHP(int hp) => overheadText.text = (hp < 0 ? "Dead" : hp.ToString() + " HP");
    public void Show(string text) => overheadText.text = text;

    public void ShowRed(string text) => MoveUpMessage(text, redText); 
    public void ShowGreen(string text) => MoveUpMessage(text, greenText);

    private void MoveUpMessage(string text, TextMeshProUGUI movingObject)
    {
        RectTransform textTransform = movingObject.gameObject.GetComponent<RectTransform>();
        movingObject.gameObject.SetActive(true);
        movingObject.text = text;

        StartCoroutine(MoveTextUp());

        IEnumerator MoveTextUp()
        {
            while (textTransform.localPosition.y < (startPosition.y + messageMoveUpDistance))
            {
                textTransform.Translate(Vector3.up * messageMoveUpDistance / messageTime * Time.deltaTime);
                yield return null;
            }
            textTransform.localPosition = startPosition;
            movingObject.gameObject.SetActive(false);
        }
    }

}
