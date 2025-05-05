using UnityEngine;
using TMPro;

public class FoundPoint : MonoBehaviour
{
	
    [SerializeField] ClickPointOnMap _clickPoint;
    [SerializeField] TextMeshProUGUI _nameText;

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

        _nameText.text = name;

        _clickPoint.Init(x, y, map);
    }
}
