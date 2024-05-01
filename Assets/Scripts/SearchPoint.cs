using UnityEngine;

public class SearchPoint : MonoBehaviour
{
    public const int SEARCH_COST = 10;

    [SerializeField] GameObject _wideAreaCircle;
    [SerializeField] SpriteRenderer _wideAreaSprite;
    [SerializeField] GameObject _tinyAreaCircle;
    [SerializeField] ClickPointOnMap _clickPoint;
    [SerializeField] private float _size; 
    
    public float X=>transform.localPosition.x;
    public float Y=>transform.localPosition.y;
    public float Size=>_size;
    public float Radius => _size / 2.0f;
    public float TinyRadius => _size / 4.0f;
    public void Init(float x, float y, float size, WorldMap map)
    {
        Vector3 positionVector = transform.localPosition;
        positionVector.x = x;
        positionVector.y = y;
        transform.localPosition = positionVector;
        _size = size;
        VisualizeSize();
        _clickPoint.Init(x, y, map);
    }
    public void Enlarge ()
    {
        //Formula to add area equals to fist created circle (with radius of 0.5)
        float enlargedRadius = Mathf.Sqrt(0.25f + (Radius*Radius));
        _size = enlargedRadius * 2.0f;
        VisualizeSize();
    }

    private void VisualizeSize()
    {
        Vector3 sizeVector = new Vector3(_size, _size, _size);
        _wideAreaCircle.transform.localScale = sizeVector;
        sizeVector *= 0.5f;
        _tinyAreaCircle.transform.localScale = sizeVector;
    }
}
