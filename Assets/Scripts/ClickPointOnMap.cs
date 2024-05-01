using UnityEngine.EventSystems;
using UnityEngine;

public class ClickPointOnMap : MonoBehaviour
{
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

        _map.OpenMovePanelToFoundPoint(_x, _y);
    }
}
