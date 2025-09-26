using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;

[ExecuteAlways] // �������� � � ���������, � � ����
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

        // 1. �������� ������� ������� ���������� ����� �������
        Vector3[] worldCorners = new Vector3[4];
        rect.GetWorldCorners(worldCorners);

        // 2. ������������ ������� ���������� � ��������� ������������ ��������
        Vector2 minLocal = parentRect.InverseTransformPoint(worldCorners[0]); // ����� ������ ����
        Vector2 maxLocal = parentRect.InverseTransformPoint(worldCorners[2]); // ������ ������� ����

        // 3. ����������� ���������� (��������� � ������� [0,1] ������������ ��������)
        Vector2 parentSize = parentRect.rect.size;
        Vector2 anchorMin = new Vector2(
            (minLocal.x / parentSize.x) + 0.5f,
            (minLocal.y / parentSize.y) + 0.5f
        );
        Vector2 anchorMax = new Vector2(
            (maxLocal.x / parentSize.x) + 0.5f,
            (maxLocal.y / parentSize.y) + 0.5f
        );

        // 4. ��������� �����, �������� ������� ������
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}