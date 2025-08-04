using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFitToObject : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _targetSprite; // Ваш 2D-объект
    [SerializeField] private float _padding = 1f; // Отступ вокруг объекта
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
        // Учитываем соотношение сторон экрана
        float screenRatio = (float)Screen.width / Screen.height;
        float targetRatio = _objectWidth / _objectHeight;

        // Выбираем, какой размер (по ширине или высоте) использовать для камеры
        if (screenRatio >= targetRatio)
        {
            // Ориентируемся по высоте
            _camera.orthographicSize = (_objectHeight / 2f) + _padding;
        }
        else
        {
            // Ориентируемся по ширине с учётом соотношения сторон
            _camera.orthographicSize = ((_objectWidth / screenRatio) / 2f) + _padding;
        }

        // Центрируем камеру на объекте
        /*
        transform.position = new Vector3(
            _bounds.center.x,
            _bounds.center.y,
            transform.position.z
        );*/
    }
}