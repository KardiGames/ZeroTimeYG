using UnityEngine;

[CreateAssetMenu(menuName = "InfoPanelData")]
public class InfoPanelData : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private string _type;
    [SerializeField] [TextArea(2,20)] private string _text;

    public string Name => _name;
    public string Type => _type;
    public string Text => _text;
}
