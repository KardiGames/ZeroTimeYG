using UnityEngine.EventSystems;
using UnityEngine;

public class ClickPointOnMap : MonoBehaviour
{
    private float FLOAT_MISTAKE_CORRECTION = 0.0001f;
    float _x;
    float _y;
    WorldMap _map;
    public void Init(float x, float y, WorldMap map)
    {
        _x = x;
        _y = y;
        _map = map;
    }

    private void OnMouseUp()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        _map.OpenMovePanelToFoundPoint((_x<0)? _x-FLOAT_MISTAKE_CORRECTION : _x+FLOAT_MISTAKE_CORRECTION, (_y < 0) ? _y - FLOAT_MISTAKE_CORRECTION : _y + FLOAT_MISTAKE_CORRECTION);
    }
}
 