using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorldMap : MonoBehaviour
{
    private const float STANDING_ON_DISTANCE=0.1f;
    
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private MainMenuUI _mainMenu;
    [SerializeField] private SaveData _saveData;
    [SerializeField] private MoveUI _movingPanel;
    [SerializeField] private SearchPoint _searchPointPrefab;
    [SerializeField] private ClickPointOnMap _foundPointPrefab;
    [SerializeField] private WorldCharacter _player;

    private List<SearchPoint> _searchPoints = new();
    private List<(int x, int y)> _foundPoints = new();
    
    private Vector3 _movePosition;
    private bool _playerIsMoving=false;

    public void Move()
    {
        Vector3 moveVector = Vector3.zero;
        int moveCost = MoveCost();
        if (_player.ActionPoints.TrySpendAP(moveCost))
        {
            _mainMenu.ExitLocation();
            moveVector = (_movePosition - _player.transform.position).normalized;

            if (moveVector == Vector3.zero)
                return;

            _playerIsMoving = true;
            StartCoroutine(MoveEveryFrame());
        }
        else
            print($"Error. Somehow you can't spent {MoveCost()} AP for moving, but tryed to start it");

        IEnumerator MoveEveryFrame()
        {
            while (((_movePosition.x - _player.transform.position.x) >= 0) == (moveVector.x >= 0)
                || ((_movePosition.y - _player.transform.position.y) >= 0) == (moveVector.y >= 0))
            {
                _player.transform.Translate(moveVector * Time.deltaTime);
                yield return null;
            }
            _player.transform.position = _movePosition;
            _saveData.SaveCharacter();
            _playerIsMoving = false;
        }
    }
    private void OnMouseUp()
    {
        if (EventSystem.current.IsPointerOverGameObject() || _playerIsMoving)
            return;
        Vector3 mousePosition = Input.mousePosition;

        if (!_movingPanel.gameObject.activeInHierarchy)
        {
            _movePosition = _mainCamera.ScreenToWorldPoint(mousePosition);
            _movePosition.z = _player.transform.position.z;

            _movingPanel.Init(MoveCost(), IsAbleToMove());
            Vector3 panelPosition = mousePosition;
            panelPosition.z = _movingPanel.transform.position.z;
            _movingPanel.transform.position = panelPosition;
        } else
        {
            _movingPanel.gameObject.SetActive(false);
        }

    }

    public void OpenMovePanelToFoundPoint (float x, float y)
    {
        if (_playerIsMoving)
            return;
        _movePosition.x = x;
        _movePosition.y = y;

        _movingPanel.Init(MoveCost(), true);

        _movePosition.z = _player.transform.position.z;
        Vector3 panelPosition = Input.mousePosition;
        panelPosition.z = _movingPanel.transform.position.z;
        _movingPanel.transform.position = panelPosition;

    }

    private bool IsAbleToMove()
    {
        int moveCost = MoveCost();
        if (moveCost == 0)
            return false;
        return moveCost <= _player.AP;
    }
    private int MoveCost(int x, int y)
    {
        if (x == _player.X && y == _player.Y)
            return 0;
        else 
            return Mathf.Max(1, Mathf.Abs(x - _player.X) + Mathf.Abs(y - _player.Y));
    }
    private int MoveCost() => MoveCost((int)_movePosition.x, (int)_movePosition.y);

    public void Search()
    {
        if (!_player.ActionPoints.TrySpendAP(SearchPoint.SEARCH_COST))
        {
            print("You haven't anough Action Points");
            return;
        }

        SearchPoint searchPoint;
        if (!IsAbleToSearch(out searchPoint))
        {
            print("You are standing inside exiting search area! Go to the center or outside of area");
            return;
        }

        if (searchPoint == null)
        {
            searchPoint = Instantiate(_searchPointPrefab, transform);
            if (searchPoint != null)
                _searchPoints.Add(searchPoint);
            else
                throw new Exception("For some reashon search point wasn't instantiated");
            searchPoint.Init(_player.transform.position.x, _player.transform.position.y, 1.0f, this);
        } else
        {
            searchPoint.Enlarge();
        }

        List<(int, int)> areasWithBuildings = new List<(int, int)>(_saveData.AreasWithBuildings().Except(_foundPoints));
        foreach ((int x, int y) area in areasWithBuildings)
        {
            if (new Vector2(area.x - searchPoint.X, area.y-searchPoint.Y).magnitude < searchPoint.Radius)
            {
                _foundPoints.Add((area.x, area.y));
                PlaceFoundPointSignOnArea(area.x, area.y);
                break;
            }
        }

        _saveData.SaveMap();
    }

    public bool AreBuildingsFound(int x, int y)
    {
        foreach ((int x, int y) area in _foundPoints) {
            if (area.x == x && area.y == y)
                return true;
        }
        return false;
    }

    private void PlaceFoundPointSignOnArea(int x, int y)
    {
        Instantiate(_foundPointPrefab, new Vector3(x, y, _foundPointPrefab.transform.position.z+transform.position.z), _foundPointPrefab.transform.rotation, transform).Init(x, y, this); 
    }
    private bool IsAbleToSearch (out SearchPoint standingOnPoint)
    {
        standingOnPoint = null;
        bool result = true;
        Vector3 playerPosition = _player.transform.position;
        float distance;
        foreach (SearchPoint sp in _searchPoints)
        {
            distance = new Vector2(sp.X - playerPosition.x, sp.Y - playerPosition.y).magnitude;
            if (distance < STANDING_ON_DISTANCE)
            {
                standingOnPoint = sp;
                return true;
            }
            else if (distance <= sp.Radius)
                result = false;
        }
        return result;
    }

    public string ToJson()
    {
        MapJsonData mapJson = new MapJsonData();
		for (int i=0; i<_searchPoints.Count; i++){
			mapJson.searchPointsXYSize.Add(_searchPoints[i].X);
			mapJson.searchPointsXYSize.Add(_searchPoints[i].Y);
			mapJson.searchPointsXYSize.Add(_searchPoints[i].Size);
		}
		for (int i=0; i<_foundPoints.Count; i++) {
			mapJson.foundPointsXY.Add(_foundPoints[i].x);
			mapJson.foundPointsXY.Add(_foundPoints[i].y);
		}
		return JsonUtility.ToJson(mapJson);
    }
	
	internal void FromJson(string jsonString)
    {
        _searchPoints.Clear();
		_foundPoints.Clear();
		
		MapJsonData mapJson = JsonUtility.FromJson<MapJsonData>(jsonString);
		int i=0;
		while (i< mapJson.searchPointsXYSize.Count) {
			_searchPoints.Add(Instantiate(_searchPointPrefab, transform));
			_searchPoints[_searchPoints.Count-1].Init(mapJson.searchPointsXYSize[i++], mapJson.searchPointsXYSize[i++], mapJson.searchPointsXYSize[i++], this);
		}
		i=0;
		while (i<mapJson.foundPointsXY.Count)
			_foundPoints.Add((mapJson.foundPointsXY[i++], mapJson.foundPointsXY[i++]));
		foreach ((int x, int y) point in _foundPoints)
			PlaceFoundPointSignOnArea(point.x, point.y);
    }
	
	[Serializable]
	private class MapJsonData
	{
		public List<float> searchPointsXYSize=new List<float>();
		public List<int> foundPointsXY= new List <int>();
	}
}
