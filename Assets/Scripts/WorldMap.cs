using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorldMap : MonoBehaviour
{
    private const float STANDING_ON_DISTANCE=0.1f;
    
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private MainMenuUI _mainMenu;
    [SerializeField] private MoveUI _movingPanel;
    [SerializeField] private SearchPoint _searchPointPrefab;
    [SerializeField] private WorldCharacter _player;

    private List<SearchPoint> _searchPoints = new();
    private List<(int, int)> _foundPoints = new();
    
    private Vector3 _movePosition;
    private bool _playerIsMoving=false;

    public void LoadMap() //TODO temporal preparation. Move this to loading from SaveSystem.
    {
        _searchPoints.Add(Instantiate(_searchPointPrefab, transform));
        _searchPoints[_searchPoints.Count-1].Init(1.0f, 1.0f, 1.0f);
        
        _searchPoints.Add(Instantiate(_searchPointPrefab, transform));
        _searchPoints[_searchPoints.Count-1].Init(-1.0f, -2.0f, 3.0f);
    }
    public void Move()
    {
        Vector3 moveVector = Vector3.zero;
        int moveCost = MoveCost();
        if (_player.ActionPoints.TrySpendAP(moveCost))
        {
            _playerIsMoving = true;
            _mainMenu.ExitLocation();
            moveVector = (_movePosition - _player.transform.position).normalized;

            StartCoroutine(MoveEveryFrame());
            //_mainCamera.transform.position = _movePosition;
            
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
            searchPoint.Init(_player.transform.position.x, _player.transform.position.y, 1.0f);
        } else
        {
            searchPoint.Enlarge();
        }

        //TODO search function
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

    private int MoveCost() => MoveCost((int)_movePosition.x, (int)_movePosition.y);
}
