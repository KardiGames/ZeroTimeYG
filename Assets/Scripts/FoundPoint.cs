using UnityEngine;
using TMPro;
using System;

public class FoundPoint : MonoBehaviour
{

    private const float MOUSE_OVER_SCALE_MULTIPLER = 3f;
    
    [SerializeField] private ClickPointOnMap _clickPoint;
    [SerializeField] private TextMeshProUGUI _nameText;

    [SerializeField] private Sprite _centralFactory;
    [SerializeField] private Sprite _factory;
    [SerializeField] private Sprite _mine;
    [SerializeField] private Sprite _laboratory;
    [SerializeField] private Sprite _alienBase;

    private string buildingName;

    public void Init(float x, float y, WorldMap map, string name)
    {
        if (map==null)
        {
            Destroy(this);
			return;
        }

		Vector3 positionVector = transform.localPosition;
        positionVector.x = x;
        positionVector.y = y;
        transform.localPosition = positionVector;

        buildingName = name;
        
        SpriteRenderer icon = GetComponent<SpriteRenderer>();
        icon.sprite=BuildingSpriteByName(buildingName);

        _clickPoint.Init(x, y, map);
    }

    private void OnMouseEnter()
    {
        Vector3 scaledSizeVector = transform.localScale * MOUSE_OVER_SCALE_MULTIPLER;
        transform.localScale = scaledSizeVector;
        _nameText.transform.parent.gameObject.SetActive(true);
        _nameText.text = GlobalUserInterface.Instance.Localisation.Translate(buildingName);
    }

    private void OnMouseExit()
    {
        Vector3 scaledSizeVector = transform.localScale / MOUSE_OVER_SCALE_MULTIPLER;
        transform.localScale = scaledSizeVector;
        _nameText.transform.parent.gameObject.SetActive(false);
    }

    private Sprite BuildingSpriteByName(string name)
    {
        if (name == "Central factory")
            return _centralFactory;

        string[] words = name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string word in words)
        {
            if (word == "factory")
                return _factory;

            if (word == "lab")
                return _laboratory;

            if (word == "base")
                return _alienBase;

            if (word == "mine")
                return _mine;

        }
        return _factory;

    }

}
