using UnityEngine;

public class SearchPoint : MonoBehaviour
{
    private const float ENLARGE_LEVEL_MULTIPLER = 0.1f;
	
	[SerializeField] GameObject _wideAreaCircle;
    [SerializeField] SpriteRenderer _wideAreaSprite;
    [SerializeField] GameObject _tinyAreaCircle;
    [SerializeField] ClickPointOnMap _clickPoint;
    [SerializeField] private float _size; 
	private WorldCharacter _player;
    
    public float X=>transform.localPosition.x;
    public float Y=>transform.localPosition.y;
    public float Size=>_size;
    public float Radius => _size / 2.0f;
    public float TinyRadius => _size / 4.0f;
	public bool Initiated => (_player!=null);
	
    public void Init(float x, float y, float size, WorldMap map, WorldCharacter player)
    {
        if (map==null || player==null)
        {
            Destroy(this);
			return;
        }
		_player=player;
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
        //Formula to add area equals to fist created circle (with radius of 0.5+skillImprovement)
        float enlargedRadius = Mathf.Sqrt(0.25f*(1+ENLARGE_LEVEL_MULTIPLER*_player.Level*_player.Skills.GetSkillMultipler("World exploring")) + (Radius*Radius));
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
