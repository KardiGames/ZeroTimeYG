using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;

[ExecuteAlways] // Работает и в редакторе, и в игре
public class JustSetAnchors : MonoBehaviour
{
    private void Update()
    {
        UpdateAnchors();
    }

    private void UpdateAnchors()
    {
        RectTransform rect = GetComponent<RectTransform>();
        RectTransform parentRect = rect.parent as RectTransform;

            print("Executing");

        // 1. Получаем текущие мировые координаты углов объекта
        Vector3[] worldCorners = new Vector3[4];
        rect.GetWorldCorners(worldCorners);

        // 2. Конвертируем мировые координаты в локальные относительно родителя
        Vector2 minLocal = parentRect.InverseTransformPoint(worldCorners[0]); // Левый нижний угол
        Vector2 maxLocal = parentRect.InverseTransformPoint(worldCorners[2]); // Правый верхний угол

        // 3. Нормализуем координаты (переводим в систему [0,1] относительно родителя)
        Vector2 parentSize = parentRect.rect.size;
        Vector2 anchorMin = new Vector2(
            (minLocal.x / parentSize.x) + 0.5f,
            (minLocal.y / parentSize.y) + 0.5f
        );
        Vector2 anchorMax = new Vector2(
            (maxLocal.x / parentSize.x) + 0.5f,
            (maxLocal.y / parentSize.y) + 0.5f
        );

        // 4. Применяем якоря, сохраняя текущий размер
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}