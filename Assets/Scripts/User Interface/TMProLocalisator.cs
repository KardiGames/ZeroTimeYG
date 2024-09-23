using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TMProLocalisator : MonoBehaviour
{
    [SerializeField] private Localisation _localisation;
    [SerializeField] private TextMeshProUGUI _translatingText;
    [TextArea(4, 15)] [SerializeField] private string _english;
    [TextArea(4, 15)] [SerializeField] private string _russian;
    
    private void OnEnable()
    {
        if (_localisation == null)
            _localisation = GlobalUserInterface.Instance.Localisation;
        
        if (_translatingText == null)
        {
            _translatingText = GetComponent<TextMeshProUGUI>();
            print("Translating component wasn't set in the inspector.");
        }
        _localisation.OnLanguageChangedEvent += ChangeText;

        if (_english == "" && _translatingText.text!="")
        {
            _english = _translatingText.text;
            print("English text on TMPro have been automaticaly copied from object");
        }
        ChangeText(_localisation.CurrentLanguage);
    }

    private void ChangeText(string language)
    {
        if (language == "ru_ru" && _russian != "")
            _translatingText.text = _russian;
        else if (language == "en_en" && _english != "")
            _translatingText.text = _english;
    }

    private void OnDisable()
    {
        _localisation.OnLanguageChangedEvent -= ChangeText;
    }
}
