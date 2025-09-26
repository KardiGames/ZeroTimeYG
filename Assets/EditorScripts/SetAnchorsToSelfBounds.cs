using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;

[ExecuteAlways] // �������� � � ���������, � � ����
public class SetAnchorsToSelfBounds : MonoBehaviour
{
    private void Update()
    {
        UpdateAnchors();
    }

    private void UpdateAnchors()
    {
        RectTransform rect = GetComponent<RectTransform>();
        RectTransform parentRect = rect.parent as RectTransform;

        if (parentRect == null || rect.localScale.x <= 1f)
        {
            print("Scale "+ rect.localScale.x + " is good of error. Not executing");
            return;
        } else
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

        rect.localScale = new Vector3(1f, 1f, 1f);

        Button button = GetComponent<Button>();
        Image image = GetComponent<Image>();
        if (button != null && image != null)
        {
            print("Object is button. Changing PPU miltipler");
            image.pixelsPerUnitMultiplier = 0.4f;

        } else
        {
            print("Object is not button");
        }

        TextMeshProUGUI childText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        if (childText != null)
        {
            print("Customizing TMPro");
            childText.fontSize *= 1.5f;
            childText.fontSizeMax *= 1.5f;
            childText.fontSizeMin *= 1.5f;
        }
        else
        {
            print("TMPro text not found");
        }
    }
}