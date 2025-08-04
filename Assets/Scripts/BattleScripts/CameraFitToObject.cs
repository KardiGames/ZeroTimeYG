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


    private void Start()
    {
        _camera.orthographic = true;
        if (_targetSprite == null)
            return;

        _bounds = _targetSprite.bounds;

        _objectWidth = _bounds.size.x;
        _objectHeight = _bounds.size.y;
    }

    private void Update()
    {
        // ��������� ����������� ������ ������
        float screenRatio = (float)Screen.width / Screen.height;
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

        // ���������� ������ �� �������
        /*
        transform.position = new Vector3(
            _bounds.center.x,
            _bounds.center.y,
            transform.position.z
        );*/
    }
}