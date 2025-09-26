using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFitToObject : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _targetSprite; // ��� 2D-������
    [SerializeField] private float _padding = 1f; // ������ ������ �������
    [SerializeField] private Camera _camera;
    private Bounds _bounds;
    private float _objectWidth;
    private float _objectHeight;
    private int _screenWidth;
    private int _screenHeight;


    private void Start()
    {
        _camera.orthographic = true;
        if (_targetSprite == null)
            return;

        _bounds = _targetSprite.bounds;

        _objectWidth = _bounds.size.x;
        _objectHeight = _bounds.size.y;
    }



    private void OnEnable()
    {
        print("Camera fitting Enabled");
    }

    private void OnDisable ()
    {
        print("Camera fitting Disabled");
    }
    private void Update()
    {
        if (Screen.width == _screenWidth && Screen.height == _screenHeight)
            return;

        _screenHeight= Screen.height;
        _screenWidth= Screen.width;

        // ��������� ����������� ������ ������
        float screenRatio = (float)_screenWidth / _screenHeight;
        float targetRatio = _objectWidth / _objectHeight;

        // ��������, ����� ������ (�� ������ ��� ������) ������������ ��� ������
        if (screenRatio >= targetRatio)
        {
            // ������������� �� ������
            _camera.orthographicSize = (_objectHeight / 2f) + _padding;
        }
        else
        {
            // ������������� �� ������ � ������ ����������� ������
            _camera.orthographicSize = ((_objectWidth / screenRatio) / 2f) + _padding;
        }
    }
}